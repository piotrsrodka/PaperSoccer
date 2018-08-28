using PaperSoccer.Enums;

namespace PaperSoccer
{
    public class Player
    {
        public PlayerOrder Order { get; set; }
        public PlayerNature Nature { get; set; }

        public Player()
        {
            Order = PlayerOrder.First;
            Nature = PlayerNature.Human;
        }

        public Player(PlayerNature playerNature)
        {
            Order = PlayerOrder.First;
            Nature = playerNature;
        }
        
        public void Flip()
        {
            if (Order == PlayerOrder.First)
            {
                Order = PlayerOrder.Second;                
            }
            else if (Order == PlayerOrder.Second)
            {
                Order = PlayerOrder.First;                
            }
        }
    }
}
