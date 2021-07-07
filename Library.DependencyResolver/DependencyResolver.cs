using Library.BLL.Interface;
using Library.DAL.Interface;
using LibraryBLL;
using LibraryDAL;

namespace Library.DependencyResolver {
	public class DependencyResolver {
		public static DependencyResolver Instance { get; } = new DependencyResolver();

		IDAO dao;
		ILogic logic;
		IDAO GetDAO() => dao != null? dao : dao = new MsSqlDAO();
		public ILogic GetLogicObject() => logic != null? logic : logic = new Logic(GetDAO());
	}
}
