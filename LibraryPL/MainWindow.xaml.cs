using Library.BLL.Interface;
using Library.DependencyResolver;
using Library.Entities;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace LibraryPL {
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {

		// panel and tab collections
		Grid[] Panels;
		Grid[] HeaderPanels;
		Grid[] MainTabs;
		Grid[] BooksGridCells;
		Grid[] LoginSignupPanels;
		Grid[] _rest;

		Grid[][] _all;

		//default images
		static readonly BitmapImage _ProfileDefaultPic = new BitmapImage(new Uri("pack://application:,,,/Img/profile.png"));
		static readonly BitmapImage _BookDefaultPic = new BitmapImage(new Uri("pack://application:,,,/Img/book.png"));

		//
		IUsersLogic UsersLogic;
		IBooksLogic BooksLogic;
		List<Book> Books;
		User user = new User();
		int currentPage;
		string searchString = string.Empty;

		Action BackBtnNext = delegate { };
		Stack<Action> BackBtnActions = new Stack<Action>();

		#region Initialisation
		private string ConnString => ConfigurationManager.ConnectionStrings["LibDB"].ConnectionString;

		public MainWindow() {
			InitializeComponent();
			UsersLogic = DependencyResolver.Instance.GetUsersLogicObject(ConnString);
			BooksLogic = DependencyResolver.Instance.GetBooksLogicObject(ConnString, UsersLogic);
			Hidden.Visibility = Visibility.Hidden;
			SetupCollections();
			OpenBooksPanel();
		}

		void SetupCollections() {
			Panels = new Grid[] {
				MainPanel,
				LoginSignupPanel,
				ProfilePanel,
				ConfirmPassPanel
			};
			HeaderPanels = new Grid[] {
				HeaderLogged,
				HeaderUnlogged
			};
			MainTabs = new Grid[] {
				BooksGrid,
				BookView,
				BookEdit
			};
			BooksGridCells = new Grid[] {
				Book_0_0,
				Book_0_1,
				Book_0_2,
				Book_0_3,
				Book_1_0,
				Book_1_1,
				Book_1_2,
				Book_1_3,
			};
			LoginSignupPanels = new Grid[] {
				Login,
				Signup
			};

			_rest = new Grid[] {
				HeaderControls
			};

			_all = new Grid[][] {
				Panels,
				HeaderPanels,
				MainTabs,
				LoginSignupPanels,
				_rest
			};
		}

		#endregion

		#region TabsControl

		#region Common
		void CloseAll() {
			foreach (var c in _all) {
				foreach (var p in c) {
					p.Visibility = Visibility.Hidden;
				}
			}
		}

		void SetBackBtn(Action action) { //todo: rework back button
			if (BackBtnNext != null) {
				BackBtnActions.Push(BackBtnNext);
			}
			BackBtnNext = action;
		}

		void ClearBackBtnStack() {
			BackBtnNext = null;
			BackBtnActions.Clear();
		}

		void Back() {
			BackBtnNext = null;
			if (BackBtnActions.Count > 0) {
				BackBtnActions.Pop()();
			}
		}

		BitmapImage GetUserImage() {
			OpenFileDialog dialog = new OpenFileDialog();
			dialog.Filter = "Image|*.png;*.jpeg;*.jpg;*.gif";
			dialog.Title = "Select an image file";
			dialog.Multiselect = false;
			var res = dialog.ShowDialog();
			if (res == null || !res.Value) return null;
			return new BitmapImage(new Uri(dialog.FileName));
		}

		#endregion

		#region BooksPanel
		void OpenMainPanel() {
			CloseAll();
			MainPanel.Visibility = Visibility.Visible;
			if (user == null || user.Id == -1) {
				HeaderUnlogged.Visibility = Visibility.Visible;
			} else {
				Header_user_name.Content = user.FirstName + ' ' + user.LastName;

				if (user.ProfileImage == null) {
					Header_user_pic.Source = _ProfileDefaultPic;
				} else {
					Header_user_pic.Source = user.ProfileImage;
				}

				HeaderLogged.Visibility = Visibility.Visible;
			}
		}

		void OpenBooksPanel(bool updateBooks = true) {
			OpenMainPanel();
			ClearBackBtnStack();
			HeaderControls.Visibility = Visibility.Visible;
			SearchInput.Text = searchString;
			if (updateBooks) {
				GetBooks();
				OpenPage(0);
			} else {
				ShowCurrenPage();
			}

			SetBackBtn(() => OpenBooksPanel(false));
		}

		void ClearBooksGrid() {
			Hidden.Children.Add(AddBookElem);
			foreach (var cell in BooksGridCells) {
				cell.Children.Clear();
			}
		}

		Grid AddBookElem {
			get {
				var p = (Grid)_AddBook.Parent;
				p.Children.Remove(_AddBook);
				return _AddBook;
			}
		}

		void GetBooks(bool useSearch = false) {
			if (useSearch && !string.IsNullOrEmpty(searchString)) {
				var authors = searchString.Split(new char[] { ',', ' ' },
					StringSplitOptions.RemoveEmptyEntries);
				var l1 = BooksLogic.GetBooksWithName(searchString);
				var l2 = BooksLogic.GetBooksWithAuthors(authors);
				Books.Clear();
				Books.AddRange(l1);
				Books.AddRange(l2);
			} else {
				Books = BooksLogic.GetBooks();
			}
		}

		void ShowCurrenPage() {
			ClearBooksGrid();
			for (int i = 0; i < 8; i++) {
				if (currentPage * 8 + i >= Books.Count) {
					if (user != null && user.Id != -1) {
						BooksGridCells[i].Children.Add(AddBookElem);
					}
					break;
				}
				var b = Books[currentPage * 8 + i];
				//if (b.BookImage == null) {
					GetBookImage(b);
				//}
				var c = CreateBookCard(b);
				BooksGridCells[i].Children.Add(c);
			}
			PageSelect_PageCount.Content = $"/ {maxPage + 1}";
			BooksGrid.Visibility = Visibility.Visible;
		}

		BookCard CreateBookCard(Book book = null) {
			var card = new BookCard(book);
			void show() {
				SearchInput.Text = "click";
				OpenBookView(book);
			}
			card.OnCLick += show;
			return card;
		}

		void GetBookImage(Book book) {
			var img = BooksLogic.GetBookImage(book.Id);
			if (img == null) {
				book.BookImage = _BookDefaultPic;
			} else {
				book.BookImage = img;
			}
		}

		void OpenBookView(Book book) {
			OpenMainPanel();
			GetBookImage(book);
			BookView_bookName.Content = book.Name;
			BookView_authors.Content = book.Authors;
			BookView_year.Content = book.YearOfPublishing;
			BookView_Response.Content = string.Empty;
			BookView_Pic.Source = book.BookImage;
			BookView.Visibility = Visibility.Visible;
			void edit() {
				OpenBookEdit(book, true);
			}
			onBookEdit = edit;

			if (user == null || user.Id == -1) {
				BookView_Edit.Visibility = Visibility.Hidden;
				BookView_Delete.Visibility = Visibility.Hidden;
			} else {
				BookView_Edit.Visibility = Visibility.Visible;
				BookView_Delete.Visibility = Visibility.Visible;
			}

			void load() {
				void onFileLoad(byte[] file, string name) {
					if (file == null || file.Length == 0 || string.IsNullOrEmpty(name)) {
						BookView_Response.Content = "This book does not have attached file";
						return;
					}
					var a = name.Split('.');
					var ext = a[a.Length - 1];
					var dialog = new SaveFileDialog();
					dialog.Filter = $"Text file |*.{ext}";
					dialog.Title = "Save a text file";
					dialog.FileName = name;
					var res = dialog.ShowDialog();

					if (res == null || !res.Value) return;

					var fs = dialog.OpenFile();
					fs.Write(file, 0, file.Length);
					fs.Close();
				}
				BooksLogic.GetBookFile(book.Id, onFileLoad);
			}
			onBookLoad = load;

			void delete() {
				void onSuccess(string _) {
					OpenBooksPanel(true);
				}

				void onReject(RejectData data) {
					BookView_Response.Content = data.message;
				}

				BooksLogic.DeleteBook(book.Id, user.Id, onSuccess, onReject);
			}
			onBookDelete = delete;

			SetBackBtn(() => OpenBookView(book));
		}

		void OpenBookEdit(Book book, bool clearForm = true) {
			OpenMainPanel();
			if (clearForm) {
				GetBookImage(book);
				BookEdit_BookName.Text = book.Name;
				BookEdit_Authors.Text = book.Authors;
				BookEdit_Year.Text = book.YearOfPublishing.ToString();
				BookEdit_Pic.Source = book.BookImage;
			}
			BookEdit_Response.Content = string.Empty;

			Action onNewBookSaveImg = delegate { };
			Action onNewBookSaveFile = delegate { };
			void save() {
				book.Name = BookEdit_BookName.Text;
				book.Authors = BookEdit_Authors.Text;
				if (int.TryParse(BookEdit_Year.Text, out int year)) {
					book.YearOfPublishing = year;
				} else {
					BookEdit_Response.Content = "Year must be an integer number";
					return;
				}

				void onSuccessNew(int id) {
					book.Id = id;
					onNewBookSaveImg();
					onNewBookSaveFile();
					onSuccessEdit(id.ToString());
				}
				void onSuccessEdit(string _) {
					GetBooks(!string.IsNullOrEmpty(searchString));
					OpenBooksPanel(false);
				}
				void onReject(RejectData data) {
					BookEdit_Response.Content = data.message;
				}

				if (book.Id == -1) {
					BooksLogic.AddBook(book, user.Id, onSuccessNew, onReject);
				} else {
					BooksLogic.EditBook(book.Id, book, user.Id, onSuccessEdit, onReject);
				}
			}
			onBookSave = save;

			void updateImage() {
				var img = GetUserImage();
				//BookEdit_Response.Content = "Adding img";
				if (img == null) {
					BookEdit_Pic.Source = _BookDefaultPic;
				} else {
					BookEdit_Pic.Source = img;
				}
				void onSuccess(BitmapImage _img) {
					if (_img == null) {
						_img = _BookDefaultPic;
					}
					BookEdit_Pic.Source = _img;
					book.BookImage = _img;
				}
				void onReject(RejectData data) {
					BookEdit_Response.Content = data.message;
				}
				if (book.Id == -1) {
					onNewBookSaveImg = () => BooksLogic.UpdateBookImage(book.Id, user.Id, img, onSuccess, onReject);
				} else {
					BooksLogic.UpdateBookImage(book.Id, user.Id, img, onSuccess, onReject);
				}
			}
			onBookImageUpdate = updateImage;

			void removeImage() {
				void onSuccess(BitmapImage _img) {
					BookEdit_Pic.Source = _BookDefaultPic;
					book.BookImage = _BookDefaultPic;
				}
				void onReject(RejectData data) {
					BookEdit_Response.Content = data.message;
				}
				if (book.Id == -1) {
					onNewBookSaveImg = delegate { };
				} else {
					BooksLogic.UpdateBookImage(book.Id, user.Id, null, onSuccess, onReject);
				}
			}
			onBookImageDelete = removeImage;

			void updateFile() {
				var dialog = new OpenFileDialog();
				dialog.Filter = "Text files |*.doc;*.docx;*.txt;*.pdf";
				dialog.Title = "Select a text file";
				dialog.Multiselect = false;
				var res = dialog.ShowDialog();
				if (res == null || !res.Value) return;
				var file = File.ReadAllBytes(dialog.FileName);
				if (file == null || file.Length == 0) return;
				void onReject(RejectData data) {
					BookEdit_Response.Content = data.message;
				}
				var path = dialog.FileName.Split(Path.DirectorySeparatorChar);
				var name = path[path.Length - 1];
				if (book.Id == -1) {
					onNewBookSaveFile = () => BooksLogic.UpdateBookFile(book.Id, user.Id, file, name, () => { }, onReject);
				} else {
					BooksLogic.UpdateBookFile(book.Id, user.Id, file, name, () => { }, onReject);
				}
			}
			onBookFileUpdate = updateFile;

			BookEdit.Visibility = Visibility.Visible;
			SetBackBtn(() => OpenBookEdit(book, false));
		}

		void SearchBooks() {
			searchString = SearchInput.Text;
			currentPage = 0;
			GetBooks(true);
			ShowCurrenPage();
		}

		int maxPage {
			get {
				if (Books != null) {
					var c = Books.Count;
					if (user != null && user.Id != -1) c++;
					return (c - 1) / 8;
				}
				return -1;
			}
		}

		void OpenPage(int page) {
			currentPage = page;
			if (currentPage < 0) currentPage = 0;
			if (currentPage > maxPage) currentPage = maxPage;
			PageSelectInput.Text = (currentPage + 1).ToString();
			ShowCurrenPage();
		}

		void LogOut() {
			UsersLogic.LogOut(user.Id);
			user = new User();
			HeaderLogged.Visibility = Visibility.Hidden;
			HeaderUnlogged.Visibility = Visibility.Visible;
			ShowCurrenPage();
		}

		#endregion

		#region Login Signup

		void OpenLoginSignup() {
			CloseAll();
			Login_Signup_response.Content = string.Empty;
			LoginSignupPanel.Visibility = Visibility.Visible;
		}

		void OpenLogin(bool clearForm = true) {
			OpenLoginSignup();
			if (clearForm) {
				Login_username.Text = string.Empty;
				Login_pass.Password = string.Empty;
				SetBackBtn(() => OpenLogin(false));
			}
			Login.Visibility = Visibility.Visible;
		}

		static readonly Regex _username_regex = new Regex(@"[^a-zA-Z0-9_.]");
		bool CheckUsername(string str) => _username_regex.IsMatch(str);

		void LogIn() {
			if (string.IsNullOrEmpty(Login_username.Text) ||
				string.IsNullOrEmpty(Login_pass.Password)) {
				Login_Signup_response.Content = "Enter all fields";
				return;
			}
			if (CheckUsername(Login_username.Text)) {
				Login_Signup_response.Content = "Incorrect username";
				return;
			}
			void onSuccess(User u) {
				user = u;
				u.ProfileImage = UsersLogic.GetProfilePicture(u.Id);
				ClearBackBtnStack();
				OpenBooksPanel();
				Login_Signup_response.Content = "Logged in";
			}
			void onReject(RejectData data) {
				if (data.type != RejectType.Exeption) {
					Login_Signup_response.Content = data.message;
				}
			}
			UsersLogic.LogIn(Login_username.Text, Login_pass.Password, onSuccess, onReject);
		}

		void OpenSignup(bool clearForm = true) {
			OpenLoginSignup();
			if (clearForm) {
				Signup_username.Text = string.Empty;
				Signup_pass.Password = string.Empty;
				Signup_pass_confirm.Password = string.Empty;
				Signup_firstname.Text = string.Empty;
				Signup_lastname.Text = string.Empty;
				Signup_bdate.SelectedDate = DateTime.Now;
				SetBackBtn(() => OpenSignup(false));
			}
			Signup.Visibility = Visibility.Visible;
		}

		void SignUp() {
			if (string.IsNullOrEmpty(Signup_username.Text) ||
				string.IsNullOrEmpty(Signup_firstname.Text) ||
				string.IsNullOrEmpty(Signup_lastname.Text) ||
				string.IsNullOrEmpty(Signup_pass.Password) ||
				string.IsNullOrEmpty(Signup_pass_confirm.Password)) {
				Login_Signup_response.Content = "Enter all fields";
				return;
			}
			if (Signup_pass.Password != Signup_pass_confirm.Password) {
				Login_Signup_response.Content = "Passwords do not match";
				return;
			}
			var name_len = Signup_username.Text.Length;
			if (name_len > 24 || name_len < 4) {
				Login_Signup_response.Content = "Username length should be in range from 4 to 24";
				return;
			}

			if (CheckUsername(Login_username.Text)) {
				Login_Signup_response.Content = "Username must consist of latin letters, numbers, dots and underscores";
				return;
			}

			if (Signup_pass.Password.Length < 8) {
				Login_Signup_response.Content = "Password should be at least 8 symbols long";
				return;
			}

			void onSuccess(User u) {
				user = u;
				ClearBackBtnStack();
				OpenBooksPanel();
			}
			void onReject(RejectData data) {
				if (data.type != RejectType.Exeption) {
					Login_Signup_response.Content = data.message;
				}
			}

			User _u = new User {
				Username = Signup_username.Text,
				FirstName = Signup_firstname.Text,
				LastName = Signup_lastname.Text,
				DateOfBirth = Signup_bdate.SelectedDate.Value.Date
			};
			UsersLogic.CreateUser(_u, Signup_pass.Password, onSuccess, onReject);
		}

		#endregion

		#region Profile

		void OpenProfileEdit(bool clearForm = true) {
			CloseAll();
			Profile_NewPass.Password = string.Empty;
			if (clearForm) {
				Profile_UserName.Text = user.Username;
				Profile_FirstName.Text = user.FirstName;
				Profile_LastName.Text = user.LastName;
				Profile_BDate.SelectedDate = user.DateOfBirth;
				ProfilePanel_Response.Text = string.Empty;

				if (user.ProfileImage == null) {
					Profile_ProfilePic.Source = _ProfileDefaultPic;
				} else {
					Profile_ProfilePic.Source = user.ProfileImage;
				}
			}

			void onReject(RejectData data) {
				OpenProfileEdit(false);
				ProfilePanel_Response.Text = data.message;
			}

			void submit() {
				void onSuccess() {
					OpenBooksPanel();
				}

				if (Profile_UserName.Text != user.Username || !string.IsNullOrEmpty(Profile_NewPass.Password)) {
					if (Profile_UserName.Text != user.Username) {
						if (UsersLogic.IsUsernameTaken(Profile_UserName.Text)) {
							ProfilePanel_Response.Text = "Username is already taken";
							return;
						}
					}
					string newPass = null;
					if (!string.IsNullOrEmpty(Profile_NewPass.Password)) {
						if (Profile_NewPass.Password.Length < 8) {
							ProfilePanel_Response.Text = "Password should be at least 8 symbols long";
							return;
						}
						newPass = Profile_NewPass.Password;
					}
					user.Username = Profile_UserName.Text;
					user.FirstName = Profile_FirstName.Text;
					user.LastName = Profile_LastName.Text;
					user.DateOfBirth = Profile_BDate.SelectedDate.Value.Date;

					void doWithPass(string pass) {
						UsersLogic.UpdateUserPassUsername(user, pass, onSuccess, onReject, newPass);
					}
					OpenConfirmPanel(doWithPass);
					return;
				}
				user.FirstName = Profile_FirstName.Text;
				user.LastName = Profile_LastName.Text;
				user.DateOfBirth = Profile_BDate.SelectedDate.Value.Date;

				UsersLogic.UpdateUserData(user, onSuccess, onReject);
			}
			onProfileSubmit = submit;

			void delete() {
				void onSuccess() {
					LogOut();
					OpenBooksPanel();
				}

				void doWithPass(string pass) {
					UsersLogic.DeleteUser(user.Id, pass, onSuccess, onReject);
				}

				OpenConfirmPanel(doWithPass);
			}
			onProfileDelete = delete;

			ProfilePanel.Visibility = Visibility.Visible;
			SetBackBtn(() => OpenProfileEdit(false));
		}

		void OpenConfirmPanel(Action<string> actionWithPass) {
			CloseAll();
			ConfirmPass_passInput.Password = string.Empty;
			ConfirmPass_passInputRepeat.Password = string.Empty;
			void confirm() {
				if (string.IsNullOrEmpty(ConfirmPass_passInput.Password)||
					string.IsNullOrEmpty(ConfirmPass_passInputRepeat.Password)) {
					ConfirmPass_response.Content = "Enter all fields";
					return;
				}
				if (!ConfirmPass_passInput.Password.Equals(ConfirmPass_passInputRepeat.Password)) {
					ConfirmPass_response.Content = "Passwords do not match";
					return;
				}
				Back();
				actionWithPass(ConfirmPass_passInput.Password);
			}
			onPassConfirm = confirm;
			ConfirmPassPanel.Visibility = Visibility.Visible;

			SetBackBtn(() => OpenConfirmPanel(actionWithPass));
		}

		void UpdateProfilePicture() {
			var img = GetUserImage();
			if (img == null) return;
			void onSuccess(BitmapImage _img) {
				Profile_ProfilePic.Source = _img;
				user.ProfileImage = _img;
			}
			void onReject(RejectData data) {
				ProfilePanel_Response.Text = data.message;
			}
			UsersLogic.UpdatePrifilePic(user.Id, img, onSuccess, onReject);
		}

		void DeleteProfilePicture() {
			user.ProfileImage = null;
			void onSuccess(BitmapImage _img) {
				Profile_ProfilePic.Source = _ProfileDefaultPic;
			}
			void onReject(RejectData data) {
				ProfilePanel_Response.Text = data.message;
			}

			UsersLogic.UpdatePrifilePic(user.Id, null, onSuccess, onReject);
		}


		#endregion

		#endregion

		#region Button Actions

		private void BtnBack(object sender, RoutedEventArgs e) {
			Back();
		}

		#region Books view

		private void AddBookbtn(object sender, RoutedEventArgs e) {
			OpenBookEdit(new Book());
		}

		private void BtnPrevPage(object sender, RoutedEventArgs e) {
			OpenPage(--currentPage);
		}

		private void BtnNextPage(object sender, RoutedEventArgs e) {
			OpenPage(++currentPage);
		}

		void PageSelectKeyDown(object s, KeyEventArgs e) {
			if (e.Key == Key.Enter) {
				Keyboard.ClearFocus();
				if (int.TryParse(PageSelectInput.Text, out int page)) {
					OpenPage(page - 1);
				} else {
					PageSelectInput.Text = (currentPage + 1).ToString();
				}
			}
		}

		void SearchKeyDown(object s, KeyEventArgs e) {
			if (e.Key == Key.Enter) {
				Keyboard.ClearFocus();
				SearchBooks();
			}
		}

		private void BtnSearch(object sender, RoutedEventArgs e) {
			SearchBooks();
		}

		Action onBookLoad = delegate { };
		private void LoadBookBtn(object sender, RoutedEventArgs e) {
			onBookLoad();
		}

		#endregion

		#region Book editing

		Action onBookSave = delegate { };
		private void BtnSaveBook(object sender, RoutedEventArgs e) {
			onBookSave();
		}

		Action onBookImageUpdate = delegate { };
		private void BtnSelectBookImg(object sender, RoutedEventArgs e) {
			onBookImageUpdate();
		}

		Action onBookImageDelete = delegate { };
		private void BtnDeleteBookImg(object sender, RoutedEventArgs e) {
			onBookImageDelete();
		}


		Action onBookFileUpdate = delegate { };
		private void BtnSelectBookFile(object sender, RoutedEventArgs e) {
			onBookFileUpdate();
		}

		Action onBookEdit = delegate { };
		private void BookViewEditBtn(object sender, RoutedEventArgs e) {
			onBookEdit();
		}

		Action onBookDelete = delegate { };
		private void BookDeleteBtn(object sender, RoutedEventArgs e) {
			onBookDelete();
		}

		#endregion

		#region Login and signup

		private void OpenLogInBtn(object sender, RoutedEventArgs e) {
			OpenLogin();
		}

		private void OpenSigbUpBtn(object sender, RoutedEventArgs e) {
			OpenSignup();
		}

		private void BtnSwitchToSignup(object sender, RoutedEventArgs e) {
			OpenSignup(false);
		}
		private void BtnLogIn(object sender, RoutedEventArgs e) {
			LogIn();
		}

		private void BtnSwitchToLogin(object sender, RoutedEventArgs e) {
			OpenLogin(false);
		}
		private void BtnSignUp(object sender, RoutedEventArgs e) {
			SignUp();
		}

		private void BtnLogOut(object sender, RoutedEventArgs e) {
			LogOut();
		}

		#endregion

		#region ProfileEditing

		private void BtnEditProfile(object sender, RoutedEventArgs e) {
			OpenProfileEdit();
		}

		Action onProfileSubmit = delegate { };
		private void BtnProfileSubmit(object sender, RoutedEventArgs e) {
			onProfileSubmit();
		}

		Action onProfileDelete = delegate { };
		private void BtnDeleteAcc(object sender, RoutedEventArgs e) {
			onProfileDelete();
		}

		Action onPassConfirm = delegate { };
		private void BtnConfirmPass(object sender, RoutedEventArgs e) {
			onPassConfirm();
		}

		private void BtnSelectProfilePic(object sender, RoutedEventArgs e) {
			UpdateProfilePicture();
		}

		private void BtnRemoveProfilePic(object sender, RoutedEventArgs e) {
			DeleteProfilePicture();
		}

		#endregion

		#endregion
	}
}
