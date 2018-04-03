using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gameserver.Core
{
    public interface IRepository : IDisposable
    {
        #region System
        int Save();
        #endregion

        #region games
        IQueryable<games> GetGames();
        int SaveGame(games item, bool withSave = true);
        bool DeleteGame(games item, bool withSave = true);
        #endregion    
        #region logs
        IQueryable<logs> GetLogs();
        int SaveLog(logs item, bool withSave = true);
        bool DeleteLog(logs item, bool withSave = true);
        #endregion 
    }
}