using System;
using System.Threading;
using Newtonsoft.Json;

namespace Library
{
    public interface INotification
    {
        int ChatId {get; set;}

        DateTime Time {get; set;}

        void Send();
    }
}