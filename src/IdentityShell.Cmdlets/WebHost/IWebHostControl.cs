namespace IdentityShell.Cmdlets.WebHost
{
    public interface IWebHostControl
    {
        void Start(string[] args);

        void Stop();
    }
}