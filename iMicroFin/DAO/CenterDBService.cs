using MySql.Data.MySqlClient;
using System.Data;
using iMicroFin.Models;
namespace iMicroFin.DAO
{
    public class CenterDBService
    {
        public static int AddCenter(Center center)
        {
            int statusCode = 0;
            using (MySqlConnection con = new MySqlConnection(ConfigHelper.GetConnectionString()))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand("AddCenter", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.Add("@pCenterCode", MySqlDbType.VarChar, 5);
                    cmd.Parameters["@pCenterCode"].Direction = ParameterDirection.Output;

                    cmd.Parameters.Add("@pCenterName", MySqlDbType.VarChar, 40);
                    cmd.Parameters["@pCenterName"].Value = center.CenterName;

                    cmd.Parameters.Add("@pBranchId", MySqlDbType.Int32);
                    cmd.Parameters["@pBranchId"].Value = center.BranchId;

                    cmd.Parameters.Add("@pStatusCode", MySqlDbType.Int32);
                    cmd.Parameters["@pStatusCode"].Direction = ParameterDirection.Output;

                    cmd.ExecuteNonQuery();
                    statusCode = Convert.ToInt32(cmd.Parameters["@pStatusCode"].Value);
                    if (statusCode == 1)
                    {

                        string centerCode = cmd.Parameters["@pCenterCode"].Value?.ToString()??"";
                        center.CenterCode = centerCode; 
                        center.CenterId = Convert.ToInt32(centerCode.Substring(3));
                    }
                }
            }
            return statusCode;
        }

        public static List<Center> GetAllCenters(int branchId)
        {
            List<Center> centers = new List<Center>();
            using (MySqlConnection con = new MySqlConnection(ConfigHelper.GetConnectionString()))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand("GetAllCenters", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add("@pBranchId", MySqlDbType.Int32);
                    cmd.Parameters["@pBranchId"].Value = branchId;
                    using (MySqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            Center center = new Center();
                            center.CenterCode = rdr["CenterCode"]?.ToString()??"";
                            center.CenterId = Convert.ToInt32(rdr["CenterId"].ToString());
                            center.CenterName = rdr["CenterName"]?.ToString()??"";
                            centers.Add(center);
                        }
                    }
                }
            }
            return centers;
        }
        public static Center? GetCenter(string centerCode)
        {
            Center? center = null;
            using (MySqlConnection con = new MySqlConnection(ConfigHelper.GetConnectionString()))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand("GetCenter", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add("@pCenterCode", MySqlDbType.VarChar,4);
                    cmd.Parameters["@pCenterCode"].Value = centerCode;
                    using (MySqlDataReader rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            center = new Center();
                            center.CenterCode = rdr["CenterCode"]?.ToString()??"";
                            center.CenterId = Convert.ToInt32(rdr["CenterId"].ToString());
                            center.CenterName = rdr["CenterName"]?.ToString()??"";
                        }
                    }
                }
            }
            return center;
        }

        public static int EditCenter(Center center)
        {

            int statusCode = 0;
            using (MySqlConnection con = new MySqlConnection(ConfigHelper.GetConnectionString()))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand("EditCenter", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.Add("@pCenterCode", MySqlDbType.VarChar, 4);
                    cmd.Parameters["@pCenterCode"].Value = center.CenterCode;

                    cmd.Parameters.Add("@pCenterName", MySqlDbType.VarChar, 40);
                    cmd.Parameters["@pCenterName"].Value = center.CenterName;

                    cmd.Parameters.Add("@pStatusCode", MySqlDbType.Int32);
                    cmd.Parameters["@pStatusCode"].Direction = ParameterDirection.Output;

                    cmd.ExecuteNonQuery();
                    statusCode = Convert.ToInt32(cmd.Parameters["@pStatusCode"].Value.ToString());
                }
            }
            return statusCode;
        }

        public static List<Center> GetAllCentersByPattern(String centerNamePattern, int branchId)
        {
            List<Center> centers = new List<Center>();
            using (MySqlConnection con = new MySqlConnection(ConfigHelper.GetConnectionString()))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand("GetAllCentersByPattern", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add("@pCenterNamePattern", MySqlDbType.VarChar, 40);
                    cmd.Parameters["@pCenterNamePattern"].Value = centerNamePattern;
                    using (MySqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            Center center = new Center();
                            center.CenterCode = rdr["CenterCode"]?.ToString()??"";
                            center.CenterName = rdr["CenterName"]?.ToString() ?? "";
                            centers.Add(center);
                        }
                    }
                }
            }
            return centers;
        }
    }
}