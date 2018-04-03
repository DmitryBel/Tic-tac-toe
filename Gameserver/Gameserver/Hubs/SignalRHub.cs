using Gameserver.Models;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Gameserver.Hubs
{
    public class SignalRHub : Hub
    {
        public Task Send(GameData turnResult)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<SignalRHub>();
            return context.Clients.All.broadcastMessage(turnResult);
            //return context.Clients.All.InvokeAsync("ON_MESSAGE_SENT", "Message from the server");
        }
    }
}