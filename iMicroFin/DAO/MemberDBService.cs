using iMicroFin.Models;
using MySql.Data.MySqlClient;
using System.Data;

namespace iMicroFin.DAO
{
    public class MemberDBService
    {

        public static int AddMember(Member member)
        {
            int statusCode = 0;
            using (MySqlConnection con = new MySqlConnection(ConfigHelper.GetConnectionString()))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand("AddMember", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.Add("@pMemberCode", MySqlDbType.VarChar, 9);
                    cmd.Parameters["@pMemberCode"].Direction = ParameterDirection.Output;

                    cmd.Parameters.Add("@pGroupCode", MySqlDbType.VarChar, 7);
                    cmd.Parameters["@pGroupCode"].Value = member.GroupCode;

                    cmd.Parameters.Add("@pMemberType", MySqlDbType.Int32);
                    cmd.Parameters["@pMemberType"].Value = member.MemberType;

                    cmd.Parameters.Add("@pMemberName", MySqlDbType.VarChar, 50);
                    cmd.Parameters["@pMemberName"].Value = member.MemberName;

                    cmd.Parameters.Add("@pGender", MySqlDbType.Int32);
                    cmd.Parameters["@pGender"].Value = member.Gender;

                    cmd.Parameters.Add("@pDOB", MySqlDbType.Date);
                    cmd.Parameters["@pDOB"].Value = member.DOB;

                    cmd.Parameters.Add("@pMaritalStatus", MySqlDbType.Int32);
                    cmd.Parameters["@pMaritalStatus"].Value = member.MaritalStatus;

                    cmd.Parameters.Add("@pReligion", MySqlDbType.Int32);
                    cmd.Parameters["@pReligion"].Value = member.Religion;

                    cmd.Parameters.Add("@pFName", MySqlDbType.VarChar, 50);
                    cmd.Parameters["@pFName"].Value = member.FName;

                    cmd.Parameters.Add("@pHName", MySqlDbType.VarChar, 50);
                    cmd.Parameters["@pHName"].Value = member.HName;

                    cmd.Parameters.Add("@pOccupation", MySqlDbType.Int32);
                    cmd.Parameters["@pOccupation"].Value = member.Occupation;

                    cmd.Parameters.Add("@pOccupationType", MySqlDbType.Int32);
                    cmd.Parameters["@pOccupationType"].Value = member.OccupationType;

                    cmd.Parameters.Add("@pAddressLine1", MySqlDbType.VarChar, 40);
                    cmd.Parameters["@pAddressLine1"].Value = member.AddressLine1;

                    cmd.Parameters.Add("@pAddressLine2", MySqlDbType.VarChar, 40);
                    cmd.Parameters["@pAddressLine2"].Value = member.AddressLine2;

                    cmd.Parameters.Add("@pAddressLine3", MySqlDbType.VarChar, 40);
                    cmd.Parameters["@pAddressLine3"].Value = member.AddressLine3;

                    cmd.Parameters.Add("@pAddressLine4", MySqlDbType.VarChar, 40);
                    cmd.Parameters["@pAddressLine4"].Value = member.AddressLine4;

                    cmd.Parameters.Add("@pTaluk", MySqlDbType.VarChar, 40);
                    cmd.Parameters["@pTaluk"].Value = member.Taluk;

                    cmd.Parameters.Add("@pPanchayat", MySqlDbType.VarChar, 40);
                    cmd.Parameters["@pPanchayat"].Value = member.Panchayat;

                    cmd.Parameters.Add("@pCity", MySqlDbType.VarChar, 30);
                    cmd.Parameters["@pCity"].Value = member.City;

                    cmd.Parameters.Add("@pPincode", MySqlDbType.VarChar, 6);
                    cmd.Parameters["@pPincode"].Value = member.Pincode;

                    cmd.Parameters.Add("@pNoOfYears", MySqlDbType.Int32);
                    cmd.Parameters["@pNoOfYears"].Value = member.NoOfYears;

                    cmd.Parameters.Add("@pHouseType", MySqlDbType.Int32);
                    cmd.Parameters["@pHouseType"].Value = member.HouseType;

                    cmd.Parameters.Add("@pPhone", MySqlDbType.VarChar, 10);
                    cmd.Parameters["@pPhone"].Value = member.Phone;

                    cmd.Parameters.Add("@pMemberAadharNumber", MySqlDbType.VarChar, 12);
                    cmd.Parameters["@pMemberAadharNumber"].Value = member.MemberAadharNumber;

                    cmd.Parameters.Add("@pPAN", MySqlDbType.VarChar, 10);
                    cmd.Parameters["@pPAN"].Value = member.PAN;

                    cmd.Parameters.Add("@pRationCardNo", MySqlDbType.VarChar, 30);
                    cmd.Parameters["@pRationCardNo"].Value = member.RationCardNo;

                    cmd.Parameters.Add("@pVoterIDNo", MySqlDbType.VarChar, 30);
                    cmd.Parameters["@pVoterIDNo"].Value = member.VoterIDNo;

                    cmd.Parameters.Add("@pAccountNumber", MySqlDbType.VarChar, 20);
                    cmd.Parameters["@pAccountNumber"].Value = member.AccountNumber;

                    cmd.Parameters.Add("@pIFSC", MySqlDbType.VarChar, 11);
                    cmd.Parameters["@pIFSC"].Value = member.IFSC;

                    cmd.Parameters.Add("@pBankCustomerId", MySqlDbType.VarChar, 20);
                    cmd.Parameters["@pBankCustomerId"].Value = member.BankCustomerId;

                    cmd.Parameters.Add("@pNomineeName", MySqlDbType.VarChar, 50);
                    cmd.Parameters["@pNomineeName"].Value = member.NomineeName;

                    cmd.Parameters.Add("@pRelationship", MySqlDbType.Int32);
                    cmd.Parameters["@pRelationship"].Value = member.Relationship;

                    cmd.Parameters.Add("@pNomineeAadharNumber", MySqlDbType.VarChar, 12);
                    cmd.Parameters["@pNomineeAadharNumber"].Value = member.NomineeAadharNumber;

                    cmd.Parameters.Add("@pNomineeDOB", MySqlDbType.Date);
                    cmd.Parameters["@pNomineeDOB"].Value = member.NomineeDOB;

                    cmd.Parameters.Add("@pStatusCode", MySqlDbType.Int32);
                    cmd.Parameters["@pStatusCode"].Direction = ParameterDirection.Output;

                    cmd.ExecuteNonQuery();
                    statusCode = Convert.ToInt32(cmd.Parameters["@pStatusCode"].Value);
                    if (statusCode == 1)
                    {
                        string memberCode = cmd.Parameters["@pMemberCode"].Value?.ToString()??"";
                        member.MemberCode = memberCode;
                    }
                }
            }
            return statusCode;
        }

        public static List<Member> GetAllMembers(string groupCode)
        {
            List<Member> members = new List<Member>();

            try
            {
                using (MySqlConnection con = new MySqlConnection(ConfigHelper.GetConnectionString()))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand("GetAllMembers", con))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add("@pGroupCode", MySqlDbType.VarChar, 6);
                        cmd.Parameters["@pGroupCode"].Value = groupCode ?? string.Empty;

                        using (MySqlDataReader rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                try
                                {
                                    Member member = new Member
                                    {
                                        MemberCode = rdr["MemberCode"]?.ToString() ?? "",
                                        MemberId = rdr["MemberId"] != DBNull.Value ? Convert.ToInt32(rdr["MemberId"]) : 0,
                                        GroupCode = rdr["GroupCode"]?.ToString() ?? "",
                                        CenterCode = rdr["CenterCode"]?.ToString() ?? "",
                                        BranchId = rdr["BranchId"] != DBNull.Value ? Convert.ToInt32(rdr["BranchId"]) : 0,
                                        GroupName = rdr["GroupName"]?.ToString() ?? "",
                                        CenterName = rdr["CenterName"]?.ToString() ?? "",
                                        LeaderName = rdr["LeaderName"]?.ToString() ?? "",
                                        MemberType = rdr["MemberType"] != DBNull.Value ? (EMemberType)Convert.ToInt32(rdr["MemberType"]) : (EMemberType)(-1),
                                        MemberName = rdr["MemberName"]?.ToString() ?? "",
                                        Gender = rdr["Gender"] != DBNull.Value ? (EGender)Convert.ToInt32(rdr["Gender"]) : (EGender)(-1),
                                        DOB = rdr["DOB"] != DBNull.Value && DateTime.TryParse(rdr["DOB"].ToString(), out DateTime dob) ? dob : DateTime.MinValue,
                                        MaritalStatus = rdr["MaritalStatus"] != DBNull.Value && !string.IsNullOrEmpty(rdr["MaritalStatus"].ToString())
                                            ? (EMaritalStatus)Convert.ToInt32(rdr["MaritalStatus"])
                                            : (EMaritalStatus)(-1),
                                        Religion = rdr["Religion"] != DBNull.Value && !string.IsNullOrEmpty(rdr["Religion"].ToString())
                                            ? (EReligion)Convert.ToInt32(rdr["Religion"])
                                            : (EReligion)(-1),
                                        FName = rdr["FName"]?.ToString() ?? "",
                                        HName = rdr["HName"]?.ToString() ?? "",
                                        Occupation = rdr["Occupation"] != DBNull.Value && !string.IsNullOrEmpty(rdr["Occupation"].ToString())
                                            ? (EOccupation)Convert.ToInt32(rdr["Occupation"])
                                            : (EOccupation)(-1),
                                        OccupationType = rdr["OccupationType"] != DBNull.Value && !string.IsNullOrEmpty(rdr["OccupationType"].ToString())
                                            ? (EOccupationType)Convert.ToInt32(rdr["OccupationType"])
                                            : (EOccupationType)(-1),
                                        AddressLine1 = rdr["AddressLine1"]?.ToString() ?? "",
                                        AddressLine2 = rdr["AddressLine2"]?.ToString() ?? "",
                                        AddressLine3 = rdr["AddressLine3"]?.ToString() ?? "",
                                        AddressLine4 = rdr["AddressLine4"]?.ToString() ?? "",
                                        Taluk = rdr["Taluk"]?.ToString() ?? "",
                                        Panchayat = rdr["Panchayat"]?.ToString() ?? "",
                                        City = rdr["City"]?.ToString() ?? "",
                                        Pincode = rdr["Pincode"]?.ToString() ?? "",
                                        NoOfYears = rdr["NoOfYears"] != DBNull.Value ? Convert.ToInt32(rdr["NoOfYears"]) : 0,
                                        HouseType = rdr["HouseType"] != DBNull.Value ? (EHouseType)Convert.ToInt32(rdr["HouseType"]) : (EHouseType)(-1),
                                        Phone = rdr["Phone"]?.ToString() ?? "",
                                        MemberAadharNumber = rdr["MemberAadharNumber"]?.ToString() ?? "",
                                        PAN = rdr["PAN"]?.ToString() ?? "",
                                        RationCardNo = rdr["RationCardNo"]?.ToString() ?? "",
                                        VoterIDNo = rdr["VoterIdNo"]?.ToString() ?? "",
                                        AccountNumber = rdr["AccountNumber"]?.ToString() ?? "",
                                        IFSC = rdr["IFSC"]?.ToString() ?? "",
                                        BankCustomerId = rdr["BankCustomerId"]?.ToString() ?? "",
                                        NomineeName = rdr["NomineeName"]?.ToString() ?? "",
                                        Relationship = rdr["Relationship"] != DBNull.Value ? (ERelationship)Convert.ToInt32(rdr["Relationship"]) : (ERelationship)(-1),
                                        NomineeAadharNumber = rdr["NomineeAadharNumber"]?.ToString() ?? "",
                                        NomineeDOB = rdr["NomineeDOB"] != DBNull.Value && DateTime.TryParse(rdr["NomineeDOB"].ToString(), out DateTime nomineeDob)
                                            ? nomineeDob
                                            : DateTime.MinValue,
                                        CurrentLoanCode = rdr["CurrentLoanCode"]?.ToString() ?? ""
                                    };

                                    // Set the "R" properties after main properties are set
                                    member.RMemberAadharNumber = member.MemberAadharNumber;
                                    member.RAccountNumber = member.AccountNumber;

                                    members.Add(member);
                                }
                                catch (Exception ex)
                                {
                                    // Log the error for this specific member record
                                    // Consider logging: ex.Message or using a logging framework
                                    // You can choose to continue processing other records or handle differently
                                    Console.WriteLine($"Error processing member record: {ex.Message}");
                                    continue; // Skip this record and continue with next
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the database connection or query error
                Console.WriteLine($"Database error in GetAllMembers: {ex.Message}");
                throw; // Re-throw to let caller handle the exception
            }

            return members;
        }

        public static Member? GetMember(string memberCode)
        {
            Member? member = null;
            using (MySqlConnection con = new MySqlConnection(ConfigHelper.GetConnectionString()))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand("GetMember", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add("@pMemberCode", MySqlDbType.VarChar, 8);
                    cmd.Parameters["@pMemberCode"].Value = memberCode;
                    using (MySqlDataReader rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            member = new Member
                            {
                                MemberCode = rdr["MemberCode"]?.ToString() ?? "",
                                MemberId = rdr["MemberId"] != DBNull.Value ? Convert.ToInt32(rdr["MemberId"]) : 0,
                                GroupCode = rdr["GroupCode"]?.ToString() ?? "",
                                CenterCode = rdr["CenterCode"]?.ToString() ?? "",
                                BranchId = rdr["BranchId"] != DBNull.Value ? Convert.ToInt32(rdr["BranchId"]) : 0,
                                GroupName = rdr["GroupName"]?.ToString() ?? "",
                                CenterName = rdr["CenterName"]?.ToString() ?? "",
                                LeaderName = rdr["LeaderName"]?.ToString() ?? "",
                                MemberType = rdr["MemberType"] != DBNull.Value ? (EMemberType)Convert.ToInt32(rdr["MemberType"]) : (EMemberType)(-1),
                                MemberName = rdr["MemberName"]?.ToString() ?? "",
                                Gender = rdr["Gender"] != DBNull.Value ? (EGender)Convert.ToInt32(rdr["Gender"]) : (EGender)(-1),
                                DOB = rdr["DOB"] != DBNull.Value && DateTime.TryParse(rdr["DOB"].ToString(), out DateTime dob) ? dob : DateTime.MinValue,
                                MaritalStatus = rdr["MaritalStatus"] != DBNull.Value && !string.IsNullOrEmpty(rdr["MaritalStatus"].ToString())
                                             ? (EMaritalStatus)Convert.ToInt32(rdr["MaritalStatus"])
                                             : (EMaritalStatus)(-1),
                                Religion = rdr["Religion"] != DBNull.Value && !string.IsNullOrEmpty(rdr["Religion"].ToString())
                                             ? (EReligion)Convert.ToInt32(rdr["Religion"])
                                             : (EReligion)(-1),
                                FName = rdr["FName"]?.ToString() ?? "",
                                HName = rdr["HName"]?.ToString() ?? "",
                                Occupation = rdr["Occupation"] != DBNull.Value && !string.IsNullOrEmpty(rdr["Occupation"].ToString())
                                             ? (EOccupation)Convert.ToInt32(rdr["Occupation"])
                                             : (EOccupation)(-1),
                                OccupationType = rdr["OccupationType"] != DBNull.Value && !string.IsNullOrEmpty(rdr["OccupationType"].ToString())
                                             ? (EOccupationType)Convert.ToInt32(rdr["OccupationType"])
                                             : (EOccupationType)(-1),
                                AddressLine1 = rdr["AddressLine1"]?.ToString() ?? "",
                                AddressLine2 = rdr["AddressLine2"]?.ToString() ?? "",
                                AddressLine3 = rdr["AddressLine3"]?.ToString() ?? "",
                                AddressLine4 = rdr["AddressLine4"]?.ToString() ?? "",
                                Taluk = rdr["Taluk"]?.ToString() ?? "",
                                Panchayat = rdr["Panchayat"]?.ToString() ?? "",
                                City = rdr["City"]?.ToString() ?? "",
                                Pincode = rdr["Pincode"]?.ToString() ?? "",
                                NoOfYears = rdr["NoOfYears"] != DBNull.Value ? Convert.ToInt32(rdr["NoOfYears"]) : 0,
                                HouseType = rdr["HouseType"] != DBNull.Value ? (EHouseType)Convert.ToInt32(rdr["HouseType"]) : (EHouseType)(-1),
                                Phone = rdr["Phone"]?.ToString() ?? "",
                                MemberAadharNumber = rdr["MemberAadharNumber"]?.ToString() ?? "",
                                PAN = rdr["PAN"]?.ToString() ?? "",
                                RationCardNo = rdr["RationCardNo"]?.ToString() ?? "",
                                VoterIDNo = rdr["VoterIdNo"]?.ToString() ?? "",
                                AccountNumber = rdr["AccountNumber"]?.ToString() ?? "",
                                IFSC = rdr["IFSC"]?.ToString() ?? "",
                                BankCustomerId = rdr["BankCustomerId"]?.ToString() ?? "",
                                NomineeName = rdr["NomineeName"]?.ToString() ?? "",
                                Relationship = rdr["Relationship"] != DBNull.Value ? (ERelationship)Convert.ToInt32(rdr["Relationship"]) : (ERelationship)(-1),
                                NomineeAadharNumber = rdr["NomineeAadharNumber"]?.ToString() ?? "",
                                NomineeDOB = rdr["NomineeDOB"] != DBNull.Value && DateTime.TryParse(rdr["NomineeDOB"].ToString(), out DateTime nomineeDob)
                                             ? nomineeDob
                                             : DateTime.MinValue,
                                CurrentLoanCode = rdr["CurrentLoanCode"]?.ToString() ?? ""
                            };

                            // Set the "R" properties after main properties are set
                            member.RMemberAadharNumber = member.MemberAadharNumber;
                            member.RAccountNumber = member.AccountNumber;
                        }
                    }
                }
            }
            return member;
        }

        public static int EditMember(Member member)
        {
            int statusCode = 0;
            using (MySqlConnection con = new MySqlConnection(ConfigHelper.GetConnectionString()))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand("EditMember", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.Add("@pMemberCode", MySqlDbType.VarChar, 8);
                    cmd.Parameters["@pMemberCode"].Value = member.MemberCode;

                    cmd.Parameters.Add("@pMemberId", MySqlDbType.Int32);
                    cmd.Parameters["@pMemberId"].Value = member.MemberId;

                    cmd.Parameters.Add("@pGroupCode", MySqlDbType.VarChar, 6);
                    cmd.Parameters["@pGroupCode"].Value = member.GroupCode;

                    cmd.Parameters.Add("@pMemberType", MySqlDbType.Int32);
                    cmd.Parameters["@pMemberType"].Value = member.MemberType;

                    cmd.Parameters.Add("@pMemberName", MySqlDbType.VarChar, 50);
                    cmd.Parameters["@pMemberName"].Value = member.MemberName;

                    cmd.Parameters.Add("@pGender", MySqlDbType.Int32);
                    cmd.Parameters["@pGender"].Value = member.Gender;

                    cmd.Parameters.Add("@pDOB", MySqlDbType.Date);
                    cmd.Parameters["@pDOB"].Value = member.DOB;

                    cmd.Parameters.Add("@pMaritalStatus", MySqlDbType.Int32);
                    cmd.Parameters["@pMaritalStatus"].Value = member.MaritalStatus;

                    cmd.Parameters.Add("@pReligion", MySqlDbType.Int32);
                    cmd.Parameters["@pReligion"].Value = member.Religion;

                    cmd.Parameters.Add("@pFName", MySqlDbType.VarChar, 50);
                    cmd.Parameters["@pFName"].Value = member.FName;

                    cmd.Parameters.Add("@pHName", MySqlDbType.VarChar, 50);
                    cmd.Parameters["@pHName"].Value = member.HName;

                    cmd.Parameters.Add("@pOccupation", MySqlDbType.Int32);
                    cmd.Parameters["@pOccupation"].Value = member.Occupation;

                    cmd.Parameters.Add("@pOccupationType", MySqlDbType.Int32);
                    cmd.Parameters["@pOccupationType"].Value = member.OccupationType;

                    cmd.Parameters.Add("@pAddressLine1", MySqlDbType.VarChar, 40);
                    cmd.Parameters["@pAddressLine1"].Value = member.AddressLine1;

                    cmd.Parameters.Add("@pAddressLine2", MySqlDbType.VarChar, 40);
                    cmd.Parameters["@pAddressLine2"].Value = member.AddressLine2;

                    cmd.Parameters.Add("@pAddressLine3", MySqlDbType.VarChar, 40);
                    cmd.Parameters["@pAddressLine3"].Value = member.AddressLine3;

                    cmd.Parameters.Add("@pAddressLine4", MySqlDbType.VarChar, 40);
                    cmd.Parameters["@pAddressLine4"].Value = member.AddressLine4;

                    cmd.Parameters.Add("@pTaluk", MySqlDbType.VarChar, 40);
                    cmd.Parameters["@pTaluk"].Value = member.Taluk;

                    cmd.Parameters.Add("@pPanchayat", MySqlDbType.VarChar, 40);
                    cmd.Parameters["@pPanchayat"].Value = member.Panchayat;

                    cmd.Parameters.Add("@pCity", MySqlDbType.VarChar, 30);
                    cmd.Parameters["@pCity"].Value = member.City;

                    cmd.Parameters.Add("@pPincode", MySqlDbType.VarChar, 6);
                    cmd.Parameters["@pPincode"].Value = member.Pincode;

                    cmd.Parameters.Add("@pNoOfYears", MySqlDbType.Int32);
                    cmd.Parameters["@pNoOfYears"].Value = member.NoOfYears;

                    cmd.Parameters.Add("@pHouseType", MySqlDbType.Int32);
                    cmd.Parameters["@pHouseType"].Value = member.HouseType;

                    cmd.Parameters.Add("@pPhone", MySqlDbType.VarChar, 10);
                    cmd.Parameters["@pPhone"].Value = member.Phone;

                    cmd.Parameters.Add("@pMemberAadharNumber", MySqlDbType.VarChar, 12);
                    cmd.Parameters["@pMemberAadharNumber"].Value = member.MemberAadharNumber;

                    cmd.Parameters.Add("@pPAN", MySqlDbType.VarChar, 10);
                    cmd.Parameters["@pPAN"].Value = member.PAN;

                    cmd.Parameters.Add("@pRationCardNo", MySqlDbType.VarChar, 30);
                    cmd.Parameters["@pRationCardNo"].Value = member.RationCardNo;

                    cmd.Parameters.Add("@pVoterIDNo", MySqlDbType.VarChar, 30);
                    cmd.Parameters["@pVoterIDNo"].Value = member.VoterIDNo;

                    cmd.Parameters.Add("@pAccountNumber", MySqlDbType.VarChar, 20);
                    cmd.Parameters["@pAccountNumber"].Value = member.AccountNumber;

                    cmd.Parameters.Add("@pIFSC", MySqlDbType.VarChar, 11);
                    cmd.Parameters["@pIFSC"].Value = member.IFSC;

                    cmd.Parameters.Add("@pBankCustomerId", MySqlDbType.VarChar, 20);
                    cmd.Parameters["@pBankCustomerId"].Value = member.BankCustomerId;

                    cmd.Parameters.Add("@pNomineeName", MySqlDbType.VarChar, 50);
                    cmd.Parameters["@pNomineeName"].Value = member.NomineeName;

                    cmd.Parameters.Add("@pRelationship", MySqlDbType.Int32);
                    cmd.Parameters["@pRelationship"].Value = member.Relationship;

                    cmd.Parameters.Add("@pNomineeAadharNumber", MySqlDbType.VarChar, 12);
                    cmd.Parameters["@pNomineeAadharNumber"].Value = member.NomineeAadharNumber;

                    cmd.Parameters.Add("@pNomineeDOB", MySqlDbType.Date);
                    cmd.Parameters["@pNomineeDOB"].Value = member.NomineeDOB;

                    cmd.Parameters.Add("@pStatusCode", MySqlDbType.Int32);
                    cmd.Parameters["@pStatusCode"].Direction = ParameterDirection.Output;

                    cmd.ExecuteNonQuery();
                    statusCode = Convert.ToInt32(cmd.Parameters["@pStatusCode"].Value);
                }
            }
            return statusCode;
        }

       

        public static string? GetGroupCode(string memberCode)
        {
            string? groupCode=null;
            using (MySqlConnection con = new MySqlConnection(ConfigHelper.GetConnectionString()))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand("SELECT GetGroupCode(@pMemberCode)", con))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.Add("@pMemberCode", MySqlDbType.VarChar, 9);
                    cmd.Parameters["@pMemberCode"].Value = memberCode;
                    var result = cmd.ExecuteScalar();
                    groupCode = result?.ToString() ?? "";
                }
            }
            return groupCode;
        }

        public static List<MemberInfo> GetMemberByAadhar(string searchText)
        {
            List<MemberInfo> memberInfoList = new List<MemberInfo>();
            MemberInfo memberInfo;
            using (MySqlConnection con = new MySqlConnection(ConfigHelper.GetConnectionString()))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand("GetMemberByAadhar", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add("@pSearchText", MySqlDbType.VarChar,20);
                    cmd.Parameters["@pSearchText"].Value = searchText;
                    using (MySqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            memberInfo = new MemberInfo();
                            memberInfo.MemberCode = rdr["MemberCode"]?.ToString() ?? "";
                            memberInfo.MemberName = rdr["MemberName"]?.ToString() ?? "";
                            memberInfoList.Add(memberInfo); 
                        }
                    }
                }
            }
            return memberInfoList;
        }

        public static List<MemberInfo> GetMemberByPhone(string searchText)
        {
            List<MemberInfo> memberInfoList = new List<MemberInfo>();
            MemberInfo memberInfo;
            using (MySqlConnection con = new MySqlConnection(ConfigHelper.GetConnectionString()))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand("GetMemberByPhone", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add("@pSearchText", MySqlDbType.VarChar, 20);
                    cmd.Parameters["@pSearchText"].Value = searchText;
                    using (MySqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            memberInfo = new MemberInfo();
                            memberInfo.MemberCode = rdr["MemberCode"]?.ToString() ?? "";
                            memberInfo.MemberName = rdr["MemberName"]?.ToString() ?? "";
                            memberInfoList.Add(memberInfo);
                        }
                    }
                }
            }
            return memberInfoList;
        }

        public static List<MemberInfo> GetMemberByName(string searchText)
        {
            List<MemberInfo> memberInfoList = new List<MemberInfo>();
            MemberInfo memberInfo;
            using (MySqlConnection con = new MySqlConnection(ConfigHelper.GetConnectionString()))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand("GetMemberByName", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add("@pSearchText", MySqlDbType.VarChar, 20);
                    cmd.Parameters["@pSearchText"].Value = searchText;
                    using (MySqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            memberInfo = new MemberInfo();
                            memberInfo.MemberCode = rdr["MemberCode"]?.ToString() ?? "";
                            memberInfo.MemberName = rdr["MemberName"]?.ToString() ?? "";
                            memberInfoList.Add(memberInfo);
                        }
                    }
                }
            }
            return memberInfoList;
        }
    }
}