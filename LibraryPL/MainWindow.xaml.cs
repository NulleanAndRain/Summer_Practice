using Library.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

		List<Book> Books;
		User user = null;
		int currentPage;
		string searchString = string.Empty;

		event Action onBackButtonClickNext = delegate { };
		event Action onBackButtonClick = delegate { };

		#region Initialisation

		public MainWindow() {
			InitializeComponent();
			Templates.Visibility = Visibility.Hidden;
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

		void SetBackBtn(Action action) {
			onBackButtonClick = onBackButtonClickNext;
			onBackButtonClickNext = action;
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

		void OpenBooksPanel() {
			OpenMainPanel();
			HeaderControls.Visibility = Visibility.Visible;
			SearchInput.Text = searchString;
			ShowCurrenPage();
			SetBackBtn(OpenBooksPanel);
		}

		void ClearBooksGrid() {
			foreach (var cell in BooksGridCells) {
				cell.Children.Clear();
			}
		}

		void ShowCurrenPage() {
			ClearBooksGrid();
			//todo: books page showing
		}

		void OpenBookView(Book book) {
			OpenMainPanel();
			BookView_bookName.Content = book.Name;
			BookView_authors.Content = book.Authosrs;
			BookView_year.Content = book.YearOfPublishing;
			//BookView_Pic //todo: book cover images

			SetBackBtn(() => OpenBookView(book));
		}

		void OpenBookEdit(Book book, bool clearForm = true) {
			OpenMainPanel();
			if (clearForm) {
				BookEdit_BookName.Text = book.Name;
				BookEdit_Authors.Text = book.Authosrs;
				BookEdit_Year.Text = book.YearOfPublishing.ToString();
			}
			SetBackBtn(() => OpenBookEdit(book, false));
		}
		#endregion

		#endregion
	}
}
