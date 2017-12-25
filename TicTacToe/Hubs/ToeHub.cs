using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TicTacToe.Models
{
    public class ToeHub : Hub
    {
        static List<Room> Rooms = new List<Room>();
        static List<User> Users = new List<User>();

        public void Hello()
        {
            Clients.All.hello();
        }

        public void Connect(string userName)
        {
            var id = Context.ConnectionId;

            if (!Users.Any(x => x.UserId == id))
            {
                Users.Add(new User { UserId = id, Name = userName });
                Clients.Caller.onConnected(id, userName, Users);
            }
        }

        public void UpdateUsers()
        {
            var id = Context.ConnectionId;
            var user = Users.FirstOrDefault(x => x.UserId == id);
            Clients.All.UpdateUsers(Users);
        }

        public void SendInvite(string toid, string fromid)
        {
            var id = Context.ConnectionId;
            var user = Users.FirstOrDefault(x => x.UserId == fromid);
            Clients.Client(toid).Invite(user);
        }

        public void IsLogged()
        {
            var id = Context.ConnectionId;
            if (id.Length > 0)
            {
                Clients.Client(id).UpdateUsers(Users);
            }
        }

        public void Nextstep(string player, string pos)
        {
            var sender = Users.FirstOrDefault(x => x.UserId == player);
            var room = Rooms.FirstOrDefault(x => x.player2 == sender || x.player1 == sender);
            var d = pos.Split('p');
            var position = Convert.ToInt32(d[1]) - 1;
            if (room != null && room.curPlayer == sender && room.field[position] == 7)
            {
                if (room.isX)
                    room.field[position] = 1;
                else
                    room.field[position] = 0;

                if ((room.field[0] == room.field[1] && room.field[0] == room.field[2] && room.field[0] != 7) ||
                   (room.field[3] == room.field[4] && room.field[3] == room.field[5] && room.field[3] != 7) ||
                   (room.field[6] == room.field[7] && room.field[6] == room.field[8] && room.field[6] != 7) ||
                   (room.field[0] == room.field[3] && room.field[0] == room.field[6] && room.field[0] != 7) ||
                   (room.field[1] == room.field[4] && room.field[1] == room.field[7] && room.field[1] != 7) ||
                   (room.field[2] == room.field[5] && room.field[2] == room.field[8] && room.field[2] != 7) ||
                   (room.field[0] == room.field[4] && room.field[0] == room.field[8] && room.field[0] != 7) ||
                   (room.field[2] == room.field[4] && room.field[2] == room.field[6] && room.field[2] != 7))
                {
                    Clients.Client(room.player1.UserId).STEP(pos, room.field[position]);
                    Clients.Client(room.player2.UserId).STEP(pos, room.field[position]);
                    Clients.Client(room.player1.UserId).WINNER(room.curPlayer);
                    Clients.Client(room.player2.UserId).WINNER(room.curPlayer);
                    Rooms.Remove(room);
                }
                if (!room.field.Contains(7))
                {
                    Rooms.Remove(room);
                }
                else
                {
                    if (room.curPlayer == room.player1)
                    {
                        room.curPlayer = room.player2;
                        room.isX = false;
                    }
                    else
                    {
                        room.isX = true;
                        room.curPlayer = room.player1;
                    }
                    Clients.Client(room.player1.UserId).STEP(pos, room.field[position]);
                    Clients.Client(room.player2.UserId).STEP(pos, room.field[position]);

                }
            }
        }

        public void Gamestart(string p1, string p2)
        {
            var room = new Room(Users.FirstOrDefault(x => x.UserId == p1), Users.FirstOrDefault(x => x.UserId == p2));
            if (room.player1 != null && room.player2 != null)
            {
                Clients.Client(room.player1.UserId).DrawField();
                Clients.Client(room.player2.UserId).DrawField();
                Rooms.Add(room);
            }
        }

        public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
        {
            var item = Users.FirstOrDefault(x => x.UserId == Context.ConnectionId);
            if (item != null)
            {
                var id = item.UserId;
                Users.Remove(item);
                Clients.All.onUserDisconnected(id, item.Name);
            }

            return base.OnDisconnected(stopCalled);
        }
    }
}