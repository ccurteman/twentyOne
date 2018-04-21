using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Casino
{
    public class Dealer
    {
        public string Name { get; set; }
        public Deck Deck { get; set; }
        public int Balance { get; set; }
        public void Deal(List<Card> Hand)
        {
            Hand.Add(Deck.Cards.First()); //adding deck.cards.first to hand
            string card = string.Format(Deck.Cards.First().ToString() + "\n");
            Console.WriteLine(card);
            using (StreamWriter file = new StreamWriter(@"C:\Users\GC\Logs\log.txt", true)) //using will close all memory after execution
            {
                file.WriteLine(DateTime.Now);
                file.WriteLine(card); //appends a new line to the file, which we opened with StreamWriter
            }
            Deck.Cards.RemoveAt(0);
        }
    }
}
