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
### Installation
There are 2 ways to install

1. Build from this source with Visual Studio Desktop 2015 (free)
2. Unzip from dist/CueMessages.zip and run CueMessage.exe

### Command Line Arguments

```
[Option('u', "url", Required = true,
  HelpText = "URL to query for messages")]

[Option('b', "background", DefaultValue = "Black",
  HelpText = "Background Color of the corsair keyboard")]

[Option("color1", DefaultValue = "White",
HelpText = "Color of Leds while typing")]

[Option("color2", DefaultValue = "Blue",
HelpText = "Second Color of Leds while typing")]

[Option('w', "waitTime", DefaultValue = 10,
HelpText = "Seconds to wait for each JSON API Call to URL for messages")]

[Option('t',"typingTime", DefaultValue = 1000,
HelpText = "Milliseconds between each key in message")]
```



### Example Website

A website is provided that you can record your private messages for which you can then connect to your corsair keyboard. Though you can make private lists which only you should be able to see, please take care to not record any sensitive information. The website is called [CueMessages](http://cue-messages.meteor.com/lists/9p3pRYB4J4WG6zarf)

#### Setup
1. Go to http://cue-messages.meteor.com/
2. Click Join. Use any username or password.  E-mails don't even have to be real.
3. Create a new list, name it, and make it private with the lock button.
4. Get the alphanumeric LIST-ID from url (e.g. 9p3pRYB4J4WG6zarf)
5. The url for your private messages is now: http://cue-messages.meteor.com/lists-api/YOUR-LIST-ID. Notice it is **lists-api**
6. Now simply start cue ```CueMessage.exe -u "YOUR-URL" ```

Here's a gif on how to make a new list (after signing in), make it private, and get the list alphanumeric.

![alt tag](https://raw.github.com/JeremyBYU/CueMessages/master/pics/private-list.gif)
