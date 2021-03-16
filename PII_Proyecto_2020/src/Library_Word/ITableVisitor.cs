namespace PII_Word_API
{
    public interface ITableVisitor
    {
        void Visit(TableContent table);

        void Visit(TableContentRow row);

        void Visit(TableContentCell cell);
    }
}