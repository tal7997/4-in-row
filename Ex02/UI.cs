using Ex02.Classes;
using Ex02.ConsoleUtils;
using System;
using System.Text;
using System.Threading;

namespace Ex02
{
    public class UI
    {
        public static void Run()
        {
            bool v_FlagPlay = true;

            Game game;
            game = SetupGame();
            Screen.Clear();
            if (game.PlayerMode)
            {
                ShowStat(game);
                while (v_FlagPlay)
                {
                    PlayerVsPlayer(game);
                    ShowStat(game);
                    v_FlagPlay = ReplayUI(game);
                }
            }
            else
            {
                ShowStat(game);
                while (v_FlagPlay)
                {
                    PlayerVsPcAI(game);
                    ShowStat(game);
                    v_FlagPlay = ReplayUI(game);
                }
            }

            Console.ReadLine();
        }

        public static bool ReplayUI(Game io_Game)
        {
            string result;
            bool v_FlagResult = true;

            Console.WriteLine("Do you want to play again? Enter Yes or No:");
            result = Console.ReadLine();
            while ("yes" != result.ToLower() && result.ToLower() != "no")
            {
                Console.WriteLine("Do you want to play again? Enter Yes or No:");
                result = Console.ReadLine();
            }

            if ("yes" == result.ToLower())
            {
                io_Game.Replay();
                Screen.Clear();
                return v_FlagResult;
            }

            return !v_FlagResult;
        }

        public static void ShowStat(Game io_Game)
        {
            int[] CurrentPlayersScore = io_Game.CurrentPlayersScore();

            Console.WriteLine($"points of player 1 (RED): {CurrentPlayersScore[0]}\npoints of player 2 (YELLOW): {CurrentPlayersScore[1]}");
        }

        public static int GetVaildUserInputBetween4to8()
        {
            int userNumber;

            while (true)
            {
                // Read the input as a string
                string userInput = Console.ReadLine();

                // Try parsing the string to an integer
                if (int.TryParse(userInput, out userNumber) && userNumber > 3 && userNumber < 9)
                {
                    break; // Exit the loop if a valid positive integer between 4 to 8 is entered
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid positive integer between 4 to 8.");
                }
            }

            return userNumber;
        }

        public static int GetVaildUserModegame()
        {
            int UserNumber;

            Console.WriteLine("Please pick who you want to compete:\npress 1 to play with another player\npress 2 to play with PC");
            while (true)
            {
                string userInput = Console.ReadLine();

                if (int.TryParse(userInput, out UserNumber) && UserNumber > 0 && UserNumber < 3)
                {
                    break; // Exit the loop if a valid positive integer between 1 to 2 is entered
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter 1 or 2");
                }
            }

            return UserNumber;
        }

        public static Game SetupGame()
        {
            int numberOfRows, numberOfColumns, userNumber;
            const bool v_PcMode = true;

            Console.WriteLine("Welcome to 4 in row!");
            Console.Write("Please enter Number of Rows between 4 to 8: ");
            numberOfRows = GetVaildUserInputBetween4to8();
            Console.Write("Please enter Number of Cols between 4 to 8: ");
            numberOfColumns = GetVaildUserInputBetween4to8();
            userNumber = GetVaildUserModegame();
            Game game = userNumber == 1 ? new Game(numberOfRows, numberOfColumns, 4, !v_PcMode) : new Game(numberOfRows, numberOfColumns, 4, v_PcMode);
            return game;
        }

        public static void PrintMatrixConsole(Game io_Game)
        {
            StringBuilder numberOfColToPrint = new StringBuilder("  ");
            StringBuilder speaceToPrint = new StringBuilder("=");

            for (int i = 1; i <= io_Game.Cols; i++)
            {
                numberOfColToPrint.Append($"{i}   ");
                speaceToPrint.Append("====");
            }

            Console.WriteLine(numberOfColToPrint);
            for (int i = 0; i < io_Game.Rows; i++)
            {
                for (int j = 0; j < io_Game.Cols; j++)
                {
                    Console.Write($"| ");
                    PrintInColor(io_Game[i, j]);
                    Console.Write($" ");
                }

                Console.WriteLine("|");
                Console.WriteLine(speaceToPrint);
            }
        }

        public static void PrintInColor(eCells color)
        {
            ConsoleColor lastColor = Console.ForegroundColor;

            if (color != eCells.Empty)
            {
                if (color == eCells.Red)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                }

                Console.Write((char)color);
                Console.ForegroundColor = lastColor;
            }
            else
            {
                Console.Write((char)color);
            }
        }

        public static void PlayerVsPlayer(Game io_Game)
        {
            int token;
            bool validInput;
            string userInput;

            while (!io_Game.IsGameOver)
            {
                PrintMatrixConsole(io_Game);
                validInput = false;
                while (!validInput)
                {
                    string playerColor = io_Game.CurrentPlayer.ToString();

                    Console.WriteLine($"{playerColor} turn,");
                    Console.Write("Enter your column to put token: ");
                    userInput = Console.ReadLine();
                    if (userInput.ToUpper() == "Q")
                    {
                        io_Game.IsGameOver = true;
                        io_Game.ChangePlayerTurn();
                        break;
                    }

                    if (int.TryParse(userInput, out token))
                    {
                        validInput = io_Game.TokenInsertion(token);
                        if (validInput == false)//check valid userInput
                        {
                            if(!io_Game.ValidCol(token-1))
                            {
                                Console.WriteLine($"Invalid Input range, please insert number between 1 to {io_Game.Cols}");
                            }
                            else
                            {
                                Console.WriteLine($"Invalid Input column is full, please insert to a diffrent column");
                            }

                            Thread.Sleep(1500);
                        }
                        else
                        {
                            Screen.Clear();
                            if (io_Game.IsGameOver)
                            {
                                PrintGameIsfinished(io_Game);
                            }
                        }  
                    }
                    else
                    {
                        Console.WriteLine($"Invalid Input, please insert number bitween 1 to {io_Game.Cols}");
                    }
                }
            }

            IncreaseNumOfWins(io_Game);
        }

        public static void PlayerVsPc(Game io_Game)
        {
            Random rnd = new Random();
            int token, pcRandom;
            bool validInput;
            string userInput;
            while (!io_Game.IsGameOver)
            {
                PrintMatrixConsole(io_Game);
                validInput = false;
                while (!validInput)
                {
                    Console.Write("Enter your column to put token: ");
                    userInput = Console.ReadLine();
                    if (userInput.ToUpper() == "Q")
                    {
                        io_Game.IsGameOver = true;
                        io_Game.CurrentPlayer = io_Game.CurrentPlayer == eCells.Red ? eCells.Yellow : eCells.Red;
                        break;
                    }

                    if (int.TryParse(userInput, out token))
                    {
                        validInput = io_Game.TokenInsertion(token);
                        if (validInput == false )//check valid userInput
                        {
                            if (!io_Game.ValidCol(token-1))
                            {
                                Console.WriteLine($"Invalid Input range, please insert number between 1 to {io_Game.Cols}");
                            }
                            else
                            {
                                Console.WriteLine($"Invalid Input column is full, please insert to a diffrent column");
                            }
                            Thread.Sleep(1500);
                        }
                        else if (io_Game.IsGameOver)
                        {
                            PrintGameIsfinished(io_Game);
                        }
                        else
                        {
                            Screen.Clear();
                            pcRandom = rnd.Next(1, io_Game.Cols + 1);
                            validInput = io_Game.TokenInsertion(pcRandom);
                            while (!validInput)
                            {
                                pcRandom = rnd.Next(1, io_Game.Cols + 1);
                                validInput = io_Game.TokenInsertion(pcRandom);
                            }
                            if(io_Game.IsGameOver)
                            {
                                PrintGameIsfinished(io_Game);
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid input, please try again");
                    }
                }
            }

            IncreaseNumOfWins(io_Game);
        }

        public static void PlayerVsPcAI(Game io_Game)
        {
            int token;
            bool validInput;
            string userInput;

            while (!io_Game.IsGameOver)
            {
                PrintMatrixConsole(io_Game);
                validInput = false;
                while (!validInput)
                {
                    Console.Write("Enter your column to put token: ");
                    userInput = Console.ReadLine();
                    if (userInput.ToUpper() == "Q")
                    {
                        io_Game.IsGameOver = true;
                        io_Game.CurrentPlayer = io_Game.CurrentPlayer == eCells.Red ? eCells.Yellow : eCells.Red;
                        break;
                    }

                    if (int.TryParse(userInput, out token))
                    {
                        validInput = io_Game.TokenInsertion(token);
                        if (validInput == false)//check valid userInput
                        {
                            if (!io_Game.ValidCol(token - 1))
                            {
                                Console.WriteLine($"Invalid Input range, please insert number between 1 to {io_Game.Cols}");
                            }
                            else
                            {
                                Console.WriteLine($"Invalid Input column is full, please insert to a diffrent column");
                            }
                            Thread.Sleep(1500);
                        }
                        else if (io_Game.IsGameOver)
                        {
                            PrintGameIsfinished(io_Game);
                        }
                        else
                        {
                            Screen.Clear();
                            io_Game.PlaySmartPC();
                            if(io_Game.IsGameOver)
                            {
                                PrintGameIsfinished(io_Game);
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid input, please try again");
                    }
                }
            }

            IncreaseNumOfWins(io_Game);
        }

        public static void IncreaseNumOfWins(Game io_Game)
        {
            if (io_Game.CurrentPlayer == eCells.Red)
            {
                io_Game.Player1.IncreaseWinsPlayer();
            }
            else if (io_Game.CurrentPlayer == eCells.Yellow)
            {
                io_Game.Player2.IncreaseWinsPlayer();
            }
        }

        public static void PrintGameIsfinished(Game io_Game)
        {
            Screen.Clear();
            PrintMatrixConsole(io_Game);
            if (io_Game.CurrentPlayer != eCells.Empty)
            {
                Console.WriteLine($"Well done!! {io_Game.CurrentPlayer} has won!");
            }
            else
            {
                Console.WriteLine("The game ended by draw!");
            }
        }
    }
}