using System.IO;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace PII_Word_API
{
    /// <summary>
    /// Visita una tabla de contenido para construir el elemento
    /// Tabla de Word.
    /// </summary>
    internal class WordTableBuilderVisitor: ITableVisitor
    {
        public Table Table { get; } = new Table(); 

        private TableRow currentRow = null;

        /// <summary>
        /// Construye la tabla (la raíz).
        /// </summary>
        /// <param name="table">La tabla de contenido.</param>
        public void Visit(TableContent table)
        {
            foreach (var row in table.Rows)
            {
                row.Accept(this);
            }
        }

        /// <summary>
        /// Construye una fila de la tabla.
        /// </summary>
        /// <param name="row"></param>
        public void Visit(TableContentRow row)
        {
            TableRow tableRow = new TableRow();
            this.currentRow = tableRow;

            foreach (var cell in row.Cells)
            {
                cell.Accept(this);
            }

            this.Table.Append(tableRow);
        }

        /// <summary>
        /// Construye una celda de la tabla y la agrega a la
        /// fila actual.
        /// </summary>
        /// <param name="cell"></param>
        public void Visit(TableContentCell cell)
        {
            TableCell tableCell = new TableCell();
            Paragraph paragraph = new Paragraph(new Run(new Text(cell.Content)));
            tableCell.Append(paragraph);
            this.currentRow.Append(tableCell);
        }
    }
}