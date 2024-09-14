using Application;
using Domain_Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class userProfileRepository : IUserProfile
    {
        private string connString;
        IRepository<userProfile> repo;
        public userProfileRepository(string connectionString, IRepository<userProfile> repo)
        {
            this.connString = connectionString;
            this.repo = repo;
        }
        public void Add(userProfile UserProfile)
        {
            if (UserProfile != null)
            {
                repo.Update(UserProfile);
            }
        }
        public int GetId(string userId)
        {
            return repo.GetId(userId);
        }
        public userProfile GetById(int id)
        {
            return repo.GetById(id);
        }
        public IEnumerable<userProfile> GetAll()
        {
            return repo.GetAll();
        }
        public void Update(userProfile entity)
        {
            repo.Update(entity);
        }
        public void Delete(int id)
        {
            repo.Delete(id);
        }
        public userProfile UserDeatils(int Id)
        {
            userProfile user = repo.GetById(Id);
            return user;
        }


    }
}
