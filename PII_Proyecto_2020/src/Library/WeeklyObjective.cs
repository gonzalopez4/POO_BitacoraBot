using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Globalization;
using System.Threading;

namespace Library
{
    /// <summary>
    /// WeeklyObjective: Clase que implementa la interfaz IElement, encargada de contener y modificar los objetivos de la semana.
    /// 
    /// Principios y patrones:
    /// SRP: Cumple el principio al tener solo una responsabilidad, almacenar los WeeklyObjective's.
    /// OCP: Cumpliria el principio en funcionalidades, no lo cumpliria si se quisiera agregar un dia mas a nuestra semana de siete dias.
    /// ISP: Cumple con el principio al no ser forzado a utilizar ni depender de objetos que no utiliza.
    /// DIP: Cumple con el principio, se creo la interfaz IElement para independizar las dependencias entre las clases.
    /// Expert: Cumple el patron al ser experto en la informacion que utiliza.
    /// </summary>

    [JsonObject("WeeklyObjective", MemberSerialization = MemberSerialization.OptIn)]
    public class WeeklyObjective : IElement
    {
        //Title: Texto del título escrito en la bitácora para los objetivos semanales.
        [JsonProperty("Title")]
        public string Title {get; set;}

        //Objectives: Array de Objectives que representa todos los objetivos semanales. Son tres o más, también llamado Victorias.
        [JsonProperty("Objectives")]
        private Objective[] Objectives {get; set;}

        //Format: Diccionario con los datos correspondientes al formato de los objetivos.
        [JsonProperty("Format")]
        public Dictionary<string, Dictionary<string, string>> Format {get; set;} = new Dictionary<string, Dictionary<string, string>>(StringComparer.Create( CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase));

        // public WeeklyObjective()
        // {
        //     this.Objectives = new Objective[3];
        // }

        public WeeklyObjective()
        {
            this.Objectives = new Objective[3];
        }

        [JsonConstructor]
        public WeeklyObjective(int Ammount)
        {
            this.Objectives = new Objective[Ammount];
        }
        
        // Create(): Método encargado de la creación de WeeklyObjective con las condiciones necesarias.
        public static WeeklyObjective Create(MessageResponse msgR)
        {
            msgR.bot.SendMessage("Ingrese la cantidad de victorias semanales que quiere tener.", msgR.chatId);
            int? n = null;
            while(n == null)
            {
                string msg = "Debe ingresar un número.";
                try
                {
                    n = Convert.ToInt32(msgR.bot.ReadMessage(msgR.chatId));
                    if(n < 3)
                    {
                        msg = "Deben ser mínimo 3 objetivos.";
                        throw new System.FormatException();
                    }
                }
                catch(System.FormatException)
                {
                    msgR.bot.SendMessage(msg, msgR.chatId);
                    n = null;
                }
            }
            return new WeeklyObjective(Convert.ToInt32(n));
        }

        public bool FullObjectives()
        {
            var list = Objectives.ToList<Objective>();
            list.RemoveAll( delegate(Objective obj){ return obj == null; } );
            return list.Count == Objectives.Length;
        }

        public List<Objective> ShowObjectives()
        {
            var list = Objectives.ToList<Objective>();
            list.RemoveAll( delegate(Objective obj){ return obj == null; } );
            return list;
        }

        public void ModifyObjectiveAmmount(MessageResponse msgR)
        {
            msgR.bot.SendMessage("Ingrese la cantidad de objetivos semanales que desea tener, o /atras para volver.", msgR.chatId);

            string msgReceived = null;
            while(String.Compare(msgReceived, "atras", CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) != 0)
            {
                msgReceived = msgR.bot.ReadMessage(msgR.chatId);
                if(msgReceived.StartsWith("/"))
                {
                    msgReceived = msgReceived.Substring(1);
                }

                if(String.Compare(msgReceived, "atras", CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) != 0)
                {
                    string msg = "Debe ingresar un número, o /atras para volver.";
                    try
                    {
                        int n = Convert.ToInt32(msgReceived);
                        if(n < 3)
                        {
                            msg = "Deben ser mínimo 3 objetivos.";
                            throw new System.FormatException();
                        }

                        var list = Objectives.ToList<Objective>();
                        list.RemoveAll( delegate(Objective obj){ return obj == null; } );
                        while(String.Compare(msgReceived, "atras", CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) != 0)
                        {
                            while(String.Compare(msgReceived, "atras", CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) != 0 && n < list.Count)
                            {
                                var amm = (list.Count - n);
                                var s = "";
                                if(amm != 1)
                                {
                                    s = "s";
                                }
                                else
                                {
                                    s = "";
                                }
                                msg = "Tiene menos objetivos que antes, debe eliminar " + amm + " objetivo" + s + " antes de continuar.\n¿Qué objetivos desea borrar?\n";
                                for( int i = 1; i <= list.Count; i++ )
                                {
                                    msg += "/" + i + " - " + list[i-1].Goal + "\n";
                                }
                                msgR.bot.SendMessage(msg + "\nO ingrese /atras para volver", msgR.chatId);

                                msgReceived = msgR.bot.ReadMessage(msgR.chatId);
                                if(msgReceived.StartsWith("/"))
                                {
                                    msgReceived = msgReceived.Substring(1);
                                }

                                if(String.Compare(msgReceived, "atras", CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) != 0)
                                {
                                    var chose = msgReceived
                                                .Split(" ", StringSplitOptions.RemoveEmptyEntries)
                                                .ToList<string>();
                                    chose.RemoveAll( delegate(string str) { return (!Regex.IsMatch(str, @"\d+") || Convert.ToInt32(str) > list.Count); });

                                    foreach( var i in chose.OrderByDescending(i => i) )
                                    {
                                        list.RemoveAt(Convert.ToInt32(i) - 1);
                                    }
                                }
                            }
                            if(String.Compare(msgReceived, "atras", CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) != 0)
                            {
                                var newArray = new Objective[Convert.ToInt32(n)];
                                for( int i = 0; i < list.Count; i++)
                                {
                                    newArray[i] = list[i];
                                }
                                this.Objectives = newArray;
                                msgR.bot.SendMessage("La cantidad de objetivos se modificó correctamente.", msgR.chatId);
                                msgReceived = "atras";
                            }
                        }
                    }
                    catch(System.FormatException)
                    {
                        msgR.bot.SendMessage(msg, msgR.chatId);
                    }
                }
            }
        }
        
        //AddObjective: Procedimiento para agregar un objetivo.
        public void AddObjective(MessageResponse msgR)
        {
            try
            {
                var ind = Array.FindIndex<Objective>(Objectives, delegate(Objective obj){ return obj == null; });
                if(ind == -1)
                {
                    throw new System.IndexOutOfRangeException();
                }

                msgR.bot.SendMessage("Ingrese su objetivo.", msgR.chatId);
                string goal = Convert.ToString(msgR.bot.ReadMessage(msgR.chatId));
                if(string.IsNullOrEmpty(goal))
                {
                    msgR.bot.SendMessage("El objetivo no puede ser vacío.", msgR.chatId);
                }
                else
                {
                    Objectives[ind] = new Objective(goal);
                    msgR.bot.SendMessage("El objetivo se agregó correctamente.", msgR.chatId);
                }
            }
            catch(System.IndexOutOfRangeException)
            {
                msgR.bot.SendMessage("Ya tiene " + Objectives.Length + " objetivos guardados.", msgR.chatId);
                Thread.Sleep(300);
                ModifyObjective(msgR);
            }
        }

        //DeleteObjective: Procedimiento para eliminar un objetivo.
        public void DeleteObjective(MessageResponse msgR)
        {
            string message = "¿Qué objetivo semanal desea eliminar?\n";
            for( int i = 1; i <= Objectives.Length; i++)
            {
                if(Objectives[i-1] != null)
                {
                    message += "/" + i + " - " + Objectives[i-1].Goal + "\n";
                }
            }

            string msgReceived = null;
            if(message == "¿Qué objetivo semanal desea eliminar?\n")
            {
                msgR.bot.SendMessage("No tiene objetivos semanales guardados.", msgR.chatId);
                msgReceived = "atras";
            }
            else
            {
                msgR.bot.SendMessage(message += "\nIngrese un número, o /atras para volver.", msgR.chatId);
            }
            
            while(String.Compare(msgReceived, "atras", CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) != 0)
            {
                msgReceived =msgR.bot.ReadMessage(msgR.chatId);
                if(msgReceived.StartsWith("/"))
                {
                    msgReceived = msgReceived.Substring(1);
                }

                if(String.Compare(msgReceived, "atras", CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) != 0)
                {
                    try
                    {
                        int n = Convert.ToInt32(msgReceived);
                        Objectives[n-1] = null;
                        for (int i = Objectives.Length-2; i >= 0; i--)
                        {
                            if (Objectives[i+1] != null && Objectives[i+1] == null)
                            {
                                Objectives[i] = Objectives[i+1];
                                Objectives[i+1] = null;
                            }
                        }

                        msgR.bot.SendMessage("El objetivo se eliminó correctamente.", msgR.chatId);
                        msgReceived = "atras";
                    }
                    catch(System.FormatException)
                    {
                        msgR.bot.SendMessage("Debe ingresar un número correspondiente a un objetivo, o /atras para volver.", msgR.chatId);
                    }
                }
            }
        }

        //ModifyObjective: Procedimiento para modificar un objetivo.
        public void ModifyObjective(MessageResponse msgR)
        {
            string message = "¿Qué objetivo semanal desea cambiar?\n";
            for( int i = 1; i <= Objectives.Length; i++)
            {
                if(Objectives[i-1] != null)
                {
                    message += ("/" + i + " - " + Objectives[i-1].Goal + "\n");
                }
            }
            msgR.bot.SendMessage(message += "\nIngrese un número, o /atras para volver.", msgR.chatId);

            string msgReceived = null;
            while(String.Compare(msgReceived, "atras", CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) != 0)
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
                        int n = Convert.ToInt32(msgReceived);
                        if(n > Objectives.Length)
                        {
                            throw new System.FormatException();
                        }

                        msgR.bot.SendMessage("Ingrese el nuevo objetivo.", msgR.chatId);
                        string goal = Convert.ToString(msgR.bot.ReadMessage(msgR.chatId));
                        if(string.IsNullOrEmpty(goal))
                        {
                            msgR.bot.SendMessage("El objetivo no puede ser vacío.", msgR.chatId);
                        }
                        else
                        {
                            Objectives[n-1] = new Objective(goal);
                            msgR.bot.SendMessage("El objetivo se agregó correctamente.", msgR.chatId);
                            msgReceived = "atras";
                        }
                    }
                    catch(System.FormatException)
                    {
                        msgR.bot.SendMessage("Debe ingresar un número correspondiente a un objetivo, o /atras para volver.", msgR.chatId);
                    }
                }
            }
        }
    }
}
