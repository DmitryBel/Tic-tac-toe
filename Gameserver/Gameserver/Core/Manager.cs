using Gameserver.Core.BLL;
using Gameserver.Core.BLL.I;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gameserver.Core
{
    public class Manager : IManager
    {
        #region System
        public Manager()
        {
            this.db = new Repository();
            _disposed = false;


        }

        private IRepository db { set; get; }

        private void Save()
        {
            db.Save();
        }
        private bool _disposed = false;
        public virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    db.Dispose();
                }
                this._disposed = true;
            }
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region Managers
        private IGameManager _game;
        public IGameManager Game
        {
            get
            {
                if (_game == null)
                {
                    _game = new GameManager(db, (IManager)this);
                }
                return _game;
            }
            set
            {
                _game = value;
            }
        }

        #endregion
    }
}