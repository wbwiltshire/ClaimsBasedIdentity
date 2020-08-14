using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using ClaimsBasedIdentity.Data.Interfaces;
using ClaimsBasedIdentity.Data.POCO;

namespace ClaimsBasedIdentity.Data.Repository
{
    public class ApplicationControllerSecurityRepository : RepositoryBase<ApplicationControllerSecurity>, IRepository<ApplicationControllerSecurity>
    {
        public ApplicationControllerSecurityRepository(AppSettingsConfiguration s, ILogger l, DBConnection d) : 
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

        public override ICollection<ApplicationControllerSecurity> FindAll()
        {
            return base.FindAll();
        }

        public object Add(ApplicationControllerSecurity entity)
        {
            throw new NotImplementedException();
        }

        public int Delete(PrimaryKey pk)
        {
            throw new NotImplementedException();
        }

        public IPager<ApplicationControllerSecurity> FindAll(IPager<ApplicationControllerSecurity> pager)
        {
            throw new NotImplementedException();
        }

        public ApplicationControllerSecurity FindByPK(IPrimaryKey pk)
        {
            throw new NotImplementedException();
        }

        public int Update(ApplicationControllerSecurity entity)
        {
            throw new NotImplementedException();
        }
    }
}
