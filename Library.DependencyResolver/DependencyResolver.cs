using Library.BLL.Interface;
using Library.DAL.Interface;
using LibraryBLL;
using LibraryDAL;

namespace Library.DependencyResolver {
	public class DependencyResolver {
		public static DependencyResolver Instance { get; } = new DependencyResolver();

		IDAOUsers daoUsers;
		IUsersLogic logicUsers;
		IDAOUsers GetDAOUsers(string ConnString) => daoUsers != null ? daoUsers : daoUsers = new MsSqlDAOUsers(ConnString);
		public IUsersLogic GetUsersLogicObject(string ConnString) => 
			logicUsers != null ? 
				logicUsers :
				logicUsers = new UsersLogic(GetDAOUsers(ConnString));

		IDAOBooks daoBooks;
		IBooksLogic logicBooks;
		IDAOBooks GetDAOBooks(string ConnString) => daoBooks != null? daoBooks : daoBooks = new MsSqlDAOBooks(ConnString);
		public IBooksLogic GetBooksLogicObject(string ConnString, IUsersLogic usersLogic) => 
			logicBooks != null? 
				logicBooks :
				logicBooks = new BooksLogic(GetDAOBooks(ConnString), usersLogic);
	}
}
