
using ShopApp.DataAccess.Abstract;
using ShopApp.Entites;
using System.Data.Entity;
using System.Security.Cryptography.X509Certificates;

namespace ShopApp.DataAccess.Concrete.EfCore
{
    public class EfCoreUserDal : EfCoreGenericRepository<AppUser, ShopContext>, IUserDal
    {
        public List<AppUser> GetWithRoleList()
        {
            throw new NotImplementedException();
        }
    }
}
