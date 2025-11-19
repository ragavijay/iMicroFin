using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using iMicroFin.DAO;
using iMicroFin.Models;

namespace iMicroFin.Controllers
{
    [Route("[controller]")]
    [Authorize]
    public class GroupController : Controller
    {
        [HttpGet]
        [Route("ManageGroup")]
        public IActionResult ManageGroup()
        {
            var branchId = HttpContext.Session.GetInt32("branchId");
            if (!branchId.HasValue)
            {
                return RedirectToAction("Login", "App");
            }

            return View();
        }

        [HttpGet]
        [Route("GetGroupsList")]
        public IActionResult GetGroupsList(string search = "", int page = 1, int pageSize = 10)
        {
            var branchId = HttpContext.Session.GetInt32("branchId");
            if (!branchId.HasValue)
            {
                return Json(new { success = false, message = "Session expired" });
            }

            List<MemberGroup> allGroups = GroupDBService.GetAllMemberGroups(branchId.Value);

            // Filter by search term
            if (!string.IsNullOrEmpty(search))
            {
                allGroups = allGroups.Where(g =>
                    g.GroupCode.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    g.GroupName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    g.CenterCode.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    g.CenterName.Contains(search, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            // Pagination
            int totalCount = allGroups.Count;
            int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var paginatedGroups = allGroups
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Json(new
            {
                success = true,
                groups = paginatedGroups,
                currentPage = page,
                totalPages = totalPages,
                totalCount = totalCount
            });
        }

        [HttpGet]
        [Route("GetGroup/{groupCode}")]
        public IActionResult GetGroup(string groupCode)
        {
            var branchId = HttpContext.Session.GetInt32("branchId");
            if (!branchId.HasValue)
            {
                return Json(new { success = false, message = "Session expired" });
            }

            if (string.IsNullOrEmpty(groupCode))
            {
                return Json(new { success = true, group = new MemberGroup() });
            }

            MemberGroup? group = GroupDBService.GetMemberGroup(groupCode);
            return Json(new { success = true, group = group });
        }

        [HttpPost]
        [Route("SaveGroup")]
        public IActionResult SaveGroup([FromBody] GroupRequest request)
        {
            var branchId = HttpContext.Session.GetInt32("branchId");
            if (!branchId.HasValue)
            {
                return Json(new { success = false, message = "Session expired" });
            }

            // Map the request to MemberGroup
            var group = new MemberGroup
            {
                GroupCode = request.GroupCode,
                GroupName = request.GroupName,
                GroupId = request.GroupId,
                CenterCode = request.CenterCode,
                CenterName = request.CenterName,
                LeaderName = request.LeaderName
            };

            if (string.IsNullOrEmpty(group.GroupCode))
            {
                // Adding new group
                int statusCode = GroupDBService.AddMemberGroup(group);
                if (statusCode == 0)
                {
                    return Json(new { success = false, message = "Group already exists" });
                }
                else if (statusCode == -1)
                {
                    return Json(new { success = false, message = "Try again" });
                }
                else
                {
                    return Json(new { success = true, message = "Group added successfully" });
                }
            }
            else
            {
                // Editing existing group
                int statusCode = GroupDBService.EditMemberGroup(group);
                if (statusCode == 0)
                {
                    return Json(new { success = false, message = "Group already exists" });
                }
                else
                {
                    return Json(new { success = true, message = "Group updated successfully" });
                }
            }
        }

        [HttpGet]
        [Route("GetGroupListByPattern/{pattern?}")]
        public IActionResult GetGroupListByPattern(string pattern = "")
        {
            var branchId = HttpContext.Session.GetInt32("branchId");
            if (!branchId.HasValue)
            {
                return Json(new { success = false, message = "Session expired" });
            }

            List<MemberGroup> groups;

            if (string.IsNullOrEmpty(pattern))
            {
                // Return all groups when no pattern is provided
                groups = GroupDBService.GetAllMemberGroups(branchId.Value);
            }
            else
            {
                // Return filtered groups
                groups = GroupDBService.GetAllMemberGroupsByPattern(pattern, branchId.Value);
            }

            return Json(new
            {
                success = true,
                groups = groups
            });
        }

    }
    // Helper class to receive camelCase JSON
    public class GroupRequest
    {
        public string GroupCode { get; set; } = string.Empty;
        public string GroupName { get; set; } = string.Empty;
        public int GroupId { get; set; }
        public string CenterCode { get; set; } = string.Empty;
        public string CenterName { get; set; } = string.Empty;
        public string LeaderName { get; set; } = string.Empty;
    }
}