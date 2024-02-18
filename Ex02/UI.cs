using Ex02.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace Ex02
{
    public class UI
    {
        public static void Run()
        {
            bool flagPlay = true;
            Game game = new Game();
            game = Game.SetupGame();
            if (game.PlayerMode)
            {
                game.ShowStat();
                while (flagPlay)
                {
                    game.PlayerVsPlayer();
                    game.ShowStat();
                    flagPlay = game.Replay();
                }
            }
            else
            {
                game.ShowStat();
                while (flagPlay)
                {
                    game.PlayerVsPcAI();
                    game.ShowStat();
                    flagPlay = game.Replay();
                }
            }
            Console.ReadLine();
        }
    }
}