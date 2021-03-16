namespace Library
{
    /// <summary>
    /// IBot: Interfaz creada para la implementacion del bot.
    /// 
    /// Principios y patrones: 
    /// SRP: Utiliza el principio al tener la unica responsabilidad del flujo de mensajes.
    /// OCP: No utiliza el principio al no ser una interfaz pensada ni diseñada para una futura extension.
    /// ISP: Interfaz creada para utilizar este principio.
    /// DIP: Interfaz creada para utilizar este principio.
    /// Expert: Utiliza el patron al ser el experto de la informacion que utiliza.
    /// Polymorphism: Aplica el patron.
    /// </summary>
    public interface IBot
    {
        //Start: Método para ejecutar el bot.
        void Start();

        //SendMessage: Procedimiento utilizado para enviar mensajes.
        void SendMessage(string text, int chatId);

        void SendDocument(string path, int chatId, string text = null);

        //ReadMessage: Procedimiento utilizado para leer el texto recibido en el mensaje.
        string ReadMessage(int chatId);
    }
}
