using System.IO;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace PII_Word_API
{
    public interface IContent
    {
        void Add(Body doc, DocumentPosition position);
    }
}