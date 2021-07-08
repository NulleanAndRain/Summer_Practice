using Library.DAL.Interface;
using Library.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace LibraryDAL {
	public class MsSqlDAOUsers : IDAOUsers, IDisposable {
		#region Common

		private SqlConnection conn;

		public MsSqlDAOUsers(string ConnString) {
			conn = new SqlConnection(ConnString);
			conn.Open();
			var cmd = new SqlCommand("sp_setapprole 'lib_app', 'DFa[7wzaVA'", conn);
			cmd.ExecuteNonQuery();
		}

		private void execNonQuerry(string querry, List<SqlParameter> _params = null) {
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

		#endregion

		#region Users

		public void CreateUser(User user) {
			var querry = $@"INSERT INTO Users (Username, PassHash, FirstName, LastName, DateOfBirth)
							VALUES (@u, @p, @fn, @ln, @d)";
			var p = new List<SqlParameter> {
				new SqlParameter("@u", user.Username),
				new SqlParameter("@p", user.PassHash),
				new SqlParameter("@fn", user.FirstName),
				new SqlParameter("@ln", user.LastName),
				new SqlParameter("@d", user.DateOfBirth)
			};
			execNonQuerry(querry, p);
		}

		private User selectUser(string querry, List<SqlParameter> _params) {
			User user = null;
			var cmd = new SqlCommand(querry, conn);
			foreach (var param in _params) {
				cmd.Parameters.Add(param);
			}
			using (var reader = cmd.ExecuteReader()) {
				if (reader.Read()) {
					user = new User();
					user.Id = reader.GetInt32(0);
					user.Username = reader.GetString(1);
					user.PassHash = reader.GetString(2);
					user.FirstName = reader.GetString(3);
					user.LastName = reader.GetString(4);
					user.DateOfBirth = reader.GetDateTime(5);
				}
			}
			return user;
		}

		public User GetUserWithName(string username) {
			var querry = "SELECT * FROM Users WHERE Username = @n";
			var p = new List<SqlParameter> { new SqlParameter("@n", username) };
			return selectUser(querry, p);
		}

		public User GetUserWithId(int id) {
			var querry = "SELECT * FROM Users WHERE ID = @id";
			var p = new List<SqlParameter> { new SqlParameter("@id", id) };
			return selectUser(querry, p);
		}

		public void UpdateUser(int id, User newData) {
			var querry = $@"UPDATE Users
				SET Username = @u, PassHash = @p,
				FirstName = @fn, LastName = @ln, DateOfBirth = @d
				WHERE ID = @id";
			var p = new List<SqlParameter> {
				new SqlParameter("@u", newData.Username),
				new SqlParameter("@p", newData.PassHash),
				new SqlParameter("@fn", newData.FirstName),
				new SqlParameter("@ln", newData.LastName),
				new SqlParameter("@d", newData.DateOfBirth),
				new SqlParameter("@id", id)
			};
			execNonQuerry(querry, p);
		}

		public void DeleteUser(int id) {
			var querry = "DELETE FROM Users WHERE ID = @id";
			var p = new List<SqlParameter> { new SqlParameter("@id", id) };
			execNonQuerry(querry, p);
		}

		#endregion
	}
}
