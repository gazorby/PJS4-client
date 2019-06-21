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

        sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        sender.Connect(ipEndPoint);

        Console.WriteLine("Socket connected to {0}",
        sender.RemoteEndPoint.ToString());
    }
    
}

public class programme
{
    private static Hero heroTurn = null;
    private static Hero thisHero = null;
    private static List<Hero> heroes = new List<Hero>();
    private static Dictionary<string, List<Card>> cards = new Dictionary<string, List<Card>>();
    private static List<Card> generics = new List<Card>();
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

                if (m.Phase == "LOBBY")
                {
                    switch (m.Type)
                    {
                        case "LOBBY_PHASE":
                            //var lobby = JsonConvert.DeserializeObject<LobbyPhase>(m.Content);
                            Console.WriteLine("Lobby ! ");
                            Console.WriteLine("Que faire ? CREATE <nb_player>, JOIN <id>");
                            var tokens = Console.ReadLine().Split(' ');
                            switch (tokens[0])
                            {
                                case "CREATE":
                                    int nbPlayer = int.Parse(tokens[1]);
                                    comm.send($"CREATE {nbPlayer}");
                                    break;
                                case "JOIN":
                                    comm.send($"JOIN {tokens[1]}");
                                    break;
                            }
                            break;
                        case "LOBBY_CREATE":
                            var lc = JsonConvert.DeserializeObject<LobbyCreate>(m.Content);
                            Console.WriteLine($"Partie créée avec succès (id: {lc.id})");
                            Console.WriteLine("En attente de joueurs...");
                            break;
                        case "LOBBY_JOIN":
                            var lj = JsonConvert.DeserializeObject<LobbyCreate>(m.Content);
                            Console.WriteLine($"Partie rejointe avec succès (id: {lj.id})");
                            Console.WriteLine("En attente de joueurs...");
                            break;
                    }
                }
                else if (m.Phase == "ERROR")
                {
                    var e = JsonConvert.DeserializeObject<Error>(m.Content);
                    Console.WriteLine($"ERROR ({m.Type}) : {e.message}");
                }
                else if (m.Phase == "INIT")
                {
                    switch (m.Type)
                    {
                        case "NAME":
                            Console.WriteLine("Début de la partie");
                            Console.Write("Ton nom : ");
                            comm.send(Console.ReadLine());
                            break;
                        case "GAME_PHASE_INIT":
                            var gpi = JsonConvert.DeserializeObject<GamePhaseInit>(m.Content);
                            heroes.AddRange(gpi.heroes);
                            board.AddRange(gpi.board);
                            generics = heroes[0].generics.Select(g => g.Value).ToList();
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
                else if (m.Phase == "TURN")
                {
                    switch (m.Type)
                    {
                        case "TURN_PHASE_INIT":
                            var tpi = JsonConvert.DeserializeObject<TurnPhaseInit>(m.Content);
                            heroTurn = tpi.hero;
                            Console.WriteLine(string.Join("\n", heroes));
                            Console.WriteLine(string.Join("\n", board));
                            if (tpi.hero.id != thisHero.id)
                            {
                                Console.WriteLine($"{heroTurn.name} réfléchit à ce qu'il doit faire...");
                                Console.WriteLine(
                                    $"Ses dés : {string.Join(" - ", heroTurn.dice.Select(d => $"{d.Key} x {d.Value}"))}");
                            }
                            else
                            {
                                Console.WriteLine("A vous de jouer !");
                                Console.WriteLine($"Vos dés : {string.Join(" - ", heroTurn.dice.Select(d => $"{d.Key} x {d.Value}"))}");
                                Console.WriteLine("Cartes utilisables : ");
                                Console.WriteLine(string.Join("\n", tpi.actions.cardsType));
                                Console.WriteLine("Actions normales possibles : ");
                                Console.WriteLine(string.Join("\n", tpi.actions.genericsType));
                                if (tpi.actions.canReplayDice)
                                    Console.WriteLine("Vous avez la possibilité de relancer vos dés");
                                Console.WriteLine(
                                    "Que faire ?  ACTION (CARD|GENERIC) <TYPE>, REPLAY_DICE [<TYPE>:int, ...], END_TURN)");
                                var tokens = Console.ReadLine().Split(" ");
                                switch (tokens[0])
                                {
                                    case "REPLAY_DICE":
                                        int weapon = 0;
                                        int armor = 0;
                                        int mount = 0;
                                        int spell = 0;

                                        for (int i = 1; i < tokens.Length; i++)
                                        {
                                            var typeAndNumber = tokens[i].Split(":");
                                            switch (typeAndNumber[0])
                                            {
                                                case "Die.WEAPON":
                                                    weapon = int.Parse(typeAndNumber[1]);
                                                    break;
                                                case "Die.ARMOR":
                                                    armor = int.Parse(typeAndNumber[1]);
                                                    break;
                                                case "Die.MOUNT":
                                                    mount = int.Parse(typeAndNumber[1]);
                                                    break;
                                                case "Die.SPELL":
                                                    spell = int.Parse(typeAndNumber[1]);
                                                    break;
                                            }
                                        }

                                        comm.send(JsonConvert.SerializeObject(
                                            new TurnPhaseDice(
                                                new Dictionary<string, int>
                                                {
                                                    {"Die.WEAPON", weapon},
                                                    {"Die.ARMOR", armor},
                                                    {"Die.MOUNT", mount},
                                                    {"Die.SPELL", spell},
                                                }
                                            )
                                        ));
                                        break;
                                    case "ACTION":
                                        comm.send(JsonConvert.SerializeObject(new TurnPhaseAction(
                                            new Dictionary<string, string>()
                                            {
                                                {"card", tokens[1]},
                                                {"type", tokens[2]}
                                            })
                                        ));
                                        break;
                                    case "END_TURN":
                                        comm.send(JsonConvert.SerializeObject(new EndTurn()));
                                        break;
                                    default:
                                        Console.WriteLine("Pas compris");
                                        break;
                                }
                            }

                            break;
                        case "TURN_PHASE_REPLAY_UPDATE":
                            //var tpru = JsonConvert.DeserializeObject<TurnPhaseReplayUpdate>(m.Content);
                            Console.WriteLine($"{heroTurn.name} a effectué une relance !");
                            break;
                        case "TURN_PHASE_ACTION_UPDATE":
                            var tpau = JsonConvert.DeserializeObject<TurnPhaseActionUpdate>(m.Content);
                            if (tpau.card == "CARD")
                                Console.WriteLine($"{heroTurn.name} a activé {heroTurn.cards[tpau.type]}");
                            else if (tpau.card == "GENERIC")
                                Console.WriteLine($"{heroTurn.name} a lancé une attaque de type {tpau.type}");
                            break;
                        case "TURN_PHASE_PLAYER":
                            var tpp = JsonConvert.DeserializeObject<TurnPhasePlayer>(m.Content);
                            heroes = tpp.heroes;
                            break;
                        case "TURN_PHASE_PENDING_POSITION":
                            var tppp = JsonConvert.DeserializeObject<TurnPhasePendingPositions>(m.Content);
                            var heroChoosingPos = heroes.First(h => h.id == tppp.heroId);
                            if (heroChoosingPos.id != thisHero.id)
                                Console.WriteLine($"{heroChoosingPos.name} choisit une position...");
                            else
                            {
                                Console.WriteLine("Choisis une position :");
                                for (var i = 0; i < tppp.positions.Count; i++)
                                    Console.WriteLine($"{i}. {tppp.positions[i]}");
                                comm.send(Console.ReadLine());
                            }
                            break;
                        case "TURN_PHASE_PENDING_POSITION_UPDATE":
                            var tpppu = JsonConvert.DeserializeObject<TurnPhasePendingPositionUpdate>(m.Content);
                            var heroUpdate = heroes.First(h => h.id == tpppu.heroId);
                            Console.WriteLine($"{heroUpdate} se déplace de {heroUpdate.position} en {tpppu.position}");
                            heroUpdate.position = tpppu.position;
                            break;
                        case "TURN_PHASE_PENDING_HERO":
                            var tpph = JsonConvert.DeserializeObject<TurnPhasePendingHero>(m.Content);
                            var heroChoosing = heroes.First(h => h.id == tpph.heroId);
                            if (heroChoosing != thisHero)
                                Console.WriteLine($"{heroChoosing.name} choisit une position...");
                            else
                            {
                                for (var j = 0; j < tpph.nbToChoose; j++) {
                                    Console.WriteLine("Choisis un héros :");
                                    for (var i = 0; i < tpph.heroesId.Count; i++)
                                        Console.WriteLine($"{i}. {heroes.First(h => h.id == tpph.heroesId[i]).name}");
                                    var indSelected = int.Parse(Console.ReadLine());
                                    comm.send(tpph.heroesId[indSelected]);
                                    tpph.heroesId.Remove(tpph.heroesId[indSelected]);
                                }
                            }
                            break;
                        case "TURN_PHASE_PENDING_HERO_UPDATE":
                            var tpphu = JsonConvert.DeserializeObject<TurnPhasePendingHeroUpdate>(m.Content);
                            var heroCaster = heroes.First(h => h.id == tpphu.heroId);
                            var heroesTarget = tpphu.heroes.Select(h => heroes.First(hs => hs.id == h));
                            Console.WriteLine($"{heroCaster} cible {string.Join(", ", heroesTarget.Select(ht => ht.name))}");
                            break;
                    }
                }
            }
            //Console.WriteLine(m.getType());
            //c.send( /*message*/"");
        }
        
    }
}