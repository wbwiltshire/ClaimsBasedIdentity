using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text;
using Microsoft.Extensions.Logging;
using ClaimsBasedIdentity.Data;
using ClaimsBasedIdentity.Data.Interfaces;
using HashidsNet;

namespace ClaimsBasedIdentity.Data.POCO
{
	public class ApplicationUser
	{
		private IHashids hashIds;

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

		// Required by MVC Controllers for Model Binding in HTTP Post requests
		public ApplicationUser()
		{
			PK = new PrimaryKey() { Key = -1, IsIdentity = true };
		}

		// Use this everywhere else
		public ApplicationUser(IHashids h)
		{
			hashIds = h;
			PK = new PrimaryKey() { Key = -1, IsIdentity = true };
		}

		public string HashId 
		{ 
			get { return hashIds.Encode(Id); }
        }

		public override string ToString()
		{
			return $"{Id}|{HashId}|{UserName}|{NormalizedUserName}|{Email}|{NormalizedEmail}|{EmailConfirmed}|{PasswordHash}|{PhoneNumber}|{PhoneNumberConfirmed}|{TwoFactorEnabled}|{Department}|{DOB}|{Active}|{ModifiedDt}|{CreateDt}|";
		}

		//Relation properties
		public ICollection<ApplicationUserClaim> Claims { get; set; }
		public string RoleBadges { get; set; }
	}
}
