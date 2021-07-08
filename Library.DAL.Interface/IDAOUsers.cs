using Library.Entities;

namespace Library.DAL.Interface {
	public interface IDAOUsers {
		void CreateUser(User user);
		User GetUserWithName(string username);
		User GetUserWithId(int id);
		void UpdateUser(int id, User newData);
		void DeleteUser(int id);
	}
}
