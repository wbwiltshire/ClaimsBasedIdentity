using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text;
using Microsoft.Extensions.Logging;
using ClaimsBasedIdentity.Data;
using ClaimsBasedIdentity.Data.Interfaces;

namespace ClaimsBasedIdentity.Data.POCO
{
	public class ApplicationUser
	{
		public PrimaryKey PK { get; set; }
		public int Id
		{
			get { return (int)PK.Key; }
			set { PK.Key = (int)value; }
		}
		[Display(Name = "User Name")]
		public string UserName { get; set; }
		public string NormalizedUserName { get; set; }
		[Display(Name = "Email")]
		public string Email { get; set; }
		public string NormalizedEmail { get; set; }
		[Display(Name = "Email Conf.")]
		public bool EmailConfirmed { get; set; }
		public string PasswordHash { get; set; }
		[Display(Name = "Phone")]
		public string PhoneNumber { get; set; }
		[Display(Name = "Phone Conf.")]
		public bool PhoneNumberConfirmed { get; set; }
		[Display(Name = "Two Factor")]
		public bool TwoFactorEnabled { get; set; }
		[Display(Name = "Date of Birth")]
		public DateTime DOB { get; set; }
		[Display(Name = "Department")]
		public string Department { get; set; }
		public bool Active { get; set; }
		public DateTime ModifiedDt { get; set; }
		public DateTime CreateDt { get; set; }

		public ApplicationUser()
		{
			PK = new PrimaryKey() { Key = -1, IsIdentity = true };
		}
		public override string ToString()
		{
			return $"{Id}|{UserName}|{NormalizedUserName}|{Email}|{NormalizedEmail}|{EmailConfirmed}|{PasswordHash}|{PhoneNumber}|{PhoneNumberConfirmed}|{TwoFactorEnabled}|{Department}|{DOB}|{Active}|{ModifiedDt}|{CreateDt}|";
		}

		//Relation properties
		public ICollection<ApplicationUserClaim> Claims { get; set; }
		public string RoleBadges { get; set; }
	}
}
