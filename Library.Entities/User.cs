using System;
using System.Windows.Media.Imaging;

namespace Library.Entities {
	public class User {
		public int Id = -1;
		public string Username;
		public string PassHash;
		public string FirstName;
		public string LastName;
		public DateTime DateOfBirth;
		public BitmapImage ProfileImage;
	}
}
