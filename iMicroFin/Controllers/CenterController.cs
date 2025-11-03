using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using iMicroFin.DAO;
using iMicroFin.Models;

namespace iMicroFin.Controllers
{
    [Route("[controller]")] 
    //[Authorize]
    public class CenterController : Controller
    {
        [HttpGet]
        [Route("GroupCenterForm/{id?}")]
        public IActionResult GroupCenterForm(string? id)
        {
            string? centerCode = id;

            if (string.IsNullOrEmpty(centerCode))
            {
                return View(new GroupCenterViewModel());
            }
            else
            {
                GroupCenterViewModel center = CenterDBService.GetGroupCenter(centerCode);
                return View(center);
            }
        }

        [HttpPost]
        [Route("GroupCenter")]
        public IActionResult GroupCenter(GroupCenterViewModel center)
        {
            var branchId = HttpContext.Session.GetInt32("branchId");

            if (!branchId.HasValue)
            {
                return RedirectToAction("Login", "App");
            }

            if (string.IsNullOrEmpty(center.CenterCode))
            {
                // Adding new center
                center.BranchId = branchId.Value;
                int statusCode = CenterDBService.AddGroupCenter(center);

                if (statusCode == 0)
                {
                    ViewBag.ErrCenterName = "Center already exists";
                    return View("GroupCenterForm", center);
                }
                else if (statusCode == -1)
                {
                    ViewBag.ErrTryAgain = "Try again";
                    return View("GroupCenterForm", center);
                }
                else
                {
                    return RedirectToAction("ViewGroupCenters");
                }
            }
            else
            {
                // Editing existing center
                center.BranchId = branchId.Value;
                int statusCode = CenterDBService.EditGroupCenter(center);

                if (statusCode == 0)
                {
                    ViewBag.ErrCenterName = "Center already exists";
                    return View("GroupCenterForm", center);
                }
                else
                {
                    return RedirectToAction("ViewGroupCenters");
                }
            }
        }

        [HttpGet]
        [Route("ViewGroupCenters")]
        public IActionResult ViewGroupCenters()
        {
            // ✅ Get BranchId from Session
            var branchId = HttpContext.Session.GetInt32("branchId");

            if (!branchId.HasValue)
            {
                return RedirectToAction("Login", "App");
            }

            List<GroupCenterViewModel> centers = CenterDBService.GetAllGroupCenters(branchId.Value);
            return View("ViewGroupCenters", centers);
        }
    }
}