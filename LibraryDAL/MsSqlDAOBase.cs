using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryDAL {
	public class MsSqlDAOBase : IDisposable {
		protected SqlConnection conn;

		public MsSqlDAOBase(string ConnString) {
			conn = new SqlConnection(ConnString);
			conn.Open();
			var cmd = new SqlCommand("sp_setapprole 'lib_app', 'DFa[7wzaVA'", conn);
			cmd.ExecuteNonQuery();
		}

		protected void execNonQuerry(string querry, List<SqlParameter> _params = null) {
			var cmd = new SqlCommand(querry, conn);
			if (_params != null) {
				foreach (var param in _params) {
					cmd.Parameters.Add(param);
				}
			}
			cmd.ExecuteNonQuery();
		}

		public void Dispose() {
			if (conn != null) conn.Dispose();
		}
	}
}
