using iMicroFin.Models;
using MySql.Data.MySqlClient;
using System.Data;

namespace iMicroFin.DAO
{
    public class ReportsDBService
    {
        public static List<FinancialYear> GetFinancialYears()
        {
            List<FinancialYear> financialYears = new List<FinancialYear>();

            using (MySqlConnection con = new MySqlConnection(ConfigHelper.GetConnectionString()))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand("GetFinancialYears", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (MySqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            FinancialYear fy = new FinancialYear
                            {
                                FYCode = rdr["FYCode"].ToString(),
                                FYDisplay = rdr["FYDisplay"].ToString(),
                                StartDate = Convert.ToDateTime(rdr["StartDate"]),
                                EndDate = Convert.ToDateTime(rdr["EndDate"])
                            };
                            financialYears.Add(fy);
                        }
                    }
                }
            }

            return financialYears;
        }

        public static FinancialReport GetFinancialReport(string fyCode, DateTime startDate, DateTime endDate)
        {
            FinancialReport report = null;

            using (MySqlConnection con = new MySqlConnection(ConfigHelper.GetConnectionString()))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand("GetFinancialReport", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@pFYCode", MySqlDbType.VarChar, 10);
                    cmd.Parameters["@pFYCode"].Value = fyCode;

                    cmd.Parameters.Add("@pStartDate", MySqlDbType.Date);
                    cmd.Parameters["@pStartDate"].Value = startDate;

                    cmd.Parameters.Add("@pEndDate", MySqlDbType.Date);
                    cmd.Parameters["@pEndDate"].Value = endDate;

                    using (MySqlDataReader rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            report = new FinancialReport
                            {
                                FYCode = rdr["FYCode"].ToString(),
                                TotalLoansCount = Convert.ToInt32(rdr["TotalLoansCount"]),
                                TotalLoansDisbursed = Convert.ToDecimal(rdr["TotalLoansDisbursed"]),
                                TotalEWIReceived = Convert.ToDecimal(rdr["TotalEWIReceived"]),
                                FutureEWIExpected = Convert.ToDecimal(rdr["FutureEWIExpected"]),
                                BadLoanPendingAmount = Convert.ToDecimal(rdr["BadLoanPendingAmount"]),
                                BadLoanDiscountProvided = Convert.ToDecimal(rdr["BadLoanDiscountProvided"]),
                                PreClosureDiscountProvided = Convert.ToDecimal(rdr["PreClosureDiscountProvided"]),
                                ActualInterestIncome = Convert.ToDecimal(rdr["ActualInterestIncome"]),
                                AnticipatedInterestIncome = Convert.ToDecimal(rdr["AnticipatedInterestIncome"])
                            };

                            // Calculate totals
                            report.TotalInterestIncome = report.ActualInterestIncome + report.AnticipatedInterestIncome;
                            report.NetProfit = report.ActualInterestIncome - report.BadLoanPendingAmount - report.BadLoanDiscountProvided;
                        }
                    }
                }
            }

            return report;
        }
    }
}