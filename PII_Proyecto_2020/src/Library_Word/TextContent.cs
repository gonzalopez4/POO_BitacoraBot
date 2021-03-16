using System.Reflection.Metadata;
using System.IO;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace PII_Word_API
{
    /// <summary>
    /// Representa una porción de contenido de texto.
    /// </summary>
    public class TextContent: IContent
    {
        /// <summary>
        /// El contenido.
        /// </summary>
        private string content;

        public TextContent(string content)
        {
            this.content = content;
        }

        /// <summary>
        /// Agrega el contenido al cuerpo del documento indicado en la posición
        /// indicada.
        /// </summary>
        /// <param name="body">El cuerpo del documento</param>
        /// <param name="position">La posición.</param>
        public void Add(Body body, DocumentPosition position)
        {
            Run run = new Run();
            Paragraph paragraph = new Paragraph();
            Text text = new Text(content);

            run.Append(text);
            paragraph.Append(run);

            if (position == DocumentPosition.TOP)
            {
                body.InsertAt(paragraph, 0);
            }
            else
            {
                body.Append(paragraph);
            }
        }
    }
}