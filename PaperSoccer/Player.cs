using PaperSoccer.Enums;

namespace PaperSoccer
{
    public class Player
    {
        public string Name { get; set; }
        public PlayerNature Nature { get; set; }
        
        public Player(string name, PlayerNature playerNature)
        {
            Name = name;
            Nature = playerNature;
        }
    }
}
