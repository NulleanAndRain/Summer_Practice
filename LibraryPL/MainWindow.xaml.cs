using Library.BLL.Interface;
using Library.DependencyResolver;
using Library.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

		//
		ILogic logic;
		List<Book> Books;
		User user = new User();
		int currentPage;
		string searchString = string.Empty;

		Action BackBtnNext = delegate { };
		Stack<Action> BackBtnActions = new Stack<Action>();

		#region Initialisation

		public MainWindow() {
			InitializeComponent();
			logic = DependencyResolver.Instance.GetLogicObject();
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
		#endregion

		#region BooksPanel
		void OpenMainPanel() {
			CloseAll();
			MainPanel.Visibility = Visibility.Visible;
			if (user == null || user.Id == -1) {
				HeaderUnlogged.Visibility = Visibility.Visible;
			} else {
				Header_user_name.Content = user.FirstName + ' ' + user.LastName;
				//Header_user_pic // todo: header profile picture
				HeaderLogged.Visibility = Visibility.Visible;
			}
		}

		void OpenBooksPanel(bool updateBooks = true) {
			OpenMainPanel();
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
				var l1 = logic.GetBooksWithName(searchString);
				var l2 = logic.GetBooksWithAuthors(authors);
				Books.Clear();
				Books.AddRange(l1);
				Books.AddRange(l2);
			} else {
				Books = logic.GetBooks();
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

		void OpenBookView(Book book) {
			OpenMainPanel();
			BookView_bookName.Content = book.Name;
			BookView_authors.Content = book.Authors;
			BookView_year.Content = book.YearOfPublishing;
			BookView_Response.Content = string.Empty;
			//BookView_Pic //todo: book cover images
			BookView.Visibility = Visibility.Visible;
			void edit() {
				OpenBookEdit(book, true);
			}
			onBookEdit = edit;

			void load() {
				//todo: book loading
			}
			onBookLoad = load;

			void delete() {
				void onSuccess(string _) {
					OpenBooksPanel(true);
				}

				void onReject(RejectData data) {
					BookView_Response.Content = data.message;
				}

				logic.DeleteBook(book.Id, user.Id, onSuccess, onReject);
			}
			onBookDelete = delete;

			SetBackBtn(() => OpenBookView(book));
		}

		void OpenBookEdit(Book book, bool clearForm = true) {
			OpenMainPanel();
			if (clearForm) {
				BookEdit_BookName.Text = book.Name;
				BookEdit_Authors.Text = book.Authors;
				BookEdit_Year.Text = book.YearOfPublishing.ToString();
				//todo: book cover images
			}
			BookEdit_Response.Content = string.Empty;

			void save() {
				book.Name = BookEdit_BookName.Text;
				book.Authors = BookEdit_Authors.Text;
				if (int.TryParse(BookEdit_Year.Text, out int year)) {
					book.YearOfPublishing = year;
				} else {
					BookEdit_Response.Content = "Year must be an integer number";
					return;
				}

				void onSuccess(string _) {
					GetBooks();
					OpenBooksPanel(false);
				}
				void onReject(RejectData data) {
					BookEdit_Response.Content = data.message;
				}

				if (book.Id == -1) {
					logic.AddBook(book, user.Id, onSuccess, onReject);
				} else {
					logic.EditBook(book.Id, book, user.Id, onSuccess, onReject);
				}
			}
			onBookSave = save;

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
			logic.LogOut(user.Id);
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
				ClearBackBtnStack();
				OpenBooksPanel();
			}
			void onReject(RejectData data) {
				if (data.type != RejectType.Exeption) {
					Login_Signup_response.Content = data.message;
				}
			}
			logic.LogIn(Login_username.Text, Login_pass.Password, onSuccess, onReject);
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
			logic.CreateUser(_u, Signup_pass.Password, onSuccess, onReject);
		}

		#endregion

		#region Profile

		void OpenProfileEdit(bool clearForm = true) {
			CloseAll();
			if (clearForm) {
				Profile_UserName.Text = user.Username;
				Profile_FirstName.Text = user.FirstName;
				Profile_LastName.Text = user.LastName;
				Profile_BDate.SelectedDate = user.DateOfBirth;
				//todo: profile pic
			}
			void submit() {
				//todo: submiting chamges
			}
			onProfileSubmit = submit;

			void delete() {

			}
			onProfileDelete = delete;

			ProfilePanel.Visibility = Visibility.Visible;
			SetBackBtn(() => OpenProfileEdit(false));
		}

		#endregion

		#endregion

		#region Button Actions

		private void BtnBack(object sender, RoutedEventArgs e) {
			Back();
		}


		private void AddBookbtn(object sender, RoutedEventArgs e) {
			OpenBookEdit(new Book());
		}

		Action onBookSave = delegate { };
		private void BtnSaveBook(object sender, RoutedEventArgs e) {
			onBookSave();
		}

		private void OpenLogInBtn(object sender, RoutedEventArgs e) {
			OpenLogin();
		}

		private void OpenSigbUpBtn(object sender, RoutedEventArgs e) {
			OpenSignup();
		}

		private void BtnEditProfile(object sender, RoutedEventArgs e) {
			OpenProfileEdit();
		}

		private void BtnLogOut(object sender, RoutedEventArgs e) {
			LogOut();
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

		private void BtnSelectBookImg(object sender, RoutedEventArgs e) {

		}


		Action onBookLoad = delegate { };
		private void LoadBookBtn(object sender, RoutedEventArgs e) {
			onBookLoad();
		}

		Action onBookEdit = delegate { };
		private void BookViewEditBtn(object sender, RoutedEventArgs e) {
			onBookEdit();
		}

		Action onBookDelete = delegate { };
		private void BookDeleteBtn(object sender, RoutedEventArgs e) {
			onBookDelete();
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

		Action onProfileSubmit = delegate { };
		private void BtnProfileSubmit(object sender, RoutedEventArgs e) {
			onProfileSubmit();
		}

		Action onProfileDelete = delegate { };
		private void BtnDeleteAcc(object sender, RoutedEventArgs e) {
			onProfileDelete();
		}
	}
	#endregion
}
