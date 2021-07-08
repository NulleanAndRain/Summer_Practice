using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.BLL.Interface {
	public struct RejectData {
		public RejectType type;
		public string message;

		public RejectData(RejectType type, string message) {
			this.type = type;
			this.message = message;
		}
	}

	public enum RejectType {
		Exeption,
		Unauthorised,
		WrongPass,
		UserNotExist,
		UserExists,
		AlreadyLogged
	}
}
