using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymMembership
{
    public static class sqlConnection
    {
        public static SqlConnection connection = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename=C:\Users\sathish\Documents\Visual Studio 2013\Projects\GymMembership\GymMembership\GymDatabase.mdf;Integrated Security=True");
    }
}
