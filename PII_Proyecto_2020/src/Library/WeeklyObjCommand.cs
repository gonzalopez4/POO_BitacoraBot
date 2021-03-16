using System;
using System.Threading;
using System.Text;
using System.Collections.Generic;
using System.Reflection;
using System.Globalization;

namespace Library
{
    /// <summary>
    /// WeeklyObjCommand: Clase que implementa la interfaz ICommand que se encarga de administrar el panel de objetivos del usuario.
    /// 
    /// Principios y patrones:
    /// SRP: Cumple el principio al tener solo una responsabilidad, proporcionar los comandos de WeeklyObjectives al usuario.
    /// OCP: Cumple el principio al poder ser extendido sin verse afectado.
    /// ISP: Cumple con el principio al no ser forzado a utilizar ni depender de objetos que no utiliza.
    /// DIP: Cumple con el principio, se creo la interfaz ICommand para independizar las dependencias entre las clases.
    /// Expert: Cumple el patron al ser experto en la informacion que utiliza.
    /// Creator: Cumple con el patron debido a que esta clase es la responsable de las instancias creadas.
    /// </summary>
    public class WeeklyObjCommand : ICommand
    {
        private Dictionary<string,string> weeklyObjCommand = new Dictionary<string,string>(StringComparer.Create( CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase))
        {
            { "ModificarCantidadDeObjetivos", "ModifyObjectiveAmmount" },
            { "VerObjetivos",                 "SeeObjectives" },
            { "AgregarObjetivos",             "AddObjective" },
            { "BorrarObjetivos",              "DeleteObjective" },
            { "ModificarObjetivos",           "ModifyObjective" },
            { "Formato",                      "Format" },
        };

        //Command: Ejecucion deseada con el mensaje command.
        public void Command(MessageResponse msgR)
        {
            var msg = new StringBuilder("Elija qué es lo que desea modificar de sus objetivos semanales:\n");
            foreach(var pair in weeklyObjCommand)
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
                        typeof(WeeklyObjCommand).GetMethod(weeklyObjCommand[msgReceived], BindingFlags.NonPublic | BindingFlags.Instance).Invoke(this, new object[1]{ msgR });
                        msgR.userData.Save(msgR.chatId);
                        Thread.Sleep(300);
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

        private void Format(MessageResponse msgR)
        {
            new FormatCommand("weeklyObj").Command(msgR);
        }

        //ModifyObjectiveAmmount: Procedimiento para modificar la cantidad de objetivos del usuario.
        private void ModifyObjectiveAmmount(MessageResponse msgR)
        {
            msgR.userData.weeklyObj.ModifyObjectiveAmmount(msgR);
        }

        //AddObjectives: Procedimiento para agregar objetivos del usuario.
        private void AddObjective(MessageResponse msgR)
        {
            msgR.userData.weeklyObj.AddObjective(msgR);
        }

        //DeleteObjectives: Procedimiento para borrar objetivos del usuario.
        private void DeleteObjective(MessageResponse msgR)
        {
            msgR.userData.weeklyObj.DeleteObjective(msgR);
        }

        //ModifyObjective: Procedimiento para modificar objetivos del usuario.
        private void ModifyObjective(MessageResponse msgR)
        {
            msgR.userData.weeklyObj.ModifyObjective(msgR);
        }

        //SeeObjectives: Procedimiento para ver los objetivos del usuario.
        private void SeeObjectives(MessageResponse msgR)
        {
            if(msgR.userData.weeklyObj.ShowObjectives().Count != 0)
            {
                var msg = "Sus objetivos guardados son:\n";
                foreach(var obj in msgR.userData.weeklyObj.ShowObjectives())
                {
                    msg += "- " + obj.Goal + "\n";
                }
                msgR.bot.SendMessage(msg, msgR.chatId);
            }
            else
            {
                msgR.bot.SendMessage("No tiene objetivos semanales guardados.", msgR.chatId);
            }
        }
    }
}