using System;
using System.Threading;
using System.Text;
using System.Collections.Generic;
using System.Reflection;
using System.Globalization;

namespace Library
{
    /// <summary>
    /// WeeklyPlanCommand: Clase que implementa la interfaz ICommand, con el objetivo de brindarle al usuario el panel de WeeklyPlan.
    /// 
    /// Principios y patrones:
    /// SRP: Cumple el principio al tener una unica responsabilidad de proporcionar los comandos de WeeklyPlan al usuario.
    /// OCP: Cumple con el principio en funcionalidades, se le pueden agregar extensiones a los comandos semanales.
    /// ISP: Cumple con el principio al no ser forzado a utilizar ni depender de objetos que no utiliza.
    /// DIP: Cumple con el principio, se creo la interfaz ICommand para independizar las dependencias entre las clases.
    /// Expert: Cumple con el patron debido a que esta clase es experta en la informacion que utiliza.
    /// Creator: Cumple con el patron debido a que esta clase es la responsable de las instancias creadas.
    /// </summary>
    public class WeeklyPlanCommand : ICommand
    {
        private Dictionary<string,string> weeklyPlanCommand = new Dictionary<string,string>(StringComparer.Create( CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase))
        {
            { "PlanesDelDia",  "SeeDayPlans" },
            { "AgregarPlan",   "AddPlan" },
            { "BorrarPlan",    "DeletePlan" },
            { "ModificarPlan", "ModifyPlan" },
            { "Formato",       "Format" },
        };

        //Command: Ejecucion deseada con el mensaje command.
        public void Command(MessageResponse msgR)
        {
            var msg = new StringBuilder("Elija qué es lo que desea modificar de la planificación semanal:\n");
            foreach(var pair in weeklyPlanCommand)
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
                        typeof(WeeklyPlanCommand).GetMethod(weeklyPlanCommand[msgReceived], BindingFlags.NonPublic | BindingFlags.Instance).Invoke(this, new object[1]{ msgR });
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
            new FormatCommand("weeklyPlan").Command(msgR);
        }

        //AddPlan: Procedimiento para agregar planes por el usuario.
        private void AddPlan(MessageResponse msgR)
        {
            msgR.userData.weeklyPlan.AddPlan(msgR);
        }

        //DeletePlan: Procedimiento para eliminar planes creados por el usuario.
        private void DeletePlan(MessageResponse msgR)
        {
            msgR.bot.SendMessage("¿En qué día se encuentra el plan que desea eliminar?", msgR.chatId);
            var msgReceived = msgR.bot.ReadMessage(msgR.chatId).ToLower();
            if(msgReceived.StartsWith("/"))
            {
                msgReceived = msgReceived.Substring(1);
            }

            CultureInfo CI = CultureInfo.CurrentCulture;
            Calendar calendar = CI.Calendar;

            DayOfWeek weekday = default(DayOfWeek);
            DateTime date = default(DateTime);
            switch(msgReceived)
            {
                case "lunes":
                case "monday":
                    weekday = DayOfWeek.Monday;
                    break;
                case "martes":
                case "tuesday":
                    weekday = DayOfWeek.Tuesday;
                    break;
                case "miercoles":
                case "miércoles":
                case "wednesday":
                    weekday = DayOfWeek.Wednesday;
                    break;
                case "jueves":
                case "thursday":
                    weekday = DayOfWeek.Thursday;
                    break;
                case "viernes":
                case "friday":
                    weekday = DayOfWeek.Friday;
                    break;
                case "sabado":
                case "sábado":
                case "saturday":
                    weekday = DayOfWeek.Saturday;
                    break;
                case "domingo":
                case "sunday":
                    weekday = DayOfWeek.Sunday;
                    break;
                case "hoy":
                    date = DateTime.Today;
                    break;
                case "ayer":
                    DateTime time = DateTime.Today.AddDays(-1);

                    if(calendar.GetWeekOfYear(time, CI.DateTimeFormat.CalendarWeekRule, CI.DateTimeFormat.FirstDayOfWeek) != calendar.GetWeekOfYear(DateTime.Today, CI.DateTimeFormat.CalendarWeekRule, CI.DateTimeFormat.FirstDayOfWeek))
                    {
                        msgR.bot.SendMessage("Debe ser una fecha de esta semana.", msgR.chatId);
                    }
                    else
                    {
                        date = time;
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
                        date = time;
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
                        date = time;
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
                        date = time;
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
                        if(calendar.GetWeekOfYear(dateTime, CI.DateTimeFormat.CalendarWeekRule, CI.DateTimeFormat.FirstDayOfWeek) != calendar.GetWeekOfYear(DateTime.Today, CI.DateTimeFormat.CalendarWeekRule, CI.DateTimeFormat.FirstDayOfWeek))
                        {
                            msgR.bot.SendMessage("Debe ser una fecha de esta semana.", msgR.chatId);
                        }
                        else
                        {
                            date = dateTime;
                        }
                    }
                    catch(System.IndexOutOfRangeException)
                    {
                        msgR.bot.SendMessage("No se eligió un día de la semana.\n", msgR.chatId);
                    }
                    break;
            }
            if(date != default(DateTime) || weekday != default(DayOfWeek))
            {
                if(date == default(DateTime))
                {
                    date = DateTime.Today;
                    if(date.DayOfWeek != weekday)
                    {
                        int daysTo = (Convert.ToInt32(weekday) - Convert.ToInt32(date.DayOfWeek)) % 7;
                        date = date.AddDays(daysTo);
                    }
                }

                if(msgR.userData.weeklyPlan.ShowDayPlans(date).Count != 0)
                {
                    msgR.userData.weeklyPlan.DeletePlan(date, msgR);
                }
                else
                {
                    msgR.bot.SendMessage("No tiene planes para este día.", msgR.chatId);
                }
            }
        }

        //ModifyPlan: Procedimiento para modificar planes del usuario.
        private void ModifyPlan(MessageResponse msgR)
        {
            msgR.userData.weeklyPlan.ModifyPlan(msgR);
        }

        //SeeDayPlans: Procedimiento para ver los planes del usuario por dias.
        private void SeeDayPlans(MessageResponse msgR)
        {
            msgR.bot.SendMessage("¿De qué día desea ver su planificación semanal?", msgR.chatId);
            var msgReceived = msgR.bot.ReadMessage(msgR.chatId).ToLower();
            if(msgReceived.StartsWith("/"))
            {
                msgReceived = msgReceived.Substring(1);
            }

            CultureInfo CI = CultureInfo.CurrentCulture;
            Calendar calendar = CI.Calendar;

            DayOfWeek weekday = default(DayOfWeek);
            DateTime date = default(DateTime);
            switch(msgReceived)
            {
                case "lunes":
                case "monday":
                    weekday = DayOfWeek.Monday;
                    break;
                case "martes":
                case "tuesday":
                    weekday = DayOfWeek.Tuesday;
                    break;
                case "miercoles":
                case "miércoles":
                case "wednesday":
                    weekday = DayOfWeek.Wednesday;
                    break;
                case "jueves":
                case "thursday":
                    weekday = DayOfWeek.Thursday;
                    break;
                case "viernes":
                case "friday":
                    weekday = DayOfWeek.Friday;
                    break;
                case "sabado":
                case "sábado":
                case "saturday":
                    weekday = DayOfWeek.Saturday;
                    break;
                case "domingo":
                case "sunday":
                    weekday = DayOfWeek.Sunday;
                    break;
                case "hoy":
                    date = DateTime.Today;
                    break;
                case "ayer":
                    DateTime time = DateTime.Today.AddDays(-1);

                    if(calendar.GetWeekOfYear(time, CI.DateTimeFormat.CalendarWeekRule, CI.DateTimeFormat.FirstDayOfWeek) != calendar.GetWeekOfYear(DateTime.Today, CI.DateTimeFormat.CalendarWeekRule, CI.DateTimeFormat.FirstDayOfWeek))
                    {
                        msgR.bot.SendMessage("Debe ser una fecha de esta semana.", msgR.chatId);
                    }
                    else
                    {
                        date = time;
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
                        date = time;
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
                        date = time;
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
                        date = time;
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
                        if(calendar.GetWeekOfYear(dateTime, CI.DateTimeFormat.CalendarWeekRule, CI.DateTimeFormat.FirstDayOfWeek) != calendar.GetWeekOfYear(DateTime.Today, CI.DateTimeFormat.CalendarWeekRule, CI.DateTimeFormat.FirstDayOfWeek))
                        {
                            msgR.bot.SendMessage("Debe ser una fecha de esta semana.", msgR.chatId);
                        }
                        else
                        {
                            date = dateTime;
                        }
                    }
                    catch(System.IndexOutOfRangeException)
                    {
                        msgR.bot.SendMessage("No se eligió un día de la semana.\n", msgR.chatId);
                    }
                    break;
            }
            if(date != default(DateTime) || weekday != default(DayOfWeek))
            {
                if(date == default(DateTime))
                {
                    date = DateTime.Today;
                    if(date.DayOfWeek != weekday)
                    {
                        int daysTo = (Convert.ToInt32(weekday) - Convert.ToInt32(date.DayOfWeek)) % 7;
                        date = date.AddDays(daysTo);
                    }
                }

                if(msgR.userData.weeklyPlan.ShowDayPlans(date).Count != 0)
                {
                    var message = "Su planificación para este día es:\n";
                    foreach(var plan in msgR.userData.weeklyPlan.ShowDayPlans(date))
                    {
                        message += "- ";
                        if(plan.ActivityTime.Second != 0)
                        {
                            message += plan.ActivityTime.ToString("HH:mm") + ", ";
                        }
                        message += plan.Goal + "\n";
                    }
                    msgR.bot.SendMessage(message, msgR.chatId);
                }
                else
                {
                    msgR.bot.SendMessage("No tiene planes para este día.", msgR.chatId);
                }
            }
        }
    }
}