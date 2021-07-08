﻿using Library.BLL.Interface;
using Library.DAL.Interface;
using Library.Entities;
using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace LibraryBLL {
	public class BooksLogic : IBooksLogic {

		IDAOBooks dao;
		IUsersLogic users;

		public BooksLogic(IDAOBooks dao, IUsersLogic users) {
			this.dao = dao;
			this.users = users;
		}

		void rejectUnauthorised(Action<RejectData> onReject) {
			onReject(new RejectData(RejectType.Unauthorised, "You are not authorised"));
		}

		#region Books

		public void AddBook(Book book, int userId, Action<string> onSuccess, Action<RejectData> onReject) {
			if (!users.IsUserLoggedIn(userId)) {
				rejectUnauthorised(onReject);
				return;
			}
			dao.AddBook(book);
			onSuccess("Book successfully added");
		}

		public List<Book> GetBooks() {
			return dao.GetBooks();
		}

		public List<Book> GetBooksWithAuthors(string[] authors) {
			return dao.GetBooksWithAuthors(authors);
		}

		public List<Book> GetBooksWithName(string name) {
			return dao.GetBooksWithName(name);
		}

		public void EditBook(int id, Book newData, int userId, Action<string> onSuccess, Action<RejectData> onReject) {
			if (!users.IsUserLoggedIn(userId)) {
				rejectUnauthorised(onReject);
				return;
			}
			dao.EditBook(id, newData);
			onSuccess("Book successfully edited");
		}

		public void DeleteBook(int id, int userId, Action<string> onSuccess, Action<RejectData> onReject) {
			if (!users.IsUserLoggedIn(userId)) {
				rejectUnauthorised(onReject);
				return;
			}
			dao.DeleteBook(id);
			onSuccess("Book successfully deleted");
		}

		public BitmapImage GetBookImage(int id) {
			return dao.GetBookPicture(id);
		}

		public void UpdateBookImage(int id, int userId, BitmapImage img, Action<BitmapImage> onSuccess, Action<RejectData> onReject) {
			try {
				if (!users.IsUserLoggedIn(userId)) rejectUnauthorised(onReject);
				dao.UpdateBookPicture(id, img);
				onSuccess(img);
			} catch (Exception e) {
				onReject(new RejectData(RejectType.Exeption, e.Message));
			}
		}

		public byte[] GetBookFile(int id) {
			throw new NotImplementedException();
		}

		public void UpdateBookFile(int id, int userId, byte[] file, Action<BitmapImage> onSuccess, Action<RejectData> onReject) {
			throw new NotImplementedException();
		}

		#endregion
	}
}