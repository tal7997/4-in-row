using System;
using System.Collections.Generic;
using System.Linq;

namespace Ex02.Classes
{
    public class Game
    {
        Player m_Player1;
        Player m_Player2;
        eCells[,] m_Cells;
        eCells m_CurrentPlayer;
        int[] m_ColumnFullnessCounterArray;
        int m_KInARow;
        int KInARow { get { return m_KInARow; } set { m_KInARow = value; } }
        bool m_ModePcGame;
        bool m_GameOver = false;

        public Game() : this(8, 8, 4, false)
        {
        }

        public Game(int rows, int cols, int K, bool modePcGame)
        {
            m_Cells = new eCells[rows, cols];
            m_CurrentPlayer = eCells.Red;
            m_ColumnFullnessCounterArray = new int[cols];
            m_KInARow = K;
            m_ModePcGame = modePcGame;
            m_Player1 = new Player(eCells.Red);
            m_Player2 = new Player(eCells.Yellow);
            initializeGameBoard();
        }

        private void initializeGameBoard()
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    m_Cells[i, j] = eCells.Empty;
                }
            }

            for (int i = 0; i < Cols; i++)
            {
                m_ColumnFullnessCounterArray[i] = Rows - 1;
            }

            m_CurrentPlayer = eCells.Red;
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
        
        public eCells this[int row, int col]
        {
            get { return m_Cells[row, col]; }
            set { m_Cells[row, col] = value; }
        }

        public int Rows
        {
            get { return m_Cells.GetLength(0); }
        }

        public int Cols
        {
            get { return m_Cells.GetLength(1); }
        }

        public eCells CurrentPlayer 
        {
            get { return m_CurrentPlayer; }
            set { m_CurrentPlayer = value; }
        }

        public bool PlayerMode { get { return !m_ModePcGame; } }

        public bool IsGameOver
        { 
            get { return m_GameOver; }
            set { m_GameOver = value; }
        }

        public void ChangePlayerTurn()
        {
            CurrentPlayer = CurrentPlayer == eCells.Red ? eCells.Yellow : eCells.Red;
        }

        public bool ValidCol(int i_UserInput)
        {
            return i_UserInput >= 0 && i_UserInput < Cols;
        }

        public bool IsValidPlay(int i_ColIndex)
        {
            return !IsGameOver && m_ColumnFullnessCounterArray[i_ColIndex] >= 0;
        }

        private bool isInBoardLimits(int i_NextRow, int i_NextCol)
        {
            return i_NextRow >= 0 && i_NextRow < Rows && i_NextCol >= 0 && i_NextCol < Cols;
        }

        private bool thereIsKInLine(int i_Row, int i_Col, int I_Diraction)
        {
            int[] diractionRow = { 0, 1, 1, 1 };
            int[] diractionCol = { 1, 1, 0, -1 };
            bool v_FlagResult = true;

            for (int k = 1; k < KInARow; k++)
            {
                int nextR = i_Row + diractionRow[I_Diraction] * k;
                int nextC = i_Col + diractionCol[I_Diraction] * k;

                if (!isInBoardLimits(nextR, nextC) || this[nextR, nextC] != this[i_Row, i_Col])
                {
                    v_FlagResult = !v_FlagResult;
                    break;
                }
            }

            return v_FlagResult;
        }

        private bool thereIs2InLineAndPotentialTofillMore(int i_Row, int i_Col, int I_Diraction, out int expcetedValCol)
        {
            int[] diractionRow = { 0, 1, 1, 1 };
            int[] diractionCol = { 1, 1, 0, -1 };
            int   backRow = i_Row - diractionRow[I_Diraction],
                  backColumn = i_Col - diractionCol[I_Diraction],
                  nextRow = i_Row + diractionRow[I_Diraction],
                  nextColumn = i_Col + diractionCol[I_Diraction],
                  nextNextRow = nextRow + diractionRow[I_Diraction],
                  nextNextColumn = nextColumn + diractionCol[I_Diraction];
            bool  backCell = isInBoardLimits(backRow, backColumn) && this[backRow, backColumn] == eCells.Empty;
            bool  forwardCell = isInBoardLimits(nextRow, nextColumn) && this[nextRow, nextColumn] == this[i_Row, i_Col];
            bool  positionThreeInRow = isInBoardLimits(nextNextRow, nextNextColumn) && this[nextNextRow, nextNextColumn] == eCells.Empty;
            bool  v_FlagResult = true;

            if (!(backCell && forwardCell))
            {
                expcetedValCol = -1;
                return false;
            }

            if (positionThreeInRow && m_ColumnFullnessCounterArray[nextNextColumn] == nextNextRow)
            {
                expcetedValCol = nextNextColumn;
                return v_FlagResult;
            }
            else if (backCell && m_ColumnFullnessCounterArray[backColumn] == backRow)
            {
                expcetedValCol = backColumn;
                return v_FlagResult;
            }
           
            expcetedValCol = -1;
            return !v_FlagResult;
        }

        private void checkWinCondition()
        {
            bool v_flag = true;

            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    if (this[i, j] != eCells.Empty)
                    {
                        for (int d = 0; d < 4; d++)
                        {
                            if (thereIsKInLine(i, j, d))
                            {
                                m_GameOver = v_flag;
                                break;
                            }
                        }
                    }
                }
            }

            if (m_ColumnFullnessCounterArray.All(c => c == -1))
            {
                if (!m_GameOver)
                {
                    m_GameOver = v_flag;
                    m_CurrentPlayer = eCells.Empty;
                }
            }
        }

        public bool TokenInsertion(int i_NumOfCol)
        {
            bool v_FlagResult = true;

            i_NumOfCol--;
            if (!ValidCol(i_NumOfCol) || !IsValidPlay(i_NumOfCol))
            {
                return !v_FlagResult;
            }

            this[m_ColumnFullnessCounterArray[i_NumOfCol], i_NumOfCol] = CurrentPlayer;
            m_ColumnFullnessCounterArray[i_NumOfCol]--;
            checkWinCondition();
            if (!IsGameOver)
            {
                ChangePlayerTurn();
            }

            return v_FlagResult;
        }

        public int[] CurrentPlayersScore()
        {
            return new int[2] { Player1.NumOfWins, Player2.NumOfWins };
        }

        public void PlaySmartPC()
        {
            int bestColumn = findBestColumn();

            TokenInsertion(bestColumn);
        }

        private int findBestColumn()
        {
            int bestAvailableColumn;

            // Iterate through each column and find the first one that allows a winning move
            bestAvailableColumn = scanBoardToWinOrBlock();
            if (bestAvailableColumn != 0)
            {
                return bestAvailableColumn;
            }
            else
            {
                ChangePlayerTurn();
                bestAvailableColumn = scanBoardToWinOrBlock();
                if(bestAvailableColumn != 0) 
                {
                    ChangePlayerTurn();

                    return bestAvailableColumn; 
                }

                ChangePlayerTurn();
            }

            if (isBlockMove(out bestAvailableColumn))
            {
                return bestAvailableColumn + 1;
            }

            // If no winning or blocking move is found, choose a random available column
            List<int> availableColumns = Enumerable.Range(1, Cols).Where(col => IsValidPlay(col - 1)).ToList();
            Random rnd = new Random();

            return availableColumns[rnd.Next(availableColumns.Count)];
        }

        private int scanBoardToWinOrBlock()
        {
            int resultColumn = 0;

            for (int col = 0; col < Cols; col++)
            {
                if (IsValidPlay(col))
                {
                    m_Cells[m_ColumnFullnessCounterArray[col], col] = m_CurrentPlayer; // Simulate placing a token in the current column
                    if (isWinningMove()) // Check if this move leads to a win
                    {
                        this[m_ColumnFullnessCounterArray[col], col] = eCells.Empty; // Undo the simulated move
                        resultColumn = col + 1; // Columns are 1-indexed
                        break;
                    }

                    m_Cells[m_ColumnFullnessCounterArray[col], col] = eCells.Empty; // Undo the simulated move
                }
            }

            return resultColumn;
        }

        private bool isWinningMove()
        {
            bool v_FlagResult = false;

            for (int i = 0; i < Rows; i++) 
            {
                for (int j = 0; j < Cols; j++)
                {
                    if (this[i, j] != eCells.Empty)
                    {
                        for (int d = 0; d < 4; d++) // Check for a win in all directions
                        {
                            if (thereIsKInLine(i, j, d))
                            {
                                v_FlagResult = !v_FlagResult;
                                break;
                            }
                        }
                    }
                }
            }

            return v_FlagResult;
        }

        private bool isBlockMove(out int expcetedValCol)
        {
            bool v_FlagResult = false;
            expcetedValCol = -1;
            
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    if (this[i, j] != eCells.Empty)
                    {
                        for (int d = 0; d < 4; d++) // Check for a win in all directions
                        {
                            if (thereIs2InLineAndPotentialTofillMore(i, j, d, out expcetedValCol) )
                            {
                                v_FlagResult = !v_FlagResult;
                                break;
                            }
                        }
                        if (v_FlagResult)
                        {
                            break;
                        }
                    }
                }
                if (v_FlagResult)
                {
                    break;
                }
            }

            return v_FlagResult;
        }

        public void Replay()
        {          
            initializeGameBoard();
        }   
    }

    public enum eCells
    {
        Empty = ' ',
        Red = 'X',
        Yellow = 'O'
    }
}