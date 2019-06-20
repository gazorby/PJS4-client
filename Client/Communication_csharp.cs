using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;
using Client;
using Client.Models;
using Newtonsoft.Json;
using System.Linq;

public interface ICommunication
    {

    void start(string host, int port);

    void end();

    void send(string msg);

    String receive();
}
public class Communication : ICommunication
{
    private Socket sender;
    public void end()
    {
        sender.Shutdown(SocketShutdown.Both);
        sender.Close();
    }

    public String receive()
    {
        byte[] bytes = new byte[10000];
        int bytesRec = sender.Receive(bytes);
        return Encoding.ASCII.GetString(bytes, 0, bytesRec);
    }

    public void send(string theMessage)
    {
        byte[] bytes = new byte[1024];
        byte[] msg = Encoding.ASCII.GetBytes(theMessage);
        int bytesSent = sender.Send(msg);
    }

    public void start(string host, int port)
    {
        IPHostEntry ipHost = Dns.GetHostEntry(host);
        IPAddress ipAddr = ipHost.AddressList[1];
        IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, port);

        sender = new Socket(AddressFamily.InterNetwork,
                                   SocketType.Stream, ProtocolType.Tcp);

        sender.Connect(ipEndPoint);

        Console.WriteLine("Socket connected to {0}",
        sender.RemoteEndPoint.ToString());
    }
    
}

public class programme
{
    private static Hero thisHero = null;
    private static List<Hero> heroes = new List<Hero>();
    private static Dictionary<string, List<Card>> cards = new Dictionary<string, List<Card>>();
    private static List<string> board = new List<string>();
    private static string cardType = "";
    
    public static void Main()
    {

        var comm = new Communication();
        comm.start("localhost", 11111);

        while (true)
        {
            var messages = comm.receive().Split("\n");
            foreach (var message in messages.Where(m => m != ""))
            {
                var m = JsonConvert.DeserializeObject<Message>(message);
                if (m.Phase == "INIT")
                {
                    switch (m.Type)
                    {
                        case "NAME":
                            Console.Write("Ton nom : ");
                            comm.send(Console.ReadLine());
                            break;
                        case "GAME_PHASE_INIT":
                            var gpi = JsonConvert.DeserializeObject<GamePhaseInit>(m.Content);
                            heroes.AddRange(gpi.heroes);
                            board.AddRange(gpi.board);
                            thisHero = heroes.First(h => h.id == gpi.id);
                            for (var i = 0; i < heroes.Count; i++)
                                Console.WriteLine($"{i}. {heroes[i]}");
                            Console.WriteLine(string.Join("\n", board));
                            break;
                    }
                }
                else if (m.Phase == "EQUIP")
                {
                    switch (m.Type)
                    {
                        case "EQUIP_PHASE_INIT":
                            var epi = JsonConvert.DeserializeObject<EquipPhaseInit>(m.Content);
                            cards[epi.cardType] = epi.availableCards;
                            cardType = epi.cardType;
                            Console.WriteLine($"Type de carte mis en jeu : {epi.cardType}");
                            Console.WriteLine("Cartes mises en jeu : ");
                            Console.WriteLine(string.Join("\n", cards[epi.cardType]));
                            Console.Write("Votre mise : ");
                            comm.send(Console.ReadLine());
                            Console.Write("En attente des autres joueurs...");
                            break;
                        case "EQUIP_PHASE_BET_SUCCESS":
                            var epbs = JsonConvert.DeserializeObject<EquipPhaseBetSuccess>(m.Content);
                            heroes = heroes.OrderByDescending(h => epbs.betSuccess[h.id]).ToList();
                            foreach (var hero in heroes)
                                hero.life_points -= epbs.betSuccess[hero.id];
                                    
                            Console.WriteLine("Résultats : ");
                            for (var i = 0; i < heroes.Count; i++)
                                Console.WriteLine($"{i+1}. {heroes[i]} a misé {epbs.betSuccess[heroes[i].id]} points de vie");
                            break;
                        case "EQUIP_PHASE_ASK":
                            var epa = JsonConvert.DeserializeObject<EquipPhaseAsk>(m.Content);
                            var heroAsked = heroes.First(h => h.id == epa.heroId);
                            if (heroAsked != thisHero)
                            {
                                Console.WriteLine($"{heroAsked} est en train de choisir sa carte...");
                            }
                            else
                            {
                                Console.WriteLine("Choisissez votre carte :");
                                for (var i = 0; i < cards[cardType].Count; i++)
                                    Console.WriteLine($"{i}. {cards[cardType][i]}");
                                int indSelected = int.Parse(Console.ReadLine());
                                comm.send(cards[cardType][indSelected].id);
                            }
                            break;
                        case "EQUIP_PHASE_PLAYER":
                            var epp = JsonConvert.DeserializeObject<EquipPhasePlayer>(m.Content);
                            var selectedCard = cards[epp.cardType].First(c => c.id == epp.cardId);
                            var heroEquip = heroes.First(h => h.id == epp.heroId);
                            heroEquip.cards[epp.cardType] = selectedCard;
                            cards[epp.cardType].Remove(selectedCard);
                            
                            Console.WriteLine($"{heroEquip.name} a choisi {selectedCard.name}");
                            break;
                    }
                }
                else if (m.Phase == "PREP")
                {
                    switch (m.Type)
                    {
                        case "PREP_PHASE_INIT":
                            var ppi = JsonConvert.DeserializeObject<PrepPhaseInit>(m.Content);
                            var heroAsked = heroes.First(h => h.id == ppi.heroId);
                            if (heroAsked != thisHero)
                            {
                                Console.WriteLine($"{heroAsked} est en train de choisir sa position...");
                            }
                            else
                            {
                                Console.WriteLine("Choisissez votre position :");
                                for (var i = 0; i < ppi.availablePositions.Count; i++)
                                    Console.WriteLine($"{i}. {ppi.availablePositions[i]}");
                                int indSelected = int.Parse(Console.ReadLine());
                                comm.send(indSelected.ToString());
                            }
                            break;
                        case "PREP_PHASE_POSITION":
                            var ppp = JsonConvert.DeserializeObject<PrepPhasePosition>(m.Content);
                            var heroPos = heroes.First(h => h.id == ppp.heroId);
                            heroPos.position = ppp.selectedPosition;
                            
                            Console.WriteLine($"{heroPos.name} se place en position {ppp.selectedPosition}");
                            break;
                    }
                }
            }
            //Console.WriteLine(m.getType());
            //c.send( /*message*/"");
        }
        
    }
}