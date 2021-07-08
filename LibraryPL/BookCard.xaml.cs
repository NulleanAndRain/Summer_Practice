using Library.Entities;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace LibraryPL {
	/// <summary>
	/// Логика взаимодействия для BookCard.xaml
	/// </summary>
	public partial class BookCard : UserControl {
		public event Action OnCLick = delegate { };
		public string BookName { get => _book_name.Content.ToString(); set => _book_name.Content = value; }
		public string Authors { get => _book_authors.Content.ToString(); set => _book_authors.Content = value; }
		public BitmapImage BookImage { get => (BitmapImage)_img.Source; set => _img.Source = value; }

		public BookCard() {
			InitializeComponent();
		}

		public BookCard(Book book) : this() {
			BookName = book.Name;
			Authors = book.Authors;
			BookImage = book.BookImage;
		}

		private void _btn_Click(object sender, RoutedEventArgs e) {
			OnCLick();
		}
	}
}
