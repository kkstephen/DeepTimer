using System;  
using UnitODB;
using UnitODB.Data;
using UnitODB.Linq;

namespace DeepCore
{
    public class UnitOfWork : OdbContainer
    {
        private IRepository<DeepLap> laps;
        public IRepository<DeepLap> Laps
        {
            get
            {
                if (this.laps == null)
                {
                    this.laps = this.CreateRepository<DeepLap>();
                }

                return this.laps;
            }
        }

        private IRepository<Team> teams;
        public IRepository<Team> Teams
        {
            get
            {
                if (this.teams == null)
                {
                    this.teams = this.CreateRepository<Team>();
                }

                return this.teams;
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
