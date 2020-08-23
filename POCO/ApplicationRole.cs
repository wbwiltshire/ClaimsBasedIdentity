using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace ClaimsBasedIdentity.Data.POCO
{
	public class ApplicationRole
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string ClaimType { get; set; }

		public override string ToString()
		{
			return $"{Id}|{Name}|{ClaimType}|";
		}

	}
}
