using Library.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.BLL.Interface {
	public interface ILogic {
		void AddBook(Book book, int userId, Action<string> onSuccess, Action<RejectData> onReject);
		List<Book> GetBooks();
		List<Book> GetBooksWithName(string name);
		List<Book> GetBooksWithAuthors(string[] authors);
		void EditBook(int id, Book newData, int userId, Action<string> onSuccess, Action<RejectData> onReject);
		void DeleteBook(int id, int userId, Action<string> onSuccess, Action<RejectData> onReject);

		void CreateUser(User user, string pass, Action<User> onSuccess, Action<RejectData> onReject);
		void LogIn(string username, string pass, Action<User> onSuccess, Action<RejectData> onReject);
		void UpdateUser(int id, User newData, string pass, Action<User> onSuccess, Action<RejectData> onReject);
		void DeleteUser(int id, string pass, Action<User> onSuccess, Action<RejectData> onReject);
		void LogOut(int id);
	}
}
