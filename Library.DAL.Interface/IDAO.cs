using Library.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.DAL.Interface {
	public interface IDAO {
		void AddBook(Book book);
		List<Book> GetBooks();
		List<Book> GetBooksWithName(string name);
		List<Book> GetBooksWithAuthors(string[] authors);
		void EditBook(int id, Book newData);
		void DeleteBook(int id);

		void CreateUser(User user);
		User GetUserWithName(string username);
		User GetUserWithId(int id); 
		void UpdateUser(int id, User newData);
		void DeleteUser(int id);
	}
}
