# CUE Messages - Corsair RGB Keyboard Messages
## Summary
An application written in C# that allows you to diplay messages on an RGB keyboard from an external source.  Utilizes the Corsair Cue SDK through [CUE.net](https://github.com/DarthAffe/CUE.NET).

Watch it in action with this youtube video
<iframe width="1280" height="720" src="https://www.youtube.com/embed/LXlp59RdMIM" frameborder="0" allowfullscreen></iframe>

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
