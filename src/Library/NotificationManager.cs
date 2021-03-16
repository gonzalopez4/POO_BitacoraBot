using System;
using System.Collections.Generic;
using System.Threading;


namespace Library
{
    /// <summary>
    /// NotificationManager: Clase encargada de controlar qué notificaciones enviar.
    /// 
    /// Principios y Patrones:
    /// SRP: Cumple el principio al tener solo la responsabilidad de ver qué notificaciones enviar y cuales no.
    /// </summary>
    public class NotificationManager
    {
        //Notifications: Lista de notificaciones registradas.
        public static List<INotification> Notifications {get; set;} = new List<INotification>();

        //SendNotification: Método encargado de comparar la hora y enviar la notificación.
        public static void SendNotification(object callback)
        {
            if(Notifications.Count != 0)
            {
                foreach(var not in Notifications)
                {
                    if(not.Time.DayOfWeek.CompareTo(DateTime.Today.DayOfWeek) == 0 && not.Time.Hour.CompareTo(DateTime.Today.Hour) == 0 && not.Time.Minute.CompareTo(DateTime.Today.Minute) == 0)
                    {
                        not.Send();
                    }
                }
            }
        }
    }
}
