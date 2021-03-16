using System;

namespace Library
{
    /// <summary>
    /// IDocumentModifier: Interfaz creada con el objetivo de invertir las dependencias con cada clase que implemente esta interfaz. 
    /// 
    /// Principios y patrones: 
    /// ISP: Interfaz creada para utilizar este principio.
    /// DIP: Interfaz creada para utilizar este principio.
    /// </summary>

    public interface IDocumentModifier
    {
        void Create(MessageResponse msgR);

        void Modify(MessageResponse msgR);
    }
}
