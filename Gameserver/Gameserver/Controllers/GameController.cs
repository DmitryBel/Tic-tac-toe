using Gameserver.Core;
using Gameserver.Hubs;
using Gameserver.Models;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Gameserver.Controllers
{
    public class GameController : Controller
    {
        public ActionResult GetGames()
        {
            var mng = new Manager();
            var items = mng.Game.GetGames();

            var res = items.OrderByDescending(x => x.created).Take(10).ToList().Select(x => new
            {
                id = x.id,
                created = x.created.GetValueOrDefault().ToString("dd/MM/yyyy"),
                opponent = x.opponent,
                winner = !string.IsNullOrWhiteSpace(x.winner) ? x.winner : "draw",
            }).ToList();

            return Json(res, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ShowGame(int gameID)
        {
            var mng = new Manager();
            var logs = mng.Game.GetLogs(gameID);

            var res = "";
            foreach (var log in logs)
            { 
                res += "Turn №" + log.turn + ", player: " + log.player + ", position: " + (log.position + 1) + ", mark: " + log.mark + ",\n";
            }
            if (!string.IsNullOrWhiteSpace(res)) 
            {
                res = res.Substring(0, res.Length - 2) + ".";
            }
            else
            {
                res = "No history found.";
            }
                
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult StartGame(string opponent, string mark)
        {
            var mng = new Manager();
            var game = mng.Game.AddGame(opponent, mark);

            var res = new
            {
                gameID = game.id,
            };
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        public ActionResult MovePlayer(int gameID, int position, string player)
        {
            var mng = new Manager();
            var game = mng.Game.GetGame(gameID);
            if (!string.IsNullOrEmpty(game.winner)) {
                return Json(new { }, JsonRequestBehavior.AllowGet);
            }
            var turns = mng.Game.GetLogs(game.id);
            var turnCount = turns.Count();
            var playerTwoMark = game.playerOneMark == "X" ? "O" : "X";
            if (turnCount > 0 && turns.OrderByDescending(x => x.turn).FirstOrDefault().player == player)
            {
                return Json(new {}, JsonRequestBehavior.AllowGet);
            }
            var mark = player == "player 1" ? game.playerOneMark : playerTwoMark;
            var item = mng.Game.AddLog(game, turnCount + 1, position, player, mark);
            var gameData = new GameData();
            var result = mng.Game.CheckWinner(game);
            switch(result.winner)
            {
                case "player 1":
                case "player 2":
                case "draw":
                    gameData = new GameData
                    {
                        gameID = game.id,
                        playerTurn = new Turn
                        {
                            mark = mark,
                            player = player,
                            position = position
                        },
                        gameResult = new GameResult
                        {
                            winner = result.winner,
                            winRow = result.winRow
                        }
                    };
                    break;
                default:
                    if (game.opponent == "computer")
                    {
                        var opponentTurn = mng.Game.MoveComputer(game);
                        result = mng.Game.CheckWinner(game);
                        switch (result.winner)
                        {
                            case "computer":
                            case "draw":
                                gameData = new GameData
                                {
                                    gameResult = new GameResult
                                    {
                                        winner = result.winner,
                                        winRow = result.winRow
                                    },
                                    opponentTurn = opponentTurn
                                };
                                break;
                            default:
                                gameData = new GameData
                                {
                                    opponentTurn = opponentTurn
                                };
                                break;
                        }
                    }
                    else {
                        gameData = new GameData
                        {
                            gameID = game.id,
                            playerTurn = new Turn
                            {
                                mark = mark,
                                player = player,
                                position = position
                            },
                        }; 
                    }                      
                    break;
            }

            if (game.opponent == "player") 
            {
                var hub = new SignalRHub();
                hub.Send(gameData);
            }
            
            var res = new
            {
                turnResult = gameData
            };
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ConnectToGame(int gameID)
        {
            var mng = new Manager();
            var game = mng.Game.GetGame(gameID);
            if (game == null || game.id == 0 || !string.IsNullOrEmpty(game.winner))
            {
                return Json( new { result = false }, JsonRequestBehavior.AllowGet);
            }
            var res = new
            {
                result = true,
                gameID = game.id,
                mark = game.playerOneMark == "X" ? "O" : "X"
            };
            var GameData = new GameData
            {
                gameID = game.id,
                playerConnected = true
            };
            var hub = new SignalRHub();
            hub.Send(GameData);
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public static int? StrToNullableInt(string input, int? defaultVal)
        {
            int res;
            if (!string.IsNullOrEmpty(input) && int.TryParse(input, out res))
                if (res > 0)
                    return res;
            return defaultVal;
        }
    }
}