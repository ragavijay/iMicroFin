namespace iMicroFin.Services
{
    public interface IPathService
    {
        string WebRootPath { get; }
    }
    public class PathService : IPathService
    {
        private readonly IWebHostEnvironment _env;

        public PathService(IWebHostEnvironment env)
        {
            _env = env;
        }
        public string WebRootPath => _env.WebRootPath;
    }
}
