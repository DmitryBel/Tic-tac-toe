using Gameserver.Core.BLL.I;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gameserver.Core
{
    public interface IManager : IDisposable
    {
        IGameManager Game { get; set; }
    }
}