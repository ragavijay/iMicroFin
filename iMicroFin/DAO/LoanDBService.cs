using MySql.Data.MySqlClient;
using System.Data;
using iMicroFin.Models;
namespace iMicroFin.DAO
{
    public class LoanDBService
    {
        public static string? CheckMember(string memberCode)
        {
            string? memberName = null;
            using (MySqlConnection con = new MySqlConnection(ConfigHelper.GetConnectionString()))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand("SELECT CheckMember(@pMemberCode)", con))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.Add("@pMemberCode", MySqlDbType.VarChar, 9);
                    cmd.Parameters["@pMemberCode"].Value = memberCode;
                    var result = cmd.ExecuteScalar();
                    memberName = result?.ToString() ?? "";
                }
            }
            return memberName;
        }

        public static string? CheckGroup(string groupCode)
        {
            string response = "error";
            using (MySqlConnection con = new MySqlConnection(ConfigHelper.GetConnectionString()))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand("SELECT CheckGroup(@pGroupCode)", con))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.Add("@pGroupCode", MySqlDbType.VarChar, 7);
                    cmd.Parameters["@pGroupCode"].Value = groupCode;

                    var result = cmd.ExecuteScalar();
                    response = result?.ToString() ?? "";
                }
            }
            return response;
        }
        public static int AddLoan(Loan loan)
        {
            int status = 0;
            using (MySqlConnection con = new MySqlConnection(ConfigHelper.GetConnectionString()))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand("AddLoan", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.Add("@pLoanCode", MySqlDbType.VarChar, 10);
                    cmd.Parameters["@pLoanCode"].Direction = ParameterDirection.Output;

                    cmd.Parameters.Add("@pMemberCode", MySqlDbType.VarChar,9);
                    cmd.Parameters["@pMemberCode"].Value = loan.MemberCode;

                    cmd.Parameters.Add("@pBranchId", MySqlDbType.Int32);
                    cmd.Parameters["@pBranchId"].Value = loan.BranchId;

                    cmd.Parameters.Add("@pLoanPurpose", MySqlDbType.VarChar, 40);
                    cmd.Parameters["@pLoanPurpose"].Value = loan.LoanPurpose;

                    cmd.Parameters.Add("@pLoanAmount", MySqlDbType.Int32);
                    cmd.Parameters["@pLoanAmount"].Value = loan.LoanAmount;

                    cmd.Parameters.Add("@pLoanDate", MySqlDbType.Date);
                    cmd.Parameters["@pLoanDate"].Value = loan.LoanDate;

                    cmd.Parameters.Add("@pLoanDisposalDate", MySqlDbType.Date);
                    cmd.Parameters["@pLoanDisposalDate"].Value = loan.LoanDisposalDate;

                    cmd.Parameters.Add("@pProcessingFeeRate", MySqlDbType.Int32);
                    cmd.Parameters["@pProcessingFeeRate"].Value = loan.ProcessingFeeRate;

                    cmd.Parameters.Add("@pProcessingFee", MySqlDbType.Int32);
                    cmd.Parameters["@pProcessingFee"].Value = loan.ProcessingFee;

                    cmd.Parameters.Add("@pInsuranceRate", MySqlDbType.Int32);
                    cmd.Parameters["@pInsuranceRate"].Value = loan.InsuranceRate;

                    cmd.Parameters.Add("@pInsurance", MySqlDbType.Int32);
                    cmd.Parameters["@pInsurance"].Value = loan.Insurance;

                    cmd.Parameters.Add("@pTenure", MySqlDbType.Int32);
                    cmd.Parameters["@pTenure"].Value = loan.Tenure;

                    cmd.Parameters.Add("@pInterestRate", MySqlDbType.Int32);
                    cmd.Parameters["@pInterestRate"].Value = loan.InterestRate;

                    cmd.Parameters.Add("@pEwi", MySqlDbType.Int32);
                    cmd.Parameters["@pEwi"].Value = loan.Ewi;

                    cmd.Parameters.Add("@pStatus", MySqlDbType.Int32);
                    cmd.Parameters["@pStatus"].Direction = ParameterDirection.Output;

                    cmd.ExecuteNonQuery();
                    var statusValue = cmd.Parameters["@pStatus"].Value;
                    status = statusValue != DBNull.Value ? Convert.ToInt32(statusValue) : 0;

                    if (status == 1)
                    {
                        // Fix for @pLoanCode - handle potential null
                        var loanCodeValue = cmd.Parameters["@pLoanCode"].Value;
                        if (loanCodeValue != null && loanCodeValue != DBNull.Value)
                        {
                            loan.LoanCode = loanCodeValue.ToString()!;
                        }
                    }
                }
            }
            return status;
        }

        public static int AddGroupLoan(GroupLoan groupLoan)
        {
            int status = 0;
            using (MySqlConnection con = new MySqlConnection(ConfigHelper.GetConnectionString()))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand("AddGroupLoan", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.Add("@pGroupCode", MySqlDbType.VarChar, 7);
                    cmd.Parameters["@pGroupCode"].Value = groupLoan.GroupCode;

                    cmd.Parameters.Add("@pBranchId", MySqlDbType.Int32);
                    cmd.Parameters["@pBranchId"].Value = groupLoan.BranchId;

                    cmd.Parameters.Add("@pLoanPurpose", MySqlDbType.VarChar, 40);
                    cmd.Parameters["@pLoanPurpose"].Value = groupLoan.LoanPurpose;

                    cmd.Parameters.Add("@pLoanAmount", MySqlDbType.Int32);
                    cmd.Parameters["@pLoanAmount"].Value = groupLoan.LoanAmount;

                    cmd.Parameters.Add("@pLoanDate", MySqlDbType.Date);
                    cmd.Parameters["@pLoanDate"].Value = groupLoan.LoanDate;

                    cmd.Parameters.Add("@pLoanDisposalDate", MySqlDbType.Date);
                    cmd.Parameters["@pLoanDisposalDate"].Value = groupLoan.LoanDisposalDate;

                    cmd.Parameters.Add("@pProcessingFeeRate", MySqlDbType.Int32);
                    cmd.Parameters["@pProcessingFeeRate"].Value = groupLoan.ProcessingFeeRate;

                    cmd.Parameters.Add("@pProcessingFee", MySqlDbType.Int32);
                    cmd.Parameters["@pProcessingFee"].Value = groupLoan.ProcessingFee;

                    cmd.Parameters.Add("@pInsuranceRate", MySqlDbType.Int32);
                    cmd.Parameters["@pInsuranceRate"].Value = groupLoan.InsuranceRate;

                    cmd.Parameters.Add("@pInsurance", MySqlDbType.Int32);
                    cmd.Parameters["@pInsurance"].Value = groupLoan.Insurance;

                    cmd.Parameters.Add("@pTenure", MySqlDbType.Int32);
                    cmd.Parameters["@pTenure"].Value = groupLoan.Tenure;

                    cmd.Parameters.Add("@pInterestRate", MySqlDbType.Int32);
                    cmd.Parameters["@pInterestRate"].Value = groupLoan.InterestRate;

                    cmd.Parameters.Add("@pEwi", MySqlDbType.Int32);
                    cmd.Parameters["@pEwi"].Value = groupLoan.Ewi;

                    cmd.Parameters.Add("@pStatus", MySqlDbType.Int32);
                    cmd.Parameters["@pStatus"].Direction = ParameterDirection.Output;

                    cmd.ExecuteNonQuery();
                    status = Convert.ToInt32(cmd.Parameters["@pStatus"].Value);
                }
            }
            return status;
        }
        public static List<Loan> GetAllLoans(int branchId, string groupCode)
        {
            Loan loan;
            List<Loan> loans = new List<Loan>();
            using (MySqlConnection con = new MySqlConnection(ConfigHelper.GetConnectionString()))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand("GetAllLoans", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add("@pBranchId", MySqlDbType.Int32);
                    cmd.Parameters["@pBranchId"].Value = branchId;
                    cmd.Parameters.Add("@pGroupCode", MySqlDbType.VarChar,7);
                    cmd.Parameters["@pGroupCode"].Value = groupCode;
                    using (MySqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            loan = new Loan();
                            loan.LoanCode = rdr["LoanCode"].ToString();
                            loan.MemberCode = rdr["MemberCode"].ToString();
                            loan.MemberName = rdr["MemberName"].ToString();
                            loan.LoanPurpose = rdr["LoanPurpose"].ToString();
                            loan.LoanAmount = Convert.ToInt32(rdr["LoanAmount"].ToString());
                            loan.ProcessingFeeRate = Convert.ToSingle(rdr["ProcessingFeeRate"].ToString());
                            loan.ProcessingFee = Convert.ToInt32(rdr["ProcessingFee"].ToString());
                            loan.InsuranceRate = Convert.ToSingle(rdr["InsuranceRate"].ToString());
                            loan.Insurance = Convert.ToInt32(rdr["Insurance"].ToString());
                            loan.Tenure = Convert.ToInt32(rdr["Tenure"].ToString());
                            loan.InterestRate = Convert.ToSingle(rdr["InterestRate"].ToString());
                            loan.Ewi = Convert.ToInt32(rdr["Ewi"].ToString());
                            loan.LoanStatus = rdr["LoanStatus"].ToString();
                            //loan.RepaymentAmount = loan.Tenure * loan.Ewi + loan.ProcessingFee + loan.Insurance;
                            loan.RepaymentAmount = loan.Tenure * loan.Ewi;
                            loan.StatusRemarks = rdr["StatusRemarks"].ToString();
                            loan.LoanCycle = Convert.ToInt32(rdr["LoanCycle"].ToString());
                            loans.Add(loan);
                        }
                    }
                }
            }
            return loans;
        }

        public static List<MemberLoan> GetAllMemberLoans(int branchId, string groupCode)
        {
            Loan loan;
            Member member;
            MemberLoan memberLoan;
            List<MemberLoan> memberLoans = new List<MemberLoan>();
            using (MySqlConnection con = new MySqlConnection(ConfigHelper.GetConnectionString()))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand("GetAllMemberLoans", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add("@pBranchId", MySqlDbType.Int32);
                    cmd.Parameters["@pBranchId"].Value = branchId;
                    cmd.Parameters.Add("@pGroupCode", MySqlDbType.VarChar, 7);
                    cmd.Parameters["@pGroupCode"].Value = groupCode;
                    using (MySqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            memberLoan = new MemberLoan();
                            loan = new Loan();
                            loan.LoanCode = rdr["LoanCode"].ToString();
                            loan.MemberCode = rdr["MemberCode"].ToString();
                            loan.MemberName = rdr["MemberName"].ToString();
                            loan.LoanAmount = Convert.ToInt32(rdr["LoanAmount"].ToString());
                            loan.LoanCycle = Convert.ToInt32(rdr["LoanCycle"].ToString());
                            member = new Member();
                            member.MemberId = loan.MemberId;
                            member.MemberName = loan.MemberName;
                            member.AccountNumber = rdr["AccountNumber"]?.ToString() ?? "";
                            member.IFSC = rdr["IFSC"]?.ToString() ?? "";
                            memberLoan.member = member;
                            memberLoan.loan = loan;
                            memberLoans.Add(memberLoan);
                        }
                    }
                }
            }
            return memberLoans;
        }

        public static List<Loan> GetAllPendingLoans(int branchId)
        {
            Loan loan;
            List<Loan> loans = new List<Loan>();
            using (MySqlConnection con = new MySqlConnection(ConfigHelper.GetConnectionString()))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand("GetAllPendingLoans", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add("@pBranchId", MySqlDbType.Int32);
                    cmd.Parameters["@pBranchId"].Value = branchId;
                    using (MySqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            loan = new Loan();
                            loan.LoanCode = rdr["LoanCode"].ToString();
                            loan.MemberCode = rdr["MemberCode"].ToString();
                            loan.MemberName = rdr["MemberName"].ToString();
                            loan.LoanPurpose = rdr["LoanPurpose"].ToString();
                            loan.LoanAmount = Convert.ToInt32(rdr["LoanAmount"].ToString());
                            loan.ProcessingFeeRate = Convert.ToSingle(rdr["ProcessingFeeRate"].ToString());
                            loan.ProcessingFee = Convert.ToInt32(rdr["ProcessingFee"].ToString());
                            loan.InsuranceRate = Convert.ToSingle(rdr["InsuranceRate"].ToString());
                            loan.Insurance = Convert.ToInt32(rdr["Insurance"].ToString());
                            loan.Tenure = Convert.ToInt32(rdr["Tenure"].ToString());
                            loan.InterestRate = Convert.ToSingle(rdr["InterestRate"].ToString());
                            loan.Ewi = Convert.ToInt32(rdr["Ewi"].ToString());
                            loan.LoanStatus = rdr["LoanStatus"].ToString();
                            //loan.RepaymentAmount = loan.Tenure * loan.Ewi + loan.ProcessingFee + loan.Insurance;
                            loan.RepaymentAmount = loan.Tenure * loan.Ewi;
                            loan.StatusRemarks = rdr["StatusRemarks"].ToString();
                            loan.LoanCycle = Convert.ToInt32(rdr["LoanCycle"].ToString());
                            loans.Add(loan);
                        }
                    }
                }
            }
            return loans;
        }
        public static Loan GetLoan(string loanCode)
        {
            Loan loan = null;
            using (MySqlConnection con = new MySqlConnection(ConfigHelper.GetConnectionString()))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand("GetLoan", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add("@pLoanCode", MySqlDbType.VarChar, 10);
                    cmd.Parameters["@pLoanCode"].Value = loanCode;
                    using (MySqlDataReader rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            loan = new Loan();
                            loan.LoanCode = rdr["LoanCode"].ToString();
                            loan.MemberCode = rdr["MemberCode"].ToString();
                            loan.MemberName = rdr["MemberName"].ToString();
                            loan.LoanPurpose = rdr["LoanPurpose"].ToString();
                            loan.LoanAmount = Convert.ToInt32(rdr["LoanAmount"].ToString());
                            loan.ProcessingFeeRate = Convert.ToSingle(rdr["ProcessingFeeRate"].ToString());
                            loan.ProcessingFee = Convert.ToInt32(rdr["ProcessingFee"].ToString());
                            loan.InsuranceRate = Convert.ToSingle(rdr["InsuranceRate"].ToString());
                            loan.Insurance = Convert.ToInt32(rdr["Insurance"].ToString());
                            loan.Tenure = Convert.ToInt32(rdr["Tenure"].ToString());
                            loan.InterestRate = Convert.ToSingle(rdr["InterestRate"].ToString());
                            loan.Ewi = Convert.ToInt32(rdr["Ewi"].ToString());
                            loan.LoanStatus = rdr["LoanStatus"].ToString();
                            //loan.RepaymentAmount = loan.Tenure * loan.Ewi + loan.ProcessingFee + loan.Insurance;
                            loan.RepaymentAmount = loan.Tenure * loan.Ewi;
                            loan.StatusRemarks = rdr["StatusRemarks"].ToString();
                            loan.LoanDate = DateTime.Parse(rdr["LoanDate"].ToString());
                            loan.LoanDisposalDate = DateTime.Parse(rdr["LoanDisposalDate"].ToString());
                            loan.LoanCycle = Convert.ToInt32(rdr["LoanCycle"].ToString());
                        }
                    }
                }
            }
            return loan;
        }

        public static int EditLoan(Loan loan)
        {
            int statusCode = 0;
            using (MySqlConnection con = new MySqlConnection(ConfigHelper.GetConnectionString()))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand("EditLoan", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.Add("@pLoanCode", MySqlDbType.VarChar, 10);
                    cmd.Parameters["@pLoanCode"].Value = loan.LoanCode;

                    cmd.Parameters.Add("@pMemberCode", MySqlDbType.VarChar, 9);
                    cmd.Parameters["@pMemberCode"].Value = loan.MemberCode;

                    cmd.Parameters.Add("@pBranchId", MySqlDbType.Int32);
                    cmd.Parameters["@pBranchId"].Value = loan.BranchId;

                    cmd.Parameters.Add("@pLoanPurpose", MySqlDbType.VarChar, 40);
                    cmd.Parameters["@pLoanPurpose"].Value = loan.LoanPurpose;

                    cmd.Parameters.Add("@pLoanAmount", MySqlDbType.Int32);
                    cmd.Parameters["@pLoanAmount"].Value = loan.LoanAmount;

                    cmd.Parameters.Add("@pLoanDate", MySqlDbType.Date);
                    cmd.Parameters["@pLoanDate"].Value = loan.LoanDate;

                    cmd.Parameters.Add("@pLoanDisposalDate", MySqlDbType.Date);
                    cmd.Parameters["@pLoanDisposalDate"].Value = loan.LoanDisposalDate;

                    cmd.Parameters.Add("@pProcessingFeeRate", MySqlDbType.Int32);
                    cmd.Parameters["@pProcessingFeeRate"].Value = loan.ProcessingFeeRate;

                    cmd.Parameters.Add("@pProcessingFee", MySqlDbType.Int32);
                    cmd.Parameters["@pProcessingFee"].Value = loan.ProcessingFee;

                    cmd.Parameters.Add("@pInsuranceRate", MySqlDbType.Int32);
                    cmd.Parameters["@pInsuranceRate"].Value = loan.InsuranceRate;

                    cmd.Parameters.Add("@pInsurance", MySqlDbType.Int32);
                    cmd.Parameters["@pInsurance"].Value = loan.Insurance;

                    cmd.Parameters.Add("@pTenure", MySqlDbType.Int32);
                    cmd.Parameters["@pTenure"].Value = loan.Tenure;

                    cmd.Parameters.Add("@pInterestRate", MySqlDbType.Int32);
                    cmd.Parameters["@pInterestRate"].Value = loan.InterestRate;

                    cmd.Parameters.Add("@pEwi", MySqlDbType.Int32);
                    cmd.Parameters["@pEwi"].Value = loan.Ewi;

                    cmd.Parameters.Add("@pStatusCode", MySqlDbType.Int32);
                    cmd.Parameters["@pStatusCode"].Direction = ParameterDirection.Output;

                    cmd.ExecuteNonQuery();
                    statusCode = Convert.ToInt32(cmd.Parameters["@pStatusCode"].Value);
                }
            }
            return statusCode;
        }
        public static void ApproveLoan(string loanCode)
        {
            using (MySqlConnection con = new MySqlConnection(ConfigHelper.GetConnectionString()))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand("ApproveLoan", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add("@pLoanCode", MySqlDbType.VarChar, 10);
                    cmd.Parameters["@pLoanCode"].Value = loanCode;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void UpdateLoanStatus(string loanCode, string loanStatus, string statusRemarks)
        {
            using (MySqlConnection con = new MySqlConnection(ConfigHelper.GetConnectionString()))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand("UpdateLoanStatus", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add("@pLoanCode", MySqlDbType.VarChar, 10);
                    cmd.Parameters["@pLoanCode"].Value = loanCode;
                    cmd.Parameters.Add("@pLoanStatus", MySqlDbType.VarChar, 1);
                    cmd.Parameters["@pLoanStatus"].Value = loanStatus;
                    cmd.Parameters.Add("@pStatusRemarks", MySqlDbType.VarChar, 50);
                    cmd.Parameters["@pStatusRemarks"].Value = statusRemarks;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static LoanRepaymentStatus GetLoanRepaymentStatus(string groupCode)
        {
            LoanRepaymentStatus loanStatus = null;
            using (MySqlConnection con = new MySqlConnection(ConfigHelper.GetConnectionString()))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand("GetRepaymentStatusGroupInfo", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add("@pGroupCode", MySqlDbType.VarChar,7);
                    cmd.Parameters["@pGroupCode"].Value = groupCode;
                    using (MySqlDataReader rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            loanStatus = new LoanRepaymentStatus();
                            loanStatus.GroupCode = groupCode;
                            loanStatus.GroupName = rdr["GroupName"].ToString();
                            loanStatus.LeaderName = rdr["LeaderName"].ToString();
                            loanStatus.LoanAmount = Convert.ToInt32(rdr["LoanAmount"].ToString());
                            loanStatus.LoanDate = Convert.ToDateTime(rdr["LoanDate"].ToString());
                            loanStatus.CollectionDay = loanStatus.LoanDate.ToString("dddd");
                            loanStatus.Tenure = Convert.ToInt32(rdr["Tenure"].ToString());
                            loanStatus.EWI = Convert.ToInt32(rdr["EWI"].ToString());
                            loanStatus.EWIs = "";
                            loanStatus.StartingDate = loanStatus.LoanDate.AddDays(7);
                            loanStatus.EndingDate = loanStatus.LoanDate.AddDays(7 * loanStatus.Tenure);

                        }
                    }
                }
            }

            if (loanStatus != null)
            {
                using (MySqlConnection con = new MySqlConnection(ConfigHelper.GetConnectionString()))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand("GetRepaymentStatusMemberCount", con))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add("@pGroupCode", MySqlDbType.VarChar, 7);
                        cmd.Parameters["@pGroupCode"].Value = groupCode;
                        using (MySqlDataReader rdr = cmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                loanStatus.MemberCount = Convert.ToInt32(rdr["MemberCount"].ToString());
                            }
                        }
                    }
                }
                loanStatus.MemberCode = new string[loanStatus.MemberCount];
                loanStatus.MemberName = new string[loanStatus.MemberCount];
                loanStatus.LoanCode = new string[loanStatus.MemberCount];
                loanStatus.MemberEWI = new int[loanStatus.MemberCount];
                loanStatus.ActualDate = new DateTime[loanStatus.Tenure];
                loanStatus.Amount = new int[loanStatus.Tenure, loanStatus.MemberCount];
                loanStatus.ColTotal = new int[loanStatus.Tenure];
                loanStatus.RowTotal = new int[loanStatus.MemberCount];
                loanStatus.TotalAmount = new int[loanStatus.MemberCount];
                loanStatus.PendingAmount = new int[loanStatus.MemberCount];
                using (MySqlConnection con = new MySqlConnection(ConfigHelper.GetConnectionString()))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand("GetRepaymentStatusMemberInfo", con))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add("@pGroupCode", MySqlDbType.VarChar,7);
                        cmd.Parameters["@pGroupCode"].Value = groupCode;
                        using (MySqlDataReader rdr = cmd.ExecuteReader())
                        {
                            int i = 0;
                            while (rdr.Read())
                            {
                                loanStatus.MemberCode[i] = rdr["MemberCode"].ToString();
                                loanStatus.MemberName[i] = rdr["MemberName"].ToString();
                                loanStatus.LoanCode[i] = rdr["LoanCode"].ToString();
                                loanStatus.MemberEWI[i] = Convert.ToInt32(rdr["EWI"].ToString());
                                if (loanStatus.EWIs == "")
                                {
                                    loanStatus.EWIs = loanStatus.MemberEWI[i].ToString();
                                }
                                else
                                {
                                    List<string> EWIs = loanStatus.EWIs.Split('/').ToList<string>();
                                    if(!EWIs.Contains(loanStatus.MemberEWI[i].ToString()))
                                    {
                                        loanStatus.EWIs = loanStatus.EWIs + "/" + loanStatus.MemberEWI[i].ToString();
                                    }
                                }
                                
                                i++;
                            }
                        }
                    }
                }
                for (int i = 0; i < loanStatus.MemberCount; i++)
                {
                    int installmetsPaid = 0;
                    using (MySqlConnection con = new MySqlConnection(ConfigHelper.GetConnectionString()))
                    {
                        con.Open();
                        using (MySqlCommand cmd = new MySqlCommand("GetPaymentDates", con))
                        {
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            cmd.Parameters.Add("@pLoanCode", MySqlDbType.VarChar, 10);
                            cmd.Parameters["@pLoanCode"].Value = loanStatus.LoanCode[i];

                            using (MySqlDataReader rdr = cmd.ExecuteReader())
                            {
                                int currentInstallments;
                                while (rdr.Read())
                                {
                                    currentInstallments = Convert.ToInt32(rdr["ReceiptAmount"].ToString()) / loanStatus.MemberEWI[i];
                                    for (int j = installmetsPaid; j <= installmetsPaid + currentInstallments - 1; j++)
                                    {
                                        if (loanStatus.ActualDate[j] < Convert.ToDateTime(rdr["ActualReceiptDate"].ToString()))
                                        {
                                            loanStatus.ActualDate[j] = Convert.ToDateTime(rdr["ActualReceiptDate"].ToString());
                                        }
                                        loanStatus.Amount[j, i] = loanStatus.MemberEWI[i];
                                        loanStatus.ColTotal[j] += loanStatus.MemberEWI[i];
                                        loanStatus.RowTotal[i] += loanStatus.MemberEWI[i];
                                    }
                                    installmetsPaid += currentInstallments;
                                }
                            }
                        }
                    }
                }
                for (int i = 0; i < loanStatus.MemberCount; i++)
                {
                    loanStatus.TotalAmount[i] = loanStatus.MemberEWI[i] * loanStatus.Tenure;
                    loanStatus.OverallTotalAmount += loanStatus.TotalAmount[i];
                    loanStatus.OverallRecdAmount += loanStatus.RowTotal[i];
                    loanStatus.PendingAmount[i] = loanStatus.TotalAmount[i] - loanStatus.RowTotal[i];
                    loanStatus.OverallPendingAmount += loanStatus.PendingAmount[i];
                }
            }
            return loanStatus;
        }

        public static List<CumulativeReport> GetCumulativeReport()
        {
            List<CumulativeReport> cumulativeReportList = new List<CumulativeReport>();
            CumulativeReport cumulativeReport;
            using (MySqlConnection con = new MySqlConnection(ConfigHelper.GetConnectionString()))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand("GetCumulativeReport", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    using (MySqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            cumulativeReport = new CumulativeReport();
                            cumulativeReport.GroupCode = rdr["GroupCode"].ToString();
                            cumulativeReport.GroupName = rdr["GroupName"].ToString();
                            cumulativeReport.LeaderName = rdr["LeaderName"].ToString();
                            cumulativeReport.EwiDay = rdr["EwiDay"].ToString();
                            cumulativeReport.TotalEwi = Convert.ToInt32(rdr["TotalEwi"].ToString());
                            cumulativeReport.TotalMembers = Convert.ToInt32(rdr["TotalMembers"].ToString());
                            cumulativeReport.Ewi = Convert.ToInt32(rdr["Ewi"].ToString());
                            cumulativeReport.Tenure = Convert.ToInt32(rdr["Tenure"].ToString());
                            cumulativeReport.TotalEwiReceived = Convert.ToInt32(rdr["TotalEwiReceived"].ToString());
                            cumulativeReport.Setup();
                            cumulativeReportList.Add(cumulativeReport);
                        }
                    }
                }
            }
            return cumulativeReportList;
        }

        public static List<MemberLoan> GetGlobalMemberLoans(int branchId)
        {
            Loan loan;
            Member member;
            MemberLoan memberLoan;
            List<MemberLoan> memberLoans = new List<MemberLoan>();
            using (MySqlConnection con = new MySqlConnection(ConfigHelper.GetConnectionString()))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand("GetGlobalMemberLoans", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add("@pBranchId", MySqlDbType.Int32);
                    cmd.Parameters["@pBranchId"].Value = branchId;
                    using (MySqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            memberLoan = new MemberLoan();
                            loan = new Loan();
                            loan.LoanCode = rdr["LoanCode"].ToString();
                            loan.LoanDate = Convert.ToDateTime(rdr["LoanDate"].ToString());
                            loan.LoanStatus = rdr["LoanStatus"].ToString();
                            loan.LoanPurpose = rdr["LoanPurpose"].ToString();
                            loan.MemberId = Convert.ToInt32(rdr["MemberId"].ToString());
                            loan.MemberName = rdr["MemberName"].ToString();
                            loan.LoanAmount = Convert.ToInt32(rdr["LoanAmount"].ToString());
                            loan.LoanCycle = Convert.ToInt32(rdr["LoanCycle"].ToString());
                            try
                            {
                                loan.LastPaymentDate = Convert.ToDateTime(rdr["LastPaymentDate"].ToString());
                            } catch(Exception e)
                            {
                                //loan.LastPaymentDate = null;
                                e.ToString();
                            }
                            loan.Tenure = Convert.ToInt32(rdr["Tenure"].ToString());
                            loan.Ewi = Convert.ToInt32(rdr["EWI"].ToString());
                            member = new Member();
                            member.MemberCode = loan.MemberCode;
                            member.MemberName = loan.MemberName;
                            member.AccountNumber = rdr["AccountNumber"].ToString();
                            member.IFSC = rdr["IFSC"].ToString();
                            memberLoan.member = member;
                            memberLoan.loan = loan;
                            memberLoans.Add(memberLoan);
                        }
                    }
                }
            }
            return memberLoans;
        }
    }
}