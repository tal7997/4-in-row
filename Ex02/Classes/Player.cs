using System;
using System.Text;

namespace Ex02.Classes
{
    internal class Player
    {
        public int m_NumOfWins { get; set; }
        Cells m_Color  { get; set; }

        public Player()
        {
            m_NumOfWins = 0;
            //m_Color = new Cells();
            m_Color = Cells.Red;

        }

        public Player(Cells i_colorPlayer)
        {
            m_NumOfWins = 0;
            //m_Color = new Cells();
            if (i_colorPlayer != Cells.Empty)
            {
                m_Color = i_colorPlayer;
            }
            
        }

        public void IncreaseWinsPlayer()
        {
            m_NumOfWins++;
        }
    }


}
