using Library.Entities;
using System;

namespace Library.BLL.Interface {
	public interface IUsersLogic {
		void CreateUser(User user, string pass, Action<User> onSuccess, Action<RejectData> onReject);
		void LogIn(string username, string pass, Action<User> onSuccess, Action<RejectData> onReject);
		void UpdateUserData(User newData, Action onSuccess, Action<RejectData> onReject);
		void UpdateUserPassUsername(User newData, string pass, Action onSuccess, Action<RejectData> onReject, string newPass = null);
		bool IsUsernameTaken(string name);
		void DeleteUser(int id, string pass, Action onSuccess, Action<RejectData> onReject);
		void LogOut(int id);
		bool IsUserLoggedIn(int id);
	}
}
