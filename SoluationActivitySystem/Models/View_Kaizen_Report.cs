using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace ActivitySystem.Models
{
    public class View_Kaizen_Report
    {
        private readonly string constr;

        public View_Kaizen_Report()
            : this(Environment.GetEnvironmentVariable("ConnectionStrings__DataConnection"))
        {
        }

        public View_Kaizen_Report(string? connectionString)
        {
            constr = connectionString
                ?? throw new InvalidOperationException("Connection string 'ConnectionStrings__DataConnection' is not configured.");
        }

        public DataTable GetView_Kaizen()
        {
            var dt = new DataTable();
            using (SqlConnection con = new SqlConnection(constr))
            {
                SqlCommand cmd = new SqlCommand("SP_Kaizen", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                con.Close();
            }
            return dt;
        }
    }
}
