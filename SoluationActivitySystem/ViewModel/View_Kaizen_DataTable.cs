using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace ActivitySystem.ViewModel
{
    public class View_Kaizen_DataTable
    {
        private readonly string constr;

        public View_Kaizen_DataTable()
            : this(Environment.GetEnvironmentVariable("ConnectionStrings__DataConnection"))
        {
        }

        public View_Kaizen_DataTable(string? connectionString)
        {
            constr = connectionString
                ?? throw new InvalidOperationException("Connection string 'ConnectionStrings__DataConnection' is not configured.");
        }

        public DataTable GetView_Kaizen()
        {
            var dt = new DataTable();
            using (SqlConnection con = new SqlConnection(constr))
            {

                SqlCommand cmd = new SqlCommand("SP_Kaizen_Query", con);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter param1 = new SqlParameter();
                param1.ParameterName = "@prStartDate";
                param1.Value = "2021-08-01";
                cmd.Parameters.Add(param1);

                SqlParameter param2 = new SqlParameter();
                param2.ParameterName = "@prEndDate";
                param2.Value = "2021-08-31";
                cmd.Parameters.Add(param2);

                con.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                con.Close();
            }

            return dt;
        }
    }
}
