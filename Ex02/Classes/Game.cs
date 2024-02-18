using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Ex02.ConsoleUtils;
using System.Threading.Tasks;

namespace Ex02.Classes
{
    internal class Game
    {
        Player m_Player1 { get; set; }
        Player m_Player2 { get; set; }
        Cells[,] m_Cells;
        Cells m_CurrentPlayer;
        int[] m_Counts;
        public int K { get; private set; }
        bool m_ModePcGame;
        private bool m_GameOver = false;

        public Game() : this(8, 8, 4, false)
        {
        }

        public Game(int rows, int cols, int K, bool modePcGame)
        {
            this.m_Cells = new Cells[rows, cols];
            this.m_CurrentPlayer = Cells.Red;
            this.m_Counts = new int[cols];
            this.K = K;
            this.m_ModePcGame = modePcGame;
            m_Player1 = new Player(Cells.Red);
            m_Player2 = new Player(Cells.Yellow);
            InitializeBoardWithSpaces();
        }

        public void InitializeBoardWithSpaces()
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    m_Cells[i, j] = Cells.Empty;
                }
            }
            for (int i = 0; i < Cols; i++)
            {
                m_Counts[i] = Rows - 1;
            }
            m_CurrentPlayer = Cells.Red;
        }

        public Cells this[int row, int col]
        {
            get { return m_Cells[row, col]; }
        }

        public int Rows
        {
            get
            {
                return m_Cells.GetLength(0);
            }
        }

        public int Cols
        {
            get
            {
                return m_Cells.GetLength(1);
            }
        }

        public Cells CurrentPlayer { get { return m_CurrentPlayer; } }

        public bool PlayerMode { get { return !m_ModePcGame; } }

        public bool IsGameOver { get { return m_GameOver; } }





        public bool ValidCol(int I_UserInput)
        {
            return I_UserInput >= 0 && I_UserInput < Cols;
        }

        public bool CanPlay(int col)
        {
            return !IsGameOver && m_Counts[col] >= 0;
        }

        public static int GetVaildUserInputBitween4to8()
        {
            int userNumber;
            while (true)
            {
                // Read the input as a string
                string userInput = Console.ReadLine();

                // Try parsing the string to an integer
                if (int.TryParse(userInput, out userNumber) && userNumber > 3 && userNumber < 9)
                {
                    Console.WriteLine($"You entered: {userNumber}");
                    break; // Exit the loop if a valid positive integer is entered
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid positive integer.");
                }
            }
            return userNumber;
        }

        public static int GetVaildUserModegame()
        {
            int UserNumber;
            Console.WriteLine("Please pick who you want to compet:\npress 1 to play with another player\npress 2 to play with PC");
            while (true)
            {
                // Read the input as a string
                string userInput = Console.ReadLine();

                // Try parsing the string to an integer
                if (int.TryParse(userInput, out UserNumber) && UserNumber > 0 && UserNumber < 3)
                {
                    break; // Exit the loop if a valid positive integer is entered
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

            Console.WriteLine("Welcome to 4 in row!");
            // Keep prompting until a valid positive integer is entered
            Console.Write("Please enter Number of Rows: ");
            numberOfRows = Game.GetVaildUserInputBitween4to8();
            Console.Write("Please enter Number of Cols: ");
            numberOfColumns = Game.GetVaildUserInputBitween4to8();
            userNumber = GetVaildUserModegame();

            Game game = userNumber == 1 ? new Game(numberOfRows, numberOfColumns, 4, false) : new Game(numberOfRows, numberOfColumns, 4, true);
            return game;
        }

        public void printMatrix()
        {
            StringBuilder numberOfColToPrint = new StringBuilder("  ");
            StringBuilder speaceToPrint = new StringBuilder("=");
            for (int i = 1; i <= Cols; i++)
            {
                numberOfColToPrint.Append($"{i}   ");
                speaceToPrint.Append("====");
            }
            Console.WriteLine(numberOfColToPrint);
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    //Console.Write($"| {(char)cells[i, j]} ");
                    Console.Write($"| ");
                    PrintInColor(m_Cells[i, j]);
                    Console.Write($" ");
                }
                Console.WriteLine("|");
                Console.WriteLine(speaceToPrint);
            }
        }

        public static void PrintInColor(Cells color)
        {
            ConsoleColor lastColor = Console.ForegroundColor;
            if (color != Cells.Empty)
            {
                if (color == Cells.Red)
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

        private bool IsIn(int nextR, int nextC)
        {
            return nextR >= 0 && nextR < Rows && nextC >= 0 && nextC < Cols;
        }

        private bool ThereIsKInLine(int row, int col, int dir)
        {
            int[] dr = { 0, 1, 1, 1 };
            int[] dc = { 1, 1, 0, -1 };

            for (int k = 1; k < this.K; k++)
            {
                int nextR = row + dr[dir] * k;
                int nextC = col + dc[dir] * k;
                if (!IsIn(nextR, nextC) || m_Cells[nextR, nextC] != m_Cells[row, col])
                {
                    return false;
                }
            }

            return true;
        }

        private void CheckWinCondition()
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    if (this[i, j] != Cells.Empty)
                    {
                        for (int d = 0; d < 4; d++)
                        {
                            if (ThereIsKInLine(i, j, d))
                            {
                                m_GameOver = true;
                                Screen.Clear();
                                this.printMatrix();
                                Console.WriteLine($"Well done!! {CurrentPlayer} is win!");
                                break;
                            }
                        }
                    }
                }
            }

            if (m_Counts.All(c => c == -1))
            {
                if (!m_GameOver)
                {
                    m_GameOver = true;
                    m_CurrentPlayer = Cells.Empty;
                    Screen.Clear();
                    printMatrix();
                }
            }
        }

        private bool play(int numOfCol)
        {
            numOfCol--;
            if (!ValidCol(numOfCol) || !CanPlay(numOfCol))
            {
                //throw new InvalidOperationException();
                Console.WriteLine("Invalid Input");
                return false;
            }


            m_Cells[m_Counts[numOfCol], numOfCol] = m_CurrentPlayer;
            m_Counts[numOfCol]--;

            CheckWinCondition();

            if (!IsGameOver)
            {
                m_CurrentPlayer = m_CurrentPlayer == Cells.Red ? Cells.Yellow : Cells.Red;
            }
            return true;
        }

        public void PlayerVsPlayer()
        {
            int token;
            bool invalidInput;
            string userInput;
            while (!this.IsGameOver)
            {
                this.printMatrix();
                invalidInput = false;
                while (!invalidInput)
                {
                    string playerColor = m_CurrentPlayer.ToString();

                    Console.WriteLine($"{playerColor} turn,");
                    Console.Write("Enter your column to put token: ");
                    userInput = Console.ReadLine();

                    if (userInput.ToUpper() == "Q")
                    {
                        m_GameOver = true;
                        m_CurrentPlayer = m_CurrentPlayer == Cells.Red ? Cells.Yellow : Cells.Red;
                        break;
                    }

                    if (int.TryParse(userInput, out token))
                    {
                        invalidInput = this.play(token);
                        if (invalidInput == false || IsGameOver)
                        {
                            Thread.Sleep(1500);
                        }
                        else
                        {
                            Screen.Clear();
                        }
                    }
                }
            }
            if (m_CurrentPlayer == Cells.Red)
            {
                m_Player1.IncreaseWinsPlayer();
            }
            else if (m_CurrentPlayer == Cells.Yellow)
            {
                m_Player2.IncreaseWinsPlayer();
            }
            else
            {
                Console.WriteLine("The game ended by draw!");
            }
        }

        public void PlayerVsPc()
        {
            Random rnd = new Random();
            int token, pcRandom;
            bool invalidInput;
            string userInput;
            while (!this.IsGameOver)
            {
                this.printMatrix();
                invalidInput = false;
                while (!invalidInput)
                {
                    Console.Write("Enter your column to put token: ");
                    userInput = Console.ReadLine();
                    if (userInput.ToUpper() == "Q")
                    {
                        m_GameOver = true;
                        m_CurrentPlayer = m_CurrentPlayer == Cells.Red ? Cells.Yellow : Cells.Red;
                        break;
                    }

                    if (int.TryParse(userInput, out token))
                    {
                        invalidInput = this.play(token);
                        if (invalidInput == false || IsGameOver)//המהלך לא תקין או המשחק נגמר
                        {
                            Thread.Sleep(1500);
                        }
                        else
                        {
                            Screen.Clear();
                            pcRandom = rnd.Next(1, Cols + 1);
                            invalidInput = this.play(pcRandom);
                            while (!invalidInput)
                            {
                                pcRandom = rnd.Next(1, Cols + 1);
                                invalidInput = this.play(pcRandom);
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid input, please try again");
                    }
                }
            }
            if (m_CurrentPlayer == Cells.Red)
            {
                m_Player1.IncreaseWinsPlayer();
            }
            else if (m_CurrentPlayer == Cells.Yellow)
            {
                m_Player2.IncreaseWinsPlayer();
            }
            else
            {
                Console.WriteLine("The game ended by draw!");
            }
        }

        public void PlayerVsPcAI()
        {
            int token;
            bool invalidInput;
            string userInput;
            while (!this.IsGameOver)
            {
                this.printMatrix();
                invalidInput = false;
                while (!invalidInput)
                {
                    Console.Write("Enter your column to put token: ");
                    userInput = Console.ReadLine();
                    if (userInput.ToUpper() == "Q")
                    {
                        m_GameOver = true;
                        m_CurrentPlayer = m_CurrentPlayer == Cells.Red ? Cells.Yellow : Cells.Red;
                        break;
                    }

                    if (int.TryParse(userInput, out token))
                    {
                        invalidInput = this.play(token);
                        if (invalidInput == false || IsGameOver)//המהלך לא תקין או המשחק נגמר
                        {
                            Thread.Sleep(1500);
                        }
                        else
                        {
                            Screen.Clear();
                            PlaySmartPC();
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid input, please try again");
                    }
                }
            }
            if (m_CurrentPlayer == Cells.Red)
            {
                m_Player1.IncreaseWinsPlayer();
            }
            else if (m_CurrentPlayer == Cells.Yellow)
            {
                m_Player2.IncreaseWinsPlayer();
            }
            else
            {
                Console.WriteLine("The game ended by draw!");
            }
        }

        public void PlayerVsAIPC()
        {

        }

        public void ShowStat()
        {
            Console.WriteLine($"points of player 1 (RED): {m_Player1.m_NumOfWins}\npoints of player 2 (YELLOW): {m_Player2.m_NumOfWins}");
        }

        public void PlaySmartPC()
        {
            int bestColumn = FindBestColumn();
            play(bestColumn);
        }

        private int FindBestColumn()
        {
            // Iterate through each column and find the first one that allows a winning move
            for (int col = 0; col < Cols; col++)
            {
                if (CanPlay(col))
                {
                    // Simulate placing a token in the current column
                    m_Cells[m_Counts[col], col] = m_CurrentPlayer;

                    // Check if this move leads to a win
                    if (IsWinningMove())
                    {
                        // Undo the simulated move
                        m_Cells[m_Counts[col], col] = Cells.Empty;
                        return col + 1; // Columns are 1-indexed
                    }

                    // Undo the simulated move
                    m_Cells[m_Counts[col], col] = Cells.Empty;
                }
            }

            // If no winning move is found, choose a random available column
            List<int> availableColumns = Enumerable.Range(1, Cols).Where(col => CanPlay(col - 1)).ToList();
            Random rnd = new Random();
            return availableColumns[rnd.Next(availableColumns.Count)];
        }

        private bool IsWinningMove()
        {
            // Check for a win in all directions
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    if (this[i, j] != Cells.Empty)
                    {
                        for (int d = 0; d < 4; d++)
                        {
                            if (ThereIsKInLine(i, j, d))
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        public bool Replay()
        {
            string result;
            Console.WriteLine("Do you want to play again? Enter Yes or No:");
            result = Console.ReadLine();
            while ("yes" != result.ToLower() && result.ToLower() != "no")
            {
                Console.WriteLine("Do you want to play again? Enter Yes or No:");
                result = Console.ReadLine();
            }

            if ("yes" == result.ToLower())
            {
                m_GameOver = false;
                Screen.Clear();
                InitializeBoardWithSpaces();
                return true;
            }
            return false;
        }
    }
    public enum Cells
    {
        Empty = ' ',
        Red = 'X',
        Yellow = 'O'
    }
}
