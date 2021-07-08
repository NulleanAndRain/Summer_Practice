using Library.DAL.Interface;
using Library.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace LibraryDAL {
	public class MsSqlDAOBooks : IDAOBooks, IDisposable {
		#region Common

		private SqlConnection conn;

		public MsSqlDAOBooks(string ConnString) {
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

		#region Books

		public void AddBook(Book book) {
			var querry = @"INSERT INTO Books (BookName, Authors, YearOfPublishing)
							 VALUES (@n, @a, @y)";
			var p = new List<SqlParameter> {
				new SqlParameter("@n", book.Name),
				new SqlParameter("@a", book.Authors),
				new SqlParameter("@y", book.YearOfPublishing)
			};
			execNonQuerry(querry, p);
		}

		private List<Book> getBooksWithQuerry(string querry, List<SqlParameter> _params = null) {
			List<Book> books = new List<Book>();
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
					book.Authors = reader.GetString(2);
					book.YearOfPublishing = reader.GetInt32(3);
					books.Add(book);
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
				querry.Append(paramName);
				_params.Add(new SqlParameter(paramName, $"%{author}%"));
				if (i != authors.Length - 1) {
					querry.Append("AND LIKE ");
				}
			}
			return getBooksWithQuerry(querry.ToString(), _params);
		}

		public List<Book> GetBooksWithName(string name) {
			var querry = "SELECT * FROM Books WHERE BookName LIKE @n";
			var p = new List<SqlParameter> { new SqlParameter("@n", $"%{name}%") };
			return getBooksWithQuerry(querry, p);
		}

		public void EditBook(int id, Book newData) {
			var querry = @"UPDATE Books
				SET BookName = @n, Authors = @a, YearOfPublishing = @y
				WHERE ID = @id";
			var p = new List<SqlParameter> {
				new SqlParameter("@n", newData.Name),
				new SqlParameter("@a", newData.Authors),
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
	}
}
