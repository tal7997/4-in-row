namespace Ex02.Classes
{
    public class Player
    {
        int m_NumOfWins;
        eCells m_Color  { get; set; }

        public Player()
        {
            m_NumOfWins = 0;
            m_Color = eCells.Red;
        }

        public Player(eCells i_colorPlayer)
        {
            m_NumOfWins = 0;
            if (i_colorPlayer != eCells.Empty)
            {
                m_Color = i_colorPlayer;
            }
        }

        public int NumOfWins
        {
            get { return m_NumOfWins; }
            set { m_NumOfWins = value;}
        }

        public void IncreaseWinsPlayer()
        {
            NumOfWins++;
        }
    }
}