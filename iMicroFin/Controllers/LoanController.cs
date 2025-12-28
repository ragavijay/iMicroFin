using iMicroFin.DAO;
using iMicroFin.Models;
using iMicroFin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace iMicroFin.Controllers
{
    [Route("[controller]")]
    [Authorize(Policy = "DirectorManager")]
    public class LoanController : Controller
    {
        [HttpGet]
        [Route("LoanForm/{id?}")]
        public ActionResult LoanForm(string id)
        {
            string loanCode = id;
            if (loanCode == null)
            {
                @ViewBag.DisplayMode = "display:none";
                return View();
            }
            else
            {
                Loan loan = LoanDBService.GetLoan(loanCode);
                return View(loan);
            }
        }

        [HttpPost]
        [Route("SaveLoan")] // Add this route
        public ActionResult Loan(Loan loan)
        {
            if (loan.LoanCode == null)
            {
                loan.BranchId = HttpContext.Session?.GetInt32("branchId") ?? 0;
                int status = LoanDBService.AddLoan(loan);
                if (status == 0)
                {
                    @ViewBag.ErrEwi = "Loan already exists";
                    @ViewBag.DisplayMode = "display:block";
                    return View("LoanForm", loan);
                }
                else if (status == -1)
                {
                    @ViewBag.ErrEwi = "Try again";
                    @ViewBag.DisplayMode = "display:block";
                    return View("LoanForm", loan);
                }
                else
                {
                    return ViewLoans(MemberDBService.GetGroupCode(loan.MemberCode));
                }
            }
            else
            {
                int statusCode = LoanDBService.EditLoan(loan);
                if (statusCode == 0)
                {
                    @ViewBag.ErrTryAgain = "Try Again";
                    return View("LoanForm", loan);
                }
                else
                {
                    return ViewLoan(loan.LoanCode);
                }
            }
        }

        [HttpGet]
        [Route("GroupLoanForm")]
        public ActionResult GroupLoanForm()
        {
            return View("GroupLoanForm");
        }

        [HttpPost]
        [Route("SaveGroupLoan")]
        public ActionResult GroupLoan(GroupLoan groupLoan)
        {

            groupLoan.BranchId = HttpContext.Session?.GetInt32("branchId")??0;
            int status = LoanDBService.AddGroupLoan(groupLoan);
            if (status == 1)
            {
                return ViewLoans(groupLoan.GroupCode.ToString());
            }
            else
            {
                return View("GroupLoanForm");
            }
        }

        [HttpGet]
        [Route("ViewLoans/{id?}")]
        public ActionResult ViewLoans(string? id)
        {
            var viewModel = new ViewLoansViewModel();

            if (!string.IsNullOrEmpty(id))
            {
                // Get group details
                MemberGroup? group = GroupDBService.GetMemberGroup(id);
                if (group != null)
                {
                    viewModel.GroupCode = group.GroupCode;
                    viewModel.GroupName = group.GroupName;
                    viewModel.CenterCode = group.CenterCode;
                    viewModel.CenterName = group.CenterName;
                    viewModel.LeaderName = group.LeaderName;
                }
            }

            return View("ViewLoans",viewModel);
        }

        [HttpGet]
        [Route("GetLoansByGroup/{groupCode}")]
        public IActionResult GetLoansByGroup(string groupCode)
        {
            try
            {
                var branchId = HttpContext.Session?.GetInt32("branchId") ?? 0;
                List<Loan> loans = LoanDBService.GetAllLoans(branchId, groupCode);

                return Json(new
                {
                    success = true,
                    loans = loans,
                    count = loans.Count
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Error loading loans: " + ex.Message
                });
            }
        }

        [HttpGet]
        [Route("LoanTransferReport/{id?}")]
        public ActionResult LoanTransferReport(string id)
        {
            string groupCode = id;
            List<MemberLoan> memberLoans = LoanDBService.GetAllMemberLoans(HttpContext.Session?.GetInt32("branchId") ?? 0, groupCode);
            ViewBag.GroupId = id;
            return View("LoanTransferReport", memberLoans);
        }

        [HttpGet]
        [Route("ExportTransferReport/{id?}")]
        public FileStreamResult ExportTransferReport(string id)
        {
            string groupCode = id;
            List<MemberLoan> memberLoans = LoanDBService.GetAllMemberLoans(HttpContext.Session?.GetInt32("branchId") ?? 0, groupCode);
            MemoryStream memory = MemberLoan.GetExportTransferReport(memberLoans);
            return File(memory, "application/vnd.ms-excel", "LoanTransfer.csv");

        }

        [HttpGet]
        public ActionResult ViewPendingLoans()
        {
            List<Loan> loans = LoanDBService.GetAllPendingLoans(HttpContext.Session?.GetInt32("branchId") ?? 0);
            return View(loans);
        }


        [Route("ViewLoan/{id?}")]
        public ActionResult ViewLoan(string id)
        {
            string loanCode = id;
            MemberLoan memberLoan = new MemberLoan();
            memberLoan.loan = LoanDBService.GetLoan(loanCode);
            memberLoan.member = MemberDBService.GetMember(memberLoan.loan.MemberCode);
            return View("ViewLoan",memberLoan);
        }

        [Route("LoanStatusForm/{id?}")]
        public ActionResult LoanStatusForm(string id)
        {
            string loanCode = id;
            MemberLoan memberLoan = new MemberLoan();
            memberLoan.loan = LoanDBService.GetLoan(loanCode);
            memberLoan.member = MemberDBService.GetMember(memberLoan.loan.MemberCode);
            return View(memberLoan);
        }

        [HttpPost]
        [Route("SaveLoanStatus")]
        public ActionResult SaveLoanStatus(string loanCode, string memberCode, string loanStatus, string statusRemarks)
        {
            string userId = HttpContext.Session?.GetString("userId") ?? "";
            LoanDBService.UpdateLoanStatus(loanCode, loanStatus, statusRemarks, userId);
            return ViewLoans(MemberDBService.GetGroupCode(memberCode));
        }

        [Route("LoanRepaymentStatus/{id?}")]
        public ActionResult LoanRepaymentStatus(string id)
        {
            string groupCode = id;
            LoanRepaymentStatus loanRepaymentStatus = LoanDBService.GetLoanRepaymentStatus(groupCode);
            return View(loanRepaymentStatus);
        }

        [HttpGet]
        [Route("CumulativeReport")]
        public ActionResult CumulativeReport()
        {
            List<CumulativeReport> cumulativeReport=LoanDBService.GetCumulativeReport();
            return View("CumulativeReport", cumulativeReport);
        }

        [HttpGet]
        [Route("ExportLoans")]
        public FileStreamResult ExportLoans()
        {
            List<MemberLoan> memberLoans = LoanDBService.GetGlobalMemberLoans(1);
            MemoryStream memory = iMicroFin.Models.Loan.GetExportLoans(memberLoans);
            return File(memory, "application/vnd.ms-excel", "Loans.csv");

        }
        [HttpGet]
        [Route("CheckMember/{memberCode}")]
        public IActionResult CheckMember(string memberCode)
        {
            string result = LoanDBService.CheckMember(memberCode);
            return Content(result, "text/plain");
        }


        [HttpGet]
        [Route("CheckGroup/{groupCode}")]
        public IActionResult CheckGroup(string groupCode)
        {
            string result = LoanDBService.CheckGroup(groupCode);
            return Content(result, "text/plain");
        }
        [HttpPost]
        [Route("MarkAsBadLoan")]
        public IActionResult MarkAsBadLoan([FromBody] BadLoanRequest request)
        {
            try
            {
                string userId = HttpContext.Session?.GetString("userId") ?? "";
                int statusCode = LoanDBService.MarkLoanAsBad(request.LoanCode, userId, request.StatusRemarks);

                if (statusCode == 1)
                {
                    return Json(new
                    {
                        success = true,
                        message = "Loan marked as bad loan successfully"
                    });
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        message = "Failed to mark loan as bad. Only ongoing loans can be marked as bad."
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Error: " + ex.Message
                });
            }
        }

        

        // Add this class at the end of the file or in a separate Models file
        public class BadLoanRequest
        {
            public string LoanCode { get; set; }
            public string StatusRemarks { get; set; }
        }
    }
}