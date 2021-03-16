using System;
using System.Collections.Generic;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Globalization;

namespace Library
{
    /// <summary>
    /// Semester: Clase responsable de registrar los datos respectivos al semestre.
    /// 
    /// Principios y patrones:
    /// SRP: Cumple el principio al tener solo una responsabilidad, las fechas de los semestres.
    /// OCP: Cumple el principio al permitir la extensión de su código sin cambios en su actual código.
    /// Expert: Cumple el patron al ser experto en la informacion que utiliza.
    /// </summary>

    [JsonObject("Semester", MemberSerialization = MemberSerialization.OptIn)]
    public class Semester
    {
        //SemesterStart: Fecha de inicio del semestre.
        [JsonProperty("SemesterStart")]
        public DateTime SemesterStart {get; set;}

        //SemesterEnd: Fecha de finalización del semestre.
        [JsonProperty("SemesterEnd")]
        public DateTime SemesterEnd {get; set;}

        //NotificationTime: Notificación correspondiente al usuario.
        [JsonProperty("NotificationTime")]
        public Notification NotificationTime {get; set;}

        private MessageResponse msgR;
        
        public Semester(DateTime semesterStart, DateTime semesterEnd, MessageResponse msgR)
        {
            SemesterStart = SemesterStart;
            SemesterEnd = semesterEnd;
            this.msgR = msgR;
        }

        public static Semester Create(MessageResponse msgR)
        {
            msgR.bot.SendMessage("Ingrese su fecha de inicio del semestre. ", msgR.chatId);
            DateTime semesterStart = default(DateTime);
            while(semesterStart == default(DateTime))
            {
                var msgReceive = msgR.bot.ReadMessage(msgR.chatId);
                if(msgReceive.StartsWith("/"))
                {
                    msgReceive = msgReceive.Substring(1);
                }

                try
                {
                    int day;
                    int month;
                    int year;
                    int hours;
                    int minutes;
                    int seconds;

                    var ind = Array.IndexOf(msgReceive.ToCharArray(), Convert.ToChar("/"));
                    if(ind >= 2)
                    {
                        day = Convert.ToInt32(msgReceive.Substring(ind-2, 2));
                    }
                    else if(ind == 1)
                    {
                        day = Convert.ToInt32(msgReceive.Substring(ind-1, 1));
                    }
                    else
                    {
                        throw new System.IndexOutOfRangeException();
                    }
                    
                    var msg1 = msgReceive.Substring(ind+1);
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

                    ind = Array.IndexOf(msgReceive.ToCharArray(), Convert.ToChar(":"));
                    if(ind >= 2)
                    {
                        hours = Convert.ToInt32(msgReceive.Substring(ind-2, 2));
                        if(msgReceive.Substring(ind+1).Length >=2)
                        {
                            minutes = Convert.ToInt32(msgReceive.Substring(ind+1, 2));
                            seconds = 1;
                        }
                        else if(msgReceive.Substring(ind+1).Length == 1)
                        {
                            minutes = Convert.ToInt32(msgReceive.Substring(ind+1, 1));
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
                        hours = Convert.ToInt32(msgReceive.Substring(ind-1, 1));
                        if(msgReceive.Substring(ind+1).Length >=2)
                        {
                            minutes = Convert.ToInt32(msgReceive.Substring(ind+1, 2));
                            seconds = 1;
                        }
                        else if(msgReceive.Substring(ind+1).Length == 1)
                        {
                            minutes = Convert.ToInt32(msgReceive.Substring(ind+1, 1));
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
                    msgR.bot.SendMessage("Esa no es una fecha válida.\n", msgR.chatId);
                    throw new System.FormatException();
                }
            }

            msgR.bot.SendMessage("Ingrese la fecha del final del semestre. ", msgR.chatId);
            DateTime semesterEnd = default(DateTime);
            while(semesterEnd == default(DateTime))
            {
                var msgRec = msgR.bot.ReadMessage(msgR.chatId);
                if(msgRec.StartsWith("/"))
                {
                    msgRec = msgRec.Substring(1);
                }

                try
                {
                    int day;
                    int month;
                    int year;
                    int hours;
                    int minutes;
                    int seconds;

                    var ind = Array.IndexOf(msgRec.ToCharArray(), Convert.ToChar("/"));
                    if(ind >= 2)
                    {
                        day = Convert.ToInt32(msgRec.Substring(ind-2, 2));
                    }
                    else if(ind == 1)
                    {
                        day = Convert.ToInt32(msgRec.Substring(ind-1, 1));
                    }
                    else
                    {
                        throw new System.IndexOutOfRangeException();
                    }
                    
                    var msg1 = msgRec.Substring(ind+1);
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

                    ind = Array.IndexOf(msgRec.ToCharArray(), Convert.ToChar(":"));
                    if(ind >= 2)
                    {
                        hours = Convert.ToInt32(msgRec.Substring(ind-2, 2));
                        if(msgRec.Substring(ind+1).Length >=2)
                        {
                            minutes = Convert.ToInt32(msgRec.Substring(ind+1, 2));
                            seconds = 1;
                        }
                        else if(msgRec.Substring(ind+1).Length == 1)
                        {
                            minutes = Convert.ToInt32(msgRec.Substring(ind+1, 1));
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
                        hours = Convert.ToInt32(msgRec.Substring(ind-1, 1));
                        if(msgRec.Substring(ind+1).Length >=2)
                        {
                            minutes = Convert.ToInt32(msgRec.Substring(ind+1, 2));
                            seconds = 1;
                        }
                        else if(msgRec.Substring(ind+1).Length == 1)
                        {
                            minutes = Convert.ToInt32(msgRec.Substring(ind+1, 1));
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
                        semesterEnd = dateTime;
                    }
                }
                catch(Exception ex) when (ex is System.IndexOutOfRangeException || ex is System.FormatException)
                {
                    msgR.bot.SendMessage("Esa no es una fecha válida.\n", msgR.chatId);
                    throw new System.FormatException();
                }
            }

            msgR.bot.SendMessage("¿Desea recibir recordatorios para actualizar su bitácora?", msgR.chatId);
            var msgReceived = msgR.bot.ReadMessage(msgR.chatId);
            if(msgReceived.StartsWith("/"))
            {
                msgReceived = msgReceived.Substring(1);
            }

            var semester = new Semester(semesterStart, semesterEnd, msgR);

            var notification = default(DateTime);
            if( msgReceived.StartsWith("si") || msgReceived.StartsWith("sí") || msgReceived.StartsWith("yes") || msgReceived == "y" || msgReceived.StartsWith("obvio") || msgReceived.Contains("dale") || msgReceived.Contains("claro que si") || msgReceived == "claro" || msgReceived.Contains("ya sabes"))
            {
                msgR.bot.SendMessage("Ingrese el día que desea recibir su primera notificación.\nUtilice el formato dd/mm hh:mm.", msgR.chatId);
                while(notification == default(DateTime) && String.Compare(msgReceived, "atras", CultureInfo.CurrentCulture, CompareOptions.IgnoreCase) != 0)
                {
                    msgReceived = msgR.bot.ReadMessage(msgR.chatId);
                    if(msgReceived.StartsWith("/"))
                    {
                        msgReceived = msgReceived.Substring(1);
                    }

                    if(String.Compare(msgReceived, "atras", CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) != 0)
                    {
                        CultureInfo CI = CultureInfo.CurrentCulture;
                        Calendar calendar = CI.Calendar;

                        DayOfWeek weekday = default(DayOfWeek);
                        switch(msgReceived)
                        {
                            case "lunes":
                            case "monday":
                                if(DateTime.Today.DayOfWeek != DayOfWeek.Monday)
                                {
                                    int daysTo = (Convert.ToInt32(weekday) - Convert.ToInt32(DateTime.Today.DayOfWeek)) % 7;
                                    notification = DateTime.Today.AddDays(daysTo);
                                }
                                break;
                            case "martes":
                            case "tuesday":
                                if(DateTime.Today.DayOfWeek != DayOfWeek.Tuesday)
                                {
                                    int daysTo = (Convert.ToInt32(weekday) - Convert.ToInt32(DateTime.Today.DayOfWeek)) % 7;
                                    notification = DateTime.Today.AddDays(daysTo);
                                }
                                break;
                            case "miercoles":
                            case "miércoles":
                            case "wednesday":
                                if(DateTime.Today.DayOfWeek != DayOfWeek.Wednesday)
                                {
                                    int daysTo = (Convert.ToInt32(weekday) - Convert.ToInt32(DateTime.Today.DayOfWeek)) % 7;
                                    notification = DateTime.Today.AddDays(daysTo);
                                }
                                break;
                            case "jueves":
                            case "thursday":
                                if(DateTime.Today.DayOfWeek != DayOfWeek.Thursday)
                                {
                                    int daysTo = (Convert.ToInt32(weekday) - Convert.ToInt32(DateTime.Today.DayOfWeek)) % 7;
                                    notification = DateTime.Today.AddDays(daysTo);
                                }
                                break;
                            case "viernes":
                            case "friday":
                                if(DateTime.Today.DayOfWeek != DayOfWeek.Friday)
                                {
                                    int daysTo = (Convert.ToInt32(weekday) - Convert.ToInt32(DateTime.Today.DayOfWeek)) % 7;
                                    notification = DateTime.Today.AddDays(daysTo);
                                }
                                break;
                            case "sabado":
                            case "sábado":
                            case "saturday":
                                if(DateTime.Today.DayOfWeek != DayOfWeek.Saturday)
                                {
                                    int daysTo = (Convert.ToInt32(weekday) - Convert.ToInt32(DateTime.Today.DayOfWeek)) % 7;
                                    notification = DateTime.Today.AddDays(daysTo);
                                }
                                break;
                            case "domingo":
                            case "sunday":
                                if(DateTime.Today.DayOfWeek != DayOfWeek.Sunday)
                                {
                                    int daysTo = (Convert.ToInt32(weekday) - Convert.ToInt32(DateTime.Today.DayOfWeek)) % 7;
                                    notification = DateTime.Today.AddDays(daysTo);
                                }
                                break;
                            case "hoy":
                                notification = DateTime.Today;
                                break;
                            case "ayer":
                                DateTime time = DateTime.Today.AddDays(-1);

                                if(calendar.GetWeekOfYear(time, CI.DateTimeFormat.CalendarWeekRule, CI.DateTimeFormat.FirstDayOfWeek) != calendar.GetWeekOfYear(DateTime.Today, CI.DateTimeFormat.CalendarWeekRule, CI.DateTimeFormat.FirstDayOfWeek))
                                {
                                    msgR.bot.SendMessage("Debe ser una fecha de esta semana.", msgR.chatId);
                                }
                                else
                                {
                                    notification = time;
                                }
                                break;
                            case "anteayer":
                                time = DateTime.Today.AddDays(-2);

                                if(calendar.GetWeekOfYear(time, CI.DateTimeFormat.CalendarWeekRule, CI.DateTimeFormat.FirstDayOfWeek) != calendar.GetWeekOfYear(DateTime.Today, CI.DateTimeFormat.CalendarWeekRule, CI.DateTimeFormat.FirstDayOfWeek))
                                {
                                    msgR.bot.SendMessage("Debe ser una fecha de esta semana.", msgR.chatId);
                                }
                                else
                                {
                                    notification = time;
                                }
                                break;
                            case "maniana":
                            case "manana":
                            case "mañana":
                                time = DateTime.Today.AddDays(1);

                                if(calendar.GetWeekOfYear(time, CI.DateTimeFormat.CalendarWeekRule, CI.DateTimeFormat.FirstDayOfWeek) != calendar.GetWeekOfYear(DateTime.Today, CI.DateTimeFormat.CalendarWeekRule, CI.DateTimeFormat.FirstDayOfWeek))
                                {
                                    msgR.bot.SendMessage("Debe ser una fecha de esta semana.", msgR.chatId);
                                }
                                else
                                {
                                    notification = time;
                                }
                                break;
                            case "pasado maniana":
                            case "pasado manana":
                            case "pasado mañana":
                            case "pasadomaniana":
                            case "pasadomanana":
                            case "pasadomañana":
                                time = DateTime.Today.AddDays(2);

                                if(calendar.GetWeekOfYear(time, CI.DateTimeFormat.CalendarWeekRule, CI.DateTimeFormat.FirstDayOfWeek) != calendar.GetWeekOfYear(DateTime.Today, CI.DateTimeFormat.CalendarWeekRule, CI.DateTimeFormat.FirstDayOfWeek))
                                {
                                    msgR.bot.SendMessage("Debe ser una fecha de esta semana.", msgR.chatId);
                                }
                                else
                                {
                                    notification = time;
                                }
                                break;
                            default:
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
                                        notification = dateTime;
                                    }
                                }
                                catch(Exception ex) when (ex is System.IndexOutOfRangeException || ex is System.FormatException)
                                {
                                    msgR.bot.SendMessage("Esa no es una fecha válida, ingrese otra o /atras para volver.\n", msgR.chatId);
                                }
                                break;
                        }
                    }
                }
            }

            if(notification != default(DateTime))
            {
                semester.NotificationTimeSet(notification);
            }

            return semester;
        }

        public void NotificationTimeSet(DateTime notifTime)
        {
            if(NotificationTime == null)
            {
                this.NotificationTime = new Notification(notifTime, msgR.chatId, msgR.bot);
                NotificationManager.Notifications.Add(NotificationTime);
            }
            else
            {
                NotificationManager.Notifications.Remove(this.NotificationTime);
                this.NotificationTime = new Notification(notifTime, msgR.chatId, msgR.bot);
                NotificationManager.Notifications.Add(NotificationTime);
            }
        }
    }
}