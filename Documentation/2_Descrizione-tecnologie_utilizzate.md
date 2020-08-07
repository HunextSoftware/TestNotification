<div align="center"> 
<img src="Images/_icon.png" alt="Immagine dell'icona"/>

# DESCRIZIONE DELLE TECNOLOGIE UTILIZZATE
</div>

In questo documento si descrive lo stack tecnologico utilizzato durante lo sviluppo del POC. Per compatibilità con gli strumenti aziendali, tutto il dominio 
tecnologico appartiene al mondo Microsoft.

Il documento è strutturato nelle seguenti sezioni:

- [Ambiente di sviluppo](#ambiente-di-sviluppo)
- [Linguaggi e framework utilizzati](#linguaggi-e-framework-utilizzati)
- [Piattaforme di notifiche push](#piattaforme-di-notifiche-push)
- [Database](#database)
- [Altri strumenti](#altri-strumenti)
- [Glossario](#glossario)

---

## Ambiente di sviluppo

- **Visual Studio 2019**: IDE, o ambiente di sviluppo integrato, utilizzato per l'intera durata del progetto che contiene una vasta gamma di librerie
ed estensioni per facilitare la programmazione sia lato frontend che backend.

<div align="right">

[Torna su](#descrizione-delle-tecnologie-utilizzate)
</div>

---

## Linguaggi e framework utilizzati

- **C#**: linguaggio di programmazione utilizzato in tutte le piattaforme utilizzate nell'ambito del progetto.

- **Xamarin**: framework open-source per lo sviluppo di mobile application multipiattaforma (per Android, iOS e Windows Phone). Xamarin permette di scrivere codice
comune a tutti i sistemi operativi, per poi specializzarsi nelle sottocartelle apposite. *Hunext Mobile* è indirizzata a dispositivi Android e iOS, quindi il
progetto ha due sottocartelle .Droid e .iOS rispettivamente per dispositivi Android e per dispositivi Apple. L'attenzione di questo progetto si è focalizzata su
Android in quanto l'account Apple Developer è a pagamento. 

- **ASP.NET Core**: framework open-source multipiattaforma, successore di ASP.NET, per la realizzazione di siti Web, applicazioni e database. Nell'ambito di questo progetto è stato
utilizzato per la realizzazione del backend, ovvero la componente d’integrazione tra la piattaforma cloud di push notification e la mobile application in grado di gestire tutte le registrazioni dei dispositivi da inoltrare all'hub di notifica di Azure ed elaborare le notifiche da inviare ai PNS specifici.

- **ASP.NET Core Razor Pages**: framework open-source specifico per la creazione di siti web multipiattaforma. Abbina il linguaggio C# per la programmazione server-side
con i linguaggi statici e dinamici per la programmazione web. Nell'ambito di questo progetto è stato utilizzato per la creazione di una web application in grado di inoltrare
notifiche personalizzate, in modo da sostituire la console di [Postman](https://www.postman.com/). 

<div align="right">

[Torna su](#descrizione-delle-tecnologie-utilizzate)
</div>

---

## Piattaforme di notifiche push

- **Microsoft Azure**: piattaforma cloud di Microsoft che eroga servizi cloud, gratuiti per l'utilizzo sperimentale dei medesimi oppure a pagamento per le esigenze aziendali.
Nel contesto del progetto, sono stati utilizzati due servizi gratuiti: 
  - **Notification Hubs**: servizio per l'invio di notifiche push a qualsiasi piattaforma PNS (come Android, iOS, Windows, Amazon, Baidu e Kindle) da qualsiasi backend, cloud o locale che sia. 
  Garantisce una scalabilità orizzontale e si pone come interlocutore tra il backend e i PNS gestendo le registrazioni dei dispositivi e l'inoltro delle notifiche. 
  Infine elabora informazioni base per la telemetria.
  - **App Service**: servizio multipiattaforma indicata per la creazione, la distribuzione e la gestione di web application. È servito nello specifico per caricare il codice backend scritto in ASP.NET Core in modo da renderlo accessibile in rete. Infine elabora informazioni base per la telemetria.
    > Il backend può funzionare allo stesso modo in locale, soprattutto per fare debugging.

- **Firebase Cloud Messaging (FCM)**: PNS che appartiene alla piattaforma Firebase dell'azienda Google indirizzato principalmente per dispositivi Android, le cui funzionalità sono estese anche a dispositivi iOS e Web. Precedentemente chiamato *Google Cloud Messaging* (GCM), è gratuito ed e utilizzato nell'ambito di questo progetto per l'invio di notifiche a dispositivi Android.

- **Apple Platform Notification System (APNS)**: PNS che appartiene all'azienda Apple indirizzato per dispositivi iOS. Non è stato utilizzato e tanto meno configurato a causa dei motivi elencati nel documento *1_Descrizione-generale-progetto*.

> Nel prossimo documento verrà approfondito il funzionamento di tali piattaforme e le ragioni per il quale è stata scelta una piattaforma anziché l'altra.

<div align="right">

[Torna su](#descrizione-delle-tecnologie-utilizzate)
</div>

---

## Database

- **LiteDB**: database NoSQL non relazionale per .NET utilizzato lato backend per la memorizzazione dei dati utente. LiteDB non ha bisogno di implementazioni in quanto è embedded, ed è particolarmente utile ed efficiente in progetti piccoli scritti in .NET. Il suo utilizzo è mirato alla simulazione dell'autenticazione degli utenti all'applicazione mobile *TestNotification* e memorizzare alcuni dati utili, corrispondenti ai **tag**, per l'invio di notifiche mirate.
Pacchetto per Visual Studio 2019 disponibile al seguente [link](https://www.nuget.org/packages/LiteDB/).

- **LiteDB.Studio**: applicativo che consente di visualizzare tutti i dati di uno specifico database. 
Download disponibile al seguente [link](https://github.com/mbdavid/LiteDB.Studio).

<div align="right">

[Torna su](#descrizione-delle-tecnologie-utilizzate)
</div>

---

## Altri strumenti

- **Postman**: applicativo utilizzato per due terzi del tempo di progetto, prima dell'effettivo sviluppo della web application, che è specifico per l'invio di richieste HTTP(S) verso il backend locale oppure al backend remoto caricato in Azure App Service. Utile per le attività di test inerenti all'invio delle notifiche.
Download disponibile al seguente [link](https://www.postman.com/downloads/).

- **GitHub Desktop**: applicativo utilizzato per la gestione locale e remota del codice sorgente mediante l'utilizzo del VCS Git. 
Download disponibile al seguente [link](https://desktop.github.com/).

<div align="right">

[Torna su](#descrizione-delle-tecnologie-utilizzate)
</div>

---
---

### Glossario

- **PNS**: acronimo di *Platform Notification System*, è un'infrastruttura specifica che permette di creare un handle con i dispositivi per inviare notifiche push
fino a quando il dispositivo è registrato al servizio. Per i dispositivi Android, l'invio delle notifiche è gestito da FCM, mentre per i dispositivi Apple da APNS.

- **VCS**: acronimo di *Version Control System*, è un software di controllo di versione che permette di tenere traccia delle modifiche e delle versioni apportate al codice sorgente del software.

<div align="right">

[Torna su](#descrizione-delle-tecnologie-utilizzate)
</div>
