using System.Data;
using MySql.Data.MySqlClient;
using iMicroFin.Models;

namespace iMicroFin.DAO
{
    public static class DBService
    {
        public static string GetUserType(LoginViewModel login)
        {
            String userType = "";
            using (MySqlConnection con = new MySqlConnection(ConfigHelper.GetConnectionString()))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand("SELECT GetUserType(@pUserId, @pPasswd)", con))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.Add("@pUserId", MySqlDbType.VarChar, 20);
                    cmd.Parameters["@pUserId"].Value = login.UserId;
                    cmd.Parameters.Add("@pPasswd", MySqlDbType.VarChar, 16);
                    cmd.Parameters["@pPasswd"].Value = login.Passwd;
                    var result = cmd.ExecuteScalar();
                    userType = result?.ToString() ?? "";
                }
            }
            return userType;
        }

        public static string GetBranchName(int branchId)
        {

            String branchName = "";
            using (MySqlConnection con = new MySqlConnection(ConfigHelper.GetConnectionString()))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand("SELECT GetBranchName(@pBranchId)", con))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.Add("@pBranchId", MySqlDbType.Decimal, 5);
                    cmd.Parameters["@pBranchId"].Value = branchId;
                    var result = cmd.ExecuteScalar();
                    branchName = result?.ToString() ?? "";
                }
            }
            return branchName;
        }
    }
}
