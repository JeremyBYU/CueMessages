
using System;
using System.Threading;
using System.Threading.Tasks;
using CUE.NET;
using CUE.NET.Brushes;
using CUE.NET.Devices.Generic.Enums;
using CUE.NET.Devices.Keyboard;
using CUE.NET.Devices.Keyboard.Enums;
using CUE.NET.Devices.Keyboard.Extensions;
using CUE.NET.Devices.Keyboard.Keys;
using CUE.NET.Exceptions;
using System.Drawing;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using System.Linq;
using CommandLine;
using CommandLine.Text;

namespace CueMessages
{
    class Options
    {
        [Option('u', "url", Required = true,
          HelpText = "URL to query for messages")]
        public string URL { get; set; }

        [Option('b', "background", DefaultValue = "Black",
          HelpText = "Background Color of the corsair keyboard")]
        public string BackgroundColor { get; set; }

        [Option("color1", DefaultValue = "White",
        HelpText = "Color of Leds while typing")]
        public string Color1 { get; set; }

        [Option("color2", DefaultValue = "Blue",
        HelpText = "Second Color of Leds while typing")]
        public string Color2 { get; set; }

        [Option('w', "waitTime", DefaultValue = 10,
        HelpText = "Seconds to wait for each JSON API Call to URL for messages")]
        public int WaitTime { get; set; }

        [Option('t',"typingTime", DefaultValue = 1000,
        HelpText = "Milliseconds between each key in message")]
        public int TypingTime { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this,
              (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
    class Program
    {
        public struct MessageKey
        {
            public string key;
            public int index;
            public MessageKey(string p1, int p2)
            {
                key = p1;
                index = p2;
            }
        }
        public class Message
        {
            public string _id { get; set; }
            public string listId { get; set; }
            public string text { get; set; }
            public bool check { get; set; }
            public DateTime createdAt { get; set; }
        }
        public class MessageList
        {
            public List<Message> todos {get; set;}
        }
        public static void Main(string[] args)
        {
            Console.WriteLine("Press any key to exit ...");
            Console.WriteLine();
            Task.Factory.StartNew(
                () =>
                {
                    Console.ReadKey();
                    Environment.Exit(0);
                });

            Options options = new Options();
            if (CommandLine.Parser.Default.ParseArguments(args, options))
            {
                // Configuration Parameters
                int waitTime = options.WaitTime; // Seconds to wait between checking for more messages through JSON API call
                int oldMessagesTime = 24; // Hours until messages are to be removed from records.  Once removed any messages with same _id will be added.
                int typingTime = options.TypingTime; //Msec to wait to type each character
                Color color1 = Color.FromName(options.Color1);
                Color color2 = Color.FromName(options.Color2);
                Color background = Color.FromName(options.BackgroundColor);
                string url = options.URL;

                // Will hold the message database received from the JSON API
                List<Message> currentMessages = new List<Message>();             
                try
                {
                    // Initialize CUE-SDK
                    CueSDK.Initialize();
                    Console.WriteLine("Initialized with " + CueSDK.LoadedArchitecture + "-SDK");

                    // Get connected keyboard or throw exception if there is no light controllable keyboard connected
                    CorsairKeyboard keyboard = CueSDK.KeyboardSDK;
                    if (keyboard == null)
                        throw new WrapperException("No keyboard found");
                    keyboard.Brush = new SolidColorBrush(background);

                    while (true)
                    {
                        // Retrieve JSON Respone                    
                        string jsonResponse = GET(url);
                        MessageList deserializedProduct = JsonConvert.DeserializeObject<MessageList>(jsonResponse);
                        if (deserializedProduct != null && deserializedProduct.todos != null)
                        {
                            addMessages(currentMessages, deserializedProduct.todos, oldMessagesTime);

                            //Iterate over currentMessages and write Sentences
                            writeSentences(keyboard, currentMessages.Where(x => x.check == false).ToList(), color1, color2, typingTime);
                            currentMessages.All(x => { x.check = true; return true; });
                        }

                        Wait(waitTime * 1000);

                    }


                }
                catch (CUEException ex)
                {
                    Console.WriteLine("CUE Exception! ErrorCode: " + Enum.GetName(typeof(CorsairError), ex.Error));
                }
                catch (WrapperException ex)
                {
                    Console.WriteLine("Wrapper Exception! Message:" + ex.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception! Message:" + ex.Message);
                }

                while (true)
                    Thread.Sleep(1000); // Don't exit after exception

            }
            else
            {
                Console.WriteLine("Incorrect Parameters!");
                while (true)
                    Thread.Sleep(1000); // Don't exit after exception

            }

        }

        private static void addMessages(List<Message> inMemoryMessages, List<Message> apiNewMessages,int oldMessageTime)
        {
            //throw new NotImplementedException();
            List<Message> newList = new List<Message>();
            // Remove Messages that are to older than oldMessagesTime (Defualt 1 day)
            apiNewMessages.RemoveAll(p => TimeSpan.Compare(DateTime.Now.ToUniversalTime() - p.createdAt, new TimeSpan(oldMessageTime, 0, 0)) >= 1);
            inMemoryMessages.RemoveAll(p => TimeSpan.Compare(DateTime.Now.ToUniversalTime() - p.createdAt , new TimeSpan(oldMessageTime, 0,0)) >= 1);

            // Remove any apiNewMessages that already exist in inMemoryMessages
            apiNewMessages.RemoveAll(p => idExists(p._id, inMemoryMessages));
            inMemoryMessages.AddRange(apiNewMessages);     
        }
        private static bool idExists(string _id, List<Message> inMemoryMessages)
        {
            var act = inMemoryMessages.SingleOrDefault(a => a._id == _id);
            if (act != null)
                return true;
            return false;
        }

        private static void Wait(int msec)
        {
            Thread.Sleep(msec);
        }
        private static CorsairKeyboardKeyId GetKeyboardID(string key)
        {
            return (CorsairKeyboardKeyId)System.Enum.Parse(typeof(CorsairKeyboardKeyId), key);
        }
        private static void writeSentences(CorsairKeyboard keyboard, List<Message> messages, Color color1, Color color2, int typingTime)
        {
            foreach(Message message in messages)
            {
                writeSentence(keyboard, message.text, color1, color2, typingTime);
                Wait(3000);
            }

        }
        // Crazy long function to print a string
        private static void writeSentence(CorsairKeyboard keyboard, string message,Color color1, Color color2, int typingTime)
        {

            ListKeyGroup keyGroup1 = new ListKeyGroup(keyboard);
            List<MessageKey> activeKeys1 = new List<MessageKey>();
            keyGroup1.Brush = new SolidColorBrush(color1);

            ListKeyGroup keyGroup2 = new ListKeyGroup(keyboard);
            List<MessageKey> activeKeys2 = new List<MessageKey>();
            keyGroup2.Brush = new SolidColorBrush(color2);

            int messageIndex = 0;
            foreach (char c in message)
            {
                Char upperChar = Char.ToUpper(c);
                string c1 = modifyChar(upperChar);
                
                CorsairKey newKey = keyboard[GetKeyboardID(c1)];
                // Change colors if repeating character
                // Check for space bar and characters. How to map charactes
                if (keyGroup1.ContainsKey(newKey))
                {
                    // Add this char to the secondary key group
                    activeKeys2.Add(new MessageKey(c1, messageIndex));
                    keyGroup2.AddKey(newKey);

                    // Remove Char from other keygroup list
                    activeKeys1.RemoveAll(s => s.key == c1);
                    keyGroup1.RemoveKey(newKey);
                }
                else
                {
                    // Add this char to the primary key group
                    activeKeys1.Add(new MessageKey(c1, messageIndex));
                    keyGroup1.AddKey(newKey);
                    if (keyGroup2.ContainsKey(newKey)){
                        // Remove Key from other keygroup list
                        activeKeys2.RemoveAll(s => s.key == c1);
                        keyGroup2.RemoveKey(newKey);
                    }
                }
               

                if(activeKeys1.Count > 0 && messageIndex - activeKeys1[0].index > 3)
                {
                    // Remove this key!
                    keyGroup1.RemoveKey(keyboard[GetKeyboardID(activeKeys1[0].key)]);
                    activeKeys1.RemoveAt(0);
                    
                }
                if (activeKeys2.Count > 0 && messageIndex - activeKeys2[0].index > 3)
                {
                    // Remove this key!
                    keyGroup2.RemoveKey(keyboard[GetKeyboardID(activeKeys2[0].key)]);
                    activeKeys2.RemoveAt(0);
                    
                }


                messageIndex++;
                keyboard.Update();
                Wait(typingTime);
            }
            keyGroup1.Detach();
            keyGroup2.Detach();
            
        }

        public static string modifyChar(char c)
        {
            string finalC = c.ToString(); 
            if(c == ' ')
            {
                finalC = "Space";
            }
            return finalC;
        }
        // Returns JSON string
        public static string GET(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            try
            {
                WebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    return reader.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                WebResponse errorResponse = ex.Response;
                using (Stream responseStream = errorResponse.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
                    String errorText = reader.ReadToEnd();
                    // log errorText
                }
                throw;
            }
        }

    }
}
