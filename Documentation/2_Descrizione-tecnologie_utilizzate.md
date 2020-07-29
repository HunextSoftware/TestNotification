# DESCRIZIONE DELLE TECNOLOGIE UTILIZZATE

In questo documento si descrive lo stack tecnologico utilizzato durante lo sviluppo del POC. Per compatibilità con gli strumenti aziendali, tutto il dominio 
tecnologico appartiene al mondo Microsoft.

Il documento è strutturato nelle seguenti sezioni:

- [Ambiente di sviluppo](#ambiente-di-sviluppo)
- [Linguaggi e framework utilizzati](#linguaggi-e-framework-utilizzati)
- [Piattaforme di notifiche push](#piattaforme-di-notifiche-push)
- [Database](#database)
- [Altri strumenti](#altri-strumenti)

---

## Ambiente di sviluppo

- **Visual Studio 2019**: IDE, o ambiente di sviluppo integrato, utilizzato per l'intera durata del progetto che contiene una vasta gamma di librerie
ed estensioni per facilitare la programmazione sia lato frontend che backend.

[Torna su](#descrizione-delle-tecnologie-utilizzate)

---

## Linguaggi e framework utilizzati

- **C#**: linguaggio di programmazione utilizzato in tutte le piattaforme utilizzate nell'ambito del progetto.

- **Xamarin**: framework open-source per lo sviluppo di mobile application multipiattaforma (per Android, iOS e Windows Phone). Xamarin permette di scrivere codice
comune a tutti i sistemi operativi, per poi specializzarsi nelle sottocartelle apposite. *Hunext Mobile* è indirizzata a dispositivi Android e iOS, quindi il
progetto ha due sottocartelle .Droid e .iOS rispettivamente per dispositivi Android e per dispositivi Apple. L'attenzione di questo progetto si è focalizzata su
Android in quanto l'account Apple Developer è a pagamento. 

- **ASP.NET Core**: framework open-source multipiattaforma, successore di ASP.NET, per la realizzazione di siti Web, applicazioni e database. Nell'ambito di questo progetto è stato
utilizzato per la realizzazione del back end, ovvero la componente d’integrazione tra la piattaforma cloud di push notification e la mobile application in grado di gestire tutte le richieste 
di registrazione/de-registrazione dei dispositivi da inoltrare all'hub di notifica di Azure e "costruire" la notifica per poi spedirla ai PNS (Platform Notification System) specifici.

- **ASP.NET Core Razor Page**: framework open-source specifico per la creazione di siti web multipiattaforma. Abbina il linguaggio C# per la programmazione server-side
con i linguaggi statici e dinamici per la programmazione web. Nell'ambito di questo progetto è stato utilizzato per la creazione di una web application in grado di inoltrare
notifiche personalizzate, in modo da sostituire la console di [Postman](https://www.postman.com/). 

[Torna su](#descrizione-delle-tecnologie-utilizzate)

---

## Piattaforme di notifiche push

- **Microsoft Azure**:

  - **Azure Hub Notification**:
  - **App Service**: condivisione codice backend in modo da renderlo accessibile pubblicamente

- **Firebase**:

- **APN**: non utilizzato

[Torna su](#descrizione-delle-tecnologie-utilizzate)

---

## Database

- **LiteDB**: database NoSQL, e quindi non relazionale, utilizzato lato backend per la memorizzazione dei dati utente. La sua particolarità è che non serve un'effettiva implementazione
in quanto è embedded, ed è particolarmente utile ed efficiente in progetti piccoli in .NET. Il suo utilizzo serve per simulare l'autenticazione degli utenti alla mobile application 
TestNotification e memorizzare dati utili, corrispondenti ai **tag**, per l'invio di notifiche mirate.

- **LiteDB.Studio**: applicativo che consente di visualizzare tutti i dati contenuti da un database selezionato. 
Download disponibile al seguente [link](https://github.com/mbdavid/LiteDB.Studio).

[Torna su](#descrizione-delle-tecnologie-utilizzate)

---

## Altri strumenti

- **Postman**: applicativo utilizzato per due terzi del progetto (prima dell'effettivo sviluppo della web application) e specifico per l'invio di richieste HTTP(S) verso il server locale
oppure al server remoto di Azure che "ospita" il codice backend. Utile soprattutto per le attività di test.

- **GitHub Desktop**: applicativo utilizzato per la gestione locale e remota del codice sorgente mediante l'utilizzo del VCS (Version Control System) Git. 
Download disponibile al seguente [link](https://desktop.github.com/).

[Torna su](#descrizione-delle-tecnologie-utilizzate)

