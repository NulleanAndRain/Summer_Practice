using Library.BLL.Interface;
using Library.DAL.Interface;
using LibraryBLL;
using LibraryDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.DependencyResolver {
	public class DependencyResolver {
		public static DependencyResolver Instance { get; } = new DependencyResolver();

		IDAO dao;
		ILogic logic;
		public IDAO GetDAO() => dao != null? dao : dao = new MsSqlDAO();
		public ILogic GetLogicObject() => logic != null? logic : logic = new Logic(GetDAO());
	}
}
