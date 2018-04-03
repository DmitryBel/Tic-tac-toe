using Gameserver.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gameserver.Core.BLL.I
{
    public interface IGameManager : IDisposable
    {
        IQueryable<games> GetGames();
        games GetGame(int id);
        games AddGame(string opponent, string mark);
        games SaveGame(games game, string winner);
        bool DeleteGame(int id);

        IQueryable<logs> GetLogs();
        List<logs> GetLogs(int gameID);
        logs GetLog(int id);
        logs AddLog(games game, int turn, int position, string player, string mark);
        bool DeleteLog(int id);

        GameResult CheckWinner(games game);
        Turn MoveComputer(games game);  
    }
}