using System.Threading;

namespace Library
{
    /// <summary>
    /// StartCommand: Clase que implementa la interfaz ICommand encargada de comenzar la interaccion del bot con el usuario.
    /// 
    /// Principios y patrones:
    /// SRP: Cumple el principio al tener solo una responsabilidad, comenzar la interaccion con el bot.
    /// ISP: Cumple con el principio al no ser forzado a utilizar ni depender de objetos que no utiliza.
    /// DIP: Cumple con el principio, se creo la interfaz ICommand para independizar las dependencias entre las clases que la utilizan.
    /// Expert: Cumple el patron al ser experto en la informacion que utiliza.
    /// <summary>
    public class StartCommand : ICommand
    {
        //Command: Ejecucion deseada con el mensaje command.
        public void Command(MessageResponse msgR)
        {
            msgR.bot.SendMessage($"¡Hola, {msgR.name}!\nSoy el bot Asistente de Bitácora, estoy aquí para ayudarte a crear tu bitácora facilmente.\nEmpecemos configurando lo basico:", msgR.chatId);
            Thread.Sleep(300);
            
            msgR.userData.metacogRef = new Reflection();
            msgR.userData.weeklyRef = new Reflection();
            msgR.userData.weeklyObj = WeeklyObjective.Create(msgR);
            msgR.userData.weeklyPlan = new WeeklyPlanning();
            msgR.userData.semester = Semester.Create(msgR);
            msgR.userData.metacogRef.Title = "Reflexión Metacognitiva";
            msgR.userData.weeklyRef.Title = "Reflexión Semanal";
            msgR.userData.weeklyPlan.Title = "Planificación Semanal";
            msgR.userData.weeklyObj.Title = "Objetivos Semanales";
            msgR.userData.Save(msgR.chatId);

            msgR.bot.SendMessage("¡Muy bien!\nAhora toca modificar los elementos de su bitácora.\nIngrese el nombre de uno de estos, o /help para ver los comandos que puedo leer.", msgR.chatId);
            Thread.Sleep(300);
        }
    }
}