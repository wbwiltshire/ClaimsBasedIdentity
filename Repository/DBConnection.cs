using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Text;
using ClaimsBasedIdentity.Data.Interfaces;
using ClaimsBasedIdentity.Data.POCO;
using Newtonsoft.Json;

namespace ClaimsBasedIdentity.Data.Repository
{
    // A very primitive InMemory Data Store
    public class DBConnection : IDBConnection
    {
        private ICollection<ApplicationUser> users = null;
        private ICollection<ApplicationUserClaim> userClaims = null;
        private ICollection<ApplicationControllerSecurity> controllerSecurity = null;

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
            else if (typeof(TEntity) == typeof(ApplicationControllerSecurity))
                return (ICollection<TEntity>)controllerSecurity;
            else
                return null;
        }

        public object Select<TEntity>(IPrimaryKey key)
        {
            if (typeof(TEntity) == typeof(ApplicationUser))
                return users.FirstOrDefault(u => (int)u.PK.Key == (int)key.Key);
            else if (typeof(TEntity) == typeof(ApplicationUserClaim))
                return userClaims.FirstOrDefault(u => (int)u.PK.Key == (int)key.Key);
            else if (typeof(TEntity) == typeof(ApplicationControllerSecurity))
                return controllerSecurity.FirstOrDefault(u => (int)u.PK.Key == (int)key.Key);
            else
                return null;
        }

        public object Add<TEntity>(TEntity entity, PrimaryKey key)
        {
            int newId = -1;

            ApplicationUser user = null;
            ApplicationUserClaim userClaim = null;

            if (typeof(TEntity) == typeof(ApplicationUser))
            {
                if (users == null)
                    newId = 1;
                else
                    newId = users.Max(u => u.Id) + 1;

                user = entity as ApplicationUser;
                key.Key = newId;
                user.PK = key;
                user.ModifiedDt = DateTime.Now; user.CreateDt = DateTime.Now;
                users.Add(user);
            } else if (typeof(TEntity) == typeof(ApplicationUserClaim))
            {
                if (userClaims == null)
                    newId = 1;
                else
                    newId = userClaims.Max(u => u.Id) + 1;

                userClaim = entity as ApplicationUserClaim;
                key.Key = newId;
                userClaim.PK = key;
                userClaims.Add(userClaim);
            }

            return newId;
        }

        public int Update<TEntity>(TEntity entity, PrimaryKey key)
        {
            int rows = 0;
            ApplicationUser user = null;
            //ApplicationUserClaim userClaim = null;

            if (typeof(TEntity) == typeof(ApplicationUser))
            {
                user = users.FirstOrDefault(u => (int)u.PK.Key == (int)key.Key);

                //user = DeepCopy<ApplicationUser>(entity as ApplicationUser);
                user.PK = key;
                user.UserName = (entity as ApplicationUser).UserName;
                user.NormalizedUserName = (entity as ApplicationUser).UserName.ToUpper();
                user.Email = (entity as ApplicationUser).Email;
                user.NormalizedEmail = (entity as ApplicationUser).Email.ToUpper();
                user.EmailConfirmed = (entity as ApplicationUser).EmailConfirmed;
                user.PhoneNumber = (entity as ApplicationUser).PhoneNumber;
                user.PhoneNumberConfirmed = (entity as ApplicationUser).PhoneNumberConfirmed;
                user.PasswordHash = (entity as ApplicationUser).PasswordHash;
                user.TwoFactorEnabled = (entity as ApplicationUser).TwoFactorEnabled;
                user.Department = (entity as ApplicationUser).Department;
                user.DOB = (entity as ApplicationUser).DOB;
                user.Active = true; user.ModifiedDt = DateTime.Now; user.CreateDt = (entity as ApplicationUser).CreateDt;
                rows++;
            }

            return rows;
        }

        public void Load()
        {
            // Admin User
            users = new List<ApplicationUser>();
            users.Add(new ApplicationUser()
            {
                PK = new PrimaryKey() { Key = 1, IsIdentity = true },
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
                Active = true, ModifiedDt = DateTime.Now, CreateDt = DateTime.Now
            });
            
            #region User Claims
            userClaims = new List<ApplicationUserClaim>();
            userClaims.Add(new ApplicationUserClaim()
            {
                PK = new PrimaryKey() { Key = 1, IsIdentity = true },
                UserId = 1,
                ClaimType = ClaimTypes.Name,
                ClaimValue = "Administrator",
                ClaimIssuer = "Local Authority",
                Active = true,
                ModifiedDt = DateTime.Now,
                CreateDt = DateTime.Now
            });
            userClaims.Add(new ApplicationUserClaim()
            {
                PK = new PrimaryKey() { Key = 2, IsIdentity = true },
                UserId = 1,
                ClaimType = ClaimTypes.NameIdentifier,
                ClaimValue = "1",
                ClaimIssuer = "Local Authority",
                Active = true,
                ModifiedDt = DateTime.Now,
                CreateDt = DateTime.Now
            });
            userClaims.Add(new ApplicationUserClaim() {
                PK = new PrimaryKey() { Key = 3, IsIdentity = true },
                UserId = 1,
                ClaimType = ClaimTypes.Role,
                ClaimValue = "Administrator",
                ClaimIssuer = "Local Authority",
                Active = true, ModifiedDt = DateTime.Now, CreateDt = DateTime.Now
            });
            userClaims.Add(new ApplicationUserClaim()
            {
                PK = new PrimaryKey() { Key = 4, IsIdentity = true },
                UserId = 1,
                ClaimType = ClaimTypes.DateOfBirth,
                ClaimValue = "1900-01-01 00:00:00.000",
                ClaimIssuer = "Local Authority",
                Active = true, ModifiedDt = DateTime.Now, CreateDt = DateTime.Now
            });
            #endregion

			#region Controller Security
			controllerSecurity = new List<ApplicationControllerSecurity>();
            controllerSecurity.Add(new ApplicationControllerSecurity()
            {
                PK = new PrimaryKey() { Key = 1, IsIdentity = true },
                RoleName = "Administrator",
                ControllerName = "Secret",
                ActionName = "AdminSecret",
                Active = true, ModifiedDt = DateTime.Now, CreateDt = DateTime.Now
            });
            controllerSecurity.Add(new ApplicationControllerSecurity()
            {
                PK = new PrimaryKey() { Key = 2, IsIdentity = true },
                RoleName = "Administrator",
                ControllerName = "Account",
                ActionName = "Claims",
                Active = true,
                ModifiedDt = DateTime.Now,
                CreateDt = DateTime.Now
            });
            controllerSecurity.Add(new ApplicationControllerSecurity()
            {
                PK = new PrimaryKey() { Key = 3, IsIdentity = true },
                RoleName = "Administrator",
                ControllerName = "Account",
                ActionName = "Index",
                Active = true,
                ModifiedDt = DateTime.Now,
                CreateDt = DateTime.Now
            });
            controllerSecurity.Add(new ApplicationControllerSecurity()
            {
                PK = new PrimaryKey() { Key = 4, IsIdentity = true },
                RoleName = "Administrator",
                ControllerName = "Account",
                ActionName = "UserDetails",
                Active = true,
                ModifiedDt = DateTime.Now,
                CreateDt = DateTime.Now
            });
            controllerSecurity.Add(new ApplicationControllerSecurity()
            {
                PK = new PrimaryKey() { Key = 5, IsIdentity = true },
                RoleName = "Administrator",
                ControllerName = "Home",
                ActionName = "LoginSuccess",
                Active = true,
                ModifiedDt = DateTime.Now,
                CreateDt = DateTime.Now
            });
            controllerSecurity.Add(new ApplicationControllerSecurity()
            {
                PK = new PrimaryKey() { Key = 6, IsIdentity = true },
                RoleName = "Basic",
                ControllerName = "Secret",
                ActionName = "BasicSecret",
                Active = true,
                ModifiedDt = DateTime.Now,
                CreateDt = DateTime.Now
            });
            controllerSecurity.Add(new ApplicationControllerSecurity()
            {
                PK = new PrimaryKey() { Key = 7, IsIdentity = true },
                RoleName = "Administrator",
                ControllerName = "Account",
                ActionName = "EditUser",
                Active = true,
                ModifiedDt = DateTime.Now,
                CreateDt = DateTime.Now
            });
            #endregion

            //Roles
            ApplicationRole.Initialize();
        }

        // Helper methods
        private TEntity DeepCopy<TEntity>(TEntity source)
        {

            var DeserializeSettings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };

            return JsonConvert.DeserializeObject<TEntity>(JsonConvert.SerializeObject(source), DeserializeSettings);

        }

    }
}
