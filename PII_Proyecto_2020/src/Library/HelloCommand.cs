using System;
using System.Globalization;

namespace Library
{
    /// <summary>
    /// HelloCommand: Clase que implementa la interfaz ICommand, con el objetivo de brindarle al usuario una bienvenida a su bitácora personal.
    /// 
    /// Principios y patrones:
    /// ISP: Cumple con el principio al no ser forzado a utilizar ni depender de objetos que no utiliza.
    /// DIP: Cumple con el principio, se creo la interfaz ICommand para independizar las dependencias entre las clases.
    /// <summary>
    public class HelloCommand : ICommand
    {
        //Command: Ejecucion deseada con el mensaje command.
        public void Command(MessageResponse msgR)
        {
            msgR.bot.SendMessage($"¡Hola, {msgR.name}!\n¿Ya actualizaste tu bitacora? 😊", msgR.chatId);
            var msg = msgR.bot.ReadMessage(msgR.chatId);

            if( msg.StartsWith("si") || msg.StartsWith("sí") || msg.StartsWith("yes") || msg == "y" || msg.StartsWith("obvio") || msg.Contains("dale") || msg.Contains("claro que si") || msg == "claro" || msg.Contains("ya sabes") || msg.Contains("hace "))
            {
                msgR.bot.SendMessage("Me alegro, hay que mantenerla al día 😋", msgR.chatId);
                msg = msgR.bot.ReadMessage(msgR.chatId);
            }
            else if( msg.ToLower().StartsWith("no") || msg.ToLower().StartsWith("negativo") || msg.ToLower().Contains("que te digo") || msg == "n" )
            {
                msgR.bot.SendMessage("¿Entonces qué tal si lo hacemos ahora? 😜\nMandame un comando.", msgR.chatId);
                msg = "help";
            }
            while( msg.ToLower() == "f" || msg.ToLower().Contains(":("))
            {
                var respArray = new string[12]{ "Ultra F", "F en el chat", "Super F", "Rip", "Recontra F", "F", "F 😢", "F 😰", "F 🙏", "🙏", "La hora sad 😞", "🥺"};
                msgR.bot.SendMessage(respArray[new Random().Next(respArray.Length)], msgR.chatId);
                msg = msgR.bot.ReadMessage(msgR.chatId);
            }

            if(msg.StartsWith("/") && String.Compare(msg, "/start", CultureInfo.CurrentCulture, CompareOptions.IgnoreCase) != 0)
            {
                msg = msg.Substring(1);
            }
            ICommand command = (ICommand) Activator.CreateInstance(msgR.msgSwitch[msg]);
            command.Command(msgR);
        }
    }
}