using System;
using System.Threading;
using Newtonsoft.Json;

namespace Library
{
    /// <summary>
    /// Notification: Clase que hereda de INotification, encargada de enviar el mensaje "¡Hora de actualizar la bitácora!".
    /// 
    /// Principios y patrones:
    /// SRP: Utiliza el principio de tener una sola responsabilidad, los planes.
    /// OCP: Utiliza el principio al poder ser extendido sin tener que ser modificado(aunque no fue pensado con ese proposito).
    /// Expert: Aplica el patron debido a que esta clase es experta en la informacion que utiliza.
    /// </summary>

    [JsonObject("Semester", MemberSerialization = MemberSerialization.OptIn)]
    public class Notification : INotification
    {
        public Notification(DateTime time, int chatId, IBot bot)
        {
            Time = time;
            ChatId = chatId;
            Bot = bot;
        }

        //ChatId: Usuario al que debe enviarse.
        [JsonProperty("ChatId")]
        public int ChatId {get; set;}
        
        //Time: Hora a la que debe enviarse.
        [JsonProperty("Time")]
        public DateTime Time {get; set;}

        public IBot Bot {get; set;}

        public void Send()
        {
            Bot.SendMessage("¡Hora de actualizar la bitácora! 🤩", ChatId);
            Time.AddDays(7);
            Thread.Sleep(300);
        }
    }
}