namespace Library
{
    /// <summary>
    /// ICommand: Interfaz creada para la implementacion de comandos, entre algunos: formato, canales de ayuda, entre otros.
    /// 
    /// Principios y patrones: 
    /// SRP: Utiliza el principio.
    /// ISP: Interfaz creada para utilizar este principio.
    /// DIP: Interfaz creada para utilizar este principio.
    /// Polymorphism: Aplica el patron.
    /// </summary>
    public interface ICommand
    {
        //Command: Ejecucion deseada con el mensaje command.
        void Command(MessageResponse msgR);
    }
}
