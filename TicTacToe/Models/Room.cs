using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TicTacToe.Models
{
    class Room
    {
        public User player1, player2, curPlayer;
        public int[] field = new int[9];
        public bool isX = true;
        public Room(User pl1, User pl2)
        {
            this.player1 = pl1;
            this.player2 = pl2;
            this.isX = true;
            this.curPlayer = player1;
            for (int i = 0; i < 9; i++)
            {
                field[i] = 7;
            }
        }
    }
}