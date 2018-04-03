using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Gameserver.Core
{
    public class Repository : IRepository
    {
        #region System
        private Model _db;
        public Model db
        {
            get
            {
                if (_db == null)
                {
                    _db = new Model();
                }
                return _db;
            }
            set
            {
                _db = value;
            }
        }


        private bool _disposed;

        public Repository()
        {
            this.db = new Model();
            _disposed = false;
        }
        public int Save()
        {
            return db.SaveChanges();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (db != null) db.Dispose();
                }
                db = null;
                _disposed = true;
            }
        }
        #endregion

        #region games
        public IQueryable<games> GetGames()
        {
            var res = db.games;
            return res;
        }

        public int SaveGame(games item, bool withSave = true)
        {
            if (item.id == 0)
            {
                db.games.Add(item);
            }
            else
            {
                db.Entry(item).State = EntityState.Modified;
            }
            if (withSave) Save();
            return item.id;
        }

        public bool DeleteGame(games item, bool withSave = true)
        {
            var res = false;
            if (item != null)
            {
                db.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                if (withSave) Save();
                res = true;
            }
            return res;
        }
        #endregion
        #region logs
        public IQueryable<logs> GetLogs()
        {
            var res = db.logs.Include(x => x.games);
            return res;
        }

        public int SaveLog(logs item, bool withSave = true)
        {
            if (item.id == 0)
            {
                db.logs.Add(item);
            }
            else
            {
                db.Entry(item).State = EntityState.Modified;
            }
            if (withSave) Save();
            return item.id;
        }

        public bool DeleteLog(logs item, bool withSave = true)
        {
            var res = false;
            if (item != null)
            {
                db.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                if (withSave) Save();
                res = true;
            }
            return res;
        }
        #endregion
    }
}