using Library.BLL.Interface;
using Library.DAL.Interface;
using Library.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Media.Imaging;

namespace LibraryBLL {
	public class UsersLogic : IUsersLogic {

		HashSet<int> loggedUsers;
		IDAOUsers dao;
		MD5 hashGen;

		public UsersLogic(IDAOUsers dao) {
			this.dao = dao;
			loggedUsers = new HashSet<int>();
			hashGen = MD5.Create();
		}

		void rejectUnauthorised(Action<RejectData> onReject) {
			onReject(new RejectData(RejectType.Unauthorised, "You are not authorised"));
		}

		void rejectPass(Action<RejectData> onReject) {
			onReject(new RejectData(RejectType.WrongPass, "Incorrect password"));
		}

		#region Users

		private string getPassHash(string pass, string username) {
			var i1 = hashGen.ComputeHash(Encoding.UTF8.GetBytes(pass));
			var i2 = hashGen.ComputeHash(i1.Concat(Encoding.UTF8.GetBytes(username)).ToArray());
			return Encoding.ASCII.GetString(i2).ToUpper();
		}

		public void CreateUser(User user, string pass, Action<User> onSuccess, Action<RejectData> onReject) {
			if (user == null || string.IsNullOrEmpty(user.Username)) throw new ArgumentNullException("User must not be null and have username");
			if (dao.GetUserWithName(user.Username) != null) {
				onReject(new RejectData(RejectType.UserExists, "User already exists"));
				return;
			}
			user.PassHash = getPassHash(pass, user.Username);
			try {
				dao.CreateUser(user);
				user = dao.GetUserWithName(user.Username);
				loggedUsers.Add(user.Id);
				onSuccess(user);
			} catch (Exception e) {
				onReject(new RejectData(RejectType.Exeption, e.Message));
			}
		}

		public void LogIn(string username, string pass, Action<User> onSuccess, Action<RejectData> onReject) {
			try {
				var user = dao.GetUserWithName(username);
				if (user == null) {
					onReject(new RejectData(RejectType.UserNotExist, "User does not exist"));
					return;
				}
				if (loggedUsers.Contains(user.Id)) {
					onReject(new RejectData(RejectType.AlreadyLogged, "You are already logged in"));
					return;
				}
				var hash = getPassHash(pass, username);
				if (!user.PassHash.Equals(hash)) {
					rejectPass(onReject);
					return;
				}
				loggedUsers.Add(user.Id);
				onSuccess(user);
			} catch (Exception e) {
				onReject(new RejectData(RejectType.Exeption, e.Message));
			}
		}

		public void LogOut(int id) {
			loggedUsers.Remove(id);
		}

		public void UpdateUserPassUsername(User newData, string pass, Action onSuccess, Action<RejectData> onReject, string newPass = null) {
			try {
				var oldData = dao.GetUserWithId(newData.Id);
				if (!oldData.PassHash.Equals(getPassHash(pass, oldData.Username))) {
					rejectPass(onReject);
					return;
				}
				if (!newData.Username.Equals(oldData.Username)) {
					var user = dao.GetUserWithName(newData.Username);
					if (user != null && user.Id != newData.Id) {
						onReject(new RejectData(RejectType.UserExists, "Username is already taken"));
						return;
					}
				}
				if (newPass != null) {
					newData.PassHash = getPassHash(newPass, newData.Username);
				}
				dao.UpdateUser(newData.Id, newData);
				onSuccess();
			} catch (Exception e) {
				onReject(new RejectData(RejectType.Exeption, e.Message));
			}
		}

		public void UpdateUserData(User newData, Action onSuccess, Action<RejectData> onReject) {
			try {
				var oldData = dao.GetUserWithId(newData.Id);
				newData.Username = oldData.Username;
				newData.PassHash = oldData.PassHash;
				dao.UpdateUser(newData.Id, newData);
				onSuccess();
			} catch (Exception e) {
				onReject(new RejectData(RejectType.Exeption, e.Message));
			}
		}

		public bool IsUsernameTaken(string name) {
			var user = dao.GetUserWithName(name);
			return user != null && user.Id != -1;
		}

		public void DeleteUser(int id, string pass, Action onSuccess, Action<RejectData> onReject) {
			try {
				var user = dao.GetUserWithId(id);
				if (user == null) {
					onReject(new RejectData(RejectType.UserNotExist, "User not found"));
					return;
				}
				if (!user.PassHash.Equals(getPassHash(pass, user.Username))) {
					rejectPass(onReject);
					return;
				}
				dao.DeleteUser(id);
				loggedUsers.Remove(id);
				onSuccess();
			} catch (Exception e) {
				onReject(new RejectData(RejectType.Exeption, e.Message));
			}
		}

		public bool IsUserLoggedIn(int id) {
			return loggedUsers.Contains(id);
		}

		public void UpdatePrifilePic(int id, BitmapImage img, Action<BitmapImage> onSuccess, Action<RejectData> onReject) {
			try {
				if (!loggedUsers.Contains(id)) {
					rejectUnauthorised(onReject);
					return;
				}
				dao.UpdateProfileImage(id, img);
				onSuccess(img);
			} catch (Exception e) {
				onReject(new RejectData(RejectType.Exeption, e.Message));
			}
		}

		public BitmapImage GetProfilePicture(int id) {
			return dao.GetProfileImage(id);
		}

		#endregion
	}
}
