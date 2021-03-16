using System;
using System.Linq;
using System.Threading;
using System.Text;
using System.Collections.Generic;
using System.Globalization;

namespace Library
{
    /// <summary>
    /// FormatCommand: Clase que implementa la interfaz ICommand, con el objetivo de brindarle un formato al usuario.
    /// 
    /// Principios y patrones:
    /// ISP: Cumple con el principio al no ser forzado a utilizar ni depender de objetos que no utiliza.
    /// DIP: Cumple con el principio, se creo la interfaz ICommand para independizar las dependencias entre las clases.
    /// <summary>
    public class FormatCommand : ICommand
    {
        private string FCkey;
        public FormatCommand(string formatCommandKey)
        {
            FCkey = formatCommandKey;
        }

        //formatCommand: Diccionario con los comandos posibles como Key, y el comando a ejecutar como Value.
        private static Dictionary<string,Dictionary<string,string>> formatCommand = new Dictionary<string,Dictionary<string,string>>()
        {
            { "metacogRef", new Dictionary<string,string>(StringComparer.Create( CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase))
                {
                    { "FuenteDelTitulo",          "Ingrese el tipo de letra que desea para el título de la reflexión.\nEj: Arial" },
                    { "TamanoDelTitulo",          "Ingrese el tamaño de letra que desea para el título de la reflexión.\nEj: 12" },
                    { "AlineacionDelTitulo",      "Ingrese la alineacion que desea para el título de la reflexión.\nEj: Centro, Justificado" },
                    { "ColorDelTitulo",           "Ingrese el color de letra que desea para el título de la reflexión. Si quiere ser mas especifico, puede ingresarlo en hexadecimal.\nEj: Rojo, #e0a051" },
                    { "InterlineadoDelTitulo",    "Ingrese el interlineado que desea para el título de la reflexión.\nEj: 1,5" },
                    { "EspaciadoAnteriorTitulo",  "Ingrese el espaciado anterior que desea para el título de la reflexión.\nEj: 8" },
                    { "EspaciadoPosteriorTitulo", "Ingrese el espaciado posterior que desea para el título de la reflexión.\nEj: 8" },
                    { "FuenteDeLetra",            "Ingrese el tipo de letra que desea para el texto de la reflexión.\nEj: Arial" },
                    { "TamanoDeLetra",            "Ingrese el tamaño de letra que desea para el texto de la reflexión.\nEj: 12" },
                    { "AlineacionDeLetra",        "Ingrese la alineacion que desea para el texto de la reflexión.\nEj: Centro, Justificado" },
                    { "ColorDeLetra",             "Ingrese el color de letra que desea para el texto de la reflexión. Si quiere ser mas especifico, puede ingresarlo en hexadecimal.\nEj: Rojo, #e0a051" },
                    { "InterlineadoDeLetra",      "Ingrese el interlineado de letra que desea para el texto de la reflexión.\nEj: 1,5" },
                    { "EspaciadoAnteriorLetra",   "Ingrese el espaciado anterior que desea para el texto de la reflexión.\nEj: 8" },
                    { "EspaciadoPosteriorLetra",  "Ingrese el espaciado posterior que desea para el texto de la reflexión.\nEj: 8" }
                }
            }, 
            { "weeklyRef", new Dictionary<string,string>(StringComparer.Create( CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase))
                {
                    { "FuenteDelTitulo",          "Ingrese el tipo de letra que desea para el título de la reflexión.\nEj: Arial" },
                    { "TamanoDelTitulo",          "Ingrese el tamaño de letra que desea para el título de la reflexión.\nEj: 12" },
                    { "AlineacionDelTitulo",      "Ingrese la alineacion que desea para el título de la reflexión.\nEj: Centro, Justificado" },
                    { "ColorDelTitulo",           "Ingrese el color de letra que desea para el título de la reflexión. Si quiere ser mas especifico, puede ingresarlo en hexadecimal.\nEj: Rojo, #e0a051" },
                    { "InterlineadoDelTitulo",    "Ingrese el interlineado que desea para el título de la reflexión.\nEj: 1,5" },
                    { "EspaciadoAnteriorTitulo",  "Ingrese el espaciado anterior que desea para el título de la reflexión.\nEj: 8" },
                    { "EspaciadoPosteriorTitulo", "Ingrese el espaciado posterior que desea para el título de la reflexión.\nEj: 8" },
                    { "FuenteDeLetra",            "Ingrese el tipo de letra que desea para el texto de la reflexión.\nEj: Arial" },
                    { "TamanoDeLetra",            "Ingrese el tamaño de letra que desea para el texto de la reflexión.\nEj: 12" },
                    { "AlineacionDeLetra",        "Ingrese la alineacion que desea para el texto de la reflexión.\nEj: Centro, Justificado" },
                    { "ColorDeLetra",             "Ingrese el color de letra que desea para el texto de la reflexión. Si quiere ser mas especifico, puede ingresarlo en hexadecimal.\nEj: Rojo, #e0a051" },
                    { "InterlineadoDeLetra",      "Ingrese el interlineado de letra que desea para el texto de la reflexión.\nEj: 1,5" },
                    { "EspaciadoAnteriorLetra",   "Ingrese el espaciado anterior que desea para el texto de la reflexión.\nEj: 8" },
                    { "EspaciadoPosteriorLetra",  "Ingrese el espaciado posterior que desea para el texto de la reflexión.\nEj: 8" }
                }
            },
            { "weeklyObj", new Dictionary<string,string>(StringComparer.Create( CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase))
                {
                    { "FuenteDelTitulo",          "Ingrese el tipo de letra que desea para el título de las victorias semanales.\nEj: Arial" },
                    { "TamanoDelTitulo",          "Ingrese el tamaño de letra que desea para el título de las victorias semanales.\nEj: 12" },
                    { "AlineacionDelTitulo",      "Ingrese la alineacion que desea para el título de las victorias semanales.\nEj: Centro, Justificado" },
                    { "ColorDelTitulo",           "Ingrese el color de letra que desea para el título de las victorias semanales. Si quiere ser mas especifico, puede ingresarlo en hexadecimal.\nEj: Rojo, #e0a051" },
                    { "InterlineadoDelTitulo",    "Ingrese el interlineado que desea para el título de las victorias semanales.\nEj: 1,5" },
                    { "EspaciadoAnteriorTitulo",  "Ingrese el espaciado anterior que desea para el título de las victorias semanales.\nEj: 8" },
                    { "EspaciadoPosteriorTitulo", "Ingrese el espaciado posterior que desea para el título de las victorias semanales.\nEj: 8" },
                    { "FuenteDeLetra",            "Ingrese el tipo de letra que desea para el texto de las victorias semanales.\nEj: Arial" },
                    { "TamanoDeLetra",            "Ingrese el tamaño de letra que desea para el texto de las victorias semanales.\nEj: 12" },
                    { "AlineacionDeLetra",        "Ingrese la alineacion que desea para el texto de las victorias semanales.\nEj: Centro, Justificado" },
                    { "ColorDeLetra",             "Ingrese el color de letra que desea para el texto de las victorias semanales. Si quiere ser mas especifico, puede ingresarlo en hexadecimal.\nEj: Rojo, #e0a051" },
                    { "InterlineadoDeLetra",      "Ingrese el interlineado de letra que desea para el texto de las victorias semanales.\nEj: 1,5" },
                    { "EspaciadoAnteriorLetra",   "Ingrese el espaciado anterior que desea para el texto de las victorias semanales.\nEj: 8" },
                    { "EspaciadoPosteriorLetra",  "Ingrese el espaciado posterior que desea para el texto de las victorias semanales.\nEj: 8" }
                }
            },
            { "weeklyPlan", new Dictionary<string,string>(StringComparer.Create( CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase))
                {
                    { "FuenteDelTitulo",          "Ingrese el tipo de letra que desea para el título de la planificación semanal.\nEj: Arial" },
                    { "TamanoDelTitulo",          "Ingrese el tamaño de letra que desea para el título de la planificación semanal.\nEj: 12" },
                    { "AlineacionDelTitulo",      "Ingrese la alineacion que desea para el título de la planificación semanal.\nEj: Centro, Justificado" },
                    { "ColorDelTitulo",           "Ingrese el color de letra que desea para el título de la planificación semanal. Si quiere ser mas especifico, puede ingresarlo en hexadecimal.\nEj: Rojo, #e0a051" },
                    { "InterlineadoDelTitulo",    "Ingrese el interlineado que desea para el título de la planificación semanal.\nEj: 1,5" },
                    { "EspaciadoAnteriorTitulo",  "Ingrese el espaciado anterior que desea para el título de la planificación semanal.\nEj: 8" },
                    { "EspaciadoPosteriorTitulo", "Ingrese el espaciado posterior que desea para el título de la planificación semanal.\nEj: 8" },
                    { "FuenteDeLetra",            "Ingrese el tipo de letra que desea para el texto de la planificación semanal.\nEj: Arial" },
                    { "TamanoDeLetra",            "Ingrese el tamaño de letra que desea para el texto de la planificación semanal.\nEj: 12" },
                    { "AlineacionDeLetra",        "Ingrese la alineacion que desea para el texto de la planificación semanal.\nEj: Centro, Justificado" },
                    { "ColorDeLetra",             "Ingrese el color de letra que desea para el texto de la planificación semanal. Si quiere ser mas especifico, puede ingresarlo en hexadecimal.\nEj: Rojo, #e0a051" },
                    { "InterlineadoDeLetra",      "Ingrese el interlineado de letra que desea para el texto de la planificación semanal.\nEj: 1,5" },
                    { "EspaciadoAnteriorLetra",   "Ingrese el espaciado anterior que desea para el texto de la planificación semanal.\nEj: 8" },
                    { "EspaciadoPosteriorLetra",  "Ingrese el espaciado posterior que desea para el texto de la planificación semanal.\nEj: 8" }
                }
            }
        };

        //Command: Ejecucion deseada con el mensaje command.
        public void Command(MessageResponse msgR)
        {
            string msgReceived = null;
            StringBuilder msg = null;
            if(DocModifierCommand.exportTypes.Count != 1)
            {
                msg = new StringBuilder("El formato depende de la extensión del documento en que vaya a exportar la bitácora.\n¿De qué tipo de documento quiere modificar su formato?\n");
                foreach(var pair in DocModifierCommand.exportTypes)
                {
                    msg.Append("- /" + pair.Key + "\n");
                }
                msgR.bot.SendMessage(msg.Append("\nO ingrese /atras para volver.").ToString(), msgR.chatId);
            }
            var element = (IElement) typeof(UserDataSaver).GetField(FCkey).GetValue(msgR.userData);

            while(String.Compare(msgReceived, "atras", CultureInfo.CurrentCulture, CompareOptions.IgnoreCase) != 0)
            {
                if(DocModifierCommand.exportTypes.Count != 1)
                {
                    msgReceived = msgR.bot.ReadMessage(msgR.chatId);
                    if(msgReceived.StartsWith("/"))
                    {
                        msgReceived = msgReceived.Substring(1);
                    }
                }
                else
                {
                    msgReceived = DocModifierCommand.exportTypes.Keys.ElementAt(0);
                }
                
                if(String.Compare(msgReceived, "atras", CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) != 0)
                {
                    if(DocModifierCommand.exportTypes.ContainsKey(msgReceived))
                    {
                        if(!element.Format.ContainsKey(msgReceived))
                        {
                            element.Format.Add(msgReceived, new Dictionary<string, string>());
                        }
                        var formatDic = element.Format[msgReceived];

                        var msgDic = formatCommand[FCkey]; //Ej: weeklyObjective
                        var msg1 = new StringBuilder("Ingrese la característica a modificar:\n");
                        foreach(var pair in msgDic)
                        {
                            msg1.Append("- /" + pair.Key + "\n");
                        }
                        msg1.Append("\nO ingrese /atras para volver.");
                        msgR.bot.SendMessage(msg1.ToString(), msgR.chatId);

                        msgReceived = null; //Ej: /TipoDeLetraTitulo
                        while(String.Compare(msgReceived, "atras", CultureInfo.CurrentCulture, CompareOptions.IgnoreCase) != 0)
                        {
                            msgReceived = String.Join("", msgR.bot.ReadMessage(msgR.chatId).Split(" ", StringSplitOptions.RemoveEmptyEntries));
                            if(msgReceived.StartsWith("/"))
                            {
                                msgReceived = msgReceived.Substring(1);
                            }

                            if(String.Compare(msgReceived, "atras", CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) != 0)
                            {
                                try
                                {
                                    msgR.bot.SendMessage(msgDic[msgReceived], msgR.chatId);

                                    var key = msgDic.First( delegate(KeyValuePair<string,string> pair) { return pair.Value == msgDic[msgReceived]; } ).Key;
                                    var value = msgR.bot.ReadMessage(msgR.chatId);
                                    try
                                    {
                                        formatDic.Add(key, value);
                                    }
                                    catch(System.ArgumentException)
                                    {
                                        formatDic[key] = value;
                                    }

                                    msgR.userData.Save(msgR.chatId);
                                    msgR.bot.SendMessage("Formato guardado correctamente.", msgR.chatId);
                                    Thread.Sleep(300);
                                    msgR.bot.SendMessage(msg1.ToString(), msgR.chatId);
                                }
                                catch(KeyNotFoundException)
                                {
                                    msgR.bot.SendMessage("Característica invalida.\nIngrese una de las características mencionadas anteriormente o /atras para volver.", msgR.chatId);
                                }
                            }
                        }
                        msgReceived = null;
                        if(msg != null)
                        {
                            msgR.bot.SendMessage(msg.ToString(), msgR.chatId);
                        }
                        else
                        {
                            msgReceived = "atras";
                        }
                    }
                    else
                    {
                        msgR.bot.SendMessage("Ese tipo de documento no está soportado, ingrese uno de los listados anteriormente o ingrese /atras para volver.", msgR.chatId);
                    }
                }
            }
        }
    }
}