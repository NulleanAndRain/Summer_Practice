using Library.Entities;
using System.Windows.Media.Imaging;

namespace Library.DAL.Interface {
	public interface IDAOUsers {
		void CreateUser(User user);
		User GetUserWithName(string username);
		User GetUserWithId(int id);
		void UpdateUser(int id, User newData);
		void DeleteUser(int id);
		BitmapImage GetProfileImage(int id);
		void UpdateProfileImage(int id, BitmapImage img);
	}
}
