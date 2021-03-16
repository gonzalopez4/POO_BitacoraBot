namespace PII_Word_API
{
    public interface ITablePart 
    {
        void Accept(ITableVisitor visitor);
    }
}