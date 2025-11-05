using iMicroFin.DAO;
using iMicroFin.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

        [Authorize]
        [HttpGet]
        public ActionResult Home()
        {
            string? userType = HttpContext.Session.GetString("userType");

            if (!string.IsNullOrEmpty(userType) &&
                (userType.Equals("A") ||
                 userType.Equals("D") ||
                 userType.Equals("M")))
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
        public async Task<IActionResult> Home(Login login)
        {
            string userType;
            userType = DBService.GetUserType(login);
            if (userType.Equals("A") || userType.Equals("D") || userType.Equals("M"))
            {
                HttpContext.Session.SetString("userType", userType);
                HttpContext.Session.SetString("userId", login.UserId);
                HttpContext.Session.SetInt32("branchId", 1);
                HttpContext.Session.SetString("branch", DBService.GetBranchName(1));

                if (userType.Equals("A"))
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, login.UserId ?? ""),
                        new Claim(ClaimTypes.Role,  "Admin"),

                    };
                    var identity = new ClaimsIdentity(claims, "MyAuthCookie");
                    var principal = new ClaimsPrincipal(identity);
                    await HttpContext.SignInAsync("MyAuthCookie", principal);
                    return View();
                }
                else 
                {
                    var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, login.UserId),
                            new Claim(ClaimTypes.Role, "User")
                        };

                    var identity = new ClaimsIdentity(claims, "MyAuthCookie");
                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync("MyAuthCookie", principal);
                    return View();
                }
            }
            else
            {
                ViewBag.ErrMsg = "Invalid Credentials";
                return View("Login");
            }
        }

  

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("MyAuthCookie");
            return RedirectToAction("Login");
        }

    }
}
