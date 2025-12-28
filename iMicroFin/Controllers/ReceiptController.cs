using iMicroFin.DAO;
using iMicroFin.Models;
using iMicroFin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Globalization;
using System.Text.Json;
namespace iMicroFin.Controllers
{
    [Authorize(Policy ="DirectorManagerCashier")]
    public class ReceiptController : Controller
    {
        public ActionResult PFReceiptForm()
        {
            return View();
        }

        public ActionResult GroupPFReceiptForm()
        {
            return View();
        }
        [HttpPost]
        public ActionResult PFReceipt(PFReceipt pfReceipt)
        {
            pfReceipt.UserId = HttpContext.Session?.GetString("userId") ?? "";
            ReceiptDBService.GeneratePFReceipt(pfReceipt);
            return View(pfReceipt);
        }

        [HttpPost]
        public ActionResult GroupPFReceipt(GroupPFReceipt groupPFReceipt)
        {
            groupPFReceipt.UserId = HttpContext.Session?.GetString("userId") ?? "";
            List<GroupPFReceiptInfo> groupPFReceiptInfoList = ReceiptDBService.GenerateGroupPFReceipt(groupPFReceipt);
            return View(groupPFReceiptInfoList);
        }


        public ActionResult InstalmentReceiptForm()
        {
            return View();
        }

        public ActionResult PreClosureForm()
        {
            return View();
        }
        public ActionResult GroupInstalmentReceiptForm()
        {
            return View();
        }

        //[HttpPost]
        //public ActionResult GroupInstalmentReceipt(GroupInstalmentReceipt groupInstalmentReceipt)
        //{
        //    groupInstalmentReceipt.UserId = HttpContext.Session?.GetString("userId") ?? "";
        //    List<GroupInstalmentReceiptInfo> groupInstalmentReceiptInfo = ReceiptDBService.GenerateGroupInstalmentReceipt(groupInstalmentReceipt);
        //    return View(groupInstalmentReceiptInfo);
        //}

        

        [HttpPost]
        public ActionResult GroupInstalmentReceipt(GroupInstalmentReceipt groupInstalmentReceipt)
        {
            groupInstalmentReceipt.UserId = HttpContext.Session?.GetString("userId") ?? "";

            // Get member instalments data from hidden field
            string memberInstalmentsDataJson = Request.Form["MemberInstalmentsData"];
            List<(string memberCode, int noOfInstalments)> memberInstalments = new();

            if (!string.IsNullOrEmpty(memberInstalmentsDataJson))
            {
                try
                {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var data = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(memberInstalmentsDataJson, options);

                    if (data != null)
                    {
                        foreach (var item in data)
                        {
                            if (item.ContainsKey("memberCode") && item.ContainsKey("noOfInstalments"))
                            {
                                var memberCode = item["memberCode"].ToString();
                                int.TryParse(item["noOfInstalments"].ToString(), out int noOfInstalments);
                                if (noOfInstalments>0)
                                    memberInstalments.Add((memberCode, noOfInstalments));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log error if needed
                    Console.WriteLine($"Error parsing member instalments: {ex.Message}");
                }
            }

            // Generate receipts for each member with their specific instalment amount
            List<GroupInstalmentReceiptInfo> groupInstalmentReceiptInfo =
                ReceiptDBService.GenerateGroupInstalmentReceipt(groupInstalmentReceipt.UserId, groupInstalmentReceipt.GroupCode, groupInstalmentReceipt.ActualReceiptDate, memberInstalments);

            return View(groupInstalmentReceiptInfo);
        }

        [HttpPost]
        public ActionResult InstalmentReceipt(InstalmentReceipt instalmentReceipt)
        {
            instalmentReceipt.UserId = HttpContext.Session?.GetString("userId") ?? "";
            ReceiptDBService.GenerateInstalmentReceipt(instalmentReceipt);
            instalmentReceipt.TotalDue = instalmentReceipt.Ewi * instalmentReceipt.NoOfInstalments;
            return View(instalmentReceipt);
        }
        [HttpGet]
        public ActionResult ViewEwiDueList()
        {
            List<EWIDue> ewiDues = ReceiptDBService.GetEwiDue(HttpContext.Session?.GetInt32("branchId") ?? 0);
            return View(ewiDues);
        }
        [HttpGet]
        public ActionResult CashReceiptStatementForm()
        {
            return View();

        }
        [HttpPost]
        public ActionResult CashReceiptStatement()
        {

            string userId = HttpContext.Session?.GetString("userId") ?? "";
            string userType = HttpContext.Session?.GetString("userType") ?? "";
            DateTime fromDate = DateTime.ParseExact(Request.Form["FromDate"],"dd/MM/yyyy",CultureInfo.InvariantCulture);
            DateTime toDate = DateTime.ParseExact(Request.Form["ToDate"], "dd/MM/yyyy", CultureInfo.InvariantCulture);
            List<CashReceiptStatement> statement = ReceiptDBService.GetCashReceiptStatement(fromDate, toDate);
            return View(statement);
        }
        [HttpGet]
        [Route("Receipt/GetPFReceipt/{loanCode}")]
        public IActionResult GetPFReceipt(string loanCode)
        {
            try
            {
                PFReceipt pfReceipt = ReceiptDBService.GetPFReceipt(loanCode);

                if (pfReceipt == null)
                {
                    return Json(new
                    {
                        success = false,
                        loanStatus = "Invalid",
                        message = "Loan not found"
                    });
                }

                return Json(new
                {
                    success = true,
                    loanCode = pfReceipt.LoanCode,
                    memberCode = pfReceipt.MemberCode,
                    memberName = pfReceipt.MemberName,
                    processingFee = pfReceipt.ProcessingFee,
                    insurance = pfReceipt.Insurance,
                    totalFee = pfReceipt.TotalFee,
                    loanStatus = pfReceipt.LoanStatus
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    loanStatus = "Error",
                    message = "Error fetching loan details: " + ex.Message
                });
            }
        }
        [HttpGet]
        [Route("Receipt/GetGroupPFReceipt/{groupCode}")]
        public IActionResult GetGroupPFReceipt(string groupCode)
        {
            try
            {
                // Assuming you have a method to get group PF receipt info
                var groupPFReceipt = ReceiptDBService.GetGroupPFReceipt(groupCode);

                if (groupPFReceipt == null || groupPFReceipt.StatusCode == 0)
                {
                    return Json(new
                    {
                        success = false,
                        statusCode = 0,
                        message = "No pending loans found for this group or receipts already generated"
                    });
                }

                return Json(new
                {
                    success = true,
                    statusCode = 1,
                    processingFee = groupPFReceipt.ProcessingFee,
                    insurance = groupPFReceipt.Insurance,
                    totalFee = groupPFReceipt.TotalFee
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    statusCode = -1,
                    message = "Error fetching group receipt details: " + ex.Message
                });
            }
        }
        [HttpGet]
        [Route("Receipt/GetInstalmentReceipt/{loanCode}")]
        public IActionResult GetInstalmentReceipt(string loanCode)
        {
            try
            {
                InstalmentReceipt instalmentReceipt = ReceiptDBService.GetInstalmentReceipt(loanCode);

                if (instalmentReceipt == null)
                {
                    return Json(new
                    {
                        success = false,
                        loanStatus = "Invalid",
                        message = "Loan not found"
                    });
                }

                return Json(new
                {
                    success = true,
                    loanCode = instalmentReceipt.LoanCode,
                    memberCode = instalmentReceipt.MemberCode,
                    memberName = instalmentReceipt.MemberName,
                    noOfInstalments = instalmentReceipt.NoOfInstalments,
                    totalPendingInstalments = instalmentReceipt.TotalPendingInstalments,
                    ewi = instalmentReceipt.Ewi,
                    totalDue = instalmentReceipt.TotalDue,
                    loanStatus = instalmentReceipt.LoanStatus
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    loanStatus = "Error",
                    message = "Error fetching instalment details: " + ex.Message
                });
            }
        }

        [HttpGet]
        [Route("Receipt/GetGroupInstalmentReceipt/{groupCode}")]
        public IActionResult GetGroupInstalmentReceipt(string groupCode)
        {
            try
            {
                GroupInstalmentReceipt groupInstalmentReceipt = ReceiptDBService.GetGroupInstalmentReceipt(groupCode);

                if (groupInstalmentReceipt == null || groupInstalmentReceipt.StatusCode == 0)
                {
                    return Json(new
                    {
                        success = false,
                        statusCode = 0,
                        message = "No active loans found for this group"
                    });
                }

                return Json(new
                {
                    success = true,
                    statusCode = 1,
                    noOfInstalments = groupInstalmentReceipt.NoOfInstalments,
                    ewi = groupInstalmentReceipt.Ewi,
                    totalDue = groupInstalmentReceipt.TotalDue
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    statusCode = -1,
                    message = "Error fetching group instalment details: " + ex.Message
                });
            }
        }

        [HttpGet]
        [Route("Receipt/GetGroupInstalmentMembers/{groupCode}")]
        public IActionResult GetGroupInstalmentMembers(string groupCode)
        {
            try
            {
                // Get all members with active loans for the group
                List<InstalmentReceipt> members = ReceiptDBService.GetGroupInstalmentMembers(groupCode);

                if (members == null || members.Count == 0)
                {
                    return Json(new
                    {
                        success = false,
                        message = "No members with active loans found for this group",
                        members = new List<object>()
                    });
                }

                // Transform to include all necessary details
                var membersData = members.Select(m => new
                {
                    memberCode = m.MemberCode,
                    memberName = m.MemberName,
                    loanCode = m.LoanCode,
                    ewi = m.Ewi,
                    noOfInstalments = m.NoOfInstalments,  // Current due
                    totalPendingInstalments = m.TotalPendingInstalments  // Total pending
                }).ToList();

                return Json(new
                {
                    success = true,
                    message = $"Found {members.Count} member(s) with active loans",
                    members = membersData,
                    count = members.Count
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Error fetching group members: " + ex.Message,
                    members = new List<object>()
                });
            }
        }


        [HttpGet]
        [Route("Receipt/GetPreClosureDetails/{loanCode}")]
        public IActionResult GetPreClosureDetails(string loanCode)
        {
            try
            {
                PreClosureReceipt preClosureReceipt = ReceiptDBService.GetPreClosureDetails(loanCode);

                if (preClosureReceipt == null)
                {
                    return Json(new
                    {
                        success = false,
                        loanStatus = "Invalid",
                        message = "Loan not found"
                    });
                }

                if (preClosureReceipt.LoanStatus != "O")
                {
                    return Json(new
                    {
                        success = false,
                        loanStatus = preClosureReceipt.LoanStatus,
                        message = "Loan is not in ongoing status"
                    });
                }

                return Json(new
                {
                    success = true,
                    loanCode = preClosureReceipt.LoanCode,
                    memberCode = preClosureReceipt.MemberCode,
                    memberName = preClosureReceipt.MemberName,
                    loanAmount = preClosureReceipt.LoanAmount,
                    interestRate = preClosureReceipt.InterestRate,
                    tenure = preClosureReceipt.Tenure,
                    loanDisposalDate = preClosureReceipt.LoanDisposalDate.ToString("yyyy-MM-dd"),
                    totalPendingInstalments = preClosureReceipt.TotalPendingInstalments,
                    ewi = preClosureReceipt.Ewi,
                    totalDue = preClosureReceipt.TotalDue,
                    suggestedDiscount = preClosureReceipt.SuggestedDiscount,
                    interestSavings = preClosureReceipt.InterestSavings,
                    delayPenalty = preClosureReceipt.DelayPenalty,
                    advanceBenefit = preClosureReceipt.AdvanceBenefit,
                    loanStatus = preClosureReceipt.LoanStatus
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    loanStatus = "Error",
                    message = "Error fetching pre-closure details: " + ex.Message
                });
            }
        }

        [HttpPost]
        public ActionResult PreClosureReceipt(PreClosureReceipt preClosureReceipt)
        {
            preClosureReceipt.UserId = HttpContext.Session?.GetString("userId") ?? "";
            ReceiptDBService.GeneratePreClosureReceipt(preClosureReceipt);
            preClosureReceipt.NetAmount = preClosureReceipt.TotalDue - preClosureReceipt.PreClosureDiscount;
            return View(preClosureReceipt);
        }

        public ActionResult BadLoanReceiptForm()
        {
            return View();
        }



        [HttpGet]
        [Route("Receipt/GetBadLoanDetails/{loanCode}")]
        public IActionResult GetBadLoanDetails(string loanCode)
        {
            try
            {
                BadLoanReceipt badLoanReceipt = ReceiptDBService.GetBadLoanDetails(loanCode);

                if (badLoanReceipt == null)
                {
                    return Json(new
                    {
                        success = false,
                        loanStatus = "Invalid",
                        message = "Loan not found"
                    });
                }

                return Json(new
                {
                    success = true,
                    loanCode = badLoanReceipt.LoanCode,
                    memberCode = badLoanReceipt.MemberCode,
                    memberName = badLoanReceipt.MemberName,
                    loanAmount = badLoanReceipt.LoanAmount,
                    totalDue = badLoanReceipt.TotalDue,
                    amountPaid = badLoanReceipt.AmountPaid,
                    pendingAmount = badLoanReceipt.PendingAmount,
                    loanStatus = badLoanReceipt.LoanStatus
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    loanStatus = "Error",
                    message = "Error fetching bad loan details: " + ex.Message
                });
            }
        }

        [HttpPost]
        public ActionResult BadLoanReceipt(BadLoanReceipt badLoanReceipt)
        {
            badLoanReceipt.UserId = HttpContext.Session?.GetString("userId") ?? "";
            ReceiptDBService.GenerateBadLoanReceipt(badLoanReceipt);
            return View(badLoanReceipt);
        }
    }
}