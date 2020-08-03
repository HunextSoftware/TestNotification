<div align="center"> 
<img src="Images/_icon.png" alt="Immagine dell'icona"/>

# DESCRIZIONE DEL FUNZIONAMENTO DELLE PIATTAFORME PER LA GESTIONE DELLE PUSH NOTIFICATION PER ANDROID ED IOS
</div>

In questo documento vengono descritte le possibili soluzioni individuate per l'implementazione del sistema di notifiche push, andando a scoprire nel dettaglio i PNS.
Infine ci sarà una sezione dedicata alla piattaforma scelta e le ragioni per il quale è stata individuata come soluzione ideale per questo progetto.

Il documento è strutturato nelle seguenti sezioni:
- [Panoramica generale sui PNS](#panoramica-generale-sui-pns)
- [La soluzione individuata: Azure Notification Hubs](#la-soluzione-individuata-azure-notification-hubs)

---

## Panoramica generale sui PNS

La mobile application Hunext Mobile è multipiattaforma e più nello specifico è indirizzata a dispositivi Android ed iOS.

Il primo passo da compiere è quindi registrare la mobile application nel PNS di riferimento, in modo da consentire a chi installa l'applicazione
la ricezione delle notifiche e tutti i vantaggi che ne conseguono (la procedura verrà approfondita nel documento *4_Descrizione-POC*).

Per poter usufruire del servizio di notifiche, ogni dispositivo deve obbligatoriamente registrarsi al PNS di riferimento, instaurando un collegamento diretto ad esso: questa procedura viene
chiamata nel gergo *PNS Handling*.

Purtroppo non esiste uno standard unico per la gestione, la configurazione e la comunicazione con i PNS, in quanto ogni ecosistema specifico offre strumenti che svolgono la stessa funzione
ma che lavorano in modo diverso. Proprio per questo motivo vanno analizzate nel dettaglio le piattaforme di notifica principali, ovvero:

- [Firebase Cloud Messaging (FCM)](#firebase-cloud-messaging-fcm) per l'ecosistema Android.
- [Apple Platform Notification System (APNS)](#apple-platform-notification-system-apns) per l'ecosistema Apple.
- [Azure Notification Hubs](#azure-notification-hubs) per l'ecosistema Android, Apple e Windows.

### Firebase Cloud Messaging (FCM)

FCM consente di inviare notifiche principalmente per i dispositivi Android. Questo servizio è comunque disponibile per i dispositivi Apple e gli applicativi Web.

La prerogativa per il funzionamento di FCM, come per ogni PNS, consiste nella registrazione dei dispositivi per la ricezione dei messaggi da FCM.
Questa procedura viene avviata da un dispositivo mobile che contatta FCM per ottenere un token di registrazione, il quale identifica in modo univoco l'istanza dell'applicazione
associata a quel dispositivo.
Il token di registrazione viene poi salvato nel backend di FCM, e da quel momento il dispositivo è abilitato a ricevere le notifiche.

<div align="center">
    <img src="Images/3.1)FCM-architecture.png" alt="Immagine dell'architettura di Firebase"/>
</div>

Il funzionamento di Firebase avviene principalmente in 4 fasi:

1) viene creata la richiesta di notifica da parte del server del provider. In un contesto reale questa responsabilità è affidata al backend aziendale, 
mentre nel contesto di questo progetto è affidata alla web application.
2) la richiesta viene passata al backend di FCM, il quale si occupa di inoltrare la richiesta ai vari livelli di trasporto.
3) la notifica viene indirizzata ai dispositivi di destinazione, applicando la configurazione della piattaforma specifica. Questo passaggio avviene
al di fuori di Firebase, nello specifico:
    
  - nell'Android Transport Layer (ATL) per dispositivi Android che supportano i Google Play Services.
  - nella piattaforma Apple Platform Notification System (APN) per dispositivi iOS.
  - mediante il protocollo push Web per applicativi web.
 
4) i dispositivi ricevono la notifica in base allo stato attuale del dispositivo e alle configurazioni precedenti.


Il payload del messaggio è formattato in JSON (uno standard per il trasferimento di dati in rete) e può avere la dimensione massima di 4 KB, con un set di chiavi fissato da Android, con la possibilità di 
personalizzare le proprie chiavi.
Le chiavi predefinite sono:

- *token*, che è una stringa alfanumerica generata da Firebase che identifica il collegamento unidirezionale da Firebase al dispositivo.
- *notification*, che contiene le informazioni base della notifica, ovvero:
    - *title*, ovvero il titolo che appare nella notifica.
    - *body*, ovvero il messaggio che appare nella notifica.
- eccetera.

I parametri opzionali sono contenuti dentro la chiave *data*, che a sua volta contiene delle sotto-chiavi personalizzabili dall'utente. 

Inoltre è possibile specificare opzioni specifiche per ogni piattaforma che vanno a sovrascrivere le opzioni base, per esempio la priorità della notifica oppure il suo tempo di permanenza massima
nel PNS. Le sotto-chiavi che identificano la piattaforma sono: *android*, *apns* e *webpush*.

Per maggiori informazioni, visitare il seguente [link](https://firebase.google.com/docs/cloud-messaging/concept-options).


### Apple Platform Notification System (APNS) 

APNS è un PNS specifico in quanto consente di inviare notifiche solo ai dispositivi Apple. 

La prerogativa per il funzionamento di APNS, come per ogni PNS, consiste nella registrazione dei dispositivi per la ricezione dei messaggi da APNS.
Questa procedura viene avviata da un dispositivo Apple che, una volta avviata l'istanza dell'applicazione, contatta APNS per ricevere il token del 
dispositivo che identifica in modo univoco l'istanza dell'applicazione associata a quel dispositivo.
Il token di registrazione viene poi inoltrato al server del provider, e da quel momento il dispositivo è abilitato a ricevere le notifiche.

<div align="center"> 
<img src="Images/3.2)APN-architecture.png" alt="Immagine processo Azure"/>
</div>

Come riportato nella documentazione, la comunicazione tra il server del provider e APNS deve avvenire tramite una connessione protetta.

La creazione di tale connessione richiede l'installazione di un certificato radice della Certificate Authority(CA) GeoTrust sul server del provider. 
Se il server del provider non è eseguito su MacOS, è necessario installare un certificato autonomamente, possibilmente da questo 
[link](https://www.geotrust.com/resources/root-certificates/).
Per avere l'autorizzazione ad inviare le notifiche e interagire con APNS, il server del provider deve creare un certificato, che può essere il più recente
basato su token (consigliato, con estensione .p8 che utilizza HTTP/2) oppure il meno recente con estensione .p12 che utilizza TLS.

Una volta che sono stati rispettati questi prerequisiti, APNS può inviare notifiche gestendo una connessione IP crittografata e permanente al dispositivo
dell'utente, archiviando e inoltrando le notifiche per un dispositivo attualmente offline e raccogliendo notifiche con lo stesso identificatore.


Il payload del messaggio è formattato in JSON (uno standard per il trasferimento di dati in rete) ed include chiavi definite da Apple. Inoltre possono essere aggiunte
chiavi personalizzabili. 
La dimensione massima del payload è di 5 KB per le notifiche VoIP (Voice over Internet Protocol), mentre è di 4 KB per tutte le altre notifiche remote.
La chiave predefinita è *aps*, che a sua volta può contenere le seguenti sotto-chiavi:
- *alert*: indica le informazioni per la visualizzazione di una notifica. A sua volta può contenere le sotto-chiavi:
    - *title*: indica titolo della notifica.
    - *subtitle*: aggiunge informazioni aggiuntive per spiegare lo scopo della notifica.
    - *body*: indica il contenuto della notifica.
- *badge*: indica il numero da visualizzare nel badge posto sull'icona della mobile application in questione.
- eccetera.

Tutte le altre chiavi personalizzabili dall'utente possono essere aggiunte nel payload allo stesso livello gerarchico di *aps*.

Per maggiori informazioni, visitare il seguente [link](https://developer.apple.com/documentation/usernotifications).

### Azure Notification Hubs

<div align="center"> 
<img src="Images/3.3)Notification-hub-diagram.png" alt="Immagine processo Azure"/>
</div>

<div align="right">

[Torna su](#descrizione-del-funzionamento-delle-piattaforme-per-la-gestione-delle-push-notification-per-android-ed-ios)
</div>

---

## La soluzione individuata: Azure Notification Hubs

<div align="right">

[Torna su](#descrizione-del-funzionamento-delle-piattaforme-per-la-gestione-delle-push-notification-per-android-ed-ios)
</div>

---

### Cosa si salva Azure Notification Hubs durante il processo di installazione del dispositivo

I seguenti dati possono essere visualizzati in tempo reale aprendo Visual Studio 2019 e, dal menu, seguire il seguente percorso:
> View -> Server Explorer -> Azure (mailoutlook@outlook.com) -> Notification Hubs -> *NomeNotificationHub*

**Pre-condizione**: l'utente ha un account Azure e ha creato un Notification Hubs con il suo spazio di nomi.

**Post-condizione**: qualunque dispositivo può registrarsi al hub di notifica di Azure.

---

- **Platform**: identifica il tipo di piattaforma utilizzata nella registrazione mediante un'apposita sigla 
    - GCM => Google 
    - APN => Apple
    - WNS => Windows
    - MPNS => Windows Phone
    - ADM => Amazon
    - BCP => Baidu
- **Type**: indica il nome del modello salvato al momento della registrazione, ovvero il formato della notifica che si vuole inviare all'utente. Se è contrassegnato come *Native*, significa che <span style="text-decoration: underline;">non</span> è stato personalizzato e si utilizza il modello standard. 
- **Tags**: etichetta/e che identificano l'utente secondo una o più categorie di appartenenza. I tag non sono obbligatori, ma sono molto utili per indirizzare le notifiche a gruppi di utenti. Se non è presente alcun tag, le notifiche saranno indirizzate a tutti gli utenti registrati in Azure Hub.
- **PNS Identifier**: token che funge da identificativo per un specifico dispositivo, che viene recuperato nell'operazione di PNS handle, eseguito in prima istanza tra mobile app e la piattaforma di notifica specifica. Come descritto nella documentazione, non è garantita l'univocità.
    - Per i dispositivi Android, l'identificativo è l'ID di registrazione di Firebase (*onNewToken()*).
    - Per i dispositivi Apple, l'identificativo è il token del dispositivo.
    - Per i dispositivi Windows, l'identificativo è un URI che identifica il canale. 
- **Registration ID**: ID generato automaticamente da Azure per identificare una singola registrazione, l'utente non ha la responsabilità di doverlo generare.
- **Expiration date**: data di scadenza della registrazione in Azure. È impostata di default alla data 31/12/9999, in modo che la registrazione non possa mai scadere.

## Dati installazione dispositivo

Il termine *installazione* indica una registrazione più avanzata ed al momento della scrittura è il metodo più recente ed avanzato. 

L'installazione comprende le seguenti operazioni:
- associare il PNS handle per un dispositivo con tag ed, eventualmente, dei modelli di registrazione. Il PNS handle può essere recuperato solo a livello client.
- creare e aggiornare un'installazione in modo idempotente, evitando registrazioni duplicate.
- il modello di installazione supporta un formato di tag speciale ( *$InstallationId:{INSTALLATION_ID}* ) che consente l'invio di una notifica direttamente al device specifico.
- eseguire aggiornamenti parziali delle registrazioni. 

---

I seguenti dati corrispondono alla classe DeviceInstallation che si trova in TestNotification.Backend/Models:

- **InstallationId**: identifica un device specifico associato all'applicazione dalla quale è partito il processo di installazione. Il suo utilizzo è legato soprattutto alla cancellazione della registrazione dal hub di notifica.
    - In Android 8.0 e superiori, il codice ```Secure.GetString(Application.Context.ContentResolver, Secure.AndroidId)``` consente di generare una stringa di 64 bit, espressa come stringa esadecimale, ottenuta dalla combinazione di: chiave firmata della mobile app, utente e dispositivo.
    - Per le restanti versioni di Android, lo stesso codice consente di generare un stringa random di 64 bit, espressa come stringa esadecimale, che rimane unica nel ciclo di vita del dispositivo dell'utente.
  
  > Il recupero di questa informazione può avvenire solo lato app.
- **Platform**: identifica la piattaforma nella quale il dispositivo si registra. 

  > Il recupero di questa informazione può avvenire solo lato app.
- **PushChannel**: è il token recuperato dal PNS handle che identifica la registrazione di un dispositivo nella piattaforma di notifica apposita (associato al *PNS Identifier* visto sopra), chiamato anche **Registration ID** (attenzione: è diverso dal RegistrationID visto sopra!!).
  Questo parametro è strettamente legato a *Platform*, tanto che ci sono modi diversi per recuperare il token in base al sistema operativo del dispositivo.

  > Il recupero di questa informazione avviene lato app, va ancora studiato un modo per capire se è possibile recuperarla tramite back end, in modo da salvaguardare la sicurezza dell'utente.  

- **Tags**: array di etichette che identificano una serie di categorie alla quale l'utente appartiene (es. se il contesto dell'applicazione è lo sport, l'utente che tifa X e segue anche la squadra Y riceverà le notifiche sia della squadra X che della squadra Y).
  Nel caso d'uso specifico dell'applicazione *Hunext Mobile*, il tag specifico da utilizzare è il GUID utente che viene recuperato dal layer di persistenza del server aziendale. In questo modo, una notifica può essere indirizzata a specifici utenti.
  Il numero di tag possibili va da 0 a 10.

  > Per questioni di sicurezza e di elaborazione, il recupero di questa informazione avviene lato back end. In questo modo, l'utente è svincolato da ogni responsabilità e non ha libertà di scelta, libertà che è concessa solamente al back end.

<div align="right">

[Torna su](#descrizione-del-funzionamento-delle-piattaforme-per-la-gestione-delle-push-notification-per-android-ed-ios)
</div>