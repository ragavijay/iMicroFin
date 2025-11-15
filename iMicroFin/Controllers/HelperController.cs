using Microsoft.AspNetCore.Mvc;

namespace iMicroFin.Controllers
{
    public class HelperController : Controller
    {
        private IWebHostEnvironment? _webHostEnvironment;

        public HelperController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public string? GetWebRootPath()
        {
            return _webHostEnvironment?.WebRootPath ?? "";
        }
    }
}