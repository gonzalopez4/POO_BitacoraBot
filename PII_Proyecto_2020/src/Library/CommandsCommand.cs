using System.Linq;
using System.Threading;
using System.Text;

namespace Library
{
    /// <summary>
    /// CommandsCommand: Clase que implementa la interfaz ICommand, con el objetivo de brindar los comandos disponibles del usuario.
    /// 
    /// Principios y patrones:
    /// ISP: Cumple con el principio al no ser forzado a utilizar ni depender de objetos que no utiliza.
    /// DIP: Cumple con el principio, se creo la interfaz ICommand para independizar las dependencias entre las clases.
    /// <summary>
    public class CommandsCommand : ICommand
    {
        //Command: Ejecucion deseada con el mensaje command.
        public void Command(MessageResponse msgR)
        {
            Thread.Sleep(300);
            var msg = new StringBuilder("Los comandos que puedo leer son los siguientes:\n");
            for( int i = 1; i < msgR.msgSwitch.Count; i++)
            {
                msg.Append("- /" + msgR.msgSwitch.ElementAt(i).Key + "\n");
            }
            msgR.bot.SendMessage(msg.ToString(), msgR.chatId);
        }
    }
}