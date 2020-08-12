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
    public class ApplicationUserClaim
    {
        public PrimaryKey PK { get; set; }
        [Display(Name = "Claim Id")]
        public int Id
        {
            get { return (int)PK.CompositeKey[0]; }
            set { PK.CompositeKey[0] = (int)value; }
        }
        [Display(Name = "User Id")]
        public int UserId
        {
            get { return (int)PK.CompositeKey[1]; }
            set { PK.CompositeKey[1] = (int)value; }
        }
        public string Name { get; set; }
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
        public string ClaimIssuer { get; set; }
        public bool Active { get; set; }
        public DateTime ModifiedDt { get; set; }
        public DateTime CreateDt { get; set; }

        public ApplicationUserClaim()
        {
            PK = new PrimaryKey() { CompositeKey = new object[] { -1, -1 }, IsComposite = true };
        }

        public override string ToString()
        {
            return $"{Id}|{UserId}|{Name}|{ClaimType}|{ClaimValue}|{Active}|{ModifiedDt}|{CreateDt}|";
        }

        // Relationships
        public ApplicationUser ApplicationUser { get; set; }

    }
}
