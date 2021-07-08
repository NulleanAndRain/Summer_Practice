using Library.DAL.Interface;
using Library.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Media.Imaging;

namespace LibraryDAL {
	public class MsSqlDAOUsers : MsSqlDAOBase, IDAOUsers {

		public MsSqlDAOUsers(string ConnString) : base(ConnString) { }

		#region Users

		public void CreateUser(User user) {
			var querry = @"INSERT INTO Users (Username, PassHash, FirstName, LastName, DateOfBirth)
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

		private User selectUser(string querrySelector, List<SqlParameter> _params) {
			User user = null;
			var querry = "SELECT ID, Username, PassHash, FirstName, LastName, DateOfBirth FROM Users WHERE ";
			var cmd = new SqlCommand(querry + querrySelector, conn);
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
			var querry = "Username = @n";
			var p = new List<SqlParameter> { new SqlParameter("@n", username) };
			return selectUser(querry, p);
		}

		public User GetUserWithId(int id) {
			var querry = "ID = @id";
			var p = new List<SqlParameter> { new SqlParameter("@id", id) };
			return selectUser(querry, p);
		}

		public void UpdateUser(int id, User newData) {
			var querry = @"UPDATE Users
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

		public BitmapImage GetProfileImage(int id) {
			var querry = "SELECT ProfilePicture FROM Users WHERE ID = @id";
			var cmd = new SqlCommand(querry, conn);
			cmd.Parameters.Add(new SqlParameter("@id", id));
			BitmapImage img = null;
			using (var reader = cmd.ExecuteReader()) {
				if (reader.Read()) {
					return ImgReader.GetImage(reader.GetStream(0));
				}
			}
			return img;
		}

		public void UpdateProfileImage(int id, BitmapImage img) {
			var querry = @"UPDATE Users 
				SET ProfilePicture = @pic 
				WHERE ID = @id";
			var p = new List<SqlParameter> {
				new SqlParameter("@pic", ImgReader.GetBytes(img)),
				new SqlParameter("@id", id) 
			};
			execNonQuerry(querry, p);
		}

		#endregion
	}
}
