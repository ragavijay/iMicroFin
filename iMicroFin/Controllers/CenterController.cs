using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using iMicroFin.DAO;
using iMicroFin.Models;

namespace iMicroFin.Controllers
{
    [Route("[controller]")]
    [Authorize(Policy ="DirectorManager")]
    public class CenterController : Controller
    {
        [HttpGet]
        [Route("ManageCenters")]
        public IActionResult ManageCenters()
        {
            var branchId = HttpContext.Session.GetInt32("branchId");
            if (!branchId.HasValue)
            {
                return RedirectToAction("Login", "App");
            }

            return View();
        }

        [HttpGet]
        [Route("GetCentersList")]
        public IActionResult GetCentersList(string search = "", int page = 1, int pageSize = 10)
        {
            var branchId = HttpContext.Session.GetInt32("branchId");
            if (!branchId.HasValue)
            {
                return Json(new { success = false, message = "Session expired" });
            }

            List<Center> allCenters = CenterDBService.GetAllCenters(branchId.Value);

            // Filter by search term
            if (!string.IsNullOrEmpty(search))
            {
                allCenters = allCenters.Where(c =>
                    c.CenterCode.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    c.CenterName.Contains(search, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            // Pagination
            int totalCount = allCenters.Count;
            int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var paginatedCenters = allCenters
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Json(new
            {
                success = true,
                centers = paginatedCenters,
                currentPage = page,
                totalPages = totalPages,
                totalCount = totalCount
            });
        }

        [HttpGet]
        [Route("GetCenter/{centerCode}")]
        public IActionResult GetCenter(string centerCode)
        {
            var branchId = HttpContext.Session.GetInt32("branchId");
            if (!branchId.HasValue)
            {
                return Json(new { success = false, message = "Session expired" });
            }

            if (string.IsNullOrEmpty(centerCode))
            {
                return Json(new { success = true, center = new Center() });
            }

            Center? center = CenterDBService.GetCenter(centerCode);
            return Json(new { success = true, center = center });
        }

        [HttpPost]
        [Route("SaveCenter")]
        public IActionResult SaveCenter([FromBody] Center center)
        {
            var branchId = HttpContext.Session.GetInt32("branchId");
            if (!branchId.HasValue)
            {
                return Json(new { success = false, message = "Session expired" });
            }

            center.BranchId = branchId.Value;

            if (string.IsNullOrEmpty(center.CenterCode))
            {
                // Adding new center
                int statusCode = CenterDBService.AddCenter(center);
                if (statusCode == 0)
                {
                    return Json(new { success = false, message = "Center already exists" });
                }
                else if (statusCode == -1)
                {
                    return Json(new { success = false, message = "Try again" });
                }
                else
                {
                    return Json(new { success = true, message = "Center added successfully" });
                }
            }
            else
            {
                // Editing existing center
                int statusCode = CenterDBService.EditCenter(center);
                if (statusCode == 0)
                {
                    return Json(new { success = false, message = "Center already exists" });
                }
                else
                {
                    return Json(new { success = true, message = "Center updated successfully" });
                }
            }
        }
        [HttpGet]
        [Route("GetCentersListByPattern/{pattern?}")]
        public IActionResult GetCentersListByPattern(string pattern = "")
        {
            var branchId = HttpContext.Session.GetInt32("branchId");
            if (!branchId.HasValue)
            {
                return Json(new { success = false, message = "Session expired" });
            }

            List<Center> centers;

            if (string.IsNullOrEmpty(pattern))
            {
                // Return all centers when no pattern is provided
                centers = CenterDBService.GetAllCenters(branchId.Value);
            }
            else
            {
                // Return filtered centers
                centers = CenterDBService.GetAllCentersByPattern(pattern, branchId.Value);
            }

            return Json(new
            {
                success = true,
                centers = centers
            });
        }
    }
    public class CenterRequest
    {
        public string CenterCode { get; set; } = string.Empty;
        public string CenterName { get; set; } = string.Empty;
        public int CenterId { get; set; }
    }
}