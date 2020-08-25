using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ClaimsBasedIdentity.Data.POCO
{
	public class ApplicationAuditLog
	{
		public PrimaryKey PK { get; set; }
		public int Id
		{
			get { return (int)PK.Key; }
			set { PK.Key = (int)value; }
		}

		[Display(Name = "Category")]
		public int CategoryId { get; set; }
		[Display(Name = "Description")]
		public string Description { get; set; }
		[Display(Name = "User Name")]
		public string UserName { get; set; }
		[Display(Name = "Log Date")]
		public DateTime CreateDt { get; set; }

		public ApplicationAuditLog()
		{
			PK = new PrimaryKey() { Key = -1, IsIdentity = true };
		}
		public override string ToString()
		{
			return $"{Id}|{CategoryId}|{Description}|{CreateDt}|";
		}

	}
}
