using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using iMicroFin.DAO;
using iMicroFin.Models;

namespace iMicroFin.Controllers
{
    public class AppController : Controller
    {
        private readonly ILogger<AppController> _logger;

        public AppController(ILogger<AppController> logger)
        {
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Home()
        {
            string userType;
            userType = HttpContext.Session.GetString("userType");
            if (userType.Equals("A") || userType.Equals("D") || userType.Equals("M"))
            {
                return View();
            }
            else
            {
                ViewBag.ErrMsg = "Invalid Credentials";
                return View("Login");
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Home(LoginViewModel login)
        {
            string userType;
            userType = DBService.GetUserType(login);
            if (userType.Equals("A") || userType.Equals("D") || userType.Equals("M"))
            {
                HttpContext.Session.SetString("userType", userType);
                HttpContext.Session.SetString("userId", login.UserId);
                HttpContext.Session.SetInt32("branchId", 1);
                HttpContext.Session.SetString("branch", DBService.GetBranchName(1));
                //FormsAuthentication.SetAuthCookie(login.UserId, true);

                var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, login.UserId ?? ""),
                        new Claim(ClaimTypes.Role, userType),
                        new Claim("BranchId", "1")
                    };



                return View();
            }
            else
            {
                ViewBag.ErrMsg = "Invalid Credentials";
                return View("Login");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
