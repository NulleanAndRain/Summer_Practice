using Library.DAL.Interface;
using Library.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryDAL {
	public class MsSqlDAO : IDAO {
		#region Common

		private string ConnString => $@"Data Source=(LocalDB)\MSSQLLocalDB;
			AttachDbFilename={Environment.CurrentDirectory}\LibDatabase.mdf;
			Integrated Security=True";

		private void execWithConnection(Action<SqlConnection> action) {
			using (SqlConnection conn = new SqlConnection(ConnString)) {
				conn.Open();
				action(conn);
			}
		}

		private void execNonQuerry(string querry, List<SqlParameter> _params = null) {
			void a(SqlConnection conn) {
				var cmd = new SqlCommand(querry, conn);
				if (_params != null) {
					foreach (var param in _params) {
						cmd.Parameters.Add(param);
					}
				}
				cmd.ExecuteNonQuery();
			}
			execWithConnection(a);
		}

		#endregion

		#region Books

		public void AddBook(Book book) {
			var querry = @"INSERT INTO Books (BookName, Authors, YearOfPublishing)
							 VALUES (@n, @a, @y)";
			var p = new List<SqlParameter> {
				new SqlParameter("@n", book.Name),
				new SqlParameter("@a", book.Authosrs),
				new SqlParameter("@y", book.YearOfPublishing)
			};
			execNonQuerry(querry, p);
		}

		private List<Book> getBooksWithQuerry(string querry, List<SqlParameter> _params = null) {
			List<Book> books = new List<Book>();
			void a(SqlConnection conn) {
				var cmd = new SqlCommand(querry, conn);
				if (_params != null) {
					foreach (var param in _params) {
						cmd.Parameters.Add(param);
					}
				}
				using (var reader = cmd.ExecuteReader()) {
					while (reader.Read()) {
						var book = new Book();
						book.Id = reader.GetInt32(0);
						book.Name = reader.GetString(1);
						book.Authosrs = reader.GetString(2);
						book.YearOfPublishing = reader.GetInt32(3);
						books.Add(book);
					}
				}
			}
			return books;
		}

		public List<Book> GetBooks() {
			var querry = "SELECT * FROM Books";
			return getBooksWithQuerry(querry);
		}

		public List<Book> GetBooksWithAuthors(string[] authors) {
			if (authors.Length == 0) return null;
			var querry = new StringBuilder("SELECT * FROM Books WHERE Authors LIKE ");
			var _params = new List<SqlParameter>();
			for (int i = 0; i < authors.Length; i++) {
				var author = authors[i];
				var paramName = $"@a{i}";
				querry.Append('%').Append(paramName).Append('%');
				_params.Add(new SqlParameter(paramName, author));
				if (i != authors.Length - 1) {
					querry.Append("AND LIKE ");
				}
			}

			return getBooksWithQuerry(querry.ToString(), _params);
		}

		public List<Book> GetBooksWithName(string name) {
			var querry = "SELECT * FROM Books WHERE BookName LIKE %@n%";
			var p = new List<SqlParameter> { new SqlParameter("@n", name) };
			return getBooksWithQuerry(querry, p);
		}

		public void EditBook(int id, Book newData) {
			var querry = @"UPDATE Books
				SET BookName = @n
				SET Authors = @a
				SET YearOfPublishing = @y
				WHERE ID = @id";
			var p = new List<SqlParameter> {
				new SqlParameter("@n", newData.Name),
				new SqlParameter("@a", newData.Authosrs),
				new SqlParameter("@y", newData.YearOfPublishing),
				new SqlParameter("@id", id)
			};
			execNonQuerry(querry, p);
		}

		public void DeleteBook(int id) {
			var querry = @"DELETE FROM Books WHERE ID = @id";
			var p = new List<SqlParameter> { new SqlParameter("@id", id) };
			execNonQuerry(querry, p);
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
			void a(SqlConnection conn) {
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
			}
			execWithConnection(a);
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
				SET Username = @u
				SET PassHash = @p
				SET FirstName = @fn
				SET LastName = @ln
				SET DateOfBirth = @d
				WHERE ID = @id";
			var p = new List<SqlParameter> {
				new SqlParameter("@u", newData.Username),
				new SqlParameter("@p", newData.PassHash),
				new SqlParameter("@fn", newData.FirstName),
				new SqlParameter("@ln", newData.LastName),
				new SqlParameter("@d", newData.DateOfBirth),
				new SqlParameter("@id", id)
			};
			execNonQuerry(querry);
		}

		public void DeleteUser(int id) {
			var querry = "DELETE FROM Users WHERE ID = @id";
			var p = new List<SqlParameter> { new SqlParameter("@id", id) };
			execNonQuerry(querry, p);
		}

		#endregion
	}
}
