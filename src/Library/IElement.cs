using System.Collections.Generic;

namespace Library
{
    /// <summary>
    /// IElement:
    /// 
    /// Principios y patrones: 
    /// SRP: Utiliza el principio
    /// ISP: Interfaz creada para utilizar este principio.
    /// DIP: Interfaz creada para utilizar este principio.

    public interface IElement
    {
        //Title: Texto del título escrito en la bitácora para la reflexión.
        string Title {get; set;}

        //Format: Diccionario con los datos correspondientes al formato de la reflexión.
        Dictionary<string, Dictionary<string, string>> Format {get; set;}
    }
}