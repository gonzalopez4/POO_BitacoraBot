using System;
using PII_Word_API;
using TelegramApi;
using Library;
using System.Linq;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using System.Collections.Generic;
using DocumentFormat.OpenXml;

namespace Program
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Bienvenido a tu bitácora personal!");
            TelegramService bot = new TelegramService();
            var timer = new Timer(new TimerCallback(NotificationManager.SendNotification), null, 10000, 35);
            bot.Start();
            timer.Dispose();
        }
    }
}
