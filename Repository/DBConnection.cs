using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Claims;
using System.Text;
using ClaimsBasedIdentity.Data.Interfaces;
using ClaimsBasedIdentity.Data.POCO;

namespace ClaimsBasedIdentity.Data.Repository
{
    public class DBConnection : IDBConnection
    {
        private ICollection<ApplicationUser> users = null;
        private ICollection<ApplicationUserClaim> userClaims = null;
        public DBConnection()
        {
            IsOpen = false;
            Load();
        }

        public bool Open()
        {
            IsOpen = true;
            return IsOpen;
        }

        public void Close()
        {
            IsOpen = false;
        }

        public bool IsOpen { get; set; }

        public ICollection<TEntity> Select<TEntity>()
        {
            if (typeof(TEntity) == typeof(ApplicationUser))
                return (ICollection<TEntity>)users;
            else if (typeof(TEntity) == typeof(ApplicationUserClaim))
                return (ICollection<TEntity>)userClaims;
            else
                return null;
        }

        public void Load()
        {
            users = new List<ApplicationUser>();
            users.Add(new ApplicationUser()
            {
                PK = new PrimaryKey() { Key = 1 },
                UserName = "Admin",
                NormalizedUserName = "Admin".ToUpper(),
                Email = "Admin@SeagullConsulting.Biz",
                NormalizedEmail = "Admin@SeagullConsulting.Biz".ToUpper(),
                EmailConfirmed = true,
                PhoneNumber = "(999) 555-1212",
                PhoneNumberConfirmed = true,
                PasswordHash = "AQAAAAEAACcQAAAAEJQpOCvbWO6rnKOkiqFifxSkquwfZZzF/W3jTX1pKtPDhAw2tDfxRbFLCWaCrmtJjA==",
                TwoFactorEnabled = false,
                Department = String.Empty,
                DOB = new DateTime(1900, 1, 1),
                Active = true,
                ModifiedDt = DateTime.Now,
                CreateDt = DateTime.Now
            });

            userClaims = new List<ApplicationUserClaim>();
            userClaims.Add(new ApplicationUserClaim() { 
                Id = 1,
                UserId = 1,
                Name = "Role",
                ClaimType = ClaimTypes.Role,
                ClaimValue = "Administrator",
                ClaimIssuer = "Local Authority",
                Active = true,
                ModifiedDt = DateTime.Now,
                CreateDt = DateTime.Now
            });
            userClaims.Add(new ApplicationUserClaim()
            {
                Id = 2,
                UserId = 1,
                Name = "DOB",
                ClaimType = ClaimTypes.DateOfBirth,
                ClaimValue = "1900-01-01 00:00:00.000",
                ClaimIssuer = "Local Authority",
                Active = true,
                ModifiedDt = DateTime.Now,
                CreateDt = DateTime.Now
            });

        }
    }
}
