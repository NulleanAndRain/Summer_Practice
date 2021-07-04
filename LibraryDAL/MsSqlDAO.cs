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

		private void execNonQuerry(string querry) {
			void a(SqlConnection conn) {
				var cmd = new SqlCommand(querry, conn);
				cmd.ExecuteNonQuery();
			}
			execWithConnection(a);
		}

		#endregion

		#region Books

		public void AddBook(Book book) {
			var querry = $@"INSERT INTO Books (BookName, Authors, YearOfPublishing)
							 VALUES ({book.Name}, {book.Authosrs}, {book.YearOfPublishing})";
			execNonQuerry(querry);
		}

		private List<Book> getBooksWithQuerry(string querry) {
			List<Book> books = new List<Book>();
			void a(SqlConnection conn) {
				var cmd = new SqlCommand(querry, conn);
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
			var last = authors.Last();
			foreach (var author in authors) {
				querry.Append('%').Append(author).Append('%');
				if (author != last) {
					querry.Append("AND LIKE ");
				}
			}
			return getBooksWithQuerry(querry.ToString());
		}

		public List<Book> GetBooksWithName(string name) {
			var querry = $"SELECT * FROM Books WHERE BookName LIKE %{name}%";
			return getBooksWithQuerry(querry.ToString());
		}

		public void EditBook(int id, Book newData) {
			var querry = $@"UPDATE Books
				SET BookName = {newData.Name}
				SET Authors = {newData.Authosrs}
				SET YearOfPublishing = {newData.YearOfPublishing}
				WHERE ID = {id}";
			execNonQuerry(querry);
		}

		public void DeleteBook(int id) {
			var querry = $@"DELETE FROM Books WHERE ID = {id}";
			execNonQuerry(querry);
		}

		#endregion

		#region Users

		public void CreateUser(User user) {
			var querry = $@"INSERT INTO Users (Username, PassHash, FirstName, LastName, DateOfBirth)
							VALUES ({user.Username}, {user.PassHash}, {user.FirstName}, {user.LastName}, {user.DateOfBirth})";
			execNonQuerry(querry);
		}
		
		public User GetUserWithName(string username) {
			User user = null;
			var querry = $"SELECT * FROM Users WHERE Username = {username}";
			void a(SqlConnection conn) {
				var cmd = new SqlCommand(querry, conn);
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

		public void UpdateUser(int id, User newData) {
			var querry = $@"UPDATE Users
				SET Username = {newData.Username}
				SET PassHash = {newData.PassHash}
				SET FirstName = {newData.FirstName}
				SET LastName = {newData.LastName}
				SET DateOfBirth = {newData.DateOfBirth.ToString("yyyy-MM-dd")}";
			execNonQuerry(querry);
		}

		public void DeleteUser(int id) {
			var querry = $"DELETE FROM Users WHERE ID = {id}";
			execNonQuerry(querry);
		}

		#endregion
	}
}
