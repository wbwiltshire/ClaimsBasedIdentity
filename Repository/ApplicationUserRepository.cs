using System;
using System.Collections.Generic;
using System.Linq;
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
            return base.Add(entity, new PrimaryKey() { Key = -1, IsIdentity = true });
        }

        public int Delete(PrimaryKey pk)
        {
            throw new NotImplementedException();
        }

        public IPager<ApplicationUser> FindAll(IPager<ApplicationUser> pager)
        {
            pager.RowCount = base.FindAllCount();
            pager.Entities = base.FindAllPaged(pager.PageNbr, pager.PageSize);
            return pager;
        }

        public override ApplicationUser FindByPK(IPrimaryKey pk)
        {
            return base.FindByPK(pk);
        }

        public ApplicationUser FindByPKView(IPrimaryKey pk)
        {
            ApplicationUserClaimRepository userClaimRepo = new ApplicationUserClaimRepository(Settings, logger, Connection);

            ApplicationUser user = base.FindByPK(pk);
            user.Claims = userClaimRepo.FindAll().Where(uc => uc.UserId == user.Id).ToList();

            return user;
        }

        public int Update(ApplicationUser entity)
        {
            return base.Update(entity, entity.PK);
        }
    }
}
