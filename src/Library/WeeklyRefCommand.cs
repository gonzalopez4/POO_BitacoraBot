using System;
using System.Threading;
using System.Text;
using System.Collections.Generic;
using System.Reflection;
using System.Globalization;

namespace Library
{ 
    /// <summary>
    /// WeeklyRefCommand: Clase que implementa la interfaz ICommand que se encarga de administrar el panel WeeklyReflection del usuario.
    /// 
    /// Principios y patrones:
    /// SRP: Cumple el principio al tener solo una responsabilidad, proporcionar los comandos de WeeklyReflection al usuario.
    /// OCP: Cumple con el principio, puede ser extendido en funcionalidades sin tener que ser modificado.
    /// ISP: Cumple con el principio al no ser forzado a utilizar ni depender de objetos que no utiliza.
    /// DIP: Cumple con el principio, se creo la interfaz ICommand para independizar las dependencias entre las clases.
    /// Expert: Cumple con el patron debido a que esta clase es experta en la informacion que utiliza.
    /// </summary>
    public class WeeklyRefCommand : ICommand
    {
        private Dictionary<string,string> weeklyRefCommand = new Dictionary<string,string>(StringComparer.Create( CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase))
        {
            { "Reflexion", "Text" },
            { "Formato",   "Format" },
        };

        //Command: Ejecucion deseada con el mensaje command.
        public void Command(MessageResponse msgR)
        {
            var msg = new StringBuilder("Elija qué es lo que desea modificar de la reflexión semanal:\n");
            foreach(var pair in weeklyRefCommand)
            {
                msg.Append("- /" + pair.Key + "\n");
            }
            msg.Append("\nO ingrese /atras para volver al menú.");
            msgR.bot.SendMessage(msg.ToString(), msgR.chatId);

            string msgReceived = null;
            while(String.Compare(msgReceived, "atras", CultureInfo.CurrentCulture, CompareOptions.IgnoreCase) != 0)
            {
                msgReceived = msgR.bot.ReadMessage(msgR.chatId);
                if(msgReceived.StartsWith("/"))
                {
                    msgReceived = msgReceived.Substring(1);
                }

                if(String.Compare(msgReceived, "atras", CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) != 0)
                {
                    try
                    {
                        typeof(WeeklyRefCommand).GetMethod(weeklyRefCommand[msgReceived], BindingFlags.NonPublic | BindingFlags.Instance).Invoke(this, new object[1]{ msgR });
                        msgR.bot.SendMessage(msg.ToString(), msgR.chatId);
                    }
                    catch(KeyNotFoundException)
                    {
                        msgR.bot.SendMessage("Elemento inválido, ingrese alguno de los anteriormente mencionados o /atras para volver.", msgR.chatId);
                    }
                }
            }
            Thread.Sleep(300);
            msgR.bot.SendMessage("Ingrese un nuevo comando, o ingrese /help para ver qué comandos puede utilizar.", msgR.chatId);
        }

        private void Text(MessageResponse msgR)
        {
            string msg;
            if(msgR.userData.weeklyRef.Text != null)
            {
                msg = "La reflexión semanal guardada actualmente es:\n\"" + msgR.userData.weeklyRef.Text + "\".";
            }
            else
            {
                msg = "No tiene una reflexión semanal guardada.";
            }
            msgR.bot.SendMessage(msg + "\n¿Desea modificarla?", msgR.chatId);

            msg = msgR.bot.ReadMessage(msgR.chatId).ToLower();
            if( msg.StartsWith("si") || msg.StartsWith("sí") || msg.StartsWith("yes") || msg == "y" || msg.StartsWith("obvio") || msg.Contains("dale") || msg.Contains("claro que si") || msg == "claro" || msg.Contains("ya sabes") )
            {
                msgR.bot.SendMessage("Ingrese su nueva reflexión semanal.\n", msgR.chatId);
                var refl = msgR.bot.ReadMessage(msgR.chatId);
                msgR.userData.weeklyRef.Text = refl;
                msgR.userData.Save(msgR.chatId);
                msgR.bot.SendMessage("La reflexión se guardo correctamente.", msgR.chatId);
            }
        }

        private void Format(MessageResponse msgR)
        {
            new FormatCommand("weeklyRef").Command(msgR);
        }
    }
}