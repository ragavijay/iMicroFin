using iMicroFin.DAO;
using iMicroFin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace iMicroFin.Controllers
{
    [Authorize(Policy = "DirectorManager")]
    public class ReportController : Controller
    {
        [HttpGet]
        public ActionResult FinancialReport()
        {
            return View();
        }

        [HttpGet]
        [Route("Report/GetFinancialYears")]
        public IActionResult GetFinancialYears()
        {
            try
            {
                List<FinancialYear> financialYears = ReportsDBService.GetFinancialYears();

                return Json(new
                {
                    success = true,
                    financialYears = financialYears
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Error fetching financial years: " + ex.Message
                });
            }
        }

        [HttpGet]
        [Route("Report/GetFinancialReport/{fyCode}")]
        public IActionResult GetFinancialReportData(string fyCode)
        {
            try
            {
                // Get all financial years to find start and end dates
                List<FinancialYear> financialYears = ReportsDBService.GetFinancialYears();
                FinancialYear selectedFY = financialYears.FirstOrDefault(f => f.FYCode == fyCode);

                if (selectedFY == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Invalid financial year selected"
                    });
                }

                FinancialReport report = ReportsDBService.GetFinancialReport(
                    selectedFY.FYCode,
                    selectedFY.StartDate,
                    selectedFY.EndDate
                );

                if (report != null)
                {
                    report.FYDisplay = selectedFY.FYDisplay;
                }

                return Json(new
                {
                    success = true,
                    report = report
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Error generating financial report: " + ex.Message
                });
            }
        }
    }
}