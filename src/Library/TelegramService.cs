using TelegramApi;
using System;
using System.Linq;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using System.Collections.Generic;
using DocumentFormat.OpenXml;

namespace Library
{
    /// <summary>
    /// TelegramService: Clase que implmenta la interfaz IBot, encargada de incorporar el servicio de telegram.
    ///
    /// Principios y patrones:
    /// SRP: Cumple el principio al tener solo una responsabilidad, proporcionar la interaccion con Telegram Service.
    /// Expert: Cumple el patron al ser experto en la informacion que utiliza.
    /// <summary>
    public class TelegramService : IBot
    {
        //chatUpd: Diccionario de "Id del chat:Lista de mensajes" para saber que mensajes corresponden a qué chat.
        private Dictionary<Int64, List<Message>> chatUpd = new Dictionary<Int64, List<Message>>();

        //activeChats: Lista de ids de chats para saber cuales estan activos
        //(y evitar ejecutar MessageSwitch más de una vez por chat).
        private List<int> activeChats = new List<int>();
        
        //bot: El cliente de Telegram.
        private ITelegramBotClient bot;

        //message: El mensaje recibido. Se mantiene sin cambios dentro del thread.
        [ThreadStatic]
        private Message message;

        //senderChatId: Atributo que devuelve el id del chat del cual se recibió un mensaje.
        private int senderChatId
        {
            get
            {
                return Convert.ToInt32(message.Chat.Id);
            }
        }

        private string senderName
        {
            get
            {
                return message.Chat.FirstName;
            }
        }

        public TelegramService()
        {
            //Obtengo una instancia de TelegramBot
            TelegramBot telegramBot = TelegramBot.Instance;
            Console.WriteLine($"Hola soy el Bot de P2, mi nombre es {telegramBot.BotName} y tengo el Identificador {telegramBot.BotId}");

            //Obtengo el cliente de Telegram
            bot = telegramBot.Client;

            //Asigno un gestor de mensajes
            bot.OnMessage += OnMessage;
        }

        public void Start()
        {
            //Inicio la escucha de mensajes
            bot.StartReceiving();

            //Espero a leer texto introducido en consola
            Console.WriteLine("Presiona una tecla para terminar");
            Console.ReadKey();

            //Detengo la escucha de mensajes
            bot.StopReceiving();
        }

        //SendMessage: Procedimiento utilizado para enviar mensajes.
        public async void SendMessage(string text, int chatId)
        {
            await bot.SendTextMessageAsync( chatId: chatId,
                                            text: text);
        }

        //SendMessage: Procedimiento utilizado para enviar mensajes.
        public void SendDocument(string path, int chatId, string text = null)
        {
            System.IO.Stream file = null;
            while(file == null)
            {
                try
                {
                    file = System.IO.File.Open(path, System.IO.FileMode.Open);
                }
                catch(System.IO.IOException)
                {}
            }
            bot.SendDocumentAsync( chatId: chatId,
                                   new Telegram.Bot.Types.InputFiles.InputOnlineFile(file, path),
                                   caption: text);
            file.Close();
            Thread.Sleep(300);
        }

        //ReadMessage: Procedimiento utilizado para leer el texto recibido en el mensaje.
        public string ReadMessage(int chatId)
        {
            while(true)
            {
                try //En caso de que no haya lista en el diccionario, o la lista sea vacía.
                {
                    var update = chatUpd[chatId].Last(); //Toma el mensaje más antiguo de los recibidos (los responde de más antiguo a más reciente)
                    chatUpd[chatId].RemoveAt(chatUpd[chatId].Count - 1); //Lo quita de la lista de mensajes por leer.
                    return update.Text; //Retorna el texto del mensaje.
                }
                catch(SystemException ex) when (ex is System.Collections.Generic.KeyNotFoundException || ex is System.InvalidOperationException)
                {
                    Thread.Sleep(100);
                }
            }
        }

        // OnMessage: Método que se ejecuta cada vez que el bot recibe un mensaje.
        private void OnMessage(object sender, MessageEventArgs messageEventArgs)
        {
            //Asigna el mensaje recibido a la variable de clase "message".
            message = messageEventArgs.Message;

            if (message.Text.ToLower() != null)
            {
                Console.WriteLine($"{senderName}: envío {message.Text}");

                //Recorre "chatUpd".
                foreach(var chatId in chatUpd)
                {
                    if(!chatId.Value.Any())         //Si la lista está vacía, 
                    {
                        chatUpd.Remove(chatId.Key); //se elimina del diccionario "chatUpd".
                    }
                }
                if(!chatUpd.ContainsKey(senderChatId))               //Si no existe una lista para el Chat.Id
                {                                                    //del mensaje en el diccionario "chatUpd", 
                    chatUpd.Add(senderChatId, new List<Message>());  //se crea una lista vacía.
                }
                chatUpd[senderChatId].Add(message);  //Se agrega el mensaje recibido a la lista.

                //Ve si no hay un thread ejecutando MessageSwitch para este chat.
                if(!activeChats.Contains(senderChatId))
                {
                    activeChats.Add(senderChatId); //Añade el id a la lista de chats activos.
                    ThreadPool.QueueUserWorkItem(new WaitCallback(delegate(object obj)
                    {
                        var msgR = new MessageResponse(this, senderName, senderChatId);        //Ejecuta MessageResponse en un thread
                        activeChats.Remove(msgR.chatId); //y lo elimina de activeChats al terminar.
                        return;
                    }));
                }
            }
        }
    }
}
