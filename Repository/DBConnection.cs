using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using ClaimsBasedIdentity.Data.Interfaces;
using ClaimsBasedIdentity.Data.POCO;

namespace ClaimsBasedIdentity.Data.Repository
{
    public class DBConnection : IDBConnection
    {
        private ICollection<ApplicationUser> users = null;

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

        public ICollection<TEntity> Query<TEntity>()
        {
            if (typeof(TEntity) == typeof(ApplicationUser))
                return (ICollection<TEntity>)users;
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
        }
    }
}
