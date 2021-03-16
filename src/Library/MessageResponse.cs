using System;
using System.Collections.Generic;
using System.Globalization;

namespace Library
{
    /// <summary>
    /// MessageResponse: Clase que cumple la funcion de menú principal para brindarle las opciones al usuario.
    /// 
    /// Principios y patrones:
    /// SRP: Utiliza el principio al tener la unica responsabilidad de brindarle el panel de opciones al usuario.
    /// OCP: Utiliza el principio al poder ser extendido en funcionalidades sin verse afectado el resto del codigo.
    /// Polymorphism: Aplica el patron.
    /// <summary>
    public class MessageResponse
    {
        public UserDataSaver userData;

        public readonly string name;
        public readonly int chatId;
        public IBot bot;

        //msgSwitch: Diccionario con los comandos posibles como Key, y el comando a ejecutar como Value.
        public Dictionary<string, Type> msgSwitch = new Dictionary<string, Type>(StringComparer.Create( CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase))
        {
            { "/start",                  typeof(StartCommand) },
            { "Comenzar",                typeof(HelpCommand) },
            { "Arrancar",                typeof(HelpCommand) },
            { "Empezar",                 typeof(HelpCommand) },
            { "Help",                    typeof(HelpCommand) },
            { "Ayuda",                   typeof(HelpCommand) },
            { "Comandos",                typeof(CommandsCommand) },
            { "Comando",                 typeof(CommandsCommand) },
            { "Commands",                typeof(CommandsCommand) },
            { "Command",                 typeof(CommandsCommand) },
            { "Hola",                    typeof(HelloCommand) },
            { "Hello",                   typeof(HelloCommand) }, 
            { "Holis",                   typeof(HelloCommand) },
            { "Buenas",                  typeof(HelloCommand) }, 
            { "ReflexionMetacognitiva",  typeof(MetacogRefCommand) }, 
            { "RefleccionMetacognitiva", typeof(MetacogRefCommand) },
            { "Metacognitiva",           typeof(MetacogRefCommand) },
            { "Metacog",                 typeof(MetacogRefCommand) },
            { "ReflexionDeLaClase",      typeof(MetacogRefCommand) },
            { "ReflexionSemanal",        typeof(WeeklyRefCommand) }, 
            { "ReflexionDeLaSemana",     typeof(WeeklyRefCommand) },
            { "ReflexionSemana",         typeof(WeeklyRefCommand) },
            { "RefleccionSemanal",       typeof(WeeklyRefCommand) }, 
            { "RefleccionDeLaSemana",    typeof(WeeklyRefCommand) },
            { "RefleccionSemana",        typeof(WeeklyRefCommand) },
            { "ObjetivosSemanales",      typeof(WeeklyObjCommand) }, 
            { "VictoriasSemanales",      typeof(WeeklyObjCommand) },
            { "MetasSemanales",          typeof(WeeklyObjCommand) },
            { "Objetivos",               typeof(WeeklyObjCommand) }, 
            { "Victorias",               typeof(WeeklyObjCommand) },
            { "Metas",                   typeof(WeeklyObjCommand) },
            { "PlanificacionSemanal",    typeof(WeeklyPlanCommand) }, 
            { "PlanificacionDeLaSemana", typeof(WeeklyPlanCommand) },
            { "PlanesDeLaSemana",        typeof(WeeklyPlanCommand) },
            { "Planificacion",           typeof(WeeklyPlanCommand) },
            { "Planificar",              typeof(WeeklyPlanCommand) },
            { "Planes",                  typeof(WeeklyPlanCommand) },
            { "GuardarDocumento",        typeof(DocModifierCommand) },
            { "ExportarDocumento",       typeof(DocModifierCommand) },
            { "DescargarDocumento",      typeof(DocModifierCommand) },
            { "GuardarWord",             typeof(DocModifierCommand) },
            { "ExportarWord",            typeof(DocModifierCommand) },
            { "DescargarWord",           typeof(DocModifierCommand) },
            { "Guardar",                 typeof(DocModifierCommand) },
            { "Exportar",                typeof(DocModifierCommand) },
            { "Descargar",               typeof(DocModifierCommand) },
            { "Notificaciones",          typeof(NotificationCommand) },
            { "PrincipioSemestre",       typeof(NotificationCommand) },
            { "InicioSemestre",          typeof(NotificationCommand) },
            { "FinalSemestre",           typeof(NotificationCommand) },
            { "PrincipioDelSemestre",    typeof(NotificationCommand) },
            { "InicioDelSemestre",       typeof(NotificationCommand) },
            { "FinalDelSemestre",        typeof(NotificationCommand) },
            { "InicioDeSemestre",        typeof(NotificationCommand) },
            { "FinalDeSemestre",         typeof(NotificationCommand) }
        };

        public MessageResponse (IBot iBot, string sendername, int senderChatId)
        {
            name = sendername;
            chatId = senderChatId;
            bot = iBot;
            userData = new UserDataSaver().Read(chatId);
            MessageSwitch();
        }

        private void MessageSwitch()
        {
            var msg = string.Join("", bot.ReadMessage(chatId).Split(" "));
            try
            {
                if (msg.StartsWith("/") && String.Compare(msg, "/start", CultureInfo.CurrentCulture, CompareOptions.IgnoreCase) != 0)
                {
                    msg = msg.Substring(1);
                }
                ICommand command = (ICommand) Activator.CreateInstance(msgSwitch[msg]);
                command.Command(this);
            }
            catch (KeyNotFoundException)
            {
                bot.SendMessage($"{name}, no comprendo lo que dices 😕", chatId);
            }
        }
    }
}