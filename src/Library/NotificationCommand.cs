using System.Threading;
using System.Text;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Globalization;
using System.Reflection;

namespace Library
{
    /// <summary>
    /// HelloCommand: Clase que implementa la interfaz ICommand, con el objetivo de brindarle al usuario un medio de ayuda para entender como utilizar su bitacora.
    /// 
    /// Principios y patrones:
    /// 
    /// <summary>
    public class NotificationCommand : ICommand
    {
        private Dictionary<string,string> notificationCommand = new Dictionary<string,string>(StringComparer.Create( CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase))
        {
            { "Notificaciones",    "NotificationConfig" },
            { "InicioDelSemestre", "SemesterStartConfig" },
            { "FinalDelSemestre",  "SemesterEndConfig" }
        };

        //Command: Ejecucion deseada con el mensaje command.
        public void Command(MessageResponse msgR)
        {
            var msg = new StringBuilder("Elija qué es lo que desea modificar de las notificaciones:\n");
            foreach(var pair in notificationCommand)
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
                        typeof(MetacogRefCommand).GetMethod(notificationCommand[msgReceived], BindingFlags.NonPublic | BindingFlags.Instance).Invoke(this, new object[1]{ msgR });
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

        public void NotificationConfig(MessageResponse msgR)
        {
            if(msgR.userData.semester.NotificationTime == null)
            {
                msgR.bot.SendMessage("No tiene guardado un día para recibir notificaciones.\n¿Le gustaría agregar uno?", msgR.chatId);
            }
            else
            {
                msgR.bot.SendMessage("Actualmente los días que recibirá notificaciones son los "+ msgR.userData.semester.NotificationTime.Time.ToString("dddd") + " a las " + msgR.userData.semester.NotificationTime.Time.ToString("HH:mm") + "\n¿Desea modificarla?", msgR.chatId);
            }

            var msgReceived = msgR.bot.ReadMessage(msgR.chatId);
            if( msgReceived.StartsWith("si") || msgReceived.StartsWith("sí") || msgReceived.StartsWith("yes") || msgReceived == "y" || msgReceived.StartsWith("obvio") || msgReceived.Contains("dale") || msgReceived.Contains("claro que si") || msgReceived == "claro" || msgReceived.Contains("ya sabes"))
            {
                var notification = default(DateTime);
                while(notification == default(DateTime) && String.Compare(msgReceived, "atras", CultureInfo.CurrentCulture, CompareOptions.IgnoreCase) != 0)
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
                            int day;
                            int month;
                            int year;
                            int hours;
                            int minutes;
                            int seconds;

                            var ind = Array.IndexOf(msgReceived.ToCharArray(), Convert.ToChar("/"));
                            if(ind >= 2)
                            {
                                day = Convert.ToInt32(msgReceived.Substring(ind-2, 2));
                            }
                            else if(ind == 1)
                            {
                                day = Convert.ToInt32(msgReceived.Substring(ind-1, 1));
                            }
                            else
                            {
                                throw new System.IndexOutOfRangeException();
                            }
                            
                            var msg1 = msgReceived.Substring(ind+1);
                            ind = Array.IndexOf(msg1.ToCharArray(), Convert.ToChar("/"));
                            if(ind >= 2)
                            {
                                month = Convert.ToInt32(msg1.Substring(ind-2, 2));
                            }
                            else if(ind == 1)
                            {
                                month = Convert.ToInt32(msg1.Substring(ind-1, 1));
                            }
                            else
                            {
                                if(msg1.Length >= 2)
                                {
                                    month = Convert.ToInt32(msg1.Substring(0, 2));
                                }
                                else if(msg1.Length == 1)
                                {
                                    month = Convert.ToInt32(msg1.Substring(0, 1));
                                }
                                else
                                {
                                    throw new System.IndexOutOfRangeException();
                                }
                            }

                            msg1 = msg1.Substring(ind+1);
                            ind = Array.IndexOf(msg1.ToCharArray(), Convert.ToChar("/"));
                            if(ind >= 4)
                            {
                                year = Convert.ToInt32(msg1.Substring(ind-4, 4));
                            }
                            else if(ind == 2)
                            {
                                year = Convert.ToInt32("20" + msg1.Substring(ind-2, 2));
                            }
                            else
                            {
                                year = DateTime.Today.Year;
                            }

                            ind = Array.IndexOf(msgReceived.ToCharArray(), Convert.ToChar(":"));
                            if(ind >= 2)
                            {
                                hours = Convert.ToInt32(msgReceived.Substring(ind-2, 2));
                                if(msgReceived.Substring(ind+1).Length >=2)
                                {
                                    minutes = Convert.ToInt32(msgReceived.Substring(ind+1, 2));
                                    seconds = 1;
                                }
                                else if(msgReceived.Substring(ind+1).Length == 1)
                                {
                                    minutes = Convert.ToInt32(msgReceived.Substring(ind+1, 1));
                                    seconds = 1;
                                }
                                else
                                {
                                    minutes = 0;
                                    seconds = 1;
                                }
                            }
                            else if(ind == 1)
                            {
                                hours = Convert.ToInt32(msgReceived.Substring(ind-1, 1));
                                if(msgReceived.Substring(ind+1).Length >=2)
                                {
                                    minutes = Convert.ToInt32(msgReceived.Substring(ind+1, 2));
                                    seconds = 1;
                                }
                                else if(msgReceived.Substring(ind+1).Length == 1)
                                {
                                    minutes = Convert.ToInt32(msgReceived.Substring(ind+1, 1));
                                    seconds = 1;
                                }
                                else
                                {
                                    minutes = 0;
                                    seconds = 1;
                                }
                            }
                            else
                            {
                                hours = 0;
                                minutes = 0;
                                seconds = 0;
                            }

                            var dateTime = new DateTime(year, month, day, hours, minutes, seconds);
                            msgR.userData.semester.NotificationTimeSet(dateTime);
                        }
                        catch(Exception ex) when (ex is System.IndexOutOfRangeException || ex is System.FormatException)
                        {
                            msgR.bot.SendMessage("Esa no es una fecha válida, ingrese otra o /atras para volver.\n", msgR.chatId);
                            throw new System.FormatException();
                        }
                    }
                }
            }
            else
            {
                msgR.bot.SendMessage("No se modificó la hora de las notificaciones.", msgR.chatId);
            }
        }

        public void SemesterStartConfig(MessageResponse msgR)
        {
            msgR.bot.SendMessage("Su fecha de inicio de semestre guardada es " + msgR.userData.semester.SemesterStart.ToString("dd/mm/yyyy") + ".\n¿Desea modificarla?", msgR.chatId);

            var msgReceived = msgR.bot.ReadMessage(msgR.chatId);
            if( msgReceived.StartsWith("si") || msgReceived.StartsWith("sí") || msgReceived.StartsWith("yes") || msgReceived == "y" || msgReceived.StartsWith("obvio") || msgReceived.Contains("dale") || msgReceived.Contains("claro que si") || msgReceived == "claro" || msgReceived.Contains("ya sabes"))
            {
                var semesterStart = default(DateTime);
                while(semesterStart == default(DateTime) && String.Compare(msgReceived, "atras", CultureInfo.CurrentCulture, CompareOptions.IgnoreCase) != 0)
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
                            int day;
                            int month;
                            int year;
                            int hours;
                            int minutes;
                            int seconds;

                            var ind = Array.IndexOf(msgReceived.ToCharArray(), Convert.ToChar("/"));
                            if(ind >= 2)
                            {
                                day = Convert.ToInt32(msgReceived.Substring(ind-2, 2));
                            }
                            else if(ind == 1)
                            {
                                day = Convert.ToInt32(msgReceived.Substring(ind-1, 1));
                            }
                            else
                            {
                                throw new System.IndexOutOfRangeException();
                            }
                            
                            var msg1 = msgReceived.Substring(ind+1);
                            ind = Array.IndexOf(msg1.ToCharArray(), Convert.ToChar("/"));
                            if(ind >= 2)
                            {
                                month = Convert.ToInt32(msg1.Substring(ind-2, 2));
                            }
                            else if(ind == 1)
                            {
                                month = Convert.ToInt32(msg1.Substring(ind-1, 1));
                            }
                            else
                            {
                                if(msg1.Length >= 2)
                                {
                                    month = Convert.ToInt32(msg1.Substring(0, 2));
                                }
                                else if(msg1.Length == 1)
                                {
                                    month = Convert.ToInt32(msg1.Substring(0, 1));
                                }
                                else
                                {
                                    throw new System.IndexOutOfRangeException();
                                }
                            }

                            msg1 = msg1.Substring(ind+1);
                            ind = Array.IndexOf(msg1.ToCharArray(), Convert.ToChar("/"));
                            if(ind >= 4)
                            {
                                year = Convert.ToInt32(msg1.Substring(ind-4, 4));
                            }
                            else if(ind == 2)
                            {
                                year = Convert.ToInt32("20" + msg1.Substring(ind-2, 2));
                            }
                            else
                            {
                                year = DateTime.Today.Year;
                            }

                            ind = Array.IndexOf(msgReceived.ToCharArray(), Convert.ToChar(":"));
                            if(ind >= 2)
                            {
                                hours = Convert.ToInt32(msgReceived.Substring(ind-2, 2));
                                if(msgReceived.Substring(ind+1).Length >=2)
                                {
                                    minutes = Convert.ToInt32(msgReceived.Substring(ind+1, 2));
                                    seconds = 1;
                                }
                                else if(msgReceived.Substring(ind+1).Length == 1)
                                {
                                    minutes = Convert.ToInt32(msgReceived.Substring(ind+1, 1));
                                    seconds = 1;
                                }
                                else
                                {
                                    minutes = 0;
                                    seconds = 1;
                                }
                            }
                            else if(ind == 1)
                            {
                                hours = Convert.ToInt32(msgReceived.Substring(ind-1, 1));
                                if(msgReceived.Substring(ind+1).Length >=2)
                                {
                                    minutes = Convert.ToInt32(msgReceived.Substring(ind+1, 2));
                                    seconds = 1;
                                }
                                else if(msgReceived.Substring(ind+1).Length == 1)
                                {
                                    minutes = Convert.ToInt32(msgReceived.Substring(ind+1, 1));
                                    seconds = 1;
                                }
                                else
                                {
                                    minutes = 0;
                                    seconds = 1;
                                }
                            }
                            else
                            {
                                hours = 0;
                                minutes = 0;
                                seconds = 0;
                            }

                            var dateTime = new DateTime(year, month, day, hours, minutes, seconds);
                            if( (dateTime - DateTime.Today).Duration() > (DateTime.Today - DateTime.Today.AddMonths(7)).Duration() )
                            {
                                msgR.bot.SendMessage("La fecha no puede tener más de 7 meses de diferencia con la fecha actual.", msgR.chatId);
                            }
                            else
                            {
                                semesterStart = dateTime;
                            }
                        }
                        catch(Exception ex) when (ex is System.IndexOutOfRangeException || ex is System.FormatException)
                        {
                            msgR.bot.SendMessage("Esa no es una fecha válida, ingrese otra o /atras para volver.\n", msgR.chatId);
                            throw new System.FormatException();
                        }
                    }
                }
            }
            else
            {
                msgR.bot.SendMessage("No se modificó el inicio de semestre actual.", msgR.chatId);
            }
        }

        public void SemesterEndConfig(MessageResponse msgR)
        {
            msgR.bot.SendMessage("Su fecha de final de semestre guardada es " + msgR.userData.semester.SemesterStart.ToString("dd/mm/yyyy") + ".\n¿Desea modificarla?", msgR.chatId);

            var msgReceived = msgR.bot.ReadMessage(msgR.chatId);
            if( msgReceived.StartsWith("si") || msgReceived.StartsWith("sí") || msgReceived.StartsWith("yes") || msgReceived == "y" || msgReceived.StartsWith("obvio") || msgReceived.Contains("dale") || msgReceived.Contains("claro que si") || msgReceived == "claro" || msgReceived.Contains("ya sabes"))
            {
                var semesterStart = default(DateTime);
                while(semesterStart == default(DateTime) && String.Compare(msgReceived, "atras", CultureInfo.CurrentCulture, CompareOptions.IgnoreCase) != 0)
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
                            int day;
                            int month;
                            int year;
                            int hours;
                            int minutes;
                            int seconds;

                            var ind = Array.IndexOf(msgReceived.ToCharArray(), Convert.ToChar("/"));
                            if(ind >= 2)
                            {
                                day = Convert.ToInt32(msgReceived.Substring(ind-2, 2));
                            }
                            else if(ind == 1)
                            {
                                day = Convert.ToInt32(msgReceived.Substring(ind-1, 1));
                            }
                            else
                            {
                                throw new System.IndexOutOfRangeException();
                            }
                            
                            var msg1 = msgReceived.Substring(ind+1);
                            ind = Array.IndexOf(msg1.ToCharArray(), Convert.ToChar("/"));
                            if(ind >= 2)
                            {
                                month = Convert.ToInt32(msg1.Substring(ind-2, 2));
                            }
                            else if(ind == 1)
                            {
                                month = Convert.ToInt32(msg1.Substring(ind-1, 1));
                            }
                            else
                            {
                                if(msg1.Length >= 2)
                                {
                                    month = Convert.ToInt32(msg1.Substring(0, 2));
                                }
                                else if(msg1.Length == 1)
                                {
                                    month = Convert.ToInt32(msg1.Substring(0, 1));
                                }
                                else
                                {
                                    throw new System.IndexOutOfRangeException();
                                }
                            }

                            msg1 = msg1.Substring(ind+1);
                            ind = Array.IndexOf(msg1.ToCharArray(), Convert.ToChar("/"));
                            if(ind >= 4)
                            {
                                year = Convert.ToInt32(msg1.Substring(ind-4, 4));
                            }
                            else if(ind == 2)
                            {
                                year = Convert.ToInt32("20" + msg1.Substring(ind-2, 2));
                            }
                            else
                            {
                                year = DateTime.Today.Year;
                            }

                            ind = Array.IndexOf(msgReceived.ToCharArray(), Convert.ToChar(":"));
                            if(ind >= 2)
                            {
                                hours = Convert.ToInt32(msgReceived.Substring(ind-2, 2));
                                if(msgReceived.Substring(ind+1).Length >=2)
                                {
                                    minutes = Convert.ToInt32(msgReceived.Substring(ind+1, 2));
                                    seconds = 1;
                                }
                                else if(msgReceived.Substring(ind+1).Length == 1)
                                {
                                    minutes = Convert.ToInt32(msgReceived.Substring(ind+1, 1));
                                    seconds = 1;
                                }
                                else
                                {
                                    minutes = 0;
                                    seconds = 1;
                                }
                            }
                            else if(ind == 1)
                            {
                                hours = Convert.ToInt32(msgReceived.Substring(ind-1, 1));
                                if(msgReceived.Substring(ind+1).Length >=2)
                                {
                                    minutes = Convert.ToInt32(msgReceived.Substring(ind+1, 2));
                                    seconds = 1;
                                }
                                else if(msgReceived.Substring(ind+1).Length == 1)
                                {
                                    minutes = Convert.ToInt32(msgReceived.Substring(ind+1, 1));
                                    seconds = 1;
                                }
                                else
                                {
                                    minutes = 0;
                                    seconds = 1;
                                }
                            }
                            else
                            {
                                hours = 0;
                                minutes = 0;
                                seconds = 0;
                            }

                            var dateTime = new DateTime(year, month, day, hours, minutes, seconds);
                            if( (dateTime - msgR.userData.semester.SemesterEnd).Duration() > (DateTime.Today - DateTime.Today.AddMonths(7)).Duration() )
                            {
                                msgR.bot.SendMessage("La fecha no puede tener más de 7 meses de diferencia con el inicio de semestre.", msgR.chatId);
                            }
                            else
                            {
                                msgR.userData.semester.SemesterEnd = dateTime;
                            }
                        }
                        catch(Exception ex) when (ex is System.IndexOutOfRangeException || ex is System.FormatException)
                        {
                            msgR.bot.SendMessage("Esa no es una fecha válida, ingrese otra o /atras para volver.\n", msgR.chatId);
                            throw new System.FormatException();
                        }
                    }
                }
            }
            else
            {
                msgR.bot.SendMessage("No se modificó el final de semestre actual.", msgR.chatId);
            }
        }
    }
}