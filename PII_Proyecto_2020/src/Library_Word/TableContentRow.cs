using System.Linq;
using System.Collections.Generic;
using System.IO;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace PII_Word_API
{
    /// <summary>
    /// Representa una fila de la tabla.
    /// Se compone de celdas (TableContentCell).
    /// </summary>
    public class TableContentRow: ITablePart
    {
        /// <typeparam name="TableContentCell">Las celdas de la tabla.</typeparam>
        private List<TableContentCell> cells = new List<TableContentCell>();

        /// <value>Las celdas de la tabla enumerables.</value>
        public IEnumerable<TableContentCell> Cells
        {
            get
            {
                return cells.AsEnumerable();
            }
        }

        public TableContentRow(List<TableContentCell> cells)
        {
            this.cells = cells;
        }

        public void Accept(ITableVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}