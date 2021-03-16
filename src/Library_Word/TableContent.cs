using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace PII_Word_API
{
    /// <summary>
    /// Representa una tabla de contenido.
    /// Se compone de filas (TableContentRow).
    /// </summary>
    public class TableContent: IContent, ITablePart
    {
        /// <typeparam name="TableContentRow">Las filas de la tabla.</typeparam>
        private List<TableContentRow> rows = new List<TableContentRow>();

        /// <value>Las filas de la table enumerables.</value>
        public IEnumerable<TableContentRow> Rows
        {
            get
            {
                return rows.AsEnumerable();
            }
        }

        public TableContent(List<TableContentRow> rows)
        {
            this.rows = rows;
        }

        /// <summary>
        /// Agrega la tabla a un cuerpo de documento provisto en la
        /// posición indicada.
        /// </summary>
        /// <param name="body">El cuerpo del documento</param>
        /// <param name="position">La posición.</param>
        public void Add(Body body, DocumentPosition position)
        {
            WordTableBuilderVisitor visitor = new WordTableBuilderVisitor();
            this.Accept(visitor);

            if (position == DocumentPosition.TOP)
            {
                body.InsertAt(visitor.Table, 0);
            }
            else
            {
                body.Append(visitor.Table);
            }
        }

        public void Accept(ITableVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}