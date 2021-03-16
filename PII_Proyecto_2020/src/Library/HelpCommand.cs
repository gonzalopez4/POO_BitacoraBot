using System.Threading;
using System.Text;

namespace Library
{
    /// <summary>
    /// HelloCommand: Clase que implementa la interfaz ICommand, con el objetivo de brindarle al usuario un medio de ayuda para entender como utilizar su bitacora.
    /// 
    /// Principios y patrones:
    /// ISP: Cumple con el principio al no ser forzado a utilizar ni depender de objetos que no utiliza.
    /// DIP: Cumple con el principio, se creo la interfaz ICommand para independizar las dependencias entre las clases.
    /// <summary>
    public class HelpCommand : ICommand
    {
        //Command: Ejecucion deseada con el mensaje command.
        public void Command(MessageResponse msgR)
        {
            Thread.Sleep(300);  
            var msg = new StringBuilder("Entre los principales comandos que puedes usar están:\n")
            .Append("/ReflexionMetacognitiva, para la reflexión de las clases, ")
            .Append("/ReflexionSemanal, para la reflexión de toda tu semana, ")
            .Append("/ObjetivosSemanales, para las victorias o metas que te propongas en la semana, ")
            .Append("/PlanificacionSemanal, para registrar los planes de tu semana, ")
            .Append("y /Guardar para que te envíe tu bitácora. 😊")
            .Append("\nIntentaré comprender tus mensajes lo mejor posible, pero si deseas ver todos los comandos que puedo leer, puedes enviarme /Comandos.");
            msgR.bot.SendMessage(msg.ToString(), msgR.chatId);
        }
    }
}