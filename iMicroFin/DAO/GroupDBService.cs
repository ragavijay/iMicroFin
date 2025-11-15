using iMicroFin.Models;
using MySql.Data.MySqlClient;
using System.Data;

namespace iMicroFin.DAO
{
    public class GroupDBService
    {
        public static int AddMemberGroup(MemberGroup group)
        {

            int statusCode = 0;
            using (MySqlConnection con = new MySqlConnection(ConfigHelper.GetConnectionString()))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand("AddMemberGroup", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.Add("@pGroupCode", MySqlDbType.VarChar, 7);
                    cmd.Parameters["@pGroupCode"].Direction = ParameterDirection.Output;

                    cmd.Parameters.Add("@pGroupName", MySqlDbType.VarChar, 40);
                    cmd.Parameters["@pGroupName"].Value = group.GroupName;

                    cmd.Parameters.Add("@pCenterCode", MySqlDbType.VarChar,4);
                    cmd.Parameters["@pCenterCode"].Value = group.CenterCode;

                    cmd.Parameters.Add("@pStatusCode", MySqlDbType.Int32);
                    cmd.Parameters["@pStatusCode"].Direction = ParameterDirection.Output;

                    cmd.ExecuteNonQuery();
                    statusCode = Convert.ToInt32(cmd.Parameters["@pStatusCode"].Value);
                    if (statusCode == 1)
                    {
                        string groupCode = cmd.Parameters["@pGroupCode"].Value?.ToString()??"";
                        int groupId = Convert.ToInt32(groupCode.Substring(4));
                        group.GroupCode = groupCode;
                        group.GroupId = groupId;

                    }
                }
            }
            return statusCode;
        }

        public static List<MemberGroup> GetAllMemberGroups(int branchId)
        {
            MemberGroup group;
            List<MemberGroup> groups = new List<MemberGroup>();
            using (MySqlConnection con = new MySqlConnection(ConfigHelper.GetConnectionString()))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand("GetAllMemberGroups", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add("@pBranchId", MySqlDbType.Int32);
                    cmd.Parameters["@pBranchId"].Value = branchId;
                    using (MySqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            group = new MemberGroup();
                            group.GroupCode = rdr["GroupCode"]?.ToString()??"";
                            group.GroupName = rdr["GroupName"]?.ToString() ?? "";
                            group.CenterCode = rdr["CenterCode"]?.ToString() ?? "";
                            group.CenterName = rdr["CenterName"]?.ToString() ?? "";
                            group.isLoanRunning = false;
                            groups.Add(group);
                        }
                    }
                }
            }
            
            foreach (MemberGroup currGroup in groups)
            {
                using (MySqlConnection con = new MySqlConnection(ConfigHelper.GetConnectionString()))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand("GetRepaymentStatusMemberCount", con))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add("@pGroupCode", MySqlDbType.VarChar, 6);
                        cmd.Parameters["@pGroupCode"].Value = currGroup.GroupCode;
                        using (MySqlDataReader rdr = cmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                int count = Convert.ToInt32(rdr["MemberCount"].ToString());
                                if (count > 0)
                                {
                                    currGroup.isLoanRunning = true;
                                }
                            }
                        }
                    }
                }
            }
            return groups;
        }
        public static MemberGroup? GetMemberGroup(string  groupCode)
        {
            MemberGroup? group = null;
            using (MySqlConnection con = new MySqlConnection(ConfigHelper.GetConnectionString()))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand("GetMemberGroup", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add("@pGroupCode", MySqlDbType.VarChar, 6);
                    cmd.Parameters["@pGroupCode"].Value = groupCode;
                    using (MySqlDataReader rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            group = new MemberGroup();
                            group.GroupCode = groupCode;
                            group.GroupId = Convert.ToInt32(rdr["GroupId"].ToString());
                            group.GroupName = rdr["GroupName"]?.ToString() ?? "";
                            group.CenterCode = rdr["CenterCode"]?.ToString() ?? "";
                            group.CenterName = rdr["CenterName"]?.ToString() ?? "";
                        }
                    }
                }
            }
            return group;
        }

        public static int EditMemberGroup(MemberGroup group)
        {
            int statusCode = 0;
            using (MySqlConnection con = new MySqlConnection(ConfigHelper.GetConnectionString()))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand("EditMemberGroup", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.Add("@pGroupCode", MySqlDbType.VarChar, 6);
                    cmd.Parameters["@pGroupCode"].Value = group.GroupCode;

                    cmd.Parameters.Add("@pGroupName", MySqlDbType.VarChar, 40);
                    cmd.Parameters["@pGroupName"].Value = group.GroupName;

                    cmd.Parameters.Add("@pCenterCode", MySqlDbType.VarChar, 4);
                    cmd.Parameters["@pCenterCode"].Value = group.CenterCode;

                    //cmd.Parameters.Add("@pBranchId", MySqlDbType.VarChar, 6);
                    //cmd.Parameters["@pBranchId"].Value = group.BranchId;

                    cmd.Parameters.Add("@pStatusCode", MySqlDbType.Int32);
                    cmd.Parameters["@pStatusCode"].Direction = ParameterDirection.Output;

                    cmd.ExecuteNonQuery();
                    statusCode = Convert.ToInt32(cmd.Parameters["@pStatusCode"].Value.ToString());
                }
            }
            return statusCode;
        }

        public static List<MemberGroup> GetAllMemberGroupsByPattern(String groupNamePattern,int branchId)
        {
            List<MemberGroup> groups = new List<MemberGroup>();
            using (MySqlConnection con = new MySqlConnection(ConfigHelper.GetConnectionString()))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand("GetAllMemberGroupsByPattern", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add("@pGroupNamePattern", MySqlDbType.VarChar, 40);
                    cmd.Parameters["@pGroupNamePattern"].Value = groupNamePattern;
                    using (MySqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            MemberGroup group = new MemberGroup();
                            group.GroupCode = rdr["GroupCode"]?.ToString() ?? "";
                            group.GroupName = rdr["GroupName"]?.ToString() ?? "";
                            group.CenterName = rdr["CenterName"]?.ToString() ?? "";
                            group.LeaderName = rdr["LeaderName"]?.ToString() ?? "";
                            groups.Add(group);
                        }
                    }
                }
            }
            return groups;
        }
    }
}