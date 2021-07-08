using Library.Entities;
using System.Collections.Generic;

namespace Library.DAL.Interface {
	public interface IDAOBooks {
		void AddBook(Book book);
		List<Book> GetBooks();
		List<Book> GetBooksWithName(string name);
		List<Book> GetBooksWithAuthors(string[] authors);
		void EditBook(int id, Book newData);
		void DeleteBook(int id);
	}
}
