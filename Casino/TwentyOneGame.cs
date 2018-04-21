using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Casino.Interfaces;
using Casino.TwentyOne;

namespace Casino
{
    public class TwentyOneGame : Game, IWalkAway
    {
        public TwentyOneDealer Dealer { get; set; }
        public override void Play() //starting a new game of 21
        {
            Dealer = new TwentyOneDealer(); //setting a new TwentyOneDealer object
            foreach (Player player in Players) //we go around each player a create a new hand
            {
                player.Hand = new List<Card>();
                player.Stay = false;
            }
            Dealer.Hand = new List<Card>();
            Dealer.Stay = false;
            Dealer.Deck = new Deck();
            Dealer.Deck.Shuffle();
            foreach (Player player in Players) //this is collecting bets from every player in the game, using a foreach loop
            {
                bool validAnswer = false;
                int bet = 0;
                while (!validAnswer)
                {
                    Console.Write("Place your bet: ");
                    validAnswer = int.TryParse(Console.ReadLine(), out bet);
                    if (!validAnswer) Console.WriteLine("Please enter numeric value only, no decimals.");
                }
                if (bet < 0)
                {
                    throw new FraudException("Security kicked this person out.");
                }
                bool sucessfullyBet = player.Bet(bet); //checking if the player CAN bet
                if (!sucessfullyBet) //if they can't, return statement gets us out
                {
                    return;
                }
                Bets[player] = bet; //Bets[] keeps track of bets by pairing each bet with the player that made it
            }
            for (int i = 0; i < 2; i++) //looping twice
            {
                Console.Write("Dealing ");
                foreach (Player player in Players) //for each player in Players
                {
                    Console.Write("{0}: ", player.Name); 
                    Dealer.Deal(player.Hand); //the Dealer is adding a card to the players hand
                    if (i == 1) //this is just checking if we are on the second loop
                    {
                        bool blackJack = TwentyOneRules.CheckForBlackJack(player.Hand); //running CheckForBlackJack for the players hand to see if they have been dealt a winning hand
                        if (blackJack) //if true
                        {
                            Console.WriteLine("Blackjack! {0} wins {1}", player.Name, Bets[player]); //write to the console that the player has been dealt a winning hand
                            player.Balance += Convert.ToInt32((Bets[player] * 1.5) + Bets[player]); //add the players bet * 1.5 to the players balance
                            return; //stop the round
                        }
                    }
                }
                Console.Write("Dealer gets: ");
                Dealer.Deal(Dealer.Hand); //dealer is giving himself a hand
                if (i == 1) //checking if the dealer has won, same as before with players
                {
                    bool blackJack = TwentyOneRules.CheckForBlackJack(Dealer.Hand);
                    if (blackJack)
                    {
                        Console.WriteLine("Dealer has Blackjack!");
                        foreach (KeyValuePair<Player, int> entry in Bets)
                        {
                            Dealer.Balance += entry.Value;
                        }
                        return;
                    }
                }
            }
            foreach (Player player in Players)
            {
                while (!player.Stay) //while the player is staying
                {
                    Console.WriteLine("Your cards are: ");
                    foreach (Card card in player.Hand) //write to the console what cards the player is holding
                    {
                        Console.Write("{0} ", card.ToString());
                    }
                    Console.Write("\n\nHit or stay: ");
                    string answer = Console.ReadLine().ToLower();
                    if (answer == "stay") //if the player wants to stay (keep cards)
                    {
                        player.Stay = true;
                        break; //return out of this
                    }
                    else if (answer == "hit") //or if the player wants another card
                    {
                        Dealer.Deal(player.Hand); //Dealer deals a card to the players hand
                    }
                    bool busted = TwentyOneRules.IsBusted(player.Hand); //check if the player is over 21
                    if (busted) //if over 21
                    {
                        Dealer.Balance += Bets[player]; //the dealer gains the bet the player made FROM the dealers balance
                        Console.WriteLine("{0} Bust! You lose your bet of {1}. Your balance is now {2}", player.Name, Bets[player], player.Balance);
                        Console.Write("Do you want to play again: ");
                        answer = Console.ReadLine().ToLower();
                        if (answer == "yes" || answer == "yeah") //checking if the player wants to stop
                        {
                            player.isActivelyPlaying = true; //if the player wants to keep playing, set this to keep while loop active
                            return;
                        }
                        else
                        {
                            player.isActivelyPlaying = false; //breaks the while loop inside the main method
                            return;
                        }
                    }
                }
            }
            Dealer.isBusted = TwentyOneRules.IsBusted(Dealer.Hand); //check if the dealer has gone over 21
            Dealer.Stay = TwentyOneRules.ShouldDealerStay(Dealer.Hand); //check if the dealer should stay
            while (!Dealer.Stay && !Dealer.isBusted) //while the dealer is not over 21 and has decided to not stay
            {
                Console.WriteLine("Dealer is hitting...");
                Dealer.Deal(Dealer.Hand); //Dealer giving Dealer a card
                Dealer.isBusted = TwentyOneRules.IsBusted(Dealer.Hand); //check if Dealer is over 21
                Dealer.Stay = TwentyOneRules.ShouldDealerStay(Dealer.Hand); //check if dealer wants to stay
            }
            if (Dealer.Stay) //if dealer is staying
            {
                Console.WriteLine("Dealer is staying.");
            }
            if (Dealer.isBusted) //if dealer's hand is over 21
            {
                Console.WriteLine("Dealer bust!");
                foreach (KeyValuePair<Player, int> entry in Bets) //give each player their bets from the dealer
                {
                    Console.WriteLine("{0} won {1}!", entry.Key.Name, entry.Value);
                    Players.Where(x => x.Name == entry.Key.Name).First().Balance += (entry.Value * 2);
                    Dealer.Balance -= entry.Value;
                }
                return; //exit out of round
            }
            foreach (Player player in Players) //checking everyone's hand now to see who is the closest to 21
            {
                bool? playerWon = TwentyOneRules.CompareHands(player.Hand, Dealer.Hand); //comparing the hands of the player and dealer
                if (playerWon == null) //if the player has tied with the dealer
                {
                    Console.WriteLine("Push!");
                    player.Balance += Bets[player]; //giving the bet to back to the player
                }
                else if (playerWon == true) //if the player has won the game
                {
                    Console.WriteLine(player.Name + "won {0}", Bets[player]);
                    player.Balance += (Bets[player] * 2); //adding 2x the player bet to their balance
                    Dealer.Balance -= Bets[player]; //subtracting 2x the player bet from the dealer balance
                }
                else //if the player has not tied, nor won (the dealer has won then)
                {
                    Console.WriteLine("Dealer wins {0}!", Bets[player]);
                    Dealer.Balance += Bets[player]; //add all the player bets to the balance of the dealer
                }
                Console.Write("Play again? ");
                string answer = Console.ReadLine().ToLower();
                if (answer == "yes" || answer == "yeah") //checking if the player wants to play again
                {
                    player.isActivelyPlaying = true;
                }
                else
                {
                    player.isActivelyPlaying = false;
                }
            }
        }

        public override void ListPlayers()
        {
            Console.WriteLine("Welcome 21 Players: ");
            base.ListPlayers();
        }

        public void WalkAway(Player player)
        {
            throw new NotImplementedException();
        }
    }
}
