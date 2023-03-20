using ShopApp.Business.Abstract;
using ShopApp.DataAccess.Abstract;
using ShopApp.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopApp.Business.Concrete
{
    public class UserManager : IUserService
    {
        private readonly IUserDal _userDal;

        public UserManager(IUserDal userDal)
        {
            _userDal = userDal;
        }

        public List<AppUser> GetWithRoleList()
        {
            return _userDal.GetWithRoleList().ToList();
            
        }

        public void Create(AppUser entity)
        {
            _userDal.Create(entity);
        }

        public void Delete(AppUser entity)
        {
            _userDal.Delete(entity);
        }

        public List<AppUser> GetALl()
        {
            return _userDal.GetAll().ToList();
        }

        public AppUser GetById(int id)
        {
            return _userDal.GetById(id);
        }

        public void Update(AppUser entity)
        {
            _userDal.Update(entity);
        }
    }
}
