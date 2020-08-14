using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text;
using Microsoft.Extensions.Logging;
using ClaimsBasedIdentity.Data;
using ClaimsBasedIdentity.Data.Interfaces;
using System.Globalization;

namespace ClaimsBasedIdentity.Data.POCO
{
    public class ApplicationUserClaim
    {
        public PrimaryKey PK { get; set; }
        public int Id
        {
            get { return (int)PK.Key; }
            set { PK.Key = (int)value; }
        }
        public int UserId { get; set; }
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
        public string ClaimIssuer { get; set; }
        public bool Active { get; set; }
        public DateTime ModifiedDt { get; set; }
        public DateTime CreateDt { get; set; }

        public ApplicationUserClaim()
        {
            PK = new PrimaryKey() { Key = -1, IsIdentity = true };
        }

        public override string ToString()
        {
            return $"{Id}|{UserId}|{ClaimType}|{ClaimValue}|{Active}|{ModifiedDt}|{CreateDt}|";
        }

        // Relationships
        public ApplicationUser ApplicationUser { get; set; }

    }
}
