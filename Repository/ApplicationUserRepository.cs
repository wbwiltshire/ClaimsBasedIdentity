using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using ClaimsBasedIdentity.Data.Interfaces;
using ClaimsBasedIdentity.Data.POCO;

namespace ClaimsBasedIdentity.Data.Repository
{
    public class ApplicationUserRepository : RepositoryBase<ApplicationUser>, IRepository<ApplicationUser>
    {
        public ApplicationUserRepository(AppSettingsConfiguration s, ILogger l, DBConnection d) : 
            base(s, l, d)
        {
            Initialize(l);
        }

        private ILogger logger;

        private void Initialize(ILogger l)
		{
            logger = l;
            OrderBy = "Id";
		}

        public override ICollection<ApplicationUser> FindAll()
        {
            return base.FindAll();
        }

        public object Add(ApplicationUser entity)
        {
            throw new NotImplementedException();
        }

        public int Delete(PrimaryKey pk)
        {
            throw new NotImplementedException();
        }

        public IPager<ApplicationUser> FindAll(IPager<ApplicationUser> pager)
        {
            throw new NotImplementedException();
        }

        public ApplicationUser FindByPK(IPrimaryKey pk)
        {
            throw new NotImplementedException();
        }

        public int Update(ApplicationUser entity)
        {
            throw new NotImplementedException();
        }
    }
}
