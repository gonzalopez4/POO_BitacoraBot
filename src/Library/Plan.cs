using System;

namespace Library
{
    /// <summary>
    /// Plan: Clase que hereda de Objective, encargada de organizar por horarios los recordatorios.
    /// 
    /// Principios y patrones:
    /// SRP: Utiliza el principio de tener una sola responsabilidad, los planes.
    /// OCP: Utiliza el principio al poder ser extendido sin tener que ser modificado(aunque no fue pensado con ese proposito).
    /// Expert: Aplica el patron debido a que esta clase es experta en la informacion que utiliza.
    /// </summary>

    public class Plan : Objective
    {
        public Plan(string goal, DateTime time) : base(goal)
        {
            this.ActivityTime = time;
        }
        
        //Timetable: Tipo de horario "DateTime" para utilizar como referencia en la bitácora.
        public DateTime ActivityTime {get; set;}
    }
}
