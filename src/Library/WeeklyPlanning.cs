using System;
using System.Globalization;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Library
{
    /// <summary>
    /// WeeklyPlanning: Clase que implementa la interfaz IElement, encargada de contener y modificar la planificación de la semana día a día.
    /// 
    /// Principios y patrones:
    /// SRP: Cumple el principio al tener solo una responsabilidad, administrar los planes semalaes del usuario.
    /// OCP: Cumple con el principio en funcionalidades pero está limitado a nuestra semana de siete dias, no lo cumpliria si se quisiera agregar un dia más a la semana.
    /// ISP: Cumple con el principio al no ser forzado a utilizar ni depender de objetos que no utiliza.
    /// DIP: Cumple con el principio, se creo la interfaz IElement para independizar las dependencias entre las clases.
    /// Expert: Cumple con el patron debido a que esta clase es experta en la informacion que utiliza.
    /// Creator: Cumple con el patron debido a que esta clase es la responsable de las instancias creadas.
    /// </summary>

    [JsonObject("WeeklyPlanning", MemberSerialization = MemberSerialization.OptIn)]
    public class WeeklyPlanning : IElement
    {
        //Title: Texto del título escrito en la bitácora para los planes de la semana.
        [JsonProperty("Title")]
        public string Title {get; set;}

        //PlanningDays: Array de "Plan" de tamaño 7 que representa los 7 dias de la semana.
        //El índice representa el día, por ejemplo: lunes tiene índice 0 y domingo tiene índice 6.
        [JsonProperty("PlanningDays")]
        private List<Plan>[] PlanningDays {get; set;} = new List<Plan>[7]{ new List<Plan>(), new List<Plan>(), new List<Plan>(),
                                                                           new List<Plan>(), new List<Plan>(), new List<Plan>(),
                                                                           new List<Plan>() };

        //Format: Diccionario con los datos correspondientes al formato de los planes semanales.
        [JsonProperty("Format")]
        public Dictionary<string, Dictionary<string, string>> Format {get; set;} = new Dictionary<string, Dictionary<string, string>>(StringComparer.Create( CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase));

        //ShowDayPlan: Método para mostrar la planificación de un día de la semana.
        public List<Plan> ShowDayPlans(DateTime time)
        {
            List<Plan> plans = new List<Plan>();
            switch(time.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    plans = PlanningDays[1];
                    break;
                case DayOfWeek.Tuesday:
                    plans = PlanningDays[2];
                    break;
                case DayOfWeek.Wednesday:
                    plans = PlanningDays[3];
                    break;
                case DayOfWeek.Thursday:
                    plans = PlanningDays[4];
                    break;
                case DayOfWeek.Friday:
                    plans = PlanningDays[5];
                    break;
                case DayOfWeek.Saturday:
                    plans = PlanningDays[6];
                    break;
                case DayOfWeek.Sunday:
                    plans = PlanningDays[0];
                    break;
            }
            return plans;
        }

        //AddPlan: Método para agregar una planificación.
        public void AddPlan(MessageResponse msgR)
        {
            CultureInfo CI = CultureInfo.CurrentCulture;
            Calendar calendar = CI.Calendar;

            try
            {
                msgR.bot.SendMessage("Ingrese lo que planea hacer.", msgR.chatId);
                string goal = msgR.bot.ReadMessage(msgR.chatId);
                if(string.IsNullOrEmpty(goal))
                {
                    msgR.bot.SendMessage("El plan no puede ser vacío.", msgR.chatId);
                    throw new System.FormatException();
                }
                msgR.bot.SendMessage("Ingrese la fecha y hora del plan, o /atras para volver.\nUtilice el formato \"dd/mm hh:mm\".", msgR.chatId);

                string msg = null;
                DateTime time = default(DateTime);
                while(String.Compare(msg, "atras", CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) != 0 && time == default(DateTime))
                {
                    msg = msgR.bot.ReadMessage(msgR.chatId);
                    if(msg.StartsWith("/"))
                    {
                        msg = msg.Substring(1);
                    }

                    if(String.Compare(msg, "atras", CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) != 0)
                    {
                        try
                        {
                            int day;
                            int month;
                            int year;
                            int hours;
                            int minutes;
                            int seconds;

                            var ind = Array.IndexOf(msg.ToCharArray(), Convert.ToChar("/"));
                            if(ind >= 2)
                            {
                                day = Convert.ToInt32(msg.Substring(ind-2, 2));
                            }
                            else if(ind == 1)
                            {
                                day = Convert.ToInt32(msg.Substring(ind-1, 1));
                            }
                            else
                            {
                                throw new System.IndexOutOfRangeException();
                            }
                            
                            var msg1 = msg.Substring(ind+1);
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

                            ind = Array.IndexOf(msg.ToCharArray(), Convert.ToChar(":"));
                            if(ind >= 2)
                            {
                                hours = Convert.ToInt32(msg.Substring(ind-2, 2));
                                if(msg.Substring(ind+1).Length >=2)
                                {
                                    minutes = Convert.ToInt32(msg.Substring(ind+1, 2));
                                    seconds = 1;
                                }
                                else if(msg.Substring(ind+1).Length == 1)
                                {
                                    minutes = Convert.ToInt32(msg.Substring(ind+1, 1));
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
                                hours = Convert.ToInt32(msg.Substring(ind-1, 1));
                                if(msg.Substring(ind+1).Length >=2)
                                {
                                    minutes = Convert.ToInt32(msg.Substring(ind+1, 2));
                                    seconds = 1;
                                }
                                else if(msg.Substring(ind+1).Length == 1)
                                {
                                    minutes = Convert.ToInt32(msg.Substring(ind+1, 1));
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
                                msgR.bot.SendMessage("La fecha del plan debe ser de esta semana.", msgR.chatId);
                            }
                            else
                            {
                                time = dateTime;
                            }
                        }
                        catch(Exception ex) when (ex is System.IndexOutOfRangeException || ex is System.FormatException)
                        {
                            msgR.bot.SendMessage("Esa no es una fecha válida, asegurese de que usó el formato \"dd/mm hh:mm\", o ingrese /atras para volver.", msgR.chatId);
                        }
                    }
                }
                
                if(String.Compare(msg, "atras", CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) != 0)
                {
                    Add(new Plan(goal, time));
                    msgR.bot.SendMessage("El plan se agregó correctamente.", msgR.chatId);
                }
            }
            catch(System.FormatException)
            {
                msgR.bot.SendMessage("No se agregó el plan.", msgR.chatId);
            }
        }

        private void Add(Plan plan)
        {
            switch(plan.ActivityTime.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    PlanningDays[1].Add(plan);
                    PlanningDays[1].Sort(delegate(Plan obj1, Plan obj2){ return DateTime.Compare(obj1.ActivityTime, obj2.ActivityTime); });
                    break;
                case DayOfWeek.Tuesday:
                        PlanningDays[2].Add(plan);
                    PlanningDays[2].Sort(delegate(Plan obj1, Plan obj2){ return DateTime.Compare(obj1.ActivityTime, obj2.ActivityTime); });
                    break;
                case DayOfWeek.Wednesday:
                    PlanningDays[3].Add(plan);
                    PlanningDays[3].Sort(delegate(Plan obj1, Plan obj2){ return DateTime.Compare(obj1.ActivityTime, obj2.ActivityTime); });
                    break;
                case DayOfWeek.Thursday:
                    PlanningDays[4].Add(plan);
                    PlanningDays[4].Sort(delegate(Plan obj1, Plan obj2){ return DateTime.Compare(obj1.ActivityTime, obj2.ActivityTime); });
                    break;
                case DayOfWeek.Friday:
                    PlanningDays[5].Add(plan);
                    PlanningDays[5].Sort(delegate(Plan obj1, Plan obj2){ return DateTime.Compare(obj1.ActivityTime, obj2.ActivityTime); });
                    break;
                case DayOfWeek.Saturday:
                    PlanningDays[6].Add(plan);
                    PlanningDays[6].Sort(delegate(Plan obj1, Plan obj2){ return DateTime.Compare(obj1.ActivityTime, obj2.ActivityTime); });
                    break;
                case DayOfWeek.Sunday:
                    PlanningDays[0].Add(plan);
                    PlanningDays[0].Sort(delegate(Plan obj1, Plan obj2){ return DateTime.Compare(obj1.ActivityTime, obj2.ActivityTime); });
                    break;
            }
        }

        //DeletePlan: Método para eliminar una planificación.
        public void DeletePlan(DateTime time, MessageResponse msgR)
        {
            var weekday = time.DayOfWeek;
            var message = "¿Qué plan desea eliminar?\n";
            int i = 1;
            foreach(var plan in msgR.userData.weeklyPlan.ShowDayPlans(time))
            {
                message += "/" + i + "- ";
                if(plan.ActivityTime.Second != 0)
                {
                    message += plan.ActivityTime.ToString("HH:mm") + ", ";
                }
                message += plan.Goal + "\n";
                i++;
            }
            msgR.bot.SendMessage(message + "Ingrese un número.", msgR.chatId);

            var msg = msgR.bot.ReadMessage(msgR.chatId);
            if(msg.StartsWith("/"))
            {
                msg = msg.Substring(1);
            }

            try
            {
                int n = Convert.ToInt32(msg);
                PlanningDays[Convert.ToInt32(weekday)].RemoveAt(n-1);
                msgR.bot.SendMessage("El plan se eliminó correctamente.", msgR.chatId);
            }
            catch(System.FormatException)
            {
                msgR.bot.SendMessage("No se eliminó ninguno de los planes.", msgR.chatId);
            }
        }

        //ModifyPlan: Método para modificar una planificación.
        public void ModifyPlan(MessageResponse msgR)
        {
            string msgError = null;
            try
            {
                msgR.bot.SendMessage("¿En qué día se encuentra su plan?\nEj: - /Lunes\n      - 03/10", msgR.chatId);

                var msgReceived = msgR.bot.ReadMessage(msgR.chatId).Trim().ToLower();
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
                                msgR.bot.SendMessage("La fecha del plan debe ser de esta semana.", msgR.chatId);
                            }
                            else
                            {
                                date = dateTime;
                            }
                        }
                        catch(Exception ex) when (ex is System.IndexOutOfRangeException || ex is System.FormatException)
                        {
                            msgError = "No se eligió un día de la semana.\n";
                            throw new System.FormatException();
                        }
                        break;
                }

                if(date == default(DateTime))
                {
                    date = DateTime.Today;
                    if(date.DayOfWeek != weekday)
                    {
                        int daysTo = (Convert.ToInt32(weekday) - Convert.ToInt32(date.DayOfWeek)) % 7;
                        date = date.AddDays(daysTo);
                    }
                }
                else
                {
                    weekday = date.DayOfWeek;
                }

                string msg = null;
                if(msgR.userData.weeklyPlan.ShowDayPlans(date).Count != 0)
                {
                    var message = "¿Qué plan desea modificar?\n";
                    int i = 1;
                    foreach(var plan in msgR.userData.weeklyPlan.ShowDayPlans(date))
                    {
                        message += "/" + i + "- ";
                        if(plan.ActivityTime.Second != 0)
                        {
                            message += plan.ActivityTime.ToString("HH:mm") + ", ";
                        }
                        message += plan.Goal + "\n";
                        i++;
                    }
                    msgR.bot.SendMessage(message + "\nIngrese un número, o /atras para volver.", msgR.chatId);
                }
                else
                {
                    msgR.bot.SendMessage("No tiene planes para este día.", msgR.chatId);
                    msg = "atras";
                }

                while(String.Compare(msg, "atras", CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) != 0)
                {
                    msg = msgR.bot.ReadMessage(msgR.chatId);
                    if(msg.StartsWith("/"))
                    {
                        msg = msg.Substring(1);
                    }
                    
                    if(String.Compare(msg, "atras", CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) != 0)
                    {
                        try
                        {
                            int n = Convert.ToInt32(msg);

                            msgR.bot.SendMessage("Ingrese su plan, o envíe \".\" para mantener el actual:\n\"" + PlanningDays[Convert.ToInt32(weekday)][n-1].Goal + "\".", msgR.chatId);
                            string goal = null;
                            while(goal == null)
                            {
                                goal = msgR.bot.ReadMessage(msgR.chatId);

                                if(string.IsNullOrEmpty(goal))
                                {
                                    msgR.bot.SendMessage("El plan no puede ser vacío.", msgR.chatId);
                                    goal = null;
                                }
                                if(goal == ".")
                                {
                                    goal = PlanningDays[Convert.ToInt32(weekday)][n-1].Goal;
                                }
                            }

                            string actualDate;
                            if(PlanningDays[Convert.ToInt32(weekday)][n-1].ActivityTime.Second != 0)
                            {
                                actualDate = PlanningDays[Convert.ToInt32(weekday)][n-1].ActivityTime.ToString("dddd dd/MM/yyyy hh:mm").ToUpper()[0] + PlanningDays[Convert.ToInt32(weekday)][n-1].ActivityTime.ToString("dddd dd/MM/yyyy hh:mm").Substring(1);
                            }
                            else
                            {
                                actualDate = PlanningDays[Convert.ToInt32(weekday)][n-1].ActivityTime.ToString("dddd dd/MM/yyyy").ToUpper()[0] + PlanningDays[Convert.ToInt32(weekday)][n-1].ActivityTime.ToString("dddd dd/MM/yyyy").Substring(1);
                            }
                            msgR.bot.SendMessage("Ingrese la fecha y hora del plan, utilice el formato \"dd/mm hh:mm\", o envíe \".\" para mantener el actual:\n" + actualDate + ".", msgR.chatId);
                            date = default(DateTime);
                            while(date == default(DateTime))
                            {
                                string read = msgR.bot.ReadMessage(msgR.chatId);

                                if(read == ".")
                                {
                                    date = PlanningDays[Convert.ToInt32(weekday)][n-1].ActivityTime;
                                }
                                else
                                {
                                    try
                                    {
                                        int day;
                                        int month;
                                        int year;
                                        int hours;
                                        int minutes;
                                        int seconds;

                                        var ind = Array.IndexOf(read.ToCharArray(), Convert.ToChar("/"));
                                        if(ind >= 2)
                                        {
                                            day = Convert.ToInt32(read.Substring(ind-2, 2));
                                        }
                                        else if(ind == 1)
                                        {
                                            day = Convert.ToInt32(read.Substring(ind-1, 1));
                                        }
                                        else
                                        {
                                            throw new System.IndexOutOfRangeException();
                                        }
                                        
                                        var msg1 = read.Substring(ind+1);
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

                                        ind = Array.IndexOf(read.ToCharArray(), Convert.ToChar(":"));
                                        if(ind >= 2)
                                        {
                                            hours = Convert.ToInt32(read.Substring(ind-2, 2));
                                            if(read.Substring(ind+1).Length >=2)
                                            {
                                                minutes = Convert.ToInt32(read.Substring(ind+1, 2));
                                                seconds = 1;
                                            }
                                            else if(read.Substring(ind+1).Length == 1)
                                            {
                                                minutes = Convert.ToInt32(read.Substring(ind+1, 1));
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
                                            hours = Convert.ToInt32(read.Substring(ind-1, 1));
                                            if(read.Substring(ind+1).Length >=2)
                                            {
                                                minutes = Convert.ToInt32(read.Substring(ind+1, 2));
                                                seconds = 1;
                                            }
                                            else if(read.Substring(ind+1).Length == 1)
                                            {
                                                minutes = Convert.ToInt32(read.Substring(ind+1, 1));
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
                                            msgR.bot.SendMessage("La fecha del plan debe ser de esta semana.", msgR.chatId);
                                        }
                                        else
                                        {
                                            date = dateTime;
                                        }
                                    }
                                    catch(Exception ex) when (ex is System.IndexOutOfRangeException || ex is System.FormatException)
                                    {
                                        msgR.bot.SendMessage("Esa no es una fecha válida, asegurese de que usó el formato \"dd/mm hh:mm\", o ingrese /atras para volver.", msgR.chatId);
                                    }
                                }
                            }
                            if(PlanningDays[Convert.ToInt32(weekday)][n-1].Goal == goal && PlanningDays[Convert.ToUInt32(weekday)][n-1].ActivityTime == date)
                            {
                                msgR.bot.SendMessage("No se modificó ninguno de los planes.", msgR.chatId);
                                msg = "atras";
                            }
                            else
                            {
                                PlanningDays[Convert.ToInt32(weekday)].RemoveAt(n-1);
                                Add(new Plan(goal, date));
                                msgR.bot.SendMessage("El plan se modificó correctamente.", msgR.chatId);
                                msg = "atras";
                            }
                        }
                        catch(System.FormatException)
                        {
                            msgR.bot.SendMessage("Debe ingresar un número correspondiente a un plan, o /atras para volver.", msgR.chatId);
                        }
                    }
                }
            }
            catch(System.FormatException)
            {
                msgR.bot.SendMessage(msgError + "No se modifcó ninguno de los planes.", msgR.chatId);
            }
        }

        public List<string> WeekDaysEmpty()
        {
            List<string> weekDaysEmpty = new List<string>(); 
            foreach(var weekday in new DayOfWeek[7]{ DayOfWeek.Sunday, DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday })
            {
                var date = DateTime.Today;
                if(date.DayOfWeek != weekday)
                {
                    int daysTo = (Convert.ToInt32(weekday) - Convert.ToInt32(date.DayOfWeek)) % 7;
                    date = date.AddDays(daysTo);
                }

                if(ShowDayPlans(date).Count == 0)
                {
                    weekDaysEmpty.Add(date.ToString("dddd", new CultureInfo("es-ES")));
                }
            }
            return weekDaysEmpty;
        }
    }
}
