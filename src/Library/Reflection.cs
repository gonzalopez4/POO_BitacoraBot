using System;
using System.Collections.Generic;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Globalization;

namespace Library
{
    /// <summary>
    /// Reflection: Clase que implementa la interfaz IElement con el objetivo de brindarle el panel de reflexiones al usuario.
    /// 
    /// Principios y patrones:
    /// SRP: Cumple el principio al tener solo una responsabilidad, las reflexiones.
    /// OCP: Cumple el principio al poder extender cualquier tipo de Reflections(Weekly, Monthly, Annual) sin verse afectado.
    /// ISP: Cumple con el principio al no ser forzado a utilizar ni depender de objetos que no utiliza.
    /// DIP: Cumple con el principio, se creo la interfaz IElement para independizar las dependencias entre las clases.
    /// Expert: Cumple el patron al ser experto en la informacion que utiliza.
    /// </summary>

    [JsonObject("Reflection", MemberSerialization = MemberSerialization.OptIn)]
    public class Reflection : IElement
    {
        //Title: Texto del título escrito en la bitácora para la reflexión.
        [JsonProperty("Title")]
        public string Title {get; set;}

        //Reflection: Texto de la reflexión.
        [JsonProperty("Text")]
        public string Text {get; set;}

        //Format: Diccionario con los datos correspondientes al formato de la reflexión.
        [JsonProperty("Format")]
        public Dictionary<string, Dictionary<string, string>> Format {get; set;} = new Dictionary<string, Dictionary<string, string>>(StringComparer.Create( CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase));
    }
}
