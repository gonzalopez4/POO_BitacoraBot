using System.IO;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace PII_Word_API
{
    public class TableContentCell: ITablePart
    {
        public string Content { get; private set; }

        public TableContentCell(string content)
        {
            this.Content = content;
        }

        public void Accept(ITableVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}