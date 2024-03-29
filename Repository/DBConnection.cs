﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Text;
using ClaimsBasedIdentity.Data.Interfaces;
using ClaimsBasedIdentity.Data.POCO;
using Newtonsoft.Json;
using HashidsNet;

namespace ClaimsBasedIdentity.Data.Repository
{
    // A very primitive InMemory Data Store
    public class DBConnection : IDBConnection
    {
        private ICollection<ApplicationUser> users = null;
        private ICollection<ApplicationUserClaim> userClaims = null;
        private ICollection<ApplicationControllerSecurity> controllerSecurity = null;
        private ICollection<ApplicationRole> roles { get; set; }
        private ICollection<ApplicationAuditLog> logs { get; set; }
        private IHashids hashIds { get; set; }

        public DBConnection(IHashids h)
        {
            hashIds = h;
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
            else if (typeof(TEntity) == typeof(ApplicationRole))
                return (ICollection<TEntity>)roles;
            else if (typeof(TEntity) == typeof(ApplicationAuditLog))
                return (ICollection<TEntity>)logs;
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
            ApplicationAuditLog auditLog = null;

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
                userClaim.ModifiedDt = DateTime.Now; userClaim.CreateDt = DateTime.Now;
                userClaims.Add(userClaim);
            }
            else if (typeof(TEntity) == typeof(ApplicationAuditLog))
            {
                if (logs.Count == 0)
                    newId = 1;
                else
                    newId = logs.Max(u => u.Id) + 1;

                auditLog = entity as ApplicationAuditLog;
                key.Key = newId;
                auditLog.PK = key;
                auditLog.CreateDt = DateTime.Now;
                logs.Add(auditLog);
            }

            return newId;
        }

        public int Update<TEntity>(TEntity entity, PrimaryKey key)
        {
            int rows = 0;
            ApplicationUser user = null;
            ApplicationUserClaim userClaim = null;

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
            else if (typeof(TEntity) == typeof(ApplicationUserClaim))
            {
                userClaim = userClaims.FirstOrDefault(u => (int)u.PK.Key == (int)key.Key);

                //user = DeepCopy<ApplicationUser>(entity as ApplicationUser);
                userClaim.PK = key;
                userClaim.ClaimType = (entity as ApplicationUserClaim).ClaimType;
                userClaim.ClaimValue = (entity as ApplicationUserClaim).ClaimValue;
                userClaim.ClaimIssuer = (entity as ApplicationUserClaim).ClaimIssuer;
                userClaim.Active = true; userClaim.ModifiedDt = DateTime.Now; userClaim.CreateDt = (entity as ApplicationUserClaim).CreateDt;
                rows++;
            }

            return rows;
        }

        public int Delete<TEntity>(IPrimaryKey key)
        {
            int rows = 0;
            ApplicationUser user = null;
            ApplicationUserClaim userClaim = null;

            if (typeof(TEntity) == typeof(ApplicationUser))
            {
                user = users.FirstOrDefault(u => u.Id == (int)key.Key);
                users.Remove(user);
                rows = 1;
            }
            else if (typeof(TEntity) == typeof(ApplicationUserClaim))
			{
                userClaim = userClaims.FirstOrDefault(u => u.Id == (int)key.Key);
                userClaims.Remove(userClaim);
                rows = 1;
            }

            return rows;
        }

        public void Load()
        {
            // Admin User
            users = new List<ApplicationUser>();
            users.Add(new ApplicationUser(hashIds)
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

            // ClaimType: User Name
            userClaims.Add(new ApplicationUserClaim()
            {
                PK = new PrimaryKey() { Key = 1, IsIdentity = true },
                UserId = 1,
                ClaimType = ClaimTypes.Name,
                ClaimValue = "Admin",
                ClaimIssuer = "Local Authority",
                Active = true,
                ModifiedDt = DateTime.Now,
                CreateDt = DateTime.Now
            });

            // ClaimType: Name Id - should be Hash Id based on User Id
            userClaims.Add(new ApplicationUserClaim()
            {
                PK = new PrimaryKey() { Key = 2, IsIdentity = true },
                UserId = 1,
                ClaimType = ClaimTypes.NameIdentifier,
                ClaimValue = hashIds.Encode(1),
                ClaimIssuer = "Local Authority",
                Active = true,
                ModifiedDt = DateTime.Now,
                CreateDt = DateTime.Now
            });

            // ClaimType: User Role
            userClaims.Add(new ApplicationUserClaim() {
                PK = new PrimaryKey() { Key = 3, IsIdentity = true },
                UserId = 1,
                ClaimType = ClaimTypes.Role,
                ClaimValue = "Administrator",
                ClaimIssuer = "Local Authority",
                Active = true, ModifiedDt = DateTime.Now, CreateDt = DateTime.Now
            });

            // ClaimType: User DOB 
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
			controllerSecurity = new List<ApplicationControllerSecurity>() {
                new ApplicationControllerSecurity() {
                    PK = new PrimaryKey() { Key = 1, IsIdentity = true },
                    RoleName = "Administrator",
                    ControllerName = "Secret",
                    ActionName = "AdminSecret",
                    Active = true, ModifiedDt = DateTime.Now, CreateDt = DateTime.Now
                },
                new ApplicationControllerSecurity() {
                    PK = new PrimaryKey() { Key = 2, IsIdentity = true },
                    RoleName = "Administrator",
                    ControllerName = "Account",
                    ActionName = "Claims",
                    Active = true,
                    ModifiedDt = DateTime.Now,
                    CreateDt = DateTime.Now
                },
                new ApplicationControllerSecurity() {
                    PK = new PrimaryKey() { Key = 3, IsIdentity = true },
                    RoleName = "Administrator",
                    ControllerName = "Account",
                    ActionName = "Index",
                    Active = true,
                    ModifiedDt = DateTime.Now,
                    CreateDt = DateTime.Now
                },
                new ApplicationControllerSecurity() {
                    PK = new PrimaryKey() { Key = 4, IsIdentity = true },
                    RoleName = "Administrator",
                    ControllerName = "Account",
                    ActionName = "UserDetails",
                    Active = true,
                    ModifiedDt = DateTime.Now,
                    CreateDt = DateTime.Now
                },
                new ApplicationControllerSecurity() {
                    PK = new PrimaryKey() { Key = 5, IsIdentity = true },
                    RoleName = "Administrator",
                    ControllerName = "Home",
                    ActionName = "LoginSuccess",
                    Active = true,
                    ModifiedDt = DateTime.Now,
                    CreateDt = DateTime.Now
                },
                new ApplicationControllerSecurity() {
                    PK = new PrimaryKey() { Key = 6, IsIdentity = true },
                    RoleName = "Basic",
                    ControllerName = "Secret",
                    ActionName = "BasicSecret",
                    Active = true,
                    ModifiedDt = DateTime.Now,
                    CreateDt = DateTime.Now
                },
                new ApplicationControllerSecurity() {
                    PK = new PrimaryKey() { Key = 7, IsIdentity = true },
                    RoleName = "Administrator",
                    ControllerName = "Account",
                    ActionName = "EditUser",
                    Active = true,
                    ModifiedDt = DateTime.Now,
                    CreateDt = DateTime.Now
                },
                new ApplicationControllerSecurity() {
                    PK = new PrimaryKey() { Key = 8, IsIdentity = true },
                    RoleName = "Manager",
                    ControllerName = "Secret",
                    ActionName = "ManagerSecret",
                    Active = true,
                    ModifiedDt = DateTime.Now,
                    CreateDt = DateTime.Now
                },
                new ApplicationControllerSecurity() {
                    PK = new PrimaryKey() { Key = 9, IsIdentity = true },
                    RoleName = "Administrator",
                    ControllerName = "Account",
                    ActionName = "ResetPassword",
                    Active = true,
                    ModifiedDt = DateTime.Now,
                    CreateDt = DateTime.Now
                }
            };
            #endregion

            // Roles            
            roles = new List<ApplicationRole>() {
                new ApplicationRole() { Id = 1, Name = "Administrator", ClaimType= ClaimTypes.Role },
                new ApplicationRole() { Id = 2, Name = "Manager", ClaimType= ClaimTypes.Role },
                new ApplicationRole() { Id = 3, Name = "Basic", ClaimType= ClaimTypes.Role }
            };

            // Logs
            logs = new List<ApplicationAuditLog>();

        }

    // Helper methods
    private TEntity DeepCopy<TEntity>(TEntity source)
        {

            var DeserializeSettings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };

            return JsonConvert.DeserializeObject<TEntity>(JsonConvert.SerializeObject(source), DeserializeSettings);

        }

    }
}
