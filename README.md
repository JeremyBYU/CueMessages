# CUE Messages - Corsair RGB Keyboard Messages
## Summary
An application written in C# that allows you to diplay messages on an RGB keyboard from an external source.  Utilizes the Corsair Cue SDK through [CUE.net](https://github.com/DarthAffe/CUE.NET).

![alt tag](https://raw.github.com/JeremyBYU/CueMessages/master/pics/cuemessage.gif)

This is a command line application.  Simply call the program in a command shell and provide the url to retrieve messages. The URL provide a JSON object of this form
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
