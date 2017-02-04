namespace MyCore.Web.Security.AntiForgery
{
    public interface IAbpAntiForgeryManager
    {
        IAbpAntiForgeryConfiguration Configuration { get; }

        string GenerateToken();
    }
}
