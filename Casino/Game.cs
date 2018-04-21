using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Casino
{
    public abstract class Game
    {
        private List<Player> _players = new List<Player>();
        private Dictionary<Player, int> _bets = new Dictionary<Player, int>();
        public List<Player> Players { get { return _players; } set { _players = value; } } //when using a list, INSTANTIATE. _players is an instantiated list - good to default to
        public string Name { get; set; }
        public Dictionary<Player, int> Bets { get { return _bets; } set { _bets = value; } }
        public abstract void Play(); //Abstract methods can only exist inside of an abstract class. All classes inheriting this class MUST have this method.
        public virtual void ListPlayers() //Virtual: this method gets inherited by a class, BUT that inheriting class has the ability to overwrite this virtual method.
        {
            foreach (Player player in Players)
            {
                Console.WriteLine(player.Name);
            }
        }
    }
}
