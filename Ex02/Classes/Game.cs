using System;
using System.Collections.Generic;
using System.Linq;


namespace Ex02.Classes
{
    public class Game
    {
        Player m_Player1;
        Player m_Player2;
        Cells[,] m_Cells;
        Cells m_CurrentPlayer;
        int[] m_Counts;
        int m_KInARow;
        public int KInARow { get { return m_KInARow; } private set { m_KInARow = value; } }
        bool m_ModePcGame;
        bool m_GameOver = false;


        public Game() : this(8, 8, 4, false)
        {
        }

        public Game(int rows, int cols, int K, bool modePcGame)
        {
            m_Cells = new Cells[rows, cols];
            m_CurrentPlayer = Cells.Red;
            m_Counts = new int[cols];
            m_KInARow = K;
            m_ModePcGame = modePcGame;
            m_Player1 = new Player(Cells.Red);
            m_Player2 = new Player(Cells.Yellow);
            initializeGameBoard();
        }

        private void initializeGameBoard()
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
            m_GameOver = false;
        }

        public Player Player1
        {
            get { return m_Player1; }
        }

        public Player Player2
        {
            get { return m_Player2; }
        }
        
        public Cells this[int row, int col]
        {
            get { return m_Cells[row, col]; }
            set { m_Cells[row, col] = value; }
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
            get { return m_Cells.GetLength(1); }
        }

        public Cells CurrentPlayer {
            get { return m_CurrentPlayer; }
            set { m_CurrentPlayer = value; }
        }

        public bool PlayerMode { get { return !m_ModePcGame; } }

        public bool IsGameOver
        { get { return m_GameOver; }
          set { m_GameOver = value; }
        }

        public void ChangePlayerTurn()
        {
            CurrentPlayer = CurrentPlayer == Cells.Red ? Cells.Yellow : Cells.Red;
        }

        public bool ValidCol(int i_UserInput)
        {
            return i_UserInput >= 0 && i_UserInput < Cols;
        }

        public bool IsValidPlay(int i_ColIndex)
        {
            return !IsGameOver && m_Counts[i_ColIndex] >= 0;
        }

        private bool isInBoardLimits(int i_NextRow, int i_NextCol)
        {
            return i_NextRow >= 0 && i_NextRow < Rows && i_NextCol >= 0 && i_NextCol < Cols;
        }

        private bool thereIsKInLine(int i_Row, int i_Col, int I_Diraction)
        {
            int[] diractionRow = { 0, 1, 1, 1 };
            int[] diractionCol = { 1, 1, 0, -1 };

            for (int k = 1; k < KInARow; k++)
            {
                int nextR = i_Row + diractionRow[I_Diraction] * k;
                int nextC = i_Col + diractionCol[I_Diraction] * k;
                if (!isInBoardLimits(nextR, nextC) || this[nextR, nextC] != this[i_Row, i_Col])
                {
                    return false;
                }
            }

            return true;
        }

        private void checkWinCondition()
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    if (this[i, j] != Cells.Empty)
                    {
                        for (int d = 0; d < 4; d++)
                        {
                            if (thereIsKInLine(i, j, d))
                            {
                                m_GameOver = true;
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
                    
                }
            }
        }

        public bool TokenInsertion(int i_NumOfCol)
        {
            i_NumOfCol--;
            if (!ValidCol(i_NumOfCol) || !IsValidPlay(i_NumOfCol))
            {
                return false;
            }

            this[m_Counts[i_NumOfCol], i_NumOfCol] = CurrentPlayer;
            m_Counts[i_NumOfCol]--;

            checkWinCondition();

            if (!IsGameOver)
            {
                ChangePlayerTurn();
            }
            return true;
        }

        public int[] CurrentPlayersScore()
        {
            int[] gameScore = new int[2] { Player1.NumOfWins, Player2.NumOfWins };
            return gameScore;
        }

        public void PlaySmartPC()
        {
            int bestColumn = findBestColumn();
            TokenInsertion(bestColumn);
        }

        private int findBestColumn()
        {
            // Iterate through each column and find the first one that allows a winning move
            for (int col = 0; col < Cols; col++)
            {
                if (IsValidPlay(col))
                {
                    // Simulate placing a token in the current column
                    m_Cells[m_Counts[col], col] = m_CurrentPlayer;

                    // Check if this move leads to a win
                    if (isWinningMove())
                    {
                        // Undo the simulated move
                        this[m_Counts[col], col] = Cells.Empty;
                        return col + 1; // Columns are 1-indexed
                    }

                    // Undo the simulated move
                    m_Cells[m_Counts[col], col] = Cells.Empty;
                }
            }

            // If no winning move is found, choose a random available column
            List<int> availableColumns = Enumerable.Range(1, Cols).Where(col => IsValidPlay(col - 1)).ToList();
            Random rnd = new Random();
            return availableColumns[rnd.Next(availableColumns.Count)];
        }

        private bool isWinningMove()
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
                            if (thereIsKInLine(i, j, d))
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        public void Replay()
        {          
            initializeGameBoard();
        }
        
    }
    public enum Cells
    {
        Empty = ' ',
        Red = 'X',
        Yellow = 'O'
    }
}