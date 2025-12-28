using iMicroFin.DAO;
using iMicroFin.Models;
using MySql.Data.MySqlClient;
using System.Data;
namespace iMicroFin.DAO
{
    public class ReceiptDBService
    {
        public static PFReceipt GetPFReceipt(string loanCode)
        {
            PFReceipt pfReceipt = null;
            using (MySqlConnection con = new MySqlConnection(ConfigHelper.GetConnectionString()))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand("GetLoanStatus", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add("@pLoanCode", MySqlDbType.VarChar, 10);
                    cmd.Parameters["@pLoanCode"].Value = loanCode;

                    cmd.Parameters.Add("@pLoanStatus", MySqlDbType.VarChar, 1);
                    cmd.Parameters["@pLoanStatus"].Direction = ParameterDirection.Output;

                    cmd.Parameters.Add("@pMemberCode", MySqlDbType.VarChar, 9);
                    cmd.Parameters["@pMemberCode"].Direction = ParameterDirection.Output;

                    cmd.Parameters.Add("@pMemberName", MySqlDbType.VarChar, 50);
                    cmd.Parameters["@pMemberName"].Direction = ParameterDirection.Output;

                    cmd.Parameters.Add("@pProcessingFee", MySqlDbType.Int32);
                    cmd.Parameters["@pProcessingFee"].Direction = ParameterDirection.Output;

                    cmd.Parameters.Add("@pInsurance", MySqlDbType.Int32);
                    cmd.Parameters["@pInsurance"].Direction = ParameterDirection.Output;

                    cmd.Parameters.Add("@pEwi", MySqlDbType.Int32);
                    cmd.Parameters["@pEwi"].Direction = ParameterDirection.Output;

                    cmd.Parameters.Add("@pNoOfInstalments", MySqlDbType.Int32);
                    cmd.Parameters["@pNoOfInstalments"].Direction = ParameterDirection.Output;

                    cmd.Parameters.Add("@pFromDate", MySqlDbType.Date);
                    cmd.Parameters["@pFromDate"].Direction = ParameterDirection.Output;

                    cmd.Parameters.Add("@pToDate", MySqlDbType.Date);
                    cmd.Parameters["@pToDate"].Direction = ParameterDirection.Output;

                    cmd.ExecuteNonQuery();
                    pfReceipt = new PFReceipt();
                    pfReceipt.LoanCode = loanCode;
                    pfReceipt.LoanStatus = cmd.Parameters["@pLoanStatus"].Value.ToString();
                    if (pfReceipt.LoanStatus == "A")
                    {
                        pfReceipt.MemberCode = cmd.Parameters["@pMemberCode"].Value.ToString();
                        pfReceipt.MemberName = cmd.Parameters["@pMemberName"].Value.ToString();
                        pfReceipt.ProcessingFee = Convert.ToInt32(cmd.Parameters["@pProcessingFee"].Value.ToString());
                        pfReceipt.Insurance = Convert.ToInt32(cmd.Parameters["@pInsurance"].Value.ToString());
                        pfReceipt.TotalFee = pfReceipt.ProcessingFee + pfReceipt.Insurance;
                    }
                }
            }
            return pfReceipt;
        }


        public static InstalmentReceipt GetInstalmentReceipt(string loanCode)
        {
            InstalmentReceipt instalmentReceipt = null;
            using (MySqlConnection con = new MySqlConnection(ConfigHelper.GetConnectionString()))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand("GetLoanStatus", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add("@pLoanCode", MySqlDbType.VarChar, 10);
                    cmd.Parameters["@pLoanCode"].Value = loanCode;

                    cmd.Parameters.Add("@pLoanStatus", MySqlDbType.VarChar, 1);
                    cmd.Parameters["@pLoanStatus"].Direction = ParameterDirection.Output;

                    cmd.Parameters.Add("@pMemberCode", MySqlDbType.VarChar, 9);
                    cmd.Parameters["@pMemberCode"].Direction = ParameterDirection.Output;

                    cmd.Parameters.Add("@pMemberName", MySqlDbType.VarChar, 50);
                    cmd.Parameters["@pMemberName"].Direction = ParameterDirection.Output;

                    cmd.Parameters.Add("@pProcessingFee", MySqlDbType.Int32);
                    cmd.Parameters["@pProcessingFee"].Direction = ParameterDirection.Output;

                    cmd.Parameters.Add("@pInsurance", MySqlDbType.Int32);
                    cmd.Parameters["@pInsurance"].Direction = ParameterDirection.Output;

                    cmd.Parameters.Add("@pEwi", MySqlDbType.Int32);
                    cmd.Parameters["@pEwi"].Direction = ParameterDirection.Output;

                    cmd.Parameters.Add("@pNoOfInstalments", MySqlDbType.Int32);
                    cmd.Parameters["@pNoOfInstalments"].Direction = ParameterDirection.Output;

                    cmd.Parameters.Add("@pFromDate", MySqlDbType.Date);
                    cmd.Parameters["@pFromDate"].Direction = ParameterDirection.Output;

                    cmd.Parameters.Add("@pToDate", MySqlDbType.Date);
                    cmd.Parameters["@pToDate"].Direction = ParameterDirection.Output;

                    cmd.Parameters.Add("@pTotalPendingInstalments", MySqlDbType.Int32);
                    cmd.Parameters["@pTotalPendingInstalments"].Direction = ParameterDirection.Output;

                    cmd.ExecuteNonQuery();
                    instalmentReceipt = new InstalmentReceipt();
                    instalmentReceipt.LoanCode = loanCode;
                    instalmentReceipt.LoanStatus = cmd.Parameters["@pLoanStatus"].Value.ToString();
                    if (instalmentReceipt.LoanStatus == "O")
                    {
                        instalmentReceipt.MemberCode = cmd.Parameters["@pMemberCode"].Value.ToString();
                        instalmentReceipt.MemberName = cmd.Parameters["@pMemberName"].Value.ToString();
                        instalmentReceipt.NoOfInstalments = Convert.ToInt32(cmd.Parameters["@pNoOfInstalments"].Value.ToString());
                        instalmentReceipt.TotalPendingInstalments = Convert.ToInt32(cmd.Parameters["@pTotalPendingInstalments"].Value.ToString());
                        instalmentReceipt.Ewi = Convert.ToInt32(cmd.Parameters["@pEwi"].Value.ToString());
                        instalmentReceipt.TotalDue = instalmentReceipt.NoOfInstalments * instalmentReceipt.Ewi;
                    }
                }
            }
            return instalmentReceipt;
        }

        public static List<InstalmentReceipt> GetGroupInstalmentMembers(string groupCode)
        {
            List<InstalmentReceipt> instalmentReceiptList = new List<InstalmentReceipt>();
            string loanCode;
            using (MySqlConnection con = new MySqlConnection(ConfigHelper.GetConnectionString()))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand("GetGroupMembersOngoingLoan", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add("@pGroupCode", MySqlDbType.VarChar, 7);
                    cmd.Parameters["@pGroupCode"].Value = groupCode;
                    MySqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        loanCode = rdr["LoanCode"].ToString();
                        InstalmentReceipt ir = GetInstalmentReceipt(loanCode);
                        instalmentReceiptList.Add(ir);
                    }
                }
            }
            return instalmentReceiptList;
        }

        public static void GeneratePFReceipt(PFReceipt pfReceipt)
        {
            using (MySqlConnection con = new MySqlConnection(ConfigHelper.GetConnectionString()))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand("GeneratePFReceipt", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add("@pLoanCode", MySqlDbType.VarChar, 10);
                    cmd.Parameters["@pLoanCode"].Value = pfReceipt.LoanCode;

                    cmd.Parameters.Add("@pUserId", MySqlDbType.VarChar, 20);
                    cmd.Parameters["@pUserId"].Value = pfReceipt.UserId;

                    cmd.Parameters.Add("@pActualReceiptDate", MySqlDbType.Date);
                    cmd.Parameters["@pActualReceiptDate"].Value = pfReceipt.ActualReceiptDate;

                    cmd.Parameters.Add("@pReceiptId", MySqlDbType.Int32);
                    cmd.Parameters["@pReceiptId"].Direction = ParameterDirection.Output;

                    cmd.ExecuteNonQuery();
                    pfReceipt.ReceiptId = Convert.ToInt32(cmd.Parameters["@pReceiptId"].Value.ToString());
                }
            }
        }


        public static List<GroupPFReceiptInfo> GenerateGroupPFReceipt(GroupPFReceipt groupPFReceipt)
        {
            string userId = groupPFReceipt.UserId;
            List<GroupPFReceiptInfo> groupPFReceiptInfoList=new List<GroupPFReceiptInfo>();
            using (MySqlConnection con = new MySqlConnection(ConfigHelper.GetConnectionString()))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand("GetGroupMembersLoan", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add("@pGroupCode", MySqlDbType.VarChar, 7);
                    cmd.Parameters["@pGroupCode"].Value = groupPFReceipt.GroupCode;
                    MySqlDataReader rdr =  cmd.ExecuteReader();
                    while(rdr.Read())
                    {
                        GroupPFReceiptInfo groupPFReceiptInfo = new GroupPFReceiptInfo();
                        groupPFReceiptInfo.ReceiptId = 0;
                        groupPFReceiptInfo.MemberCode = rdr["MemberCode"].ToString();
                        groupPFReceiptInfo.MemberName = rdr["MemberName"].ToString();
                        groupPFReceiptInfo.LoanCode = rdr["LoanCode"].ToString();
                        groupPFReceiptInfo.ProcessingFee = Convert.ToInt32(rdr["ProcessingFee"].ToString());
                        groupPFReceiptInfo.Insurance = Convert.ToInt32(rdr["Insurance"].ToString());
                        groupPFReceiptInfo.TotalFee = groupPFReceiptInfo.ProcessingFee + groupPFReceiptInfo.Insurance;
                        groupPFReceiptInfoList.Add(groupPFReceiptInfo);
                    }
                }
            }
            foreach(GroupPFReceiptInfo groupPFReceiptInfo in groupPFReceiptInfoList) {
                using (MySqlConnection con = new MySqlConnection(ConfigHelper.GetConnectionString()))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand("GeneratePFReceipt", con))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add("@pLoanCode", MySqlDbType.VarChar, 10);
                        cmd.Parameters["@pLoanCode"].Value = groupPFReceiptInfo.LoanCode;

                        cmd.Parameters.Add("@pUserId", MySqlDbType.VarChar, 20);
                        cmd.Parameters["@pUserId"].Value = userId;

                        cmd.Parameters.Add("@pActualReceiptDate", MySqlDbType.Date);
                        cmd.Parameters["@pActualReceiptDate"].Value = groupPFReceipt.ActualReceiptDate;

                        cmd.Parameters.Add("@pReceiptId", MySqlDbType.Int32);
                        cmd.Parameters["@pReceiptId"].Direction = ParameterDirection.Output;

                        cmd.ExecuteNonQuery();
                        groupPFReceiptInfo.ReceiptId = Convert.ToInt32(cmd.Parameters["@pReceiptId"].Value.ToString());
                    }
                }
            }
            return groupPFReceiptInfoList;
        }

        public static void GenerateInstalmentReceipt(InstalmentReceipt instalmentReceipt)
        {
            using (MySqlConnection con = new MySqlConnection(ConfigHelper.GetConnectionString()))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand("GenerateInstalmentReceipt", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add("@pLoanCode", MySqlDbType.VarChar, 10);
                    cmd.Parameters["@pLoanCode"].Value = instalmentReceipt.LoanCode;

                    cmd.Parameters.Add("@pNoOfInstalments", MySqlDbType.Int32);
                    cmd.Parameters["@pNoOfInstalments"].Value = instalmentReceipt.NoOfInstalments;

                    cmd.Parameters.Add("@pActualReceiptDate", MySqlDbType.Date);
                    cmd.Parameters["@pActualReceiptDate"].Value = instalmentReceipt.ActualReceiptDate;

                    cmd.Parameters.Add("@pUserId", MySqlDbType.VarChar, 20);
                    cmd.Parameters["@pUserId"].Value = instalmentReceipt.UserId;

                    cmd.Parameters.Add("@pReceiptId", MySqlDbType.Int32);
                    cmd.Parameters["@pReceiptId"].Direction = ParameterDirection.Output;

                    cmd.Parameters.Add("@pNextDueDate", MySqlDbType.Date);
                    cmd.Parameters["@pNextDueDate"].Direction = ParameterDirection.Output;

                    cmd.ExecuteNonQuery();
                    instalmentReceipt.ReceiptId = Convert.ToInt32(cmd.Parameters["@pReceiptId"].Value.ToString());
                    if (cmd.Parameters["@pNextDueDate"].Value == DBNull.Value)
                    {
                        instalmentReceipt.NextDueDate = new DateTime(2000,1,1);
                    }
                    else
                    {
                        instalmentReceipt.NextDueDate = Convert.ToDateTime(cmd.Parameters["@pNextDueDate"].Value.ToString());
                    }
                }
            }
        }

     

        public static List<GroupInstalmentReceiptInfo> GenerateGroupInstalmentReceipt(string userId, string groupCode, DateTime actualReceiptDate,
                                                                                       List<(string memberCode, int noOfInstalments)> memberInstalments)
        {
            List<GroupInstalmentReceiptInfo> groupInstalmentReceiptInfoList = new List<GroupInstalmentReceiptInfo>();

            // Create a dictionary for quick lookup of member-specific instalments
            var memberInstalmentMap = memberInstalments?.ToDictionary(x => x.memberCode, x => x.noOfInstalments)
                ?? new Dictionary<string, int>();

            using (MySqlConnection con = new MySqlConnection(ConfigHelper.GetConnectionString()))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand("GetGroupMembersOngoingLoan", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add("@pGroupCode", MySqlDbType.VarChar, 9);
                    cmd.Parameters["@pGroupCode"].Value = groupCode;

                    MySqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        string memberCode = rdr["MemberCode"].ToString();
                        int ewi = Convert.ToInt32(rdr["Ewi"]);

                        // Get member-specific instalments or use default
                        int noOfInstalments = memberInstalmentMap.ContainsKey(memberCode)
                            ? memberInstalmentMap[memberCode]
                            : 0;
                        if (noOfInstalments>0) {     
                            GroupInstalmentReceiptInfo groupInstalmentReceiptInfo = new GroupInstalmentReceiptInfo();
                            groupInstalmentReceiptInfo.ReceiptId = 0;
                            groupInstalmentReceiptInfo.MemberCode = memberCode;
                            groupInstalmentReceiptInfo.MemberName = rdr["MemberName"].ToString();
                            groupInstalmentReceiptInfo.LoanCode = rdr["LoanCode"].ToString();
                            groupInstalmentReceiptInfo.NoOfInstalments = noOfInstalments;
                            groupInstalmentReceiptInfo.Ewi = ewi;
                            groupInstalmentReceiptInfo.TotalDue = noOfInstalments * ewi;
                            groupInstalmentReceiptInfoList.Add(groupInstalmentReceiptInfo);
                        }
                    }
                }
            }

            // Generate receipt for each member with their specific instalments
            foreach (GroupInstalmentReceiptInfo groupInstalmentReceiptInfo in groupInstalmentReceiptInfoList)
            {
                try
                {
                    using (MySqlConnection con = new MySqlConnection(ConfigHelper.GetConnectionString()))
                    {
                        con.Open();
                        using (MySqlCommand cmd = new MySqlCommand("GenerateInstalmentReceipt", con))
                        {
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            cmd.Parameters.Add("@pLoanCode", MySqlDbType.VarChar, 13);
                            cmd.Parameters["@pLoanCode"].Value = groupInstalmentReceiptInfo.LoanCode;
                            cmd.Parameters.Add("@pNoOfInstalments", MySqlDbType.Int32);
                            cmd.Parameters["@pNoOfInstalments"].Value = groupInstalmentReceiptInfo.NoOfInstalments;
                            cmd.Parameters.Add("@pUserId", MySqlDbType.VarChar, 20);
                            cmd.Parameters["@pUserId"].Value = userId;
                            cmd.Parameters.Add("@pActualReceiptDate", MySqlDbType.Date);
                            cmd.Parameters["@pActualReceiptDate"].Value = actualReceiptDate;
                            cmd.Parameters.Add("@pReceiptId", MySqlDbType.Int32);
                            cmd.Parameters["@pReceiptId"].Direction = ParameterDirection.Output;
                            cmd.Parameters.Add("@pNextDueDate", MySqlDbType.Date);
                            cmd.Parameters["@pNextDueDate"].Direction = ParameterDirection.Output;

                            cmd.ExecuteNonQuery();

                            groupInstalmentReceiptInfo.ReceiptId = Convert.ToInt32(cmd.Parameters["@pReceiptId"].Value.ToString());

                            if (cmd.Parameters["@pNextDueDate"].Value == DBNull.Value)
                            {
                                groupInstalmentReceiptInfo.NextDueDate = new DateTime(2000, 1, 1);
                            }
                            else
                            {
                                groupInstalmentReceiptInfo.NextDueDate = Convert.ToDateTime(cmd.Parameters["@pNextDueDate"].Value.ToString());
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log error but continue processing other members
                    Console.WriteLine($"Error generating receipt for member {groupInstalmentReceiptInfo.MemberCode}: {ex.Message}");
                    throw; // Re-throw if you want to stop on first error
                }
            }

            return groupInstalmentReceiptInfoList;
        }

        public static List<EWIDue> GetEwiDue(int branchId)
        {
            EWIDue ewiDue;
            List<EWIDue> ewiDues = new List<EWIDue>();
            using (MySqlConnection con = new MySqlConnection(ConfigHelper.GetConnectionString()))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand("GetEwiDue", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add("@pBranchId", MySqlDbType.Int32);
                    cmd.Parameters["@pBranchId"].Value = branchId;
                    using (MySqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            ewiDue = new EWIDue();
                            ewiDue.LoanCode = rdr["LoanCode"].ToString();
                            ewiDue.BranchId = Convert.ToInt32(rdr["BranchId"].ToString());
                            ewiDue.MemberCode = rdr["MemberCode"].ToString();
                            ewiDue.MemberName = rdr["MemberName"].ToString();
                            ewiDue.Phone = rdr["Phone"].ToString();
                            ewiDue.NoOfInstalments = Convert.ToInt32(rdr["NoOfInstalments"].ToString());
                            ewiDue.Ewi = Convert.ToInt32(rdr["Ewi"].ToString());
                            ewiDue.PendingAmount = Convert.ToInt32(rdr["PendingAmount"].ToString());
                            ewiDue.DueDate = Convert.ToDateTime(rdr["DueDate"]);
                            ewiDues.Add(ewiDue);
                        }
                    }
                }
            }
            return ewiDues;
        }

        public static List<UserCashReceiptStatement> GetUserCashReceiptStatement(string userId, string userType, DateTime fromDate, DateTime toDate)
        {
            List<UserCashReceiptStatement> statement = new List<UserCashReceiptStatement>();
            UserCashReceiptStatement statementRow;
            using (MySqlConnection con = new MySqlConnection(ConfigHelper.GetConnectionString()))
            {
                con.Open();
                String qry;
                if (userType == "D" || userType == "A")
                {
                    qry = "SELECT R.UserId, U.UserName, R.Amount FROM AppUser U INNER JOIN "
                        + "( SELECT  UserId, SUM(ReceiptAmount) AS Amount FROM CashReceipt WHERE "
                        + "ReceiptDate BETWEEN @pFromDate AND @pToDate GROUP BY UserId)  R "
                        + "ON U.UserId = R.UserId";
                }
                else if (userType == "M")
                {
                    qry = "SELECT R.UserId, U.UserName, R.Amount FROM (SELECT * FROM AppUser WHERE UserId=@pUserId OR "
                        + "UserId IN(Select UserId FROM BranchUser WHERE BranchId IN"
                        + "(SELECT BranchId FROM Branch WHERE BranchManager=@pUserId))) U INNER JOIN "
                        + "(SELECT  UserId, SUM(ReceiptAmount) AS Amount FROM CashReceipt WHERE ReceiptDate BETWEEN @pFromDate AND @pToDate GROUP BY UserId)  R "
                        + "ON U.UserId = R.UserId";
                }
                else
                {
                    qry = "SELECT R.UserId, U.UserName, R.Amount FROM (SELECT * FROM AppUser WHERE UserId=@pUserId) U INNER JOIN "
                        + "( SELECT  UserId, SUM(ReceiptAmount) AS Amount FROM CashReceipt WHERE ReceiptDate BETWEEN @pFromDate AND @pToDate GROUP BY UserId)  R "
                        + "ON U.UserId = R.UserId";
                }
                using (MySqlCommand cmd = new MySqlCommand(qry, con))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.Add("@pFromDate", MySqlDbType.Date);
                    cmd.Parameters["@pFromDate"].Value = fromDate;
                    cmd.Parameters.Add("@pToDate", MySqlDbType.Date);
                    cmd.Parameters["@pToDate"].Value = toDate;
                    if (userType == "M" || userType == "C")
                    {
                        cmd.Parameters.Add("@pUserId", MySqlDbType.VarChar, 20);
                        cmd.Parameters["@pUserId"].Value = userId;
                    }
                    using (MySqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            statementRow = new UserCashReceiptStatement();
                            statementRow.UserId = rdr["UserId"].ToString();
                            statementRow.UserName = rdr["UserName"].ToString();
                            statementRow.Amount = Convert.ToInt32(rdr["Amount"].ToString());
                            statement.Add(statementRow);
                        }
                    }
                }
            }
            return statement;
        }

        public static List<CashReceiptStatement> GetCashReceiptStatement(DateTime fromDate, DateTime toDate)
        {
            List<CashReceiptStatement> statementList = new List<CashReceiptStatement>();
            CashReceiptStatement statementRow;
            using (MySqlConnection con = new MySqlConnection(ConfigHelper.GetConnectionString()))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand("GetCashReceiptStatement", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add("@pFromDate", MySqlDbType.Date);
                    cmd.Parameters["@pFromDate"].Value = fromDate;
                    cmd.Parameters.Add("@pToDate", MySqlDbType.Date);
                    cmd.Parameters["@pToDate"].Value = toDate;
                    using (MySqlDataReader rdr = cmd.ExecuteReader())
                    {
                        int i = 0;
                        while (rdr.Read())
                        {
                            statementRow = new CashReceiptStatement();
                            statementRow.SNo = ++i;
                            statementRow.ReceiptId = Convert.ToInt32(rdr["ReceiptId"].ToString());
                            statementRow.MemberCode = rdr["MemberCode"].ToString();
                            statementRow.MemberName = rdr["MemberName"].ToString();
                            statementRow.LoanCode = rdr["LoanCode"].ToString();
                            statementRow.Description = rdr["Description"].ToString();
                            statementRow.Amount = Convert.ToInt32(rdr["Amount"].ToString());
                            statementRow.ActualReceiptDate = Convert.ToDateTime(rdr["ActualReceiptDate"].ToString());
                            statementList.Add(statementRow);
                        }
                    }
                }
            }
            return statementList;
        }

        public static GroupPFReceipt GetGroupPFReceipt(string groupCode)
        {
            GroupPFReceipt groupPfReceipt = null;
            using (MySqlConnection con = new MySqlConnection(ConfigHelper.GetConnectionString()))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand("GetGroupPFStatus", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add("@pGroupCode", MySqlDbType.VarChar, 6);
                    cmd.Parameters["@pGroupCode"].Value = groupCode;

                    cmd.Parameters.Add("@pStatusCode", MySqlDbType.Int32);
                    cmd.Parameters["@pStatusCode"].Direction = ParameterDirection.Output;

                    
                    cmd.Parameters.Add("@pProcessingFee", MySqlDbType.Int32);
                    cmd.Parameters["@pProcessingFee"].Direction = ParameterDirection.Output;

                    cmd.Parameters.Add("@pInsurance", MySqlDbType.Int32);
                    cmd.Parameters["@pInsurance"].Direction = ParameterDirection.Output;

                    
                    cmd.ExecuteNonQuery();
                    groupPfReceipt = new GroupPFReceipt();
                    groupPfReceipt.GroupCode = groupCode;
                    groupPfReceipt.StatusCode = Convert.ToInt32(cmd.Parameters["@pStatusCode"].Value.ToString());
                    if (groupPfReceipt.StatusCode == 1)
                    {

                        groupPfReceipt.ProcessingFee = Convert.ToInt32(cmd.Parameters["@pProcessingFee"].Value.ToString());
                        groupPfReceipt.Insurance = Convert.ToInt32(cmd.Parameters["@pInsurance"].Value.ToString());
                        groupPfReceipt.TotalFee = groupPfReceipt.ProcessingFee + groupPfReceipt.Insurance;
                    }
                }
            }
            return groupPfReceipt;
        }

        public static GroupInstalmentReceipt GetGroupInstalmentReceipt(string groupCode)
        {
            GroupInstalmentReceipt groupInstalmentReceipt = null;
            using (MySqlConnection con = new MySqlConnection(ConfigHelper.GetConnectionString()))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand("GetGroupInstalmentStatus", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add("@pGroupCode", MySqlDbType.VarChar, 6);
                    cmd.Parameters["@pGroupCode"].Value = groupCode;

                    cmd.Parameters.Add("@pStatusCode", MySqlDbType.Int32);
                    cmd.Parameters["@pStatusCode"].Direction = ParameterDirection.Output;


                    cmd.Parameters.Add("@pNoOfInstalments", MySqlDbType.Int32);
                    cmd.Parameters["@pNoOfInstalments"].Direction = ParameterDirection.Output;

                    cmd.Parameters.Add("@pEWI", MySqlDbType.Int32);
                    cmd.Parameters["@pEWI"].Direction = ParameterDirection.Output;


                    cmd.ExecuteNonQuery();
                    groupInstalmentReceipt = new GroupInstalmentReceipt();
                    groupInstalmentReceipt.GroupCode = groupCode;
                    groupInstalmentReceipt.StatusCode = Convert.ToInt32(cmd.Parameters["@pStatusCode"].Value.ToString());
                    if (groupInstalmentReceipt.StatusCode == 1)
                    {

                        groupInstalmentReceipt.NoOfInstalments = Convert.ToInt32(cmd.Parameters["@pNoOfInstalments"].Value.ToString());
                        groupInstalmentReceipt.Ewi = Convert.ToInt32(cmd.Parameters["@pEWI"].Value.ToString());
                        groupInstalmentReceipt.TotalDue = groupInstalmentReceipt.NoOfInstalments * groupInstalmentReceipt.Ewi;
                    }
                }
            }
            return groupInstalmentReceipt;
        }

        public static PreClosureReceipt GetPreClosureDetails(string loanCode)
        {
            PreClosureReceipt preClosureReceipt = null;
            using (MySqlConnection con = new MySqlConnection(ConfigHelper.GetConnectionString()))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand("GetPreClosureDetails", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.Add("@pLoanCode", MySqlDbType.VarChar, 10);
                    cmd.Parameters["@pLoanCode"].Value = loanCode;

                    cmd.Parameters.Add("@pLoanStatus", MySqlDbType.VarChar, 1);
                    cmd.Parameters["@pLoanStatus"].Direction = ParameterDirection.Output;

                    cmd.Parameters.Add("@pMemberCode", MySqlDbType.VarChar, 9);
                    cmd.Parameters["@pMemberCode"].Direction = ParameterDirection.Output;

                    cmd.Parameters.Add("@pMemberName", MySqlDbType.VarChar, 50);
                    cmd.Parameters["@pMemberName"].Direction = ParameterDirection.Output;

                    cmd.Parameters.Add("@pLoanAmount", MySqlDbType.Int32);
                    cmd.Parameters["@pLoanAmount"].Direction = ParameterDirection.Output;

                    cmd.Parameters.Add("@pInterestRate", MySqlDbType.Decimal);
                    cmd.Parameters["@pInterestRate"].Direction = ParameterDirection.Output;

                    cmd.Parameters.Add("@pTenure", MySqlDbType.Int32);
                    cmd.Parameters["@pTenure"].Direction = ParameterDirection.Output;

                    cmd.Parameters.Add("@pLoanDisposalDate", MySqlDbType.Date);
                    cmd.Parameters["@pLoanDisposalDate"].Direction = ParameterDirection.Output;

                    cmd.Parameters.Add("@pEwi", MySqlDbType.Int32);
                    cmd.Parameters["@pEwi"].Direction = ParameterDirection.Output;

                    cmd.Parameters.Add("@pTotalPendingInstalments", MySqlDbType.Int32);
                    cmd.Parameters["@pTotalPendingInstalments"].Direction = ParameterDirection.Output;

                    cmd.Parameters.Add("@pTotalDue", MySqlDbType.Int32);
                    cmd.Parameters["@pTotalDue"].Direction = ParameterDirection.Output;

                    cmd.Parameters.Add("@pSuggestedDiscount", MySqlDbType.Int32);
                    cmd.Parameters["@pSuggestedDiscount"].Direction = ParameterDirection.Output;

                    cmd.Parameters.Add("@pInterestSavings", MySqlDbType.Int32);
                    cmd.Parameters["@pInterestSavings"].Direction = ParameterDirection.Output;

                    cmd.Parameters.Add("@pDelayPenalty", MySqlDbType.Int32);
                    cmd.Parameters["@pDelayPenalty"].Direction = ParameterDirection.Output;

                    cmd.Parameters.Add("@pAdvanceBenefit", MySqlDbType.Int32);
                    cmd.Parameters["@pAdvanceBenefit"].Direction = ParameterDirection.Output;

                    cmd.ExecuteNonQuery();

                    preClosureReceipt = new PreClosureReceipt();
                    preClosureReceipt.LoanCode = loanCode;
                    preClosureReceipt.LoanStatus = cmd.Parameters["@pLoanStatus"].Value.ToString();

                    if (preClosureReceipt.LoanStatus == "O")
                    {
                        preClosureReceipt.MemberCode = cmd.Parameters["@pMemberCode"].Value.ToString();
                        preClosureReceipt.MemberName = cmd.Parameters["@pMemberName"].Value.ToString();
                        preClosureReceipt.LoanAmount = Convert.ToInt32(cmd.Parameters["@pLoanAmount"].Value);
                        preClosureReceipt.InterestRate = Convert.ToDecimal(cmd.Parameters["@pInterestRate"].Value);
                        preClosureReceipt.Tenure = Convert.ToInt32(cmd.Parameters["@pTenure"].Value);
                        preClosureReceipt.LoanDisposalDate = Convert.ToDateTime(cmd.Parameters["@pLoanDisposalDate"].Value);
                        preClosureReceipt.Ewi = Convert.ToInt32(cmd.Parameters["@pEwi"].Value);
                        preClosureReceipt.TotalPendingInstalments = Convert.ToInt32(cmd.Parameters["@pTotalPendingInstalments"].Value);
                        preClosureReceipt.TotalDue = Convert.ToInt32(cmd.Parameters["@pTotalDue"].Value);
                        preClosureReceipt.SuggestedDiscount = Convert.ToInt32(cmd.Parameters["@pSuggestedDiscount"].Value);
                        preClosureReceipt.InterestSavings = Convert.ToInt32(cmd.Parameters["@pInterestSavings"].Value);
                        preClosureReceipt.DelayPenalty = Convert.ToInt32(cmd.Parameters["@pDelayPenalty"].Value);
                        preClosureReceipt.AdvanceBenefit = Convert.ToInt32(cmd.Parameters["@pAdvanceBenefit"].Value);
                    }
                }
            }
            return preClosureReceipt;
        }

        public static void GeneratePreClosureReceipt(PreClosureReceipt preClosureReceipt)
        {
            using (MySqlConnection con = new MySqlConnection(ConfigHelper.GetConnectionString()))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand("GeneratePreClosureReceipt", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.Add("@pLoanCode", MySqlDbType.VarChar, 10);
                    cmd.Parameters["@pLoanCode"].Value = preClosureReceipt.LoanCode;

                    cmd.Parameters.Add("@pPreClosureDiscount", MySqlDbType.Int32);
                    cmd.Parameters["@pPreClosureDiscount"].Value = preClosureReceipt.PreClosureDiscount;

                    cmd.Parameters.Add("@pActualReceiptDate", MySqlDbType.Date);
                    cmd.Parameters["@pActualReceiptDate"].Value = preClosureReceipt.ActualReceiptDate;

                    cmd.Parameters.Add("@pUserId", MySqlDbType.VarChar, 20);
                    cmd.Parameters["@pUserId"].Value = preClosureReceipt.UserId;

                    cmd.Parameters.Add("@pReceiptId", MySqlDbType.Int32);
                    cmd.Parameters["@pReceiptId"].Direction = ParameterDirection.Output;

                    cmd.ExecuteNonQuery();

                    preClosureReceipt.ReceiptId = Convert.ToInt32(cmd.Parameters["@pReceiptId"].Value.ToString());
                }
            }
        }
        public static BadLoanReceipt GetBadLoanDetails(string loanCode)
        {
            BadLoanReceipt badLoanReceipt = null;
            using (MySqlConnection con = new MySqlConnection(ConfigHelper.GetConnectionString()))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand("GetBadLoanDetails", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.Add("@pLoanCode", MySqlDbType.VarChar, 10);
                    cmd.Parameters["@pLoanCode"].Value = loanCode;

                    cmd.Parameters.Add("@pLoanStatus", MySqlDbType.VarChar, 1);
                    cmd.Parameters["@pLoanStatus"].Direction = ParameterDirection.Output;

                    cmd.Parameters.Add("@pMemberCode", MySqlDbType.VarChar, 9);
                    cmd.Parameters["@pMemberCode"].Direction = ParameterDirection.Output;

                    cmd.Parameters.Add("@pMemberName", MySqlDbType.VarChar, 50);
                    cmd.Parameters["@pMemberName"].Direction = ParameterDirection.Output;

                    cmd.Parameters.Add("@pLoanAmount", MySqlDbType.Int32);
                    cmd.Parameters["@pLoanAmount"].Direction = ParameterDirection.Output;

                    cmd.Parameters.Add("@pTotalDue", MySqlDbType.Int32);
                    cmd.Parameters["@pTotalDue"].Direction = ParameterDirection.Output;

                    cmd.Parameters.Add("@pAmountPaid", MySqlDbType.Int32);
                    cmd.Parameters["@pAmountPaid"].Direction = ParameterDirection.Output;

                    cmd.Parameters.Add("@pPendingAmount", MySqlDbType.Int32);
                    cmd.Parameters["@pPendingAmount"].Direction = ParameterDirection.Output;

                    cmd.ExecuteNonQuery();

                    badLoanReceipt = new BadLoanReceipt();
                    badLoanReceipt.LoanCode = loanCode;
                    badLoanReceipt.LoanStatus = cmd.Parameters["@pLoanStatus"].Value.ToString();

                    if (badLoanReceipt.LoanStatus == "b")
                    {
                        badLoanReceipt.MemberCode = cmd.Parameters["@pMemberCode"].Value.ToString();
                        badLoanReceipt.MemberName = cmd.Parameters["@pMemberName"].Value.ToString();
                        badLoanReceipt.LoanAmount = Convert.ToInt32(cmd.Parameters["@pLoanAmount"].Value);
                        badLoanReceipt.TotalDue = Convert.ToInt32(cmd.Parameters["@pTotalDue"].Value);
                        badLoanReceipt.AmountPaid = Convert.ToInt32(cmd.Parameters["@pAmountPaid"].Value);
                        badLoanReceipt.PendingAmount = Convert.ToInt32(cmd.Parameters["@pPendingAmount"].Value);
                    }
                }
            }
            return badLoanReceipt;
        }

        public static void GenerateBadLoanReceipt(BadLoanReceipt badLoanReceipt)
        {
            using (MySqlConnection con = new MySqlConnection(ConfigHelper.GetConnectionString()))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand("GenerateBadLoanReceipt", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.Add("@pLoanCode", MySqlDbType.VarChar, 10);
                    cmd.Parameters["@pLoanCode"].Value = badLoanReceipt.LoanCode;

                    cmd.Parameters.Add("@pPaymentAmount", MySqlDbType.Int32);
                    cmd.Parameters["@pPaymentAmount"].Value = badLoanReceipt.PaymentAmount;

                    cmd.Parameters.Add("@pSettlementDiscount", MySqlDbType.Int32);
                    cmd.Parameters["@pSettlementDiscount"].Value = badLoanReceipt.SettlementDiscount;

                    cmd.Parameters.Add("@pActualReceiptDate", MySqlDbType.Date);
                    cmd.Parameters["@pActualReceiptDate"].Value = badLoanReceipt.ActualReceiptDate;

                    cmd.Parameters.Add("@pUserId", MySqlDbType.VarChar, 20);
                    cmd.Parameters["@pUserId"].Value = badLoanReceipt.UserId;

                    cmd.Parameters.Add("@pReceiptId", MySqlDbType.Int32);
                    cmd.Parameters["@pReceiptId"].Direction = ParameterDirection.Output;

                    cmd.Parameters.Add("@pRemainingBalance", MySqlDbType.Int32);
                    cmd.Parameters["@pRemainingBalance"].Direction = ParameterDirection.Output;

                    cmd.Parameters.Add("@pIsFullSettlement", MySqlDbType.Byte);
                    cmd.Parameters["@pIsFullSettlement"].Direction = ParameterDirection.Output;

                    cmd.ExecuteNonQuery();

                    badLoanReceipt.ReceiptId = Convert.ToInt32(cmd.Parameters["@pReceiptId"].Value);
                    badLoanReceipt.RemainingBalance = Convert.ToInt32(cmd.Parameters["@pRemainingBalance"].Value);
                    badLoanReceipt.IsFullSettlement = Convert.ToBoolean(cmd.Parameters["@pIsFullSettlement"].Value);
                }
            }
        }
    }
}