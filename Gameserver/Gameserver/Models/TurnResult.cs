using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gameserver.Models
{
    public class GameData
    {
        public int gameID { get; set; }
        public bool playerConnected { get; set; }
        public Turn playerTurn { get; set; }
        public GameResult gameResult { get; set; }
        public Turn opponentTurn { get; set; }
    }
}