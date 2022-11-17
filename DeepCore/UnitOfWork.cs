using System;  
using UnitODB;
using UnitODB.Data;
using UnitODB.Linq;

namespace DeepCore
{   
    public class UnitOfWork : OdbContainer
    {
        private IRepository<DeepLap> cars;
        public IRepository<DeepLap> Cars
        {
            get
            {
                if (this.cars == null)
                {
                    this.cars = this.CreateRepository<DeepLap>();
                }

                return this.cars;
            }
        }
        public UnitOfWork(IDbContext context) : base(context)
        {
            this.Context = context;
        } 

        public IRepository<T> CreateRepository<T>() where T : IEntity
        {
            var repo = new OdbRepository<T>(this);

            repo.SetDepth(2);

            return repo;
        }
    }
}
