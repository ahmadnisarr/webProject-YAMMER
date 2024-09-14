using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain_Core;

namespace Application
{
    //Implement Bussiness Rule / USE CASES
    public class RepositoryService<TEntity> : IRepositoryService<TEntity>
    {
        private readonly IRepository<TEntity> repository;
        public RepositoryService(IRepository<TEntity> repository)
        {
           this.repository = repository;
        }
        public void Add(TEntity entity)
        {
           repository.Add(entity);
        }
        public void Update(TEntity entity)
        { repository.Update(entity); }      

        public void Delete(int id)
        { repository.Delete(id); }
        public IEnumerable<TEntity> GetAll()
        { return repository.GetAll(); }
        public TEntity GetById(int id)
        { return repository.GetById(id); }
        public int GetId(string userId)
        { return repository.GetId(userId); }
    }
}
