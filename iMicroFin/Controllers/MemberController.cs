using iMicroFin.DAO;
using iMicroFin.Models;
using iMicroFin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace iMicroFin.Controllers
{
    [Route("[controller]")]
    [Authorize]
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
        public ActionResult ViewMembers(string id)
        {
            string groupCode = id;
            groupCode = "0010101";
            List<Member> members = MemberDBService.GetAllMembers(groupCode);
            ViewBag.GroupCode = id;
            return View("ViewMembers", members);
        }

        [HttpGet]
        [Route("ViewMember/{id?}")]
        public ActionResult ViewMember(string id)
        {
            Member? member = MemberDBService.GetMember(id);
            if(member!=null) 
                member.FamilyMembers = MemberDBService.GetFamilyMembers(id);
            return View(member);
        }

        [HttpGet]
        [Route("ViewFamilyMembers/{id?}")]
        public ActionResult ViewFamilyMembers(string id)
        {
            List<FamilyMember> familyMembers = MemberDBService.GetFamilyMembers(id);
            @ViewBag.MemberName = MemberDBService.GetMember(id)?.MemberName??"";
            @ViewBag.MemberCode= id;
            return View("ViewFamilyMembers",familyMembers);
        }

        [HttpGet]
        [Route("FamilyMemberForm/{id?}")]
        public ActionResult FamilyMemberForm(string id)
        {
            string[] input= id.Split(' ');
            if (input.Length == 1)
            {
                Member? member = MemberDBService.GetMember(id);
                FamilyMember familyMember = new FamilyMember();
                familyMember.MemberCode = member?.MemberCode??"";
                familyMember.OccupationType = EOccupationType.None;
                return View(familyMember);
            } else
            {
                string memberCode = input[0];
                int sNo = Convert.ToInt32(input[1]);
                FamilyMember? familyMember = MemberDBService.GetFamilyMember(memberCode, sNo);
                return View(familyMember);
            }

        }

        [HttpPost]
        [Route("SaveFamilyMember")]
        public ActionResult FamilyMember(FamilyMember familyMember)
        {
            int statusCode;
            if (familyMember.SNo == 0)
            {
                statusCode = MemberDBService.AddFamilyMember(familyMember);

            }
            else
            {
                statusCode = MemberDBService.EditFamilyMember(familyMember);
            }
            if (statusCode == 0)
            {
                @ViewBag.ErrTryAgain = "Try again";
                return View("FamilyMemberForm", familyMember);

            }
            else
            {
               return ViewFamilyMembers(familyMember.MemberCode);
            }
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
    }
}