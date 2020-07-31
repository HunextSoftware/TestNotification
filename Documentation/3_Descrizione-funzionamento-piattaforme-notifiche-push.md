<div align="center"> 
<img src="Images/icon.png" alt="Immagine dell'icona"/>

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

Per poter usufruire del servizio di notifiche, ogni dispositivo deve obbligatoriamente registrarsi al PNS di riferimento, instaurando un collegamento diretto ad esso: questa procedura viene
chiamata nel gergo *PNS Handling*.

Purtroppo non esiste uno standard unico per la gestione, la configurazione e la comunicazione con i PNS, in quanto ogni ecosistema specifico offre strumenti che svolgono la stessa funzione
ma che lavorano in modo diverso. Proprio per questo motivo vanno analizzate nel dettaglio le due piattaforme di notifica principali, ovvero Firebase Cloud Messaging (FCM) per l'ecosistema 
Android e Apple Platform Notification System (APNS) per l'ecosistema Apple.

### Firebase Cloud Messaging (FCM)

Questo PNS consente di inviare notifiche principalmente per i dispositivi Android, ma non solo: infatti anche i dispositivi Apple e gli applicativi Web sono inclusi in questa cerchia.
Il payload del messaggio è formattato in JSON (uno standard per il trasferimento di dati in rete) e può avere la dimensione massima di 4 KB, con una set di parametri fisso e una parte 
dedicata alla personalizzazione della notifica.
I parametri fissi sono:

- *token*, che è una stringa alfanumerica generata da Firebase che identifica il collegamento unidirezionale da Firebase al dispositivo.
- *notification*, che contiene le informazioni base della notifica, ovvero:
    - *title*, ovvero il titolo che appare nella notifica.
    - *body*, ovvero il messaggio che appare nella notifica.

I parametri opzionali sono contenuti dentro la chiave *data*, che a sua volta contiene delle sotto-chiavi personalizzabili dall'utente. Ovviamente per questa evenienza c'è bisogno di salvare
un particolare modello.

### Apple Platform Notification System (APNS) 


[Torna su](#descrizione-del-funzionamento-delle-piattaforme-per-la-gestione-delle-push-notification-per-android-ed-ios)

---

## La soluzione individuata: Azure Notification Hubs

<div align="center"> 
<img src="Images/notification-hub-diagram.png" alt="Immagine processo Azure"/>
</div>

[Torna su](#descrizione-del-funzionamento-delle-piattaforme-per-la-gestione-delle-push-notification-per-android-ed-ios)












---

## Cosa si salva Azure Notification Hubs durante il processo di installazione del dispositivo

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
