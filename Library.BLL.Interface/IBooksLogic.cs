using Library.Entities;
using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace Library.BLL.Interface {
	public interface IBooksLogic {
		void AddBook(Book book, int userId, Action<int> onSuccess, Action<RejectData> onReject);
		List<Book> GetBooks();
		List<Book> GetBooksWithName(string name);
		List<Book> GetBooksWithAuthors(string[] authors);
		void EditBook(int id, Book newData, int userId, Action<string> onSuccess, Action<RejectData> onReject);
		void DeleteBook(int id, int userId, Action<string> onSuccess, Action<RejectData> onReject);
		BitmapImage GetBookImage(int id);
		void UpdateBookImage(int id, int userId, BitmapImage img, Action<BitmapImage> onSuccess, Action<RejectData> onReject);
		void GetBookFile(int id, Action<byte[], string> onFileLoad);
		void UpdateBookFile(int id, int userId, byte[] file, string filename, Action onSuccess, Action<RejectData> onReject);
	}
}
