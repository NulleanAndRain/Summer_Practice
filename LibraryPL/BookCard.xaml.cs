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
	/// Логика взаимодействия для BookCard.xaml
	/// </summary>
	public partial class BookCard : UserControl {
		public event Action OnCLick = delegate { };
		public string BookName { get => _book_name.Content.ToString(); set => _book_name.Content = value; }
		public string Authors { get => _book_authors.Content.ToString(); set => _book_authors.Content = value; }

		public BookCard() {
			InitializeComponent();
		}

		private void _btn_Click(object sender, RoutedEventArgs e) {
			Console.WriteLine("btn click");
			OnCLick();
		}
	}
}
