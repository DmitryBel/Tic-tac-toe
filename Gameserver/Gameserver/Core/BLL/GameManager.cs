using Gameserver.Core.BLL.I;
using Gameserver.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gameserver.Core.BLL
{
    public class GameManager : IGameManager
    {
        #region System
        public IRepository db;
        public IManager mng;
        private List<string> _opponents = new List<string> { "player", "computer" };
        private List<string> _players = new List<string> { "player 1", "player 2", "computer" };
        private List<string> _marks = new List<string> { "X", "O" };
        private string[] _field = new string[9];
        private int _turn = new int();
        private bool _disposed;

        public GameManager(IRepository db, IManager mng)
        {
            this.db = db;
            this.mng = mng;
            _disposed = false;
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

                }
                _disposed = true;
            }
        }
        #endregion

        #region game
        public IQueryable<games> GetGames()
        {
            var res = db.GetGames().OrderBy(x => x.created);
            return res;
        }
        public games GetGame(int id)
        {
            var res = new games();
            msg = string.Empty;
            try
            {
                res = GetGames().FirstOrDefault(x => x.id == id);
            }
            catch (Exception ex)
            {

            }
            return res;
        }
        public games AddGame(string opponent, string mark)
        {
            var res = new games();
            try
            {
                if (!_opponents.Contains(opponent)) {
                    return res;
                }
                if (!_marks.Contains(mark)) {
                    return res;
                }

                res = new games
                {
                    id = 0,
                    opponent = opponent,
                    created = DateTime.Now,
                    playerOneMark = mark
                };
                db.SaveGame(res);
            }
            catch (Exception ex)
            {
            }
            return res;
        }
        public games SaveGame(games game, string winner)
        {
            var res = new games();
            try
            {
                if (game == null || game.id == 0)
                {
                    return res;
                }
                if (!_players.Contains(winner) || (winner == "computer" && game.opponent == "player") || (winner == "player 2" && game.opponent == "computer")) 
                {
                    if (winner != "draw") 
                    {
                        return res;
                    }
                    
                }

                game.winner = winner;
                db.SaveGame(game);
                res = game;
            }
            catch (Exception ex)
            {
            }
            return res;
        }
        public bool DeleteGame(int id)
        {
            var res = false;
            try
            {
                var item = GetGame(id);
                if (item == null || item.id == 0)
                {
                    return res;
                }
                var logs = GetLogs().Where(x => x.gameID == item.id).ToList();
                foreach (var log in logs)
                {
                    db.DeleteLog(log, false);
                }

                res = db.DeleteGame(item);
            }
            catch (Exception ex)
            {
            }
            return res;
        }
        #endregion
        #region logs
        public IQueryable<logs> GetLogs()
        {
            var res = db.GetLogs();
            return res;
        }
        public List<logs> GetLogs(int gameID)
        {
            var res = GetLogs().Where(x => x.gameID == gameID).OrderBy(x => x.turn).ToList();
            return res;
        }
        public logs GetLog(int id)
        {
            var res = new logs();
            try
            {
                res = GetLogs().FirstOrDefault(x => x.id == id);
            }
            catch (Exception ex)
            {
            }
            return res;
        }
        public logs AddLog(games game, int turn, int position, string player, string mark)
        {
            var res = new logs();
            try
            {
                if (game == null || game.id == 0)
                {
                    return res;
                }
                if (!_players.Contains(player))
                {
                    return res;
                }
                if (!_marks.Contains(mark))
                {
                    return res;
                }
                if (turn < 1 || 9 < turn)
                {
                    return res;
                }
                if (position < 0 || 8 < position)
                {
                    return res;
                }

                res = new logs
                {
                    id = 0,
                    gameID = game.id,
                    player = player,
                    mark = mark.ToString(),
                    turn = turn,
                    position = position
                };
                db.SaveLog(res);
            }
            catch (Exception ex)
            {
            }
            return res;
        }
        public bool DeleteLog(int id)
        {
            var res = false;
            try
            {
                var item = GetLog(id);
                if (item == null || item.id == 0)
                {
                    return res;
                }
                res = db.DeleteLog(item);
            }
            catch (Exception ex)
            {
            }
            return res;
        }
        #endregion

        public GameResult CheckWinner(games game)
        {
            var res = new GameResult();
            try
            {
                if (game == null || game.id == 0)
                {
                    return res;
                }

                var items = GetLogs(game.id);
                foreach (var item in items) 
                {
                    if (item.position.HasValue)
                    {
                        _field[item.position.Value] = item.mark; 
                    }
                }
                _turn = items.Count();

                if (_turn < 3) 
                {
                    return res;
                }
                if (_field[0] != null && _field[1] != null && _field[2] != null && _field[0] == _field[1] && _field[0] == _field[2])                //Horizontal                                                                             
                {
                    res.winner = items.FirstOrDefault(x => x.position == 0).player;
                    res.winRow = "0,1,2";
                }
                if (_field[3] != null && _field[4] != null && _field[5] != null && _field[3] == _field[4] && _field[3] == _field[5]) 
                { 
                    res.winner = items.FirstOrDefault(x => x.position == 3).player;
                    res.winRow = "3,4,5";
                }
                if (_field[6] != null && _field[7] != null && _field[8] != null && _field[6] == _field[7] && _field[6] == _field[8])
                {
                    res.winner = items.FirstOrDefault(x => x.position == 6).player;
                    res.winRow = "6,7,8";
                }

                if (_field[0] != null && _field[3] != null && _field[6] != null && _field[0] == _field[3] && _field[0] == _field[6])                //Vertical
                {
                    res.winner = items.FirstOrDefault(x => x.position == 0).player;
                    res.winRow = "0,3,6";
                }
                if (_field[1] != null && _field[4] != null && _field[7] != null && _field[1] == _field[4] && _field[1] == _field[7])
                {
                    res.winner = items.FirstOrDefault(x => x.position == 1).player;
                    res.winRow = "1,4,7";
                }
                if (_field[2] != null && _field[5] != null && _field[8] != null && _field[2] == _field[5] && _field[2] == _field[8]) 
                {
                    res.winner = items.FirstOrDefault(x => x.position == 2).player;
                    res.winRow = "2,5,8";
                }

                if (_field[0] != null && _field[4] != null && _field[8] != null && _field[0] == _field[4] && _field[0] == _field[8])                //Cross
                {
                    res.winner = items.FirstOrDefault(x => x.position == 0).player;
                    res.winRow = "0,4,8";
                }
                if (_field[2] != null && _field[4] != null && _field[6] != null && _field[2] == _field[4] && _field[2] == _field[6])
                {
                    res.winner = items.FirstOrDefault(x => x.position == 2).player;
                    res.winRow = "2,4,6";
                }

                if (!string.IsNullOrEmpty(res.winner))
                {
                    SaveGame(game, res.winner);
                }
                else 
                {
                    if (_turn == 9)
                    {
                        SaveGame(game, "draw");
                        res.winner = "draw";
                    }
                }            
            }
            catch (Exception ex)
            {
            }
            return res;
        }
        public Turn MoveComputer(games game)
        {
            var res = new Turn();
            try
            {
                if (game == null || game.id == 0)
                {
                    return res;
                }
                
                var computerMark = game.playerOneMark == "X" ? "O" : "X";
                var position = -1;

                #region win
                for (var i = 0; i <= 2; i++)
                {
                    if (_field[i * 3] == computerMark && _field[i * 3] == _field[i * 3 + 1] && string.IsNullOrEmpty(_field[i * 3 + 2])) position = i * 3 + 2;                    //Horizontal
                    if (_field[i * 3] == computerMark && _field[i * 3] == _field[i * 3 + 2] && string.IsNullOrEmpty(_field[i * 3 + 1])) position = i * 3 + 1;
                    if (_field[i * 3 + 1] == computerMark && _field[i * 3 + 1] == _field[i * 3 + 2] && string.IsNullOrEmpty(_field[i * 3])) position = i * 3;

                    if (_field[i] == computerMark && _field[i] == _field[i + 3] && string.IsNullOrEmpty(_field[i + 6])) position = i + 6;                    //Vertical
                    if (_field[i] == computerMark && _field[i] == _field[i + 6] && string.IsNullOrEmpty(_field[i + 3])) position = i + 3;
                    if (_field[i + 3] == computerMark && _field[i + 3] == _field[i + 6] && string.IsNullOrEmpty(_field[i])) position = i;
                }
                if (_field[0] == computerMark && _field[0] == _field[4] && string.IsNullOrEmpty(_field[8])) position = 8;                                    //Cross
                if (_field[0] == computerMark && _field[0] == _field[8] && string.IsNullOrEmpty(_field[4])) position = 4;
                if (_field[4] == computerMark && _field[4] == _field[8] && string.IsNullOrEmpty(_field[0])) position = 0;

                if (_field[2] == computerMark && _field[2] == _field[4] && string.IsNullOrEmpty(_field[6])) position = 6;                                    
                if (_field[2] == computerMark && _field[2] == _field[6] && string.IsNullOrEmpty(_field[4])) position = 4;
                if (_field[4] == computerMark && _field[4] == _field[6] && string.IsNullOrEmpty(_field[2])) position = 2;
                #endregion
                #region not lose
                if (position == -1)
                {          
                    for (var i = 0; i <= 2; i++)                                        
                    {
                        if (_field[i * 3] == game.playerOneMark && _field[i * 3] == _field[i * 3 + 1] && string.IsNullOrEmpty(_field[i * 3 + 2])) position = i * 3 + 2;          //Horizontal
                        if (_field[i * 3] == game.playerOneMark && _field[i * 3] == _field[i * 3 + 2] && string.IsNullOrEmpty(_field[i * 3 + 1])) position = i * 3 + 1;
                        if (_field[i * 3 + 1] == game.playerOneMark && _field[i * 3 + 1] == _field[i * 3 + 2] && string.IsNullOrEmpty(_field[i * 3])) position = i * 3;

                        if (_field[i] == game.playerOneMark && _field[i] == _field[i + 3] && string.IsNullOrEmpty(_field[i + 6])) position = i + 6;          //Vertical
                        if (_field[i] == game.playerOneMark && _field[i] == _field[i + 6] && string.IsNullOrEmpty(_field[i + 3])) position = i + 3;
                        if (_field[i + 3] == game.playerOneMark && _field[i + 3] == _field[i + 6] && string.IsNullOrEmpty(_field[i])) position = i;
                    }

                    if (_field[0] == game.playerOneMark && _field[0] == _field[4] && string.IsNullOrEmpty(_field[8])) position = 8;                          //Cross
                    if (_field[0] == game.playerOneMark && _field[0] == _field[8] && string.IsNullOrEmpty(_field[4])) position = 4;
                    if (_field[4] == game.playerOneMark && _field[4] == _field[8] && string.IsNullOrEmpty(_field[0])) position = 0;

                    if (_field[2] == game.playerOneMark && _field[2] == _field[4] && string.IsNullOrEmpty(_field[6])) position = 6;                         
                    if (_field[2] == game.playerOneMark && _field[2] == _field[6] && string.IsNullOrEmpty(_field[4])) position = 4;
                    if (_field[4] == game.playerOneMark && _field[4] == _field[6] && string.IsNullOrEmpty(_field[2])) position = 2;
                }
                #endregion
                #region free turn
                if (position == -1)
                {
                    var freePositions = new List<int>();

                    for (var i = 0; i <= 8; i++) {
                        if (string.IsNullOrEmpty(_field[i])) 
                        {
                            freePositions.Add(i);
                        }
                    }
                    if (freePositions.Contains(4)) position = 4;
                    else if (freePositions.Contains(0)) position = 0;
                    else if (freePositions.Contains(2)) position = 2;
                    else if (freePositions.Contains(6)) position = 6;
                    else if (freePositions.Contains(8)) position = 8;
                    else if (freePositions.Contains(1)) position = 1;
                    else if (freePositions.Contains(3)) position = 3;
                    else if (freePositions.Contains(5)) position = 5;
                    else if (freePositions.Contains(7)) position = 7;
                }
                #endregion

                _field[position] = computerMark;
                AddLog(game, _turn + 1, position, "computer", computerMark);

                res = new Turn
                {
                    player = "computer",
                    mark = computerMark,
                    position = position
                };
            }
            catch (Exception ex)
            {
            }
            return res;
        }
    }
}