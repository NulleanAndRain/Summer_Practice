using Library.Entities;
using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace Library.DAL.Interface {
	public interface IDAOBooks {
		void AddBook(Book book);
		List<Book> GetBooks();
		List<Book> GetBooksWithName(string name);
		List<Book> GetBooksWithAuthors(string[] authors);
		void EditBook(int id, Book newData);
		void DeleteBook(int id);
		void UpdateBookPicture(int id, BitmapImage img);
		BitmapImage GetBookPicture(int id);
		void UpdateBookFile(int id, byte[] file, string filename);
		void GetBookFile(int id, Action<byte[], string> onFileLoad);
	}
}
