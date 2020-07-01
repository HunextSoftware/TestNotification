# COME INVIARE UNA NOTIFICA

Lo stagista ha deciso di non creare una console per l'invio delle notifiche, bensì di utilizzare [Postman](https://www.postman.com/), un programma adatto per inoltrare richieste a determinati endpoint indipendentemente dalla posizione del server.
La prima cosa da fare è creare una nuova richiesta, rispettando i seguenti passaggi:
- nella barra in alto, selezionare il metodo **POST** e inserire l'URL relativo all'inoltro della notifica https://serverpushnotification.azurewebsites.net/api/notifications/requests.
- selezionare la voce **Headers** presente nel tab, e inserire Content-Type** nella colonna KEY e **application/json** nella colonna VALUE. Attivare la checkbox presente all'inizio della riga.
- selezionare la voce **Body** presente nel tab, attivare il radiobutton **raw** e selezionare **JSON** nel menu a tendina. Nella textbox sottostante, scrivere il corpo del messaggio di notifica:
```
{
    "text": "Hey man! How are things?",
    "action": "action_a",
    "tags": [
        "tag1",
        "tag2"
    ],
    "silent": true
}
```
I tag possono essere in un numero che va da 0 fino a 6.
Silent è un valore booleano che indica se la notifica deve essere visibile solo all'interno dell'app (true) oppure in tutti gli stati che passa l'applicazione (false).
- cliccare il bottone **Send** e inviare la notifica
