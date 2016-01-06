# CUE Messages - Corsair RGB Keyboard Messages
## Summary
An application written in C# that allows you to display messages on an RGB keyboard from an external source.  Utilizes the Corsair Cue SDK through [CUE.net](https://github.com/DarthAffe/CUE.NET).

![alt tag](https://raw.github.com/JeremyBYU/CueMessages/master/pics/cuemessage.gif)

### Details
This is a command line application.  Simply call the program in a command shell and provide the URL to retrieve messages. The URL provide a JSON object of this form
```
{
  "todos": [ //Must be an object named todos
    {
      "_id": "L9i76KNg6cnzQM2v7", // Any unique ID
      "listId": "JreTNhpDrX862tMnm", //A unique listID asscociated with this
      "text": "Message 1", // The text that will be displayed on they keyboard
      "checked": false, // Whether this message should be displayed.  If true, message is ignored.
      "createdAt": "2016-01-06T19:54:13.309Z"  // UTC time code when the message was created.
    }
  ]
}
```

### Example Website

A website is provided that you can record your private messages for which you can then connect to your corsair keyboard. Though you can make private lists which only you should be able to see, please take care to not record any sensitive information. The website is called [CueMessages](http://cue-messages.meteor.com/lists/9p3pRYB4J4WG6zarf)

#### Setup
1. Go to http://cue-messages.meteor.com/
2. Click Join. Use any username or password.  E-mails dont even have to be real.
3. Create a new list and name it.
4. Get the alphanumeric name of list from url (e.g. 9p3pRYB4J4WG6zarf)
5. The url for your private messages is now: http://cue-messages.meteor.com/lists-api/9p3pRYB4J4WG6zarf
6. Now simple start cue ```CueMessage.exe -u "YOUR-URL"
```

Heres a gif on how to make a new list (after signing in), make it private, and get the list alphanumeric.

![alt tag](https://raw.github.com/JeremyBYU/CueMessages/master/pics/private-list.gif)
