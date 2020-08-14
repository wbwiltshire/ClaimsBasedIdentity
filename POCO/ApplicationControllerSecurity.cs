using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace ClaimsBasedIdentity.Data.POCO
{
	public class ApplicationControllerSecurity
	{
		public PrimaryKey PK { get; set; }
		public int Id
		{
			get { return (int)PK.Key; }
			set { PK.Key = (int)value; }
		}
		[Display(Name = "Role Name")]
		public string RoleName { get; set; }
		[Display(Name = "Controller Name")]
		public string ControllerName { get; set; }
		[Display(Name = "Action Name")]
		public string ActionName { get; set; }
		public bool Active { get; set; }
		public DateTime ModifiedDt { get; set; }
		public DateTime CreateDt { get; set; }

		public ApplicationControllerSecurity()
		{
			PK = new PrimaryKey() { Key = -1, IsIdentity = true };
		}
		public override string ToString()
		{
			return $"{Id}|{RoleName}|{ControllerName}|{ActionName}|{Active}|{ModifiedDt}|{CreateDt}|";
		}

	}
}
