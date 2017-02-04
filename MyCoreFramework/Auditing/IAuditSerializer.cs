namespace MyCoreFramework.Auditing
{
    public interface IAuditSerializer
    {
        string Serialize(object obj);
    }
}