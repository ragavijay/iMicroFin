using iMicroFin.DAO;
using iMicroFin.Models;
using iMicroFin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace iMicroFin.Controllers
{
    [Route("[controller]")]
    [Authorize(Policy = "DirectorManager")]
    public class MemberController : Controller
    {
        private readonly IPathService _pathService;

        public MemberController(IPathService pathService)
        {
            _pathService = pathService;
        }

        [HttpGet]
        [Route("MemberForm/{id?}")]
        public ActionResult MemberForm(string id)
        {
            string memberCode = id;
            if (memberCode == null)
            {
                return View();
            }
            else
            {
                Member? member = MemberDBService.GetMember(memberCode);
                return View(member);
            }
        }

        [HttpPost]
        [Route("SaveMember")]
        public async Task<IActionResult> Member(Member member)
        {
            int statusCode;
            if (member.MemberCode == null)
            {
                statusCode = MemberDBService.AddMember(member);
               
            }
            else
            {
                statusCode = MemberDBService.EditMember(member);
            }
            if (statusCode == -1)
            {
                @ViewBag.ErrMemberAadharNumber = "Aadhar already existes";
                return View("MemberForm", member);
            }
            else if (statusCode == -2)
            {
                @ViewBag.ErrMemberType = "Leader already exists for the group";
                return View("MemberForm", member);

            }
            else if (statusCode == -3)
            {
                @ViewBag.ErrTryAgain = "Try again";
                return View("MemberForm", member);

            }
            else
            {
                string? path, directory, fileName;
                path = _pathService.WebRootPath;                
                if (member.Photo != null && member.Photo.Length>0)
                {
                    directory = path +  @"/FileUploads/Img/Member/";
                    directory = directory.Replace("\\", "/");
                    fileName = directory + member.MemberCode + ".jpg";
                    using (var fileStream = new FileStream(fileName, FileMode.Create))
                    {
                        await member.Photo.CopyToAsync(fileStream);
                    }

                }
                if (member.Aadhar != null && member.Aadhar.Length > 0)
                {
                    directory = path + @"/FileUploads/Img/Aadhar/";
                    directory = directory.Replace("\\", "/");
                    fileName = directory + member.MemberCode + ".jpg";
                    using (var fileStream = new FileStream(fileName, FileMode.Create))
                    {
                        await member.Aadhar.CopyToAsync(fileStream);
                    }
                }
                return ViewMembers(member.GroupCode.ToString());
            }
        }

        [HttpGet]
        [Route("ViewMembers/{id?}")]
        public ActionResult ViewMembers(string? id)
        {
            var viewModel = new ViewMembersViewModel();

            if (!string.IsNullOrEmpty(id))
            {
                // Get group details
                MemberGroup? group = GroupDBService.GetMemberGroup(id);
                if (group != null)
                {
                    viewModel.GroupCode = group.GroupCode;
                    viewModel.GroupName = group.GroupName;
                    viewModel.LeaderName = group.LeaderName;
                    viewModel.CenterCode = group.CenterCode;
                    viewModel.CenterName = group.CenterName;
                }
            }

            return View("ViewMembers",viewModel);
        }


        [HttpGet]
        [Route("ViewMember/{id?}")]
        public ActionResult ViewMember(string id)
        {
            Member? member = MemberDBService.GetMember(id);
            return View(member);
        }

        [HttpGet]
        public ActionResult SearchMember()
        {
            return View();
        }

        [HttpGet]
        [Route("ExportMembers/{id?}")]
        public FileStreamResult ExportMembers(string id)
        {
            string groupCode = id;
            List<Member> members = MemberDBService.GetAllMembers(groupCode);
            MemoryStream memory = iMicroFin.Models.Member.GetExportMembers(members);
            return File(memory, "application/vnd.ms-excel", "Members.csv");

        }

        [HttpGet]
        [Route("GetMemberImages/{memberCode}")]
        public IActionResult GetMemberImages(string memberCode)
        {
            string path = _pathService.WebRootPath;

            string photoPath = path + @"/FileUploads/Img/Member/" + $"{memberCode}.jpg";
            string aadharPath = path + @"/FileUploads/Img/Aadhar/" + $"{memberCode}.jpg";

            bool photoExists = System.IO.File.Exists(photoPath);
            bool aadharExists = System.IO.File.Exists(aadharPath);

            return Json(new
            {
                success = true,
                photoExists = photoExists,
                aadharExists = aadharExists,
                photoUrl = photoExists ? $"/FileUploads/Img/Member/{memberCode}.jpg" : null,
                aadharUrl = aadharExists ? $"/FileUploads/Img/Aadhar/{memberCode}.jpg" : null
            });
        }

        [HttpGet]
        [Route("GetMemberPhoto/{memberCode}")]
        public IActionResult GetMemberPhoto(string memberCode)
        {
            string path = _pathService.WebRootPath;
            string photoPath = path + @"/FileUploads/Img/Member/" + $"{memberCode}.jpg";

            if (System.IO.File.Exists(photoPath))
            {
                var image = System.IO.File.OpenRead(photoPath);
                return File(image, "image/jpeg");
            }

            return NotFound();
        }

        [HttpGet]
        [Route("GetMemberAadhar/{memberCode}")]
        public IActionResult GetMemberAadhar(string memberCode)
        {
            string path = _pathService.WebRootPath;
            string aadharPath = path + @"/FileUploads/Img/Aadhar/" + $"{memberCode}.jpg";

            if (System.IO.File.Exists(aadharPath))
            {
                var image = System.IO.File.OpenRead(aadharPath);
                return File(image, "image/jpeg");
            }

            return NotFound();
        }

        [HttpGet]
        [Route("GetMembersByGroup/{groupCode}")]
        public IActionResult GetMembersByGroup(string groupCode)
        {
            try
            {
                List<Member> members = MemberDBService.GetAllMembers(groupCode);

                return Json(new
                {
                    success = true,
                    members = members,
                    count = members.Count
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Error loading members: " + ex.Message
                });
            }
        }
        [HttpGet]
        [Route("GetMemberByAadhar/{searchText}")]
        public IActionResult GetMemberByAadhar(string searchText)
        {
            try
            {
                var members = MemberDBService.GetMemberByAadhar(searchText);

                if (members == null || members.Count == 0)
                {
                    return Json(new
                    {
                        success = false,
                        statusCode = 0,
                        message = "No members found with this Aadhar",
                        members = new List<object>()
                    });
                }

                return Json(new
                {
                    success = true,
                    statusCode = 1,
                    members = members
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    statusCode = -1,
                    message = "Error fetching members: " + ex.Message,
                    members = new List<object>()
                });
            }
        }

        [HttpGet]
        [Route("GetMemberByPhone/{searchText}")]
        public IActionResult GetMemberByPhone(string searchText)
        {
            try
            {
                var members = MemberDBService.GetMemberByPhone(searchText);

                if (members == null || members.Count == 0)
                {
                    return Json(new
                    {
                        success = false,
                        statusCode = 0,
                        message = "No members found with this phone number",
                        members = new List<object>()
                    });
                }

                return Json(new
                {
                    success = true,
                    statusCode = 1,
                    members = members
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    statusCode = -1,
                    message = "Error fetching members: " + ex.Message,
                    members = new List<object>()
                });
            }
        }

        [HttpGet]
        [Route("GetMemberByName/{searchText}")]
        public IActionResult GetMemberByName(string searchText)
        {
            try
            {
                var members = MemberDBService.GetMemberByName(searchText);

                if (members == null || members.Count == 0)
                {
                    return Json(new
                    {
                        success = false,
                        statusCode = 0,
                        message = "No members found with this name",
                        members = new List<object>()
                    });
                }

                return Json(new
                {
                    success = true,
                    statusCode = 1,
                    members = members
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    statusCode = -1,
                    message = "Error fetching members: " + ex.Message,
                    members = new List<object>()
                });
            }
        }
    }
}