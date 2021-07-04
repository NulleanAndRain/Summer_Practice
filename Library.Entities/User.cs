using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Entities {
	public class User {
		public int Id;
		public string Username;
		public string PassHash;
		public string FirstName;
		public string LastName;
		public DateTime DateOfBirth;
	}
}
