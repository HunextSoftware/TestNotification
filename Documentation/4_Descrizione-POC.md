<div align="center"> 
<img src="Images/_icon.png" alt="Immagine dell'icona"/>

# DESCRIZIONE DEL PROTOTIPO SOFTWARE SVILUPPATO
</div>

L'obiettivo richiesto da parte dell'azienda è implementare un prototipo *ex novo* (chiamato *TestNotification*) per l'implementazione del sistema di notifiche push per l'applicazione mobile *Hunext Mobile*.

Prima ancora dello sviluppo del prototipo, è stato necessario configurare le piattaforme di notifica apposite. Come anticipato nel documento *1_Descrizione-generale-progetto*, non è stata
considerata la configurazione della piattaforma Apple (APNS) e tanto meno lo sviluppo specifico in *TestNotification.iOS*, ma sarà fornita la dovuta documentazione per gli sviluppatori che si faranno 
carico di integrare il prototipo nelle applicazioni aziendali.

Il prototipo comprende esattamente tre componenti, delle quali due fondamentali ai fini della realizzazione del progetto:
- un backend scritto in ASP.NET Core, con l'obiettivo di gestire le installazioni dei dispositivi e le richieste di notifica, obbligatoria ai fini del progetto.
- un'applicazione mobile scritta in Xamarin.Forms, con l'obiettivo di ricevere notifiche mirate, obbligatoria ai fini del progetto.
- una web application scritta in ASP.NET Core Razor Pages, con l'obiettivo di inviare notifiche personalizzate. Non obbligatoria ai fini del progetto, ma molto utile in quanto esegue una specifica parte 
del lavoro che dovrebbe essere in gestione del backend.

La posizione delle tre componenti nell'elenco segue l'ordine cronologico di sviluppo. Da evidenziare che backend e applicazione mobile sono le uniche componenti che hanno avuto uno sviluppo parallelo, 
rappresentando quasi il 90% del tempo di sviluppo del prototipo. La web application è stata sviluppata dopo la conclusione di queste due componenti e ha rappresentato il restante 10%.

Il documento è strutturato nelle seguenti sezioni:
- [Configurazione di Firebase ed Azure Notification Hubs](#configurazione-di-firebase-ed-azure-notification-hubs)
- [Sviluppo backend](#sviluppo-backend)
- [Sviluppo applicazione mobile](#sviluppo-applicazione-mobile)
- [Sviluppo web application](#sviluppo-web-application)

---

## Configurazione di Firebase ed Azure Notification Hubs

Prima di iniziare, è necessario accertarsi di avere un account Google e una sottoscrizione in [Azure](https://portal.azure.com/). Una volta effettuati gli accertamenti del caso, il primo passo è inizializzare la configurazione di Firebase Cloud Messaging (FCM). 

Ecco la sequenza di azioni da effettuare:
1) accedere alla [console di Firebase](https://firebase.google.com/console/), cliccare su *Aggiungi progetto* e inserire il nome del progetto che deve essere univoco (in questo caso è *TestPushNotification*).
2) una volta effettuata la creazione del progetto, selezionare *Aggiungi Firebase alla tua app Android* e si aprirà una pagina con un form da compilare.
    - in *Nome pacchetto Android* inserire un nome per il pacchetto (in questo caso è *com.hunext.testnotification*) e clicca il bottone *Registra app*.
    - cliccare il bottone *Scarica google-services.json.* e copiarlo in locale: successivamente, verrà copiato in *TestNotification.Android*. Cliccare il bottone *Avanti*.
    - infine cliccare il bottone *Vai alla console*.
3) sulla tendina a sinistra, andare sulle impostazioni di *Panoramica del progetto* e selezionare la voce *Impostazioni progetto*.
    - passare alla scheda *Cloud Messaging* e cercare in *Credenziali di progetto* il valore di *Chiave server*. A questo punto, copiare il token e salvarlo in un file per utilizzarlo successivamente.

<div align="center">
<img src="Images/4_Document/Configuration/4.1)Create-Firebase-project.png" alt="Creazione progetto Firebase"/>
</div>

Una volta configurato le varie piattaforme, il passo successivo è creare un hub di notifica in Azure. 

Ecco la sequenza di azioni da effettuare:
1) accedere ad [Azure](https://portal.azure.com/).
2) dalla pagina principale, sotto la voce *Servizi di Azure* selezionare la voce *Crea una risorsa*. 
    - una volta che la pagina è aperta, digitare sulla barra di ricerca *Notification Hub*, selezionarlo e cliccare sul pulsante *Crea*.
3) inserire tutti i campi richiesti per creare correttamente il nuovo hub di notifica.
    - *Sottoscrizione*: scegliere la sottoscrizione di destinazione dall'elenco a discesa.
    - *Gruppo di risorse*: creare un nuovo gruppo di risorse o selezionarne uno esistente.
    - *Spazio dei nomi di Hub di notifica*: inserire un nome univoco globale (__nuovo__) per lo spazio dei nomi, o *namespace*, di *Hub di notifica*.
    - *Hub di notifica*: inserire un nome per il nuovo hub di notifica.
    - *Località*: scegliere la zona desiderabile per localizzare il server dall'elenco a discesa.
    - *Piano tariffario*: scegliere il tipo di piano tra gratuito, basic e standard. In questo progetto è stato selezionata la voce *gratuito*.
4) confermare la creazione della risorsa.
5) dalla pagina principale, selezionare l'hub di notifica appena creato.
    - selezionare dal menu della risorsa la voce *Access Policies*.
        - salvare in un file le due *Connection String* inerenti alle *Policy Name*: *DefaultListenSharedAccessSignature* e *DefaultFullSharedAccessSignature*.
6) configurare l'hub di notifica con le informazioni di FCM.
    - selezionare dal menu della risorsa la voce *Google (GCM/FCM)*.
    - inserire la *Chiave server* che è stata copiata nell'ultimo passo nella console di Firebase.
    - cliccare il pulsante *Save*.

<div align="center">
<img src="Images/4_Document/Configuration/4.2)Create-notification-hub.png" alt="Creazione hub di notifica in Azure"/>
</div>
<p></p>

> Per configurare Apple Push Notification Services (APNS) e l'hub di notifica in Azure con le informazioni di APNS, visitare la seguente [sezione](https://docs.microsoft.com/it-it/azure/developer/mobile-apps/notification-hubs-backend-service-xamarin-forms#set-up-push-notification-services-and-azure-notification-hub).

A questo punto, l'hub di notifica e i PNS sono configurati correttamente e possono svolgere la loro funzione.
Il prossimo passo è lo sviluppo del codice, a partire dal backend.

<div align="right">

[Torna su](#descrizione-del-prototipo-software-sviluppato)
</div>

---

## Sviluppo backend

Il progetto *TestNotification.Backend* è stato sviluppato a partire da un progetto ASP.NET Core Web API, con il framework .NET Core 3.1, supportando il design pattern della
*Dependency Injection (DI)*.

Il backend è una parte fondamentale di questo progetto in quanto gestisce le chiamate per la gestione delle installazioni dei dispositivi e le richieste di
invio notifiche destinate all'hub di notifica di Azure, che a sua volta gestirà autonomamente l'invio delle notifiche ai PNS di riferimento.

### Le parti principali del codice

Il primo passaggio essenziale per lo sviluppatore è inserire valori di configurazione localmente utilizzando lo strumento *Secret Manager*. 
Aprire il terminale ed inserire i seguenti comandi:

```
dotnet user-secrets init
dotnet user-secrets set "NotificationHub:Name" <value>
dotnet user-secrets set "NotificationHub:ConnectionString" <value>
```

Il placeholder *\<value\>* va rimpiazzato in questo modo:
- **NotificationHub:Name** è la voce *Name** che si trova in *Informazioni di base* nella pagina principale dell'hub di notifica appena creato.
- **NotificationHub:ConnectionString** è il valore *DefaultFullSharedAccessSignature* copiato nel passaggio 5) della creazione dell'hub di notifica.

Il passaggio successivo è aggiungere le dipendenze necessarie al backend dal servizio *NuGet* di Visual Studio:
- *Microsoft.Azure.NotificationHubs*, la libreria che permette di effettuare specifiche chiamate ad Azure Notification Hubs.
- *LiteDB*, la libreria che permette di creare e gestire un database locale NoSQL. 

Da questo momento, è importante analizzare le tappe cruciali per la realizzazione del backend.
Il progetto è strutturato in questa sequenza:
- *Properties*, la cartella che contiene tutti i parametri per avviare il backend e i file contenenti un riferimento con lo strumento *Secret Manager*.
- *Controllers*, la cartella che contiene le classi responsabili della logica di controllo, in particolare
    - in *LoginController* viene gestito un unico endpoint per l'accesso autenticato degli utenti dall'applicazione mobile.
    - in *NotificationsController* vengono gestiti gli endpoint per l'installazione dei dispositivi, la cancellazione della medesima e l'invio delle notifiche.
- *Models*, la cartella che contiene tutte le classi con la logica di business e di convalida.
- *Services*, la cartella che contiene tutti i servizi che vengono richiamati dal controller, responsabili delle chiamate alle librerie di progetto.

Prima di spiegare le parti più importanti e significative del codice, va fatto un piccolo appunto sul layer di persistenza del backend.
L'utilizzo di LiteDB è fondamentale per simulare le dinamiche del backend aziendale, dove molte informazioni vengono carpite dal database aziendale.
Nel caso di questo progetto il database non ha bisogno di implementazione (essendo NoSQL) ed è talmente leggero che non va a gravare sulle prestazioni del backend.
Viene utilizzato prevalentemente per due motivi:
- per l'autenticazione degli utenti dall'applicazione mobile nella procedura di login.
- una volta che l'utente è autorizzato, si vogliono ottenere dati utili per l'installazione del dispositivo e per la visualizzazione di dati utente nella parte autorizzata 
dell'applicazione mobile.


Ora che è stata fatta chiarezza sull'utilizzo del database, si può passare l'attenzione alle classi più importanti del backend.

**1) DeviceInstallation.cs**

Questa classe contiene i dati, tutti obbligatori, che sono gestiti e devono essere spediti dall'applicazione mobile dopo il recupero del PNS handle:
- **InstallationId**.
- **Platform**.
- **PushChannel**.

> Per capire il significato di questi dati, andare a vedere la sezione *Approfondimento sul processo di installazione del dispositivo* nel documento *3_Descrizione-funzionamento-piattaforme-notifiche-push*.

Un altro dato che servirà nel processo di installazione del dispositivo è **Tags**, un vettore nel quale verrà salvato il GUID dell'utente che ha effettuato il login, in modo da 
poter indirizzare le richieste ad un utente specifico come richiesto dal tutor aziendale. Lo stesso dato viene utilizzato per essere inserito nell'header delle richieste HTTP(S) 
per certificare che le richieste effettuate dall'applicazione mobile siano autenticate.

> **Tags** verrà inserito nella classe *NotificationsController* all'endpoint PUT *~/installations*, dopo aver fatto la verifica che la chiamata sia autorizzata.

**2) PushTemplates.cs**

Questa classe contiene i payload di notifica nel formato stringa (pronto per essere serializzato in formato JSON) sia per dispositivi Android che per iOS. 
All'interno sono presenti due classi interne:
- *Generic*, che rappresenta la notifica classica che viene ricevuta da tutti i dispositivi. È l'unica classe utilizzata in questo prototipo.
- *Silent*, che rappresenta una notifica silenziosa, ovvero non invasiva a livello visivo in quanto presente solo nel caso in cui l'utente scrolli la tendina delle notifiche. Solitamente è utilizzata per
segnalare servizi attivi in background, come per esempio la navigazione su Google Maps. Non è utile ai fini del progetto, ma se nel futuro l'azienda volesse utilizzare questo tipo di notifica
l'implementazione sarebbe quasi immediata.

*Payload generico Android:*
```
{ 
    "notification": 
    { 
        "title" : "TestNotification", 
        "body" : "$(alertMessage)"
    }
}"
```

*Payload generico iOS:*
```
{ 
    "aps" : 
    {
        "alert" : "$(alertMessage)"
    } 
}
```

Il placeholder *$(alertMessage)* verrà sostituito in *NotificatioHubsService.cs* con il metodo seguente:
```
string PrepareNotificationPayload(string template, string text) => template
    .Replace("$(alertMessage)", text, StringComparison.InvariantCulture); 
```

Utilizzando questo payload non viene registrato alcun modello di notifica, in quanto quello utilizzato è un modello che, nel gergo tecnico, viene chiamato *nativo*.
Per l'installazione del dispositivo è comunque prevista la registrazione di modelli personalizzati, inserendo ulteriori chiavi come analizzato nel documento *3_Descrizione-funzionamento-piattaforme-notifiche-push*.

**3) NotificationRequest.cs**

Questa classe contiene i dati che vengono passati all'invio della notifica (nel caso di questo progetto dalla web application) e rimpiazzano i placeholder nei payload di 
notifica. Nello specifico *Text* va a sostituire $(alertMessage), mentre *Tags* viene elaborato e poi inserito come parametro nella chiamata all'API di Azure Notification Hubs 
che si occupa nello specifico di inviare notifiche con una specifica *tagExpression*.

> La classe *NotificationRequestTemplate.cs* non è stata implementata, ma sarà pronta all'uso nel momento in cui gli sviluppatori implementeranno la logica di invio delle notifiche dal backend anziché 
dalla web application.
 
**4) NotificationsController.cs**

Qui risiede il fulcro della Web API RESTful: infatti questa classe funge un ruolo fondamentale per la comunicazione e la gestione delle richieste tra i dispositivi e il backend stesso.
Grazie all'inserimento in cima alla classe dell'attributo *[ApiController]*, viene abilitato il routing delle richieste a specifici endpoint e le risposte HTTP sono automatiche.

Gli endpoint che possono essere accettati dal backend sono i seguenti:
- GET *~/api/notifications/users/all*: ritorna una risposta HTTP 200 con annesso oggetto che contiene tutti gli id utente con annesso username. Necessario per creare dinamicamente la tendina selezionabile della web 
application per l'invio di notifiche mirate ad utenti precisi.
- PUT *~/api/notifications/installations*: ritorna una risposta HTTP 200 con i tag del dispositivo se la creazione dell'installazione di un dispositivo o il suo conseguente aggiornamento avviene con successo.
- DELETE *~/api/notifications/installations/\{installationId}*: ritorna una risposta HTTP 200 se la cancellazione dell'installazione del dispositivo specificato dal parametro *installationId* avviene con 
successo. 
- POST *~/api/notifications/requests*: ritorna una risposta HTTP 200 se la richiesta di invio della notifica è andata a buon fine.

**5) NotificationHubsService.cs**

In questa classe sono presenti i metodi che sono stati richiamati dagli endpoint di *NotificationsController*. In particolare la logica è incentrata sulle operazioni da effettuare con Azure, infatti è
alto l'utilizzo dell'API di Azure Notification Hubs.

I metodi principali sono i seguenti:
- *CreateOrUpdateInstallationAsync(DeviceInstallation deviceInstallation, CancellationToken token, string[] tags)*: chiamato dal codice dell'endpoint PUT *~/api/notifications/installations*, viene prima 
creato l'oggetto *Installation* che contiene tutti i dati di *DeviceInstallation* e il vettore stringa *tags*, ed infine effettua una chiamata all'API con 
*_hub.CreateOrUpdateInstallationAsync(installation, token)* per creare oppure aggiornare l'installazione del dispositivo. Se la chiamata va a buon fine, il metodo ritorna il tag salvato nel processo di 
installazione (ovvero l'id utente) che servirà successivamente all'applicazione mobile per essere aggiunto nell'header di specifiche richieste HTTP ed essere gestito come "token" per l'autorizzazione.
- *DeleteInstallationByIdAsync(string installationId, CancellationToken token)*: chiamato dal codice dell'endpoint DELETE *~/api/notifications/installations/\{installationId}*, procede alla cancellazione di 
una specifica installazione (il parametro *installationId*) effettuando una chiamata all'API con * _hub.DeleteInstallationAsync(installationId, token)*. Ritorna true se la cancellazione è andata a buon fine.
- *RequestNotificationAsync(NotificationRequest notificationRequest, CancellationToken token)*: chiamato dal codice dell'endpoint POST *~/api/notifications/requests*, viene prima elaborato  l'oggetto 
*NotificationRequest* per preparare il payload di notifica, sia per Android che per iOS, e l'espressione logica di tag (*tagExpression*) per indirizzare la notifica a categorie di utenti specifici (in questo caso all'id
di uno specifico utente). 
    - Se non sono presenti tag viene richiamato il metodo *SendPlatformNotificationsAsync(androidPayload, iOSPayload, token)* che inoltra la notifica in modalità broadcast, 
    - altrimenti viene fissato un limite massimo di 10 tag (perché l'espressione logica contiene solo operatori logici AND (&&)) e viene richiamato il metodo 
    *SendPlatformNotificationsAsync(androidPayload, iOSPayload, tagExpression.ToString(), token)* per inviare notifiche mirate grazie all'inserimento del parametro *tagExpression()*.
    
    I seguenti metodi sono responsabili delle chiamate all'API come si può vedere dal seguente codice:
    ```
    Task SendPlatformNotificationsAsync(string androidPayload, string iOSPayload, CancellationToken token)
    {
        var sendTasks = new Task[]
        {
            _hub.SendFcmNativeNotificationAsync(androidPayload, token),
            _hub.SendAppleNativeNotificationAsync(iOSPayload, token)
        };

        return Task.WhenAll(sendTasks);
    }

    Task SendPlatformNotificationsAsync(string androidPayload, string iOSPayload, string tagExpression, CancellationToken token)
    {
        var sendTasks = new Task[]
        {
            _hub.SendFcmNativeNotificationAsync(androidPayload, tagExpression, token),
            _hub.SendAppleNativeNotificationAsync(iOSPayload, tagExpression, token)
        };

        return Task.WhenAll(sendTasks);
    }
    ```

    Il vantaggio che si trae da questi metodi è la gestione contemporanea dell'invio delle notifiche con un modello di notifiche nativo a più piattaforme, in questo caso specifico ad FCM e APNS.

**6) LoginController.cs**

Come *NotificationsController*, questa classe è abilitata a ricevere richieste da determinati endpoint grazie alla presenza dell'attributo *[ApiController]*.
Questa classe è stata creata appositamente per separare le operazioni che coinvolgono le chiamate all'API di Azure Notifications Hubs dalle operazioni che interessano l'autenticazione dell'utente.

In questo caso l'endpoint presente è solo uno:
- POST *~/login*: ritorna una risposta HTTP 200 con l'annesso oggetto di risposta *LoginResponse* se le credenziali contenute nel corpo della richiesta sono presenti all'interno del database di LiteDB.

**7) UserManagerService.cs**

In questa classe sono presenti metodi che, in base al loro scopo, vengono utilizzati sia da *LoginController* che da *NotificationsController*. In particolare vengono gestite tutte le operazioni con 
LiteDB.

I metodi principali sono i seguenti:
- *GetAllUsers()*: ritorna la lista di tutti gli utenti, in particolare gli id associati agli username. Viene utilizzato nel codice dell'endpoint GET *~/api/notifications/users/all*
- *GetUserByUsername(string username)*: ritorna un utente in base allo username passato come parametro del metodo. Viene utilizzato nel codice dell'endpoint POST *~/login*.
- *GetUserById(Guid id)*: ritorna un utente in base all'id passato come parametro del metodo. Viene utilizzato nel codice degli endpoint PUT *~/api/notifications/installations* e 
DELETE *~/api/notifications/installations/\{installationId}* per accertarsi che la richiesta HTTP sia autorizzata, verificando che nell'header sia presente l'id utente.
- *AuthenticateUser(string username, string password)*: ritorna true se le credenziali dell'utente sono presenti nel database. Viene utilizzato nel codice dell'endpoint POST *~/login*.

### Attività facoltative

#### Creazione dell'App Service su Azure

In questo progetto è risultato necessario creare un'API Application nel servizio di Azure *App Service* per ospitare il backend e averlo a disposizione in rete in qualsiasi momento.
Ovviamente ai fini dell'azienda questo passaggio è inutile in quanto il backend aziendale è già ospitato in altri servizi di hosting.

Nonostante questa premessa, si vuole mostrare quali misure ha adottato lo stagista per sopperire a questo problema.
Di seguito vengono elencati in sequenza i passaggi da effettuare per attivare questo servizio:
1) Se si è già registrati, accedere ad [Azure](https://portal.azure.com/).
2) Dalla pagina principale, sotto la voce *Servizi di Azure* selezionare la voce *Crea una risorsa*.
    - nella barra di ricerca digitare *API App*, selezionare il risultato ottenuto e cliccare il bottone *Crea*.
3) Inserire i dati nei rispettivi campi:
    - **Nome app**: inserire un nome univo che identifica l'API App.
    - **Sottoscrizione**: selezionare la stessa sottoscrizione scelta durante la creazione dell'hub di notifica.
    - **Gruppo di risorse**: selezionare lo stesso gruppo di risorse scelta durante la creazione dell'hub di notifica.
    - **Località**: scegliere la zona desiderabile per localizzare il server dall'elenco a discesa.
    - **Application Insights:**: lasciare le opzioni suggerite da Azure.
4) Una volta inserito i dati del punto 3), confermare la creazione del servizio ed entrare all'interno della risorsa.
5) Salvare in un file a parte l'URL *https://<app_name>.azurewebsites.net* che si trova in *Informazioni di base*, in quanto verrà utilizzato nell'applicazione mobile.
6) Selezionare dal menu della risorsa la voce *Configurazione*.
    - cliccare sul pulsante *Nuova impostazione applicazione* ed inserire le seguenti chiavi con gli annessi valori (salvati in precedenza):
        - *NotificationHub:Name*.
        - *NotificationHub:ConnectionString*.
    - cliccare sul bottone *Salva* e poi su *Continua*.

#### Pubblicare il backend

Questa attività è strettamente legata alla creazione dell'App Service su Azure descritta nel paragrafo precedente, e viene svolta dall'ambiente di sviluppo Visual Studio 2019.

Per distribuire il backend e renderlo pubblico su Azure, seguire in sequenza i seguenti passaggi:
1) cambiare la configurazione del progetto da *Debug* a *Release*.
2) fare click destro sul progetto *TestNotificationBackend*.
3) Selezionare la voce *Publish*,
    - selezionare *Azure* come target.
    - selezionare *Azure App Service (Windows)* come target specifico.
    - connettersi all'account di Azure se non è già stato fatto.
    - selezionare il gruppo di risorse alla quale appartiene l'App Service creata precedentemente.
    - infine selezionare il pulsante *Finish*.

#### Note a margine

L'utilizzo del backend può essere sia locale che remoto. Lo stagista ha preferito creare un App Service in modo da poter testare in una modo più fedele alla realtà il funzionamento dell'applicazione 
mobile e dell'intera infrastruttura. D'altro canto Azure non offre strumenti di debug del backend caricato, quindi l'utilizzo del backend in locale si è rivelato molto utile per poter fare debugging 
in profondità per rilevare e risolvere i problemi del codice backend.

Di default il backend funziona solo su Azure. Nel caso lo sviluppatore sia interessato a farlo funzionare in locale, cambiare le configurazioni nel file *Program.cs* e cambiare l'indirizzo del server 
nell'applicazione mobile.

<div align="right">

[Torna su](#descrizione-del-prototipo-software-sviluppato)
</div>

---

## Sviluppo applicazione mobile

Il progetto *TestNotification* è stato sviluppato a partire da un progetto Xamarin.Forms nella quale sono state selezionate le destinazioni Android e iOS.
A fronte di ciò, il progetto che viene generato è composto da tre parti:
- *TestNotification*, che contiene parte di codice condivisa e comune sia ad Android che iOS.
- *TestNotification.Android*, che contiene la parte specializzata di Android.
- *TestNotification.iOS*, che contiene la parte specializzata di iOS.

> La parte inerente a *TestNotification.iOS* non verrà modificata per i motivi citati nei precedenti documenti. All'azienda verrà fornita la relativa documentazione.

L'applicazione mobile è la parte obbligatoria da sviluppare nel frontend ed è il componente fondamentale per testare la ricezione di notifiche da parte della web application (nel contesto aziendale sarà
dal backend). Le notifiche sono push, quindi la visualizzazione avviene sia in primo piano (notifiche *heads-up*) che nel centro notifiche, e in qualsiasi stato si trovi il dispositivo, con la prerogativa
che esso sia connesso alla rete. In caso contrario, solo una volta che viene riattivata la rete allora il dispositivo sarà in grado di ricevere tutte le notifiche che sono rimaste accodate nei PNS di 
riferimento.

La struttura dell'applicazione mobile è divisa in due parti. 
Nella prima parte l'utente accede per la prima volta all'applicazione e deve inserire le proprie credenziali mediante l'apposito form di login. 
Se le credenziali sono corrette, a livello di codice avviene l'installazione del dispositivo nell'hub di notifica e poi si viene reindirizzati ad una pagina che mostra alcune informazioni personali 
ricavate dal database del backend. 
Questo passaggio è fondamentale ai fini della dimostrazione del funzionamento delle notifiche push, in quanto solo se l'utente è autenticato all'app è interessato a ricevere le notifiche, 
ed è ciò che avviene realmente.
Nella pagina è presente un pulsante di logout che ha il compito di disconnettere l'utente e cancellare l'installazione del dispositivo dall'hub di notifica. 
Alla fine di questo passaggio l'utente è reindirizzato alla pagina iniziale di login e viene dimostrato che a questo punto non vengono più recapitate notifiche al dispositivo.

### Le parti principali del codice TestNotification

Il primo passaggio necessario per il corretto funzionamento è aggiungere le dipendenze necessarie all'applicazione mobile dal servizio *NuGet* di Visual Studio:
- *Newtonsoft.Json*, la libreria che permette di gestire il codice JSON.

Il secondo passaggio è creare una classe *Config.cs* che ha il compito di mantenere tutti i valori segreti fuori dal controllo del codice sorgente
Il codice è il seguente:
```
public static partial class Config
{
    public static string BackendServiceEndpoint = "BACKEND_SERVICE_ENDPOINT";
}
```

Il valore *BACKEND_SERVICE_ENDPOINT* verrà sostituito da un'altra nuova classe *Config.local_secrets.cs* che non dovrà mai essere pubblicata in rete (controllare il file .gitignore) e quindi 
rimanere in locale.
Il codice è il seguente:
```
public static partial class Config
{
    static Config()
    {
        // Uncomment ONLY one BackendServiceEndpoint string to start back end

        // Use this string connection if the back end works on Web Service Azure, because it is loaded there 
        //BackendServiceEndpoint = "<your_api_app_url>";

        /* RECOMMENDED FOR DEBUG --> deactivate the firewall completely!
         * 
         * Use this string connection if your back end works on localhost 
         * http(s)://<your-local-ipv4-address>:<service-port>
         */
        //BackendServiceEndpoint = "http(s)://<your-local-ipv4-address>:<service-port>";
    }
}
```

Se il servizio backend è istanziato nell'App Service di Azure, allora de-commentare la riga *BackendServiceEndpoint = "<your_api_app_url>";* e sostituire il placeholder con l'URL dell'App Service 
salvata precedentemente su un file a parte.
Nel caso contrario in cui il servizio backend sia locale, de-commentare la riga *BackendServiceEndpoint = "http(s)://\<your-local-ipv4-address>:\<service-port>";* e sostituire il placeholder con
l'indirizzo locale del nodo nella quale il server è in funzione nella rete interna e la porta per accedere al servizio.

Da questo momento, è importante analizzare le tappe cruciali per la realizzazione dell'applicazione mobile.
Il progetto è strutturato in questa sequenza:
- *Models*, la cartella che contiene tutte le classi con la logica di business e di convalida.
- *Services*, la cartella che contiene tutte le classi che si occupano di costruire richieste HTTP da inviare al backend.
- *AuthorizedUserPage.xaml*, il file che contiene il codice inerente all'activity nella quale l'utente ha accesso solo dopo aver inserito correttamente le credenziali.
- *LoginPage.xaml*, il file che contiene il codice inerente l'activity principale con il form di login.

Ora l'attenzione passa sulla focalizzazione delle classi più significative di TestNotification.

**1) DeviceInstallation.cs**

Questa classe è analoga all'omonima classe presente nel codice backend, con l'unica differenza che ogni variabile è associata alla proprietà *JsonProperty*.

> Tutte le classi di *Models* associano la proprietà *JsonProperty* alle variabili interne, per la corretta serializzazione degli oggetti generati che verranno inseriti nelle chiamate HTTP.

Va evidenziato il fatto che in questa classe sono presenti solo le informazioni minime che devono essere gestite obbligatoriamente dal dispositivo.
La responsabilità di ricavare i tag appartiene al backend in quanto è meno vulnerabile rispetto ad un'applicazione mobile e rende le operazioni più veloci offrendo un'esperienza utente migliore.

**2) NotificationRegistrationService**

Questa classe contiene tutti i metodi che corrispondono alle richieste che il dispositivo può inoltrare al backend. In particolare, sono presenti i metodi per la gestione dell'installazione del 
dispositivo.

I metodi principali sono i seguenti:
- *RegisterDeviceAsync()*: si occupa di prelevare tutti i dati per l'installazione del dispositivo (che vengono recapitati da TestNotification.Android oppure TestNotification.Apple) per poi essere
inviati nella specifica richiesta HTTP. Se la procedura va a buon fine, il dispositivo riceverà come risposta i tag, che successivamente verranno salvati segretamente al pari del token che 
rappresenta il PNS handle.
- *RefreshRegistrationAsync()*: si occupa di aggiornare l'installazione del dispositivo se ancora presente nell'hub di notifica di Azure. Se il PNS handle e i tag sono presenti nella memoria segreta locale
e il token del PNS handle è diverso dal token appena recapitato dal PNS, allora viene richiamato nuovamente il metodo *RegisterDeviceAsync()*, altrimenti l'aggiornamento non viene effettuato in quanto
non necessario.
- *DeregisterDeviceAsync()*: si occupa di cancellare l'installazione del dispositivo. Se la procedura va a buon fine, il dispositivo cancellerà i segreti che erano stati salvati a partire dalla 
procedura di installazione.

Ogni chiamata HTTP viene costruita a partire dal metodo *SendAsync\<T\>(HttpMethod requestType, string requestUri, T obj)*, che ha il compito di inserire nell'header la chiave *User-Id* che corrisponde
all'id dell'utente che fa la richiesta, che a sua volta è anche il tag salvato nell'installazione del dispositivo nell'hub di notifica; inoltre viene costruito il *content* della richiesta che corrisponde
ai parametri che vengono ricevuti nei metodi corrispondenti agli endpoint descritti nel backend. Infine ritorna il codice della risposta HTTP.

Di seguito viene illustrato il codice:
```
private async Task<HttpResponseMessage> SendAsync<T>(HttpMethod requestType, string requestUri, T obj)
{
    if (!_client.DefaultRequestHeaders.Contains("User-Id"))
    {
        // Add token authentication on header HTTP(S) request
        var tokenAuthentication = await SecureStorage.GetAsync(App.TokenAuthenticationKey);
        _client.DefaultRequestHeaders.Add("User-Id", tokenAuthentication);
    }

    var request = new HttpRequestMessage(requestType, new Uri($"{_baseApiUrl}{requestUri}"));

    if (obj != null)
    {
        string serializedContent = null;
        await Task.Run(() => serializedContent = JsonConvert.SerializeObject(obj)).ConfigureAwait(false);
        if (serializedContent != null)
            request.Content = new StringContent(serializedContent, Encoding.UTF8, "application/json");
    }

    return await _client.SendAsync(request).ConfigureAwait(false);
}
```

**3) LoginService.cs**

Questa classe contiene l'unico metodo utile per inoltrare la richiesta di autenticazione al backend, che è il seguente:
- *Login(string username, string password)*: questo metodo si occupa di costruire la richiesta HTTP con il *content* che corrisponde all'oggetto *LoginRequest*. Una volta che la 
richiesta viene inoltrata, si aspetta la risposta che corrisponde all'oggetto *LoginResponse* che, in caso di esito positivo, verrà inoltrata al metodo 
*OnLoginButtonClicked(object sender, EventArgs e)* della classe *LoginPage.xaml.cs* che salverà nei segreti locali sia l'id dell'utente che i dati restanti che serviranno per la 
seconda activity.

**4) LoginPage.xaml.cs**

Questa classe contiene tutta la logica della pagina *LoginPage* che corrisponde alla prima activity che contiene il form di login.

> La grafica della pagina *LoginPage* viene codificata con il linguaggio XAML e risiede nel file *LoginPage.xaml*.

L'evento più significativo avviene quando viene premuto il pulsante di Login e avviene sia la procedura di login che quella di installazione del dispositivo.
Il procedimento è il seguente: se i campi del form sono stati compilati, allora viene richiamato il metodo *Login(usernameEntry.Text, passwordEntry.Text)*. 
Se tutto va a buon fine, allora viene salvato nei segreti locali l'id utente e viene richiamato il metodo della medesima classe *RegistrationDevice()*. 
A questo punto viene invocato in modo asincrono il metodo *RegisterDeviceAsync()* della classe *NotificationRegistrationService*: se il task va a buon fine allora l'utente verrà indirizzato 
nell'activity successiva *AuthorizedUserPage*.

> In caso di successo, l'ultima operazione è il salvataggio di *Username*, *Company* e *SectorCompany* nel segreto locale di *App.CachedDataAuthorizedUserKey*. Se ancora presenti localmente, questi dati 
vengono recuperati all'avvio dell'applicazione insieme all'activity *AuthorizedUserPage*. Questa operazione è fondamentale per salvare la sessione utente attuale, altrimenti l'applicazione ripartirebbe
da zero e richiederebbe un altro login, cosa che non deve succedere se un utente è già autenticato oppure non ha fatto richiesta di disconnessione dal servizio.
>
> Il codice è consultabile in *App.xaml.cs*.

Di seguito viene illustrato il codice:
```
async void OnLoginButtonClicked(object sender, EventArgs e)
{
    if (usernameEntry.Text != null && passwordEntry.Text != null)
    {
        loginButton.IsVisible = resetButton.IsVisible = false;
        loginActivityIndicator.IsRunning = true;
        try
        {
            var result = await _loginService.Login(usernameEntry.Text, passwordEntry.Text);

            // Save locally "token" authentication to save on every header HTTP request
            await SecureStorage.SetAsync(App.TokenAuthenticationKey, result.Id.ToString());
            RegistrationDevice();

            loginActivityIndicator.IsRunning = false;
            await Navigation.PushAsync(new AuthorizedUserPage(result.Username, result.Company, result.SectorCompany));
            loginButton.IsVisible = resetButton.IsVisible = true;
            Toast.MakeText(Android.App.Application.Context, "Successful login: device registered.", ToastLength.Short).Show();

            // This block needs to recover AuthorizedUserPage activity, when the app is closed but the user has logged in yet
            string[] userDataAuthorized = { result.Username, result.Company, result.SectorCompany };
            await SecureStorage.SetAsync(App.CachedDataAuthorizedUserKey, JsonConvert.SerializeObject(userDataAuthorized));
        }
        catch (Exception ex)
        {
            loginActivityIndicator.IsRunning = false;
            loginButton.IsVisible = resetButton.IsVisible = true;
            Console.WriteLine($"Exception: {ex.Message}");
            Toast.MakeText(Android.App.Application.Context, "Login error: inserted fields not right.", ToastLength.Long).Show();
        }
    }
    else
        Toast.MakeText(Android.App.Application.Context, "Please, complete all the fields.", ToastLength.Long).Show();
}

async void RegistrationDevice()
{
    await _notificationRegistrationService.RegisterDeviceAsync().ContinueWith(async (task)
                            =>
    {
        if (task.IsFaulted)
        {
            Console.WriteLine($"Exception: {task.Exception.Message}");
            await Navigation.PushAsync(new LoginPage());
            Toast.MakeText(Android.App.Application.Context, "Error during device registration: retry to log in.", ToastLength.Long).Show();
        }
        else
            Console.WriteLine("Device registered: now is available to receive push notification.");
    });
}
```

 
<div align="right"> 

[Torna su](#descrizione-del-prototipo-software-sviluppato)
</div>

---

## Sviluppo web application

TODO



### Come inviare una notifica con Postman

Lo stagista ha creato una web application che permette di inviare una notifica da un'interfaccia web semplice, presente nel progetto *TestNotificationWebApp*.

Prima di creare la web application si è cimentato nell'utilizzo di [Postman](https://www.postman.com/), un programma adatto per inoltrare richieste a determinati endpoint indipendentemente dalla posizione del server.

In seguito vengono illustrate le due modalità per l'utilizzo di Postman.

**PRIMA MODALITÁ**

La prima cosa da fare è creare una nuova richiesta, rispettando i seguenti passaggi:
- nella barra in alto, selezionare il metodo **POST** e inserire l'URL relativo all'inoltro della notifica https://serverpushnotification.azurewebsites.net/api/notifications/requests (oppure avviare il back end locale all'indirizzo http://localhost:5000/api/notifications/requests).
- selezionare la voce **Headers** presente nel tab, e:
    - Attivare la checkbox della key **Content-Type** e scrivere come value **application/json**.
- selezionare la voce **Body** presente nel tab, attivare il radio button **raw** e selezionare **JSON** nel menu a tendina. Nella text box sottostante, scrivere il corpo del messaggio di notifica:
```
{
    "text": "Hey man! How are things?"
    "tags": [
        "guid_user",
    ]
}
```
1) Il parametro **text** indica il testo che verrà inserito nella notifica. Il titolo è fissato di default come il nome dell'applicazione corrente.

2) Il parametro **tags** invece indica un insieme di valori che specificano quali device, registrati con i relativi tag, possono essere raggiunti. È possibile inserire 0 tag (che equivale a raggiungere tutti i dispositivi registrati in Azure Hub Notification),
e il numero massimo inseribile è di 10 tag. Questa limitazione è dovuta al fatto che la *tagExpression*, che viene costruita ed elaborata dal back end, è un'operazione logica di soli AND (&&). In merito a questa operazione, la documentazione è chiara, infatti utilizzando solo && 
è possibile inserire al massimo 10 tag.

> Nel caso d'uso specifico di Hunext, il tag da inserire è il GUID al quale si vuole inviare la notifica.
Va evidenziato che, dopo vari tentativi di test, non è possibile inserire più tag diversi all'interno del parametro **tags**.
La best practice da tenere è inviare una notifica per ogni utente, quindi N notifiche per N utenti.

- cliccare il bottone **Send** e inviare la notifica.

**SECONDA MODALITÁ**

Nel caso non si volesse preparare manualmente la richiesta, è possibile scaricare il file con alcune richieste di default che si trova, a partire dalla radice di questo repository, in *Archive/PostmanTestNotificationRequests*.

> In alternativa, è possibile scaricare il suddetto file tramite questo [link](https://drive.google.com/file/d/1oDxEGqBFsqdU1l6WnUaa6LrwBHqt1HEP/view?usp=sharing).

Successivamente, aprire l'applicativo Postman, cliccare il bottone **Import** presente in alto sotto il menu e selezionare il file appena scaricato.

Il file contiene due richieste HTTP avente lo stesso body ed indirizzate al tag dell'utente *Mario.Rossi*. La prima richiesta è indirizzata all'URL del back end locale che dev'essere avviato manualmente, mentre la seconda all'URL dello stesso back end ma caricato su Azure ed accessibile pubblicamente.

**Pre-condizione**: almeno un device deve essere autenticato come *Mario.Rossi*.

**Post-condizione**: una volta che la richiesta HTTP viene inoltrata, tutti i device che sono autenticati come *Mario.Rossi* ricevono una notifica.

#### Errore HTTP 422

Come descritto nella issue [#8](https://github.com/HunextSoftware/TestNotification/issues/8), l'invio della notifica comporta una risposta HTTP 422 (lo si può sia visualizzare da Postman che dalla console della web application). 

Questo errore semantico è dovuto al fatto che è stato configurata la piattaforma FCM ma non APNS, infatti il codice backend è già predisposto per l'invio di notifiche a dispositivi Apple.

È stato testato che, rimuovendo il codice specifico di TestNotification.Backend in Model/PushTemplates e Services/NotificationHubsService, l'invio della notifica comporta una risposta HTTP 200, con avvenuta ricezione della notifica.

Ergo, questo problema verrà risolto non appena verrà ultimata la configurazione dell'APN, con il codice backend che deve rimanere intatto.

<div align="right">

[Torna su](#descrizione-del-prototipo-software-sviluppato)
</div>

---
























## Apple

https://docs.microsoft.com/it-it/azure/notification-hubs/xamarin-notification-hubs-ios-push-notification-apns-get-started#generate-the-certificate-signing-request-file

## Mobile app

Casi d'uso limite:

- app disinstallata --> Infine, quando FCM tenta di recapitare un messaggio al dispositivo e l'app è stata disinstallata, FCM elimina immediatamente quel messaggio e annulla il token di registrazione.
I tentativi futuri di inviare un messaggio a quel dispositivo generano un NotRegisterederrore.
 
---





<div align="right">

[Torna su](#descrizione-del-prototipo-software-sviluppato)
</div>

---

> Per maggiori informazioni, visitare il seguente [link](https://docs.microsoft.com/it-it/azure/developer/mobile-apps/notification-hubs-backend-service-xamarin-forms).
