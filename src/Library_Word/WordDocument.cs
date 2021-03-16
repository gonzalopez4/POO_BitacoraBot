using System;
using System.IO;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace PII_Word_API
{
    /// <summary>
    /// Representa un documento de Word.
    /// 
    /// Permite agregar contenido en distintos formatos (texto y tabla).
    /// </summary>
    public class WordDocument
    {
        /// <summary>
        /// La ruta al archivo.
        /// </summary>
        private string path;

        /// <param name="path">La ruta del archivo</param>
        public WordDocument(string path)
        {
            this.path = path;
        }

        /// <summary>
        /// Agrega contenido al documento en la posición especificada.
        /// Si el documento no existe, se crea automáticamente.
        /// </summary>
        /// <param name="content"></param>
        /// <param name="position"></param>
        public void AddContent(IContent content, DocumentPosition position = DocumentPosition.BOTTOM)
        {
            WordprocessingDocument wordDocument = null;

            try {
                Body docBody;

                if (File.Exists(this.path)) 
                {
                    wordDocument = WordprocessingDocument.Open(
                        this.path,
                        true
                    );

                    docBody = wordDocument.MainDocumentPart.Document.Body;
                }
                else
                {
                    wordDocument = WordprocessingDocument.Create(
                        this.path, 
                        WordprocessingDocumentType.Document, 
                        true
                    );

                    MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
                    mainPart.Document = new DocumentFormat.OpenXml.Wordprocessing.Document();
                    docBody = new Body();
                    mainPart.Document.Body = docBody;
                }

                content.Add(docBody, position);
            }
            finally 
            {
                if (wordDocument != null)
                {
                    wordDocument.Dispose();
                }
            }
        }

    }
}