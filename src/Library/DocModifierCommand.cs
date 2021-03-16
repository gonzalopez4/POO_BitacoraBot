using System;
using System.IO;
using System.Text;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using PII_Word_API;
using System.Collections.Generic;
using System.Collections;
using TelegramApi;
using System.Linq;
using System.Threading;
using System.Reflection;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System.Text.Json;

namespace Library
{
    /// <summary>
    /// DocumentModifier: Clase principal(main) encargada de unir la informacion de todas las clases.
    /// 
    /// Principios y patrones:
    /// ISP: Cumple con el principio al no ser forzado a utilizar ni depender de objetos que no utiliza.
    /// DIP: Cumple con el principio, se creo la interfaz ICommand para independizar las dependencias entre las clases.
    /// </summary>
    public class DocModifierCommand : ICommand
    {
        public static Dictionary<string, Type> exportTypes = new Dictionary<string, Type>(StringComparer.Create( CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase))
        {
            { "Word",     typeof(WordDocumentModifier) },
            //{ "Markdown", typeof(MarkdownDocumentModifier) }
        };

        private Dictionary<string, string> elementMethods = new Dictionary<string, string>(StringComparer.Create( CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase))
        {
            { "ReflexionMetacognitiva", "metacogRefCheck" },
            { "ReflexionSemanal",       "weeklyRefCheck" },
            { "ObjetivosSemanales",     "weeklyObjCheck" },
            { "PlanificacionSemanal",   "weeklyPlanCheck" },
            { "Orden", "orderCheck" }
        };

        public void Command(MessageResponse msgR)
        {
            try
            {
                foreach(var method in elementMethods)
                {
                    if((bool) typeof(DocModifierCommand).GetMethod(method.Value, BindingFlags.NonPublic | BindingFlags.Instance).Invoke(this, new object[1]{ msgR }))
                    {
                        throw new KeyNotFoundException();
                    }
                }

                if(exportTypes.Count != 1)
                {
                    var msg = "¿En qué formato de los siguientes quiere exportarlo?\n";
                    foreach(var pair in exportTypes)
                    {
                        msg += "- /" + pair.Key + "\n";
                    }
                    msgR.bot.SendMessage(msg + "\nIngrese uno de los siguientes, o /atras para salir.", msgR.chatId);

                    string msgReceived = null;
                    while(String.Compare(msgReceived, "atras", CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) != 0 && !exportTypes.ContainsKey(msgReceived))
                    {
                        msgReceived = msgR.bot.ReadMessage(msgR.chatId);
                        if(msgReceived.StartsWith("/"))
                        {
                            msgReceived = msgReceived.Substring(1);
                        }

                        if(String.Compare(msgReceived, "atras", CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) != 0 && !exportTypes.ContainsKey(msgReceived))
                        {
                            msgR.bot.SendMessage("Ese formato no está soportado, ingrese uno de los anteriormente mencionados o /atras para salir.", msgR.chatId);
                        }
                    }
                    if(String.Compare(msgReceived, "atras", CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) != 0)
                    {
                        IDocumentModifier docMod = (IDocumentModifier) Activator.CreateInstance(exportTypes[msgReceived]);

                        if(File.Exists(@"..\Userdata\" + msgR.chatId + ".docx"))
                        {
                            msgR.bot.SendMessage("¿Desea agregar una nueva entrada a su bitácora o recibir la ya guardada con los cambios de formato?\n- /Nueva\n- /Guardada", msgR.chatId);
                            while(String.Compare(msgReceived, "atras", CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) != 0)
                            {
                                msgReceived = msgR.bot.ReadMessage(msgR.chatId);
                                if(msgReceived.StartsWith("/"))
                                {
                                    msgReceived = msgReceived.Substring(1);
                                }

                                if(String.Compare(msgReceived, "atras", CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) != 0)
                                {
                                    if(String.Compare(msgReceived, "nueva", CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) == 0)
                                    {
                                        docMod.Create(msgR);
                                        msgReceived = "atras";
                                    }
                                    else if(String.Compare(msgReceived, "guardada", CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) == 0)
                                    {
                                        docMod.Modify(msgR);
                                        msgReceived = "atras";
                                    }
                                    else
                                    {
                                        msgR.bot.SendMessage("Ingrese una de las opciones anteriores, o /atras para volver.", msgR.chatId);
                                    }
                                }
                            }
                        }
                        else
                        {
                            docMod.Create(msgR);
                        }
                    }
                }
                else
                {
                    IDocumentModifier docMod = (IDocumentModifier) Activator.CreateInstance(exportTypes.First().Value);
                    
                    if(File.Exists(@"..\Userdata\" + msgR.chatId + ".docx"))
                    {
                        string msgReceived = null;
                        msgR.bot.SendMessage("¿Desea agregar una nueva entrada a su bitácora o recibir la ya guardada con los cambios de formato?\n- /Nueva\n- /Guardada", msgR.chatId);
                        while(String.Compare(msgReceived, "atras", CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) != 0)
                        {
                            msgReceived = msgR.bot.ReadMessage(msgR.chatId);
                            if(msgReceived.StartsWith("/"))
                            {
                                msgReceived = msgReceived.Substring(1);
                            }

                            if(String.Compare(msgReceived, "atras", CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) != 0)
                            {
                                if(String.Compare(msgReceived, "nueva", CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) == 0)
                                {
                                    docMod.Create(msgR);
                                    msgR.bot.SendDocument(@"..\Userdata\" + msgR.chatId + ".docx", msgR.chatId, "Aqui tienes tu bitácora 😜");
                                    msgReceived = "atras";
                                }
                                else if(String.Compare(msgReceived, "guardada", CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) == 0)
                                {
                                    docMod.Modify(msgR);
                                    msgR.bot.SendDocument(@"..\Userdata\" + msgR.chatId + ".docx", msgR.chatId, "Aqui tienes tu bitácora 😋");
                                    msgReceived = "atras";
                                }
                                else
                                {
                                    msgR.bot.SendMessage("Ingrese una de las opciones anteriores, o /atras para volver.", msgR.chatId);
                                }
                            }
                        }
                    }
                    else
                    {
                        docMod.Create(msgR);
                        msgR.bot.SendDocument(@"..\Userdata\" + msgR.chatId + ".docx", msgR.chatId, "Aqui tienes tu bitácora 🤩");
                    }
                }
                msgR.bot.SendMessage("Ingrese un nuevo comando, o ingrese /help para ver qué comandos puede utilizar.", msgR.chatId);
            }
            catch(KeyNotFoundException)
            {}
        }

        private bool metacogRefCheck(MessageResponse msgR)
        {
            if(msgR.userData.metacogRef.Format.Count != exportTypes.Count)
            {
                foreach(var exp in exportTypes)
                {
                    if(!msgR.userData.metacogRef.Format.ContainsKey(exp.Key))
                    {
                        msgR.userData.metacogRef.Format.Add(exp.Key, new Dictionary<string, string>());
                    }
                }
            }

            if(msgR.userData.metacogRef.Text == null)
            {
                msgR.bot.SendMessage("No tiene una reflexión metacognitiva guardada.\n¿Desea agregar una?", msgR.chatId);
                var msgReceived = msgR.bot.ReadMessage(msgR.chatId).ToLower();
                if( msgReceived.StartsWith("si") || msgReceived.StartsWith("sí") || msgReceived.StartsWith("yes") || msgReceived == "y" || msgReceived.StartsWith("obvio") || msgReceived.Contains("dale") || msgReceived.Contains("claro que si") || msgReceived == "claro" || msgReceived.Contains("ya sabes"))
                {
                    new MetacogRefCommand().Command(msgR);
                    return true;
                }
            }
            return false;
        }

        private bool weeklyRefCheck(MessageResponse msgR)
        {
            if(msgR.userData.weeklyRef.Format.Count != exportTypes.Count)
            {
                foreach(var exp in exportTypes)
                {
                    if(!msgR.userData.weeklyRef.Format.ContainsKey(exp.Key))
                    {
                        msgR.userData.weeklyRef.Format.Add(exp.Key, new Dictionary<string, string>());
                    }
                }
            }

            if(msgR.userData.weeklyRef.Text == null)
            {
                msgR.bot.SendMessage("No tiene una reflexión semanal guardada.\n¿Desea agregar una?", msgR.chatId);
                var msgReceived = msgR.bot.ReadMessage(msgR.chatId).ToLower();
                if( msgReceived.StartsWith("si") || msgReceived.StartsWith("sí") || msgReceived.StartsWith("yes") || msgReceived == "y" || msgReceived.StartsWith("obvio") || msgReceived.Contains("dale") || msgReceived.Contains("claro que si") || msgReceived == "claro" || msgReceived.Contains("ya sabes") )
                {
                    new WeeklyRefCommand().Command(msgR);
                    return true;
                }
            }
            return false;
        }

        private bool weeklyObjCheck(MessageResponse msgR)
        {
            if(msgR.userData.weeklyObj.Format.Count != exportTypes.Count)
            {
                foreach(var exp in exportTypes)
                {
                    if(!msgR.userData.weeklyObj.Format.ContainsKey(exp.Key))
                    {
                        msgR.userData.weeklyObj.Format.Add(exp.Key, new Dictionary<string, string>());
                    }
                }
            }

            if(!msgR.userData.weeklyObj.FullObjectives())
            {
                msgR.bot.SendMessage("Su lista de objetivos semanales no esta completa.\n¿Desea completarla?", msgR.chatId);
                var msgReceived = msgR.bot.ReadMessage(msgR.chatId).ToLower();
                if( msgReceived.StartsWith("si") || msgReceived.StartsWith("sí") || msgReceived.StartsWith("yes") || msgReceived == "y" || msgReceived.StartsWith("obvio") || msgReceived.Contains("dale") || msgReceived.Contains("claro que si") || msgReceived == "claro" || msgReceived.Contains("ya sabes") )
                {
                    new WeeklyObjCommand().Command(msgR);
                    return true;
                }
            }
            return false;
        }

        private bool weeklyPlanCheck(MessageResponse msgR)
        {
            if(msgR.userData.weeklyPlan.Format.Count != exportTypes.Count)
            {
                foreach(var exp in exportTypes)
                {
                    if(!msgR.userData.weeklyPlan.Format.ContainsKey(exp.Key))
                    {
                        msgR.userData.weeklyPlan.Format.Add(exp.Key, new Dictionary<string, string>());
                    }
                }
            }

            if(msgR.userData.weeklyPlan.WeekDaysEmpty().Count != 0)
            {
                var weekDaysEmpty = msgR.userData.weeklyPlan.WeekDaysEmpty();

                if(weekDaysEmpty.Count > 1)
                {
                    var last = weekDaysEmpty[weekDaysEmpty.Count - 1];
                    weekDaysEmpty.RemoveAt(weekDaysEmpty.Count - 1);
                    msgR.bot.SendMessage("Los días " + string.Join(", ", weekDaysEmpty) + " y " + last + " no tienen ningun plan.\n¿Desea agregarles?", msgR.chatId);
                }
                else
                {
                    msgR.bot.SendMessage("El día " + weekDaysEmpty[0] + " no tiene ningun plan\n¿Desea agregarle?", msgR.chatId);
                }

                var msgReceived = msgR.bot.ReadMessage(msgR.chatId).ToLower();
                if( msgReceived.StartsWith("si") || msgReceived.StartsWith("sí") || msgReceived.StartsWith("yes") || msgReceived == "y" || msgReceived.StartsWith("obvio") || msgReceived.Contains("dale") || msgReceived.Contains("claro que si") || msgReceived == "claro" || msgReceived.Contains("ya sabes") )
                {
                    new WeeklyPlanCommand().Command(msgR);
                    return true;
                }
            }
            return false;
        }

        private bool orderCheck(MessageResponse msgR)
        {
            var ord = msgR.userData.order;
            string msg1 = "No tiene un orden guardado para los elementos de su bitácora. ";
            if(ord != null)
            {
                var elements = elementMethods;
                elements.Remove("Orden");

                var msg = "El orden de elementos en su bitácora es:\n";
                foreach(var element in msgR.userData.order)
                {
                    msg += "- " + elements.First( delegate(KeyValuePair<string,string> pair){ return pair.Value.Substring(0, pair.Value.Length-5) == element; } ).Key + "\n";
                }
                msgR.bot.SendMessage(msg + "\n¿Desea modificarlo?", msgR.chatId);

                var msgReceived = msgR.bot.ReadMessage(msgR.chatId).ToLower();
                if( msgReceived.StartsWith("si") || msgReceived.StartsWith("sí") || msgReceived.StartsWith("yes") || msgReceived == "y" || msgReceived.StartsWith("obvio") || msgReceived.Contains("dale") || msgReceived.Contains("claro que si") || msgReceived == "claro" || msgReceived.Contains("ya sabes") )
                {
                    ord = null;
                    msg1 = null;
                }
            }
            if(ord == null)
            {
                var elements = elementMethods;
                elements.Remove("Orden");

                List<string> order = new List<string>();
                string msgReceived = null;
                while(String.Compare(msgReceived, "atras", CultureInfo.CurrentCulture, CompareOptions.IgnoreCase) != 0 && order.Count != elements.Count)
                {
                    msg1 += "Ingrese los siguientes elementos de su bitácora en el orden deseado.\n";
                    foreach(var element in elements)
                    {
                        if(order.Contains(element.Value.Substring(0, element.Value.Length-5)))
                        {
                            msg1 += (order.IndexOf(element.Value.Substring(0, element.Value.Length-5)) + 1) + "- /" + element.Key + "\n";
                        }
                        else
                        {
                            msg1 += "  - /" + element.Key + "\n";
                        }
                    }
                    msgR.bot.SendMessage(msg1 + "\nO ingrese /atras para volver.", msgR.chatId);
                    msg1 = null;

                    msgReceived = msgR.bot.ReadMessage(msgR.chatId);
                    if(msgReceived.StartsWith("/"))
                    {
                        msgReceived = msgReceived.Substring(1);
                    }

                    if(String.Compare(msgReceived, "atras", CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) != 0)
                    {
                        if(elements.ContainsKey(msgReceived))
                        {
                            order.Add(elements[msgReceived].Substring(0, elements[msgReceived].Length-5));
                        }
                        else
                        {
                            msgR.bot.SendMessage("Ese no es un elemento válido, ingrese uno de los anteriormente mencionados o /atras para volver.", msgR.chatId);
                            Thread.Sleep(300);
                        }
                    }
                }
                if(String.Compare(msgReceived, "atras", CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) == 0)
                {
                    Thread.Sleep(300);
                    msgR.bot.SendMessage("Ingrese un nuevo comando, o ingrese /help para ver qué comandos puede utilizar.", msgR.chatId);
                    return true;
                }
                msgR.userData.order = order.ToArray();
                msgR.userData.Save(msgR.chatId);
            }
            return false;
        }
    }
}

