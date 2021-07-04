using Library.BLL.Interface;
using Library.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryBLL {
	public class Logic : ILogic {

		#region Books

		public void AddBook(Book book, int userId, Action<string> onSuccess, Action<string> onReject) {
			throw new NotImplementedException();
		}

		public List<Book> GetBooks() {
			throw new NotImplementedException();
		}

		public List<Book> GetBooksWithAuthors(string[] authors) {
			throw new NotImplementedException();
		}

		public List<Book> GetBooksWithName(string name) {
			throw new NotImplementedException();
		}

		public void EditBook(int id, Book newData, int userId, Action<string> onSuccess, Action<string> onReject) {
			throw new NotImplementedException();
		}

		public void DeleteBook(int id, int userId, Action<string> onSuccess, Action<string> onReject) {
			throw new NotImplementedException();
		}
		#endregion

		#region Users

		public void CreateUser(User user, Action<User> onSuccess, Action<string> onReject) {
			throw new NotImplementedException();
		}

		public void LogIn(string username, string pass, Action<User> onSuccess, Action<string> onReject) {
			throw new NotImplementedException();
		}

		public void LogOut(int id) {
			throw new NotImplementedException();
		}

		public void UpdateUser(int id, User newData, string pass, Action<User> onSuccess, Action<string> onReject) {
			throw new NotImplementedException();
		}

		public void DeleteUser(int id, string pass, Action<User> onSuccess, Action<string> onReject) {
			throw new NotImplementedException();
		}

		#endregion
	}
}
