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

		public static ICollection<ApplicationRole> Roles { get; set; }
		public static void Initialize()
		{
			Roles = new List<ApplicationRole>() { 
				new ApplicationRole() { Id =1, Name = "Administrator", ClaimType= ClaimTypes.Role },
				new ApplicationRole() { Id =1, Name = "Manager", ClaimType= ClaimTypes.Role },
				new ApplicationRole() { Id =1, Name = "Basic", ClaimType= ClaimTypes.Role }
			};
		}

	}
}
