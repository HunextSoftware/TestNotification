# PANORAMICA STAGE HUNEXT

L'obiettivo del progetto è implementare un prototipo di push notification per l’app multipiattaforma Hunext Mobile.
Ecco i vari step:

1. Creare un progetto blank in Xamarin (Xamarin.Forms), da provare prevalentemente su Android (per provare l’emulatore iOS si dovrebbe far girare il sorgente in una macchina Apple), per capire il suo funzionamento e la sintassi del linguaggio C#.
-	Il package name da inserire è com.hunext, con nome dell’applicazione TestNotification. Questo serve ad identificare univocamente l’applicazione negli store Android ed Apple.
-	Xamarin ha una cartella Model, che contiene ciò che viene 

2.	Capire il funzionamento e la logica di configurazione dei vari servizi di push notification (dove i server registrano dei particolari identificativi dell’applicazione installata su un device) e identificare quale sia più conveniente tra:
-	Firebase (Android).
-	APN (Apple) -> più a livello conoscitivo.
-	Azure Notification Hubs -> sistema di Microsoft già integrato con Xamarin che gestisce e discrimina in modo astratto lo splitting tra notifiche a device Android e notifiche a device iOS (e quindi tra Firebase e APN).

3.	Implementare un sistema di notifica semplice sul proprio repository.

4.	Scoprire nel dettaglio quali sono i parametri da registrare per permettere alle applicazioni mobile di ricevere la notifica sul proprio dispositivo, ANCHE QUANDO L’APPLICAZIONE È CHIUSA.
-	tag device (deviceId??): identificherebbe il DISPOSITIVO (e non l’utente) nel server delle notifiche. Quindi ad un unico utente che ha N device (con N>= 1) deve poter ricevere le notifiche dell’app Hunext Mobile su tutti gli N device.
-	tag aziendale (tokenOrganization/organizationId??): identificherebbe il comparto aziendale specifico (Hunext HRSolutions è divisa in Hunext Payroll, Hunext Consulting e Hunext Software) al quale inviare la notifica.
-	altro.

5.	Implementare il prototipo in maggior dettaglio.

NB) Tutti gli account che verranno creati per registrarsi ai servizi di push notification saranno di proprietà dello stagista.
