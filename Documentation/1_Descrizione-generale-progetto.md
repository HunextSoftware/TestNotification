<div align="center"> 
<img src="Images/_icon.png" alt="Immagine dell'icona"/>

# DESCRIZIONE GENERALE DEL PROGETTO
</div>

Hunext Mobile è un'applicazione mobile multipiattaforma scritta in Xamarin che viene utilizzata dai dipendenti delle aziende che hanno aderito al sistema di Hunext per la gestione dei 
servizi HR (Human Resources) aziendali.

Nonostante le molteplici funzionalità offerte all'utente, all'applicazione manca un sistema per la ricezione di notifiche push.

La quasi totalità delle applicazioni mobile implementano le notifiche push, in quanto consentono di aggiornare l'utente in tempo reale su ciò che accade nell'applicazione in qualsiasi 
situazione, sia che esso stia navigando tra le pagine dell'applicazione sia che stia svolgendo altre attività al di fuori di Hunext Mobile.

A questo punto, l'obiettivo del progetto è chiaro: implementare un sistema di push notification per l’applicazione Hunext Mobile, in modo che un avviso importante oppure una scadenza particolare 
venga notificata __solo__ a determinati utenti.

A causa di molteplici fattori quali:
- l'elevata quantità di codice presente nel repository aziendale,
- il linguaggio C# da apprendere,
- la mancanza della conoscenza del dominio HR, 

lo stagista non sarà coinvolto direttamente nei processi aziendali e tanto meno non modificherà il codice aziendale esistente, ma partirà da un progetto vuoto che alla fine del periodo 
di stage rappresenterà il prototipo (o più nello specifico POC, acronimo di Proof Of Concept) dalla quale gli sviluppatori di Hunext partiranno per lo sviluppo di questa feature specifica. 

Il documento è strutturato nelle seguenti sezioni:

- [Studio dei vari servizi di push notification](#studio-dei-vari-servizi-di-push-notification)
- [Sviluppo del prototipo](#sviluppo-del-prototipo)

---

## Studio dei vari servizi di push notification

Prima di implementare il codice, è fondamentale studiare tutte le varie tipologie di piattaforme dedicate appositamente per le push notification.

Non esiste un sistema unico e standard, infatti ogni sistema, in particolare ogni sistema operativo, ha il suo modo di interfacciarsi con i propri dispositivi, e per dialogare dispone di 
sistemi di configurazione totalmente diversi.
Di seguito viene stilato un elenco delle piattaforme principali che vengono utilizzati maggiormente:

- FCM, acronimo di Firebase Cloud Messaging, che è la generazione successiva di GCM (Google Cloud Messaging), per tutti i dispositivi Google, iOS e Web.
- APN, acronimo di Apple Push Notification, per tutti i dispositivi Apple.
- WNS, acronimo di Windows push Notification Services, per tutti i dispositivi Windows.

Inoltre esiste un servizio di Microsoft "tuttofare", ovvero Azure Notification Hub, che funge da broker tra le piattaforme di push notification e i dispositivi che devono ricevere le 
notifiche, grazie ad una configurazione minimale delle varie piattaforme su Azure.

L'implementazione del servizio può avvenire scegliendo una delle seguenti soluzioni:

- implementare il servizio di push notification di ogni piattaforma (FCM, APN, WNS), senza l'ausilio di intermediari.
- implementare il servizio di push notification mediante un broker (come Azure) che gestisce la registrazione nascondendo tutti i dettagli di comunicazione con le piattaforme apposite.

<div align="right">

[Torna su](#descrizione-generale-del-progetto)
</div>

---

## Sviluppo del prototipo

Una volta che viene presa una decisione in merito alla piattaforma di push notification da utilizzare, si può iniziare a sviluppare il prototipo.

Il progetto si dirama in due parti di sviluppo che hanno funzioni differenti:
- la parte *frontend*, dove verrà creata un'applicazione mobile scritta in Xamarin.Forms che ha l'obiettivo di mostrare la ricezione di notifiche, solitamente mirate ad un particolare utente. 
Inoltre verrà creata una web application scritta in ASP.NET Core Razor Page che permette l'invio di notifiche personalizzate e la visualizzazione di tutti i dispositivi registrati nell'hub di notifica.
- la parte *backend*, dove verrà creato un server scritto in ASP.NET Core che fungerà da API Web REST, che servono per interagire in parte con l'applicazione mobile (p.e. per inviare i dati di registrazione di un dispositivo al backend, che poi si occuperà di elaborarli) e in parte con i Platform Notification System (PNS).

### Frontend

L'applicazione mobile è multipiattaforma ed è quindi disponibile per tutti i dispositivi Android (superiori alla versione 5.0) ed Apple.

Il prototipo sviluppato è incentrato per dispositivi Android, in quanto lo sviluppo per un ambiente Apple comporta alcuni vincoli stretti:

- un account Apple Developer a pagamento, della quale ne l'azienda ne lo stagista si fanno carico della spesa.
- una macchina Apple per provare l'emulatore iOS, che in azienda è presente ma sempre in utilizzo dagli altri sviluppatori.

Pertanto, in accordo con il tutor aziendale, l'unica attività possibile per dispositivi Apple sarà recuperare la documentazione necessaria per la configurazione di APN e l'implementazione del codice 
lato frontend.

> La struttura della directory dell'applicazione mobile è la seguente:
> 
> - *TestNotification*: qui è presente il codice globale che vale sia per Android che per Apple.
> - *TestNotification.Android*: qui è presente il codice specializzato per Android.
> - *TestNotification.iOS*: qui è presente il codice specializzato per Apple.

La web application è disponibile localmente ed è necessaria per visualizzare il corretto funzionamento del sistema di notifiche, infatti è stata progettata appositamente per inviare notifiche mirate ed personalizzate.
Va a sostituire uno strumento di test più tecnico quale è *Postman*, soprattutto per rendere più semplice e user-friendly l'interfaccia grafica.

> La web application è disponibile nella directory *TestNotificationWebApp*.

### Backend

Il server è necessario per gestire le operazioni CRUD (Create, Read, Update, Delete) tramite degli endpoint particolari che gestiscono le comunicazioni tra l'applicazione mobile e la piattaforma di push
notification.

È stato scritto in ASP.NET per una serie di motivi qui elencati:

- è il linguaggio utilizzato nel backend aziendale, il riutilizzo del codice sarebbe più immediato.
- l'azienda utilizza strumenti e tecnologie Microsoft, pertanto non avrebbe avuto senso scrivere il codice backend in Java (più pratico per lo stagista).
- è lo standard *de facto* nella documentazione di Microsoft.

> Il backend è disponibile nella directory *TestNotification.Backend*.

<div align="right">

[Torna su](#descrizione-generale-del-progetto)
</div>
