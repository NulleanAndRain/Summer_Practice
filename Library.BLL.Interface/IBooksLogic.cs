using Library.Entities;
using System;
using System.Collections.Generic;

namespace Library.BLL.Interface {
	public interface IBooksLogic {
		void AddBook(Book book, int userId, Action<string> onSuccess, Action<RejectData> onReject);
		List<Book> GetBooks();
		List<Book> GetBooksWithName(string name);
		List<Book> GetBooksWithAuthors(string[] authors);
		void EditBook(int id, Book newData, int userId, Action<string> onSuccess, Action<RejectData> onReject);
		void DeleteBook(int id, int userId, Action<string> onSuccess, Action<RejectData> onReject);
	}
}
