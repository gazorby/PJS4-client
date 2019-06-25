using Client;
using Client.Models;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    public readonly static ConcurrentQueue<Action> RunOnMainThread = new ConcurrentQueue<Action>();
    public InputField numPort;
    public Communication comm;
    public GameObject emptyCase;
    public GameObject pillarCase;
    public GameObject fontaine;
    public GameObject mine;
    public GameObject player;
    public GameObject chatpanel, textObject;
    public InputField chatBox;
    public GameObject realPlayer;
    public ScrollRect chatscroll;
    public Text lifePointText;
    public Button create_game;
    public Button join_game;
    public Dropdown nb_joueur;
    public InputField lien_invit;
    public InputField pseudo;
    public GameObject niougame;
    public GameObject equip_phase;
    public Text nom_phase;
    public Text description_carte;
    public Text nom_carte;
    public Image cout_1;
    public Image cout_2;
    public Image[] couts_cartes;
    public Dropdown choix_carte;
    public Dropdown choix_mise;
    public Button valider_mise;
    public Sprite atk_sprite;
    public Sprite mov_sprite;
    public Sprite pow_sprite;
    public Sprite arm_sprite;
    private bool updateScroll = false;
    [SerializeField]
    List<Message> messageList = new List<Message>();
    int maxMessage = 50;
    private Vector3 destPositionPlayer;
    private int x, y;
    public GameObject panel_carte;
    public GameObject carte1;
    public GameObject carte2;
    public GameObject carte3;
    public GameObject carte4;
    public Image atk_cout_1;
    public Image atk_cout_2;
    public Image mov_cout_1;
    public Image mov_cout_2;
    public Image shield_cout_1;
    public Image shield_cout_2;
    public Image pow_cout_1;
    public Image pow_cout_2;
    public Text txt_carte_1;
    public Text txt_carte_2;
    public Text txt_carte_3;
    public Text txt_carte_4;
    public Text fatiguePointText;
    public Text specialPointText;
    public Text atkTxt;
    public Text movTxt;
    public Text powTkt;
    public Text armTxt;
    public Button atkBtn;
    public Button movBtn;
    public Button armBtn;
    public Button powBtn;
    public Button endturnBtn;
    public Button relanceBtn;
    public Button fermeture_panel_ennemi_btn;
    public GameObject panel_cartes_ennemi;
    public Text nom_carte_ennemi_1;
    public Text nom_carte_ennemi_2;
    public Text nom_carte_ennemi_3;
    public Text nom_carte_ennemi_4;
    public Text description_carte_ennemi_1;
    public Text description_carte_ennemi_2;
    public Text description_carte_ennemi_3;
    public Text description_carte_ennemi_4;
    public Image cout_1_carte_ennemi_1;
    public Image cout_2_carte_ennemi_1;
    public Image cout_1_carte_ennemi_2;
    public Image cout_2_carte_ennemi_2;
    public Image cout_1_carte_ennemi_3;
    public Image cout_2_carte_ennemi_3;
    public Image cout_1_carte_ennemi_4;
    public Image cout_2_carte_ennemi_4;
    public GameObject panelCbt;
    public Text nom_joueur_panel_ennemi;
    public GameObject panel_ennemi_1;
    public GameObject panel_ennemi_2;
    public GameObject panel_ennemi_3;
    public GameObject panel_ennemi_4;
    public GameObject[] panel_ennemi;
    public bool clicTrigger = false;
    public bool clicTriggerFdp = false;
    public List<Position> availablePositions;
    public List<Hero> heroesFdp;
    public GameObject sp_perso1;
    public GameObject sp_perso2;
    public GameObject sp_perso3;
    public GameObject sp_perso4;
    public GameObject[] sp_perso;
    public Hero thisHero = null;
    // Initialisation du jeu
    void Start()
    {
        equip_phase.SetActive(false);
        panel_carte.SetActive(false);
        panel_cartes_ennemi.SetActive(false);
        panelCbt.SetActive(false);
        chatBox.interactable = false;
        panel_ennemi_1.SetActive(false);
        panel_ennemi_2.SetActive(false);
        panel_ennemi_3.SetActive(false);
        panel_ennemi_4.SetActive(false);
        availablePositions = new List<Position>();
        heroesFdp = new List<Hero>();
        comm = new Communication();
        try
        {

            comm.start("91.121.86.62", 11122);
            comm.receive().Split('\n');
            ExThread obj = new ExThread(this);
            Thread thr = new Thread(new ThreadStart(obj.srv_in));
            thr.Start();
            couts_cartes = new Image[] { cout_1, cout_2 };
            panel_ennemi = new GameObject[] { panel_ennemi_1, panel_ennemi_2, panel_ennemi_3, panel_ennemi_4 };
            sp_perso = new GameObject[] { sp_perso1, sp_perso2, sp_perso3, sp_perso4 };

        }
        catch (Exception e)
        {
            SendMessageToChat("Connexion au serveur impossible.", Color.red);
            SendMessageToChat(e.Message);
        }


    }

    //renvoie la position de la cellule cliquée
    System.Object Getclic()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (hit.collider != null)
            if (hit.collider != null && clicTrigger)
            {
                clicTrigger = false;
                for (int i = 0; i < availablePositions.Count(); ++i)
                {;
                    if (availablePositions[i].x / 2 == hit.collider.gameObject.GetComponent<posserv>().x && availablePositions[i].y == hit.collider.gameObject.GetComponent<posserv>().y)
                    {
                        comm.send(i.ToString());
                    }
                }
            }
            else if (hit.collider != null && clicTriggerFdp)
            {
                clicTriggerFdp = false;
                for (int i = 0; i < availablePositions.Count(); ++i)
                {
                    if (availablePositions[i].x / 2 == hit.collider.gameObject.GetComponent<posserv>().y && availablePositions[i].y == hit.collider.gameObject.GetComponent<posserv>().x)
                    {
                        foreach (var v in heroesFdp)
                        {
                            if (v.position.x / 2 == hit.collider.gameObject.GetComponent<posserv>().y && v.position.y == hit.collider.gameObject.GetComponent<posserv>().x)
                            {
                                comm.send(v.id);
                            }
                        }
                    }
                }
            }
        }
        return null;
    }

    //Execution des commandes serveur
    void Update_server()
    {
        if (!RunOnMainThread.IsEmpty)
        {
            while (RunOnMainThread.TryDequeue(out Action action))
            {
                action.Invoke();
            }
        }
    }

    // appellé à chaque frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (chatBox.text != "")
            {
                Massage m = new Massage()
                {
                    Phase = "CHAT",
                    Type = "CHAT",
                    Content = chatBox.text
                };
                comm.send(JsonConvert.SerializeObject(m));
                chatBox.text = "";
            }
            chatBox.Select();
            chatBox.ActivateInputField();
        }

        if (updateScroll)
        {
            chatscroll.verticalNormalizedPosition = 0;
            updateScroll = false;
        }
        Update_server();
        Getclic();
    }

    public void SendMessageToChat(string text)
    {
        SendMessageToChat(text, Color.white);
    }

    //gestion de la fenêtre de chat
    public void SendMessageToChat(string text, Color color)
    {
        if (messageList.Count >= maxMessage) //suppression des anciens messages
        {
            Destroy(messageList[0].textObject.gameObject);
            messageList.Remove(messageList[0]);
        }
        Message newMessage = new Message();
        newMessage.text = text;
        GameObject newText = Instantiate(textObject, chatpanel.transform);
        newMessage.textObject = newText.GetComponent<Text>();
        newMessage.textObject.text = newMessage.text;
        newMessage.textObject.color = color;
        messageList.Add(newMessage);
        updateScroll = true;
    }

}

[Serializable]
public class Message
{
    public string text;
    public Text textObject;
}

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
        byte[] bytes = new byte[50000];
        int bytesRec = sender.Receive(bytes);
        return Encoding.ASCII.GetString(bytes, 0, bytesRec);
    }

    public void send(string theMessage)
    {
        byte[] bytes = new byte[50000];
        byte[] msg = Encoding.ASCII.GetBytes(theMessage);
        int bytesSent = sender.Send(msg);
    }

    public void start(string host, int port)
    {
        IPHostEntry ipHost = Dns.GetHostEntry(host);
        IPAddress ipAddr = ipHost.AddressList[0];
        IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, port);

        sender = new Socket(AddressFamily.InterNetwork,
                                   SocketType.Stream, ProtocolType.Tcp);

        sender.Connect(ipEndPoint);

        Console.WriteLine("Socket connected to {0}",
        sender.RemoteEndPoint.ToString());
    }

}

//récéption des message du serveur
public class ExThread
{
    Manager man;
    ColorBlock cb;

    private static List<Hero> heroes = new List<Hero>();
    private static Dictionary<string, List<Card>> cards = new Dictionary<string, List<Card>>();
    //private static List<Card>

    public Hero heroTurn = null;
    private static List<Card> generics = new List<Card>();
    private static List<string> board = new List<string>();
    private static string cardType = "";
    private Vector2[][] position_unity;
    EquipPhaseInit epi;
    bool mise = true;
    List<GameObject> board_unity;
    int ordreJeu = 0;
    //connexion avec la classe principale pour gérer les requêtes
    public ExThread(Manager man)
    {
        this.man = man;
        man.create_game.onClick.AddListener(creation);
        man.join_game.onClick.AddListener(join);
        man.choix_carte.onValueChanged.AddListener(affichage_carte_select);
        man.valider_mise.onClick.AddListener(envoi_mise);
        man.relanceBtn.onClick.AddListener(relancer);
        man.endturnBtn.onClick.AddListener(passer);
        man.atkBtn.onClick.AddListener(btnAtkHandler);
        man.movBtn.onClick.AddListener(btnMovHandler);
        man.powBtn.onClick.AddListener(btnPowHandler);
        man.armBtn.onClick.AddListener(btnArmHandler);
        man.fermeture_panel_ennemi_btn.onClick.AddListener(fermeturePanelEnnemi);

        board_unity = new List<GameObject>();
    }

    void fermeturePanelEnnemi()
    {
        man.panel_cartes_ennemi.SetActive(false);
    }
    private void btnMovHandler()
    {
        man.comm.send(JsonConvert.SerializeObject(new TurnPhaseAction(new Dictionary<string, string>()
                                            {
                                                {"card", "GENERIC"},
                                                {"type", "Die.MOUNT"}
                                            })));
    }
    private void btnPowHandler()
    {
        man.comm.send(JsonConvert.SerializeObject(new TurnPhaseAction(new Dictionary<string, string>()
                                            {
                                                {"card", "GENERIC"},
                                                {"type", "Die.SPELL"}
                                            })));
    }
    private void btnArmHandler()
    {
        man.comm.send(JsonConvert.SerializeObject(new TurnPhaseAction(new Dictionary<string, string>()
                                            {
                                                {"card", "GENERIC"},
                                                {"type", "Die.ARMOR"}
                                            })));
    }
    private void btnAtkHandler()
    {
        man.comm.send(JsonConvert.SerializeObject(new TurnPhaseAction(new Dictionary<string, string>()
                                            {
                                                {"card", "GENERIC"},
                                                {"type", "Die.WEAPON"}
                                            })));
    }
    private void passer()
    {
        man.comm.send(JsonConvert.SerializeObject(new EndTurn()));
    }
    private void relancer()
    {
        man.relanceBtn.interactable = false;
        man.comm.send(JsonConvert.SerializeObject(new TurnPhaseDice(new Dictionary<string, int>
                                                {
                                                    {"Die.WEAPON",man.thisHero.dice["Die.WEAPON"]},
                                                    {"Die.ARMOR", man.thisHero.dice["Die.ARMOR"]},
                                                    {"Die.MOUNT", man.thisHero.dice["Die.MOUNT"]},
                                                    {"Die.SPELL", man.thisHero.dice["Die.SPELL"]},
                                                }
                                            )));
    }

    private void envoi_mise()
    {
        if (mise)
        {
            man.comm.send((man.choix_mise.value).ToString());
            man.thisHero.life_points -= man.choix_mise.value;
            man.lifePointText.text = man.thisHero.life_points.ToString();
            man.valider_mise.interactable = false;
            man.choix_mise.interactable = false;
            chat("En attente des autres joueurs...");
        }
        else
        {
            man.comm.send(cards[cardType][man.choix_carte.value].id);
        }
    }

    Sprite getSprite(string txt)
    {
        if (txt == "Die.WEAPON")
            return man.atk_sprite;
        if (txt == "Die.MOUNT")
            return man.mov_sprite;
        if (txt == "Die.ARMOR")
            return man.arm_sprite;
        if (txt == "Die.SPELL")
            return man.pow_sprite;
        return null;
    }
    private void affichage_carte_select(int arg0)
    {
        man.nom_carte.text = cards[epi.cardType][man.choix_carte.value].name;
        man.description_carte.text = cards[epi.cardType][man.choix_carte.value].description;
        int nb_couts = 0;
        for (int i = 0; i < cards[epi.cardType][man.choix_carte.value].requirements["Die.WEAPON"]; ++i)
        {
            man.couts_cartes[nb_couts++].sprite = man.atk_sprite;
        }
        for (int i = 0; i < cards[epi.cardType][man.choix_carte.value].requirements["Die.MOUNT"]; ++i)
        {
            man.couts_cartes[nb_couts++].sprite = man.mov_sprite;
        }
        for (int i = 0; i < cards[epi.cardType][man.choix_carte.value].requirements["Die.ARMOR"]; ++i)
        {
            man.couts_cartes[nb_couts++].sprite = man.arm_sprite;
        }
        for (int i = 0; i < cards[epi.cardType][man.choix_carte.value].requirements["Die.SPELL"]; ++i)
        {
            man.couts_cartes[nb_couts++].sprite = man.pow_sprite;
        }
        if (nb_couts == 1)
            man.couts_cartes[nb_couts].sprite = null;
    }

    void chat(string message)
    {
        Manager.RunOnMainThread.Enqueue(() =>
        {
            man.SendMessageToChat(message);
        });
    }

    void chat(string message, Color c)
    {
        Manager.RunOnMainThread.Enqueue(() =>
        {
            man.SendMessageToChat(message, c);
        });
    }
    void join()
    {
        flash_colors();
        if (man.lien_invit.text != "" && man.pseudo.text != "")
        {
            man.comm.send($"JOIN {man.lien_invit.text}");
        }
        else
        {
            if (man.lien_invit.text == "")
            {
                cb = man.lien_invit.colors;
                cb.normalColor = Color.red;
                man.lien_invit.colors = cb;
            }
            if (man.pseudo.text == "")
            {
                cb = man.pseudo.colors;
                cb.normalColor = Color.red;
                man.pseudo.colors = cb;
            }
        }
    }

    void flash_colors()
    {
        cb = man.lien_invit.colors;
        cb.normalColor = Color.white;
        man.lien_invit.colors = cb;
        cb = man.pseudo.colors;
        cb.normalColor = Color.white;
        man.pseudo.colors = cb;
    }

    void lancement_serveur()
    {

    }

    void creation()
    {
        flash_colors();
        if (man.pseudo.text != "")
            man.comm.send($"CREATE {(man.nb_joueur.value + 2).ToString()}");
        else
        {
            cb = man.pseudo.colors;
            cb.normalColor = Color.red;
            man.pseudo.colors = cb;
        }
    }

    void maj_board()
    {
        board_unity.Clear();
        string[] cases = transformation(board);
        position_unity = new Vector2[cases.Length][];

        var size = new Vector2(
            man.emptyCase.GetComponent<SpriteRenderer>().size.x * man.emptyCase.transform.localScale.x,
            man.emptyCase.GetComponent<SpriteRenderer>().size.y * man.emptyCase.transform.localScale.y
            );
        float offset = size.x / 2;
        for (int y = 0; y < cases.Length; ++y)
        {
            offset = y % 2 != 0 ? 0 : size.x / 2 * 1.4f;
            position_unity[y] = new Vector2[cases[y].Length];
            for (int x = 0; x < cases[y].Length; ++x)
            {
                position_unity[y][x] = new Vector2(x * 1.4f * size.x + offset, y * 1.35f * size.y);
                if (cases[y][x] != 'O')
                {
                    if (cases[y][x] == 'F')
                        board_unity.Add(maj_cell(man.fontaine, x, y));
                    else if (cases[y][x] == 'T')
                        board_unity.Add(maj_cell(man.mine, x, y));
                    else if (cases[y][x] == 'P')
                        board_unity.Add(maj_cell(man.pillarCase, x, y, false));
                    else if (cases[y][x] == 'X')
                        board_unity.Add(maj_cell(man.pillarCase, x, y, false));
                    else
                        board_unity.Add(maj_cell(man.emptyCase, x, y));
                }
            }
        }
    }
    GameObject maj_cell(GameObject o, int x, int y, bool cliquable = true)
    {
        GameObject res = UnityEngine.Object.Instantiate(o, position_unity[y][x], Quaternion.identity);
        if (cliquable)
        {
            res.GetComponent<posserv>().x = x;
            res.GetComponent<posserv>().y = y;
        }
        return res;
    }
    private string[] transformation(List<String> tableau)
    {
        string[] res = new string[tableau.Count];
        for (int i = 0; i < tableau.Count; ++i)
        {
            string tmp = "";
            for (int j = 0; j < tableau[i].Length; ++j)
            {
                if (tableau[i][j] != '-')
                {
                    tmp += tableau[i][j];
                }
            }
            res[i] = tmp;
        }
        return res;
    }
    void charge_sprite(Dictionary<string, int> carte, Image[] img)
    {
        int carteok = 0;
        foreach (var v in carte)
        {
            for (int i = 0; i < v.Value; ++i)
            {
                img[carteok++].sprite = getSprite(v.Key);
            }
        }
        if (carteok == 1)
            img[carteok].sprite = null;
    }
    int convertX(int x)
    {
        return x / 2;
    }

    void maj_pv()
    {
        for (int i = 0; i < heroes.Count; ++i)
        {
            if (heroes[i].id == man.thisHero.id)
            {
                man.lifePointText.text = heroes[i].life_points.ToString();
            }
            else
            {
                foreach (var v in man.panel_ennemi[heroes[i].timeline].GetComponents<Text>())
                {
                    if (v.gameObject.name == "hp")
                    {
                        v.text = heroes[i].life_points.ToString();
                    }
                }
            }
        }
    }

    public void maj_des()
    {
        foreach (Hero hd in heroes)
        {
            int atk = 0;
            int mov = 0;
            int pow = 0;
            int arm = 0;
            foreach (var v in hd.dice)
            {
                if (v.Key == "Die.WEAPON")
                    atk = v.Value;
                if (v.Key == "Die.MOUNT")
                    mov = v.Value;
                if (v.Key == "Die.SPELL")
                    pow = v.Value;
                if (v.Key == "Die.ARMOR")
                    arm = v.Value;
            }
            if (hd.id == man.thisHero.id)
            {
                man.atkTxt.text = atk.ToString();
                man.movTxt.text = mov.ToString();
                man.powTkt.text = pow.ToString();
                man.armTxt.text = arm.ToString();
                man.fatiguePointText.text = hd.fatigue_points.ToString();
                man.specialPointText.text = hd.armor_points.ToString();
            }
            else
            {
                foreach (var v in man.panel_ennemi[hd.timeline].GetComponents<Text>())
                {
                    if (v.gameObject.name == "atk")
                        v.text = atk.ToString();
                    if (v.gameObject.name == "mov")
                        v.text = mov.ToString();
                    if (v.gameObject.name == "pow")
                        v.text = pow.ToString();
                    if (v.gameObject.name == "shield")
                        v.text = arm.ToString();
                    if (v.gameObject.name == "fatigue")
                        v.text = hd.fatigue_points.ToString();
                    if (v.gameObject.name == "special")
                        v.text = hd.armor_points.ToString();
                }
            }
        }
    }

    public void srv_in()
    {
        while (true)
        {
            var messages = man.comm.receive().Split('\n');
            foreach (var message in messages.Where(x => x != ""))
            {
                var m = JsonConvert.DeserializeObject<Massage>(message);
                if (m.Phase == "LOBBY")
                {
                    switch (m.Type)
                    {
                        case "LOBBY_CREATE":
                            Manager.RunOnMainThread.Enqueue(() =>
                            {
                                man.niougame.SetActive(false);
                            });
                            var lc = JsonConvert.DeserializeObject<LobbyCreate>(m.Content);
                            chat($"Partie créée avec succès (id: {lc.id})");
                            chat("En attente de joueurs...");
                            break;
                        case "LOBBY_JOIN":
                            Manager.RunOnMainThread.Enqueue(() =>
                            {
                                man.niougame.SetActive(false);
                            });
                            var lj = JsonConvert.DeserializeObject<LobbyCreate>(m.Content);
                            chat($"Partie rejointe avec succès (id: {lj.id})");
                            chat("En attente de joueurs...");
                            break;
                    }
                }
                else if (m.Phase == "ERROR")
                {
                    var e = JsonConvert.DeserializeObject<Error>(m.Content);
                    chat(e.message, Color.red);
                }
                else if (m.Phase == "INIT")
                {
                    switch (m.Type)
                    {
                        case "NAME":
                            man.comm.send(man.pseudo.text);
                            break;
                        case "GAME_PHASE_INIT":
                            var gpi = JsonConvert.DeserializeObject<GamePhaseInit>(m.Content);
                            heroes.AddRange(gpi.heroes);
                            board.AddRange(gpi.board);
                            generics = heroes[0].generics.Select(g => g.Value).ToList();
                            man.thisHero = heroes.First(h => h.id == gpi.id);

                            Manager.RunOnMainThread.Enqueue(() =>
                            {
                                man.chatBox.interactable = true;
                                maj_board();
                            });
                            chat("début de partie");
                            break;
                    }
                }
                else if (m.Phase == "CHAT")
                {
                    switch (m.Type)
                    {
                        case "CHAT_MESSAGE":
                            var msg = JsonConvert.DeserializeObject<ChatMessage>(m.Content);
                            chat(msg.author + " : " + msg.content, Color.blue);
                            break;
                    }
                }

                else if (m.Phase == "EQUIP")
                {
                    switch (m.Type)
                    {
                        case "EQUIP_PHASE_INIT":
                            Manager.RunOnMainThread.Enqueue(() =>
                            {
                                man.equip_phase.SetActive(true);
                                man.panel_carte.SetActive(true);
                                man.valider_mise.enabled = true;
                                man.choix_mise.interactable = true;
                                epi = JsonConvert.DeserializeObject<EquipPhaseInit>(m.Content);
                                cards[epi.cardType] = epi.availableCards;
                                cardType = epi.cardType;
                                if (epi.cardType == "Die.WEAPON")
                                    man.nom_phase.text = "Choisissez votre arme";
                                else if (epi.cardType == "Die.MOUNT")
                                    man.nom_phase.text = "Quelle sera votre monture ?";
                                else if (epi.cardType == "Die.SPELL")
                                    man.nom_phase.text = "Prenez donc un sortilège";
                                else if (epi.cardType == "Die.ARMOR")
                                    man.nom_phase.text = "Sortez couvert.";
                                List<String> cartes_restantes = new List<string>();
                                for (int i = 0; i < cards[epi.cardType].Count; ++i)
                                {
                                    cartes_restantes.Add("Carte " + (i + 1));
                                }
                                man.choix_carte.ClearOptions();
                                man.choix_carte.AddOptions(cartes_restantes);
                                affichage_carte_select(0);
                                man.choix_mise.ClearOptions();
                                List<String> num_mise = new List<string>();
                                for (int i = 0; i < man.thisHero.life_points; ++i)
                                {
                                    num_mise.Add(i + "pv");
                                }
                                man.choix_mise.AddOptions(num_mise);
                                mise = true;
                            });
                            break;
                        case "EQUIP_PHASE_ASK":
                            var epa = JsonConvert.DeserializeObject<EquipPhaseAsk>(m.Content);
                            var heroAsked = heroes.First(h => h.id == epa.heroId);
                            if (heroAsked != man.thisHero)
                            {
                                chat($"{heroAsked.name} est en train de choisir sa carte...");
                            }
                            else
                            {
                                Manager.RunOnMainThread.Enqueue(() =>
                                {
                                    man.valider_mise.interactable = true;
                                });
                                chat("Choisissez votre carte :");
                                mise = false;
                            }
                            break;
                        case "EQUIP_PHASE_PLAYER":
                            var epp = JsonConvert.DeserializeObject<EquipPhasePlayer>(m.Content);
                            var selectedCard = cards[epp.cardType].First(c => c.id == epp.cardId);
                            var heroEquip = heroes.First(h => h.id == epp.heroId);
                            heroEquip.cards[epp.cardType] = selectedCard;
                            chat($"{heroEquip.name} a choisi {selectedCard.name}");
                            if (man.thisHero.id != heroEquip.id && epp.cardType == "Die.SPELL")
                            {
                                Manager.RunOnMainThread.Enqueue(() =>
                                {
                                    heroEquip.timeline = ordreJeu++;
                                    if (ordreJeu == 1)
                                    {
                                        man.panel_ennemi_1.GetComponent<clikenmi>().man = man;
                                        man.panel_ennemi_1.GetComponent<clikenmi>().h = heroEquip;
                                    }
                                    else if (ordreJeu == 2)
                                    {
                                        man.panel_ennemi_2.GetComponent<clikenmi>().man = man;
                                        man.panel_ennemi_2.GetComponent<clikenmi>().h = heroEquip;
                                    }
                                    else if (ordreJeu == 3)
                                    {
                                        man.panel_ennemi_3.GetComponent<clikenmi>().man = man;
                                        man.panel_ennemi_3.GetComponent<clikenmi>().h = heroEquip;
                                    }
                                    else if (ordreJeu == 4)
                                    {
                                        man.panel_ennemi_4.GetComponent<clikenmi>().man = man;
                                        man.panel_ennemi_4.GetComponent<clikenmi>().h = heroEquip;
                                    }
                                });
                            }
                            break;
                    }
                }
                else if (m.Phase == "PREP")
                {
                    Manager.RunOnMainThread.Enqueue(() =>
                    {
                        man.equip_phase.SetActive(false);
                        man.panel_carte.SetActive(false);
                        man.panelCbt.SetActive(true);
                        man.carte1.GetComponent<over>().man = man;
                        man.carte1.GetComponent<over>().type = "Die.WEAPON";
                        man.carte2.GetComponent<over>().man = man;
                        man.carte2.GetComponent<over>().type = "Die.MOUNT";
                        man.carte3.GetComponent<over>().man = man;
                        man.carte3.GetComponent<over>().type = "Die.ARMOR";
                        man.carte4.GetComponent<over>().man = man;
                        man.carte4.GetComponent<over>().type = "Die.SPELL";

                        man.txt_carte_1.text = man.thisHero.cards["Die.WEAPON"].name;
                        man.txt_carte_2.text = man.thisHero.cards["Die.MOUNT"].name;
                        man.txt_carte_3.text = man.thisHero.cards["Die.ARMOR"].name;
                        man.txt_carte_4.text = man.thisHero.cards["Die.SPELL"].name; // grouper les txt cartes pour remplir avec un double for
                        Image[] couts_atk = { man.atk_cout_1, man.atk_cout_2 };
                        Image[] couts_mov = { man.mov_cout_1, man.mov_cout_2 };
                        Image[] couts_arm = { man.shield_cout_1, man.shield_cout_2 };
                        Image[] couts_pow = { man.pow_cout_1, man.pow_cout_2 };
                        charge_sprite(man.thisHero.cards["Die.WEAPON"].requirements, couts_atk);
                        charge_sprite(man.thisHero.cards["Die.MOUNT"].requirements, couts_mov);
                        charge_sprite(man.thisHero.cards["Die.ARMOR"].requirements, couts_arm);
                        charge_sprite(man.thisHero.cards["Die.SPELL"].requirements, couts_pow);
                        man.endturnBtn.interactable = false;
                        man.relanceBtn.interactable = false;    
                        maj_pv();
                        for (int i = 0; i < heroes.Count - 1; ++i)
                        {
                            man.panel_ennemi[i].SetActive(true);
                        }
                        switch (m.Type)
                        {
                            case "PREP_PHASE_INIT":
                                var ppi = JsonConvert.DeserializeObject<PrepPhaseInit>(m.Content);
                                var heroAsked = heroes.First(h => h.id == ppi.heroId);
                                if (heroAsked != man.thisHero)
                                {
                                    chat($"{heroAsked.name} est en train de choisir sa position...");
                                }
                                else
                                {
                                    chat("Choisissez votre position :");
                                    man.availablePositions = ppi.availablePositions;
                                    man.clicTrigger = true;
                                }
                                break;
                            case "PREP_PHASE_POSITION":
                                var ppp = JsonConvert.DeserializeObject<PrepPhasePosition>(m.Content);
                                var heroPos = heroes.First(h => h.id == ppp.heroId);
                                heroPos.position = ppp.selectedPosition;
                                if (man.thisHero.id != heroPos.id)
                                {
                                    heroPos.realplayer = UnityEngine.Object.Instantiate(man.sp_perso[heroPos.timeline], new Vector3(position_unity[heroPos.position.y][heroPos.position.x / 2].x, position_unity[heroPos.position.y][heroPos.position.x / 2].y, -1), Quaternion.identity);
                                }
                                else
                                {
                                    heroPos.realplayer = UnityEngine.Object.Instantiate(man.player, new Vector3(position_unity[heroPos.position.y][heroPos.position.x / 2].x, position_unity[heroPos.position.y][heroPos.position.x / 2].y, -1), Quaternion.identity);
                                }
                                break;
                        }
                    });
                }
                else if (m.Phase == "TURN")
                {
                    Manager.RunOnMainThread.Enqueue(() =>
                    {
                        switch (m.Type)
                        {
                            case "TURN_PHASE_INIT"://maj fatigue / armure
                                var tpi = JsonConvert.DeserializeObject<TurnPhaseInit>(m.Content);
                                heroTurn = tpi.hero;
                                maj_des();
                                if (tpi.hero.id != man.thisHero.id)
                                    chat($"{heroTurn.name} réfléchit à ce qu'il doit faire...");
                                else
                                {
                                    man.endturnBtn.interactable = true;
                                    chat("A vous de jouer !");
                                    if (tpi.actions.canReplayDice)
                                        man.relanceBtn.interactable = true;
                                    //Console.WriteLine("Que faire ?  ACTION (CARD|GENERIC) <TYPE>, REPLAY_DICE [<TYPE>:int, ...], END_TURN)");
                                    //gerer les cartes
                                }
                                break;
                            case "TURN_PHASE_REPLAY_UPDATE":
                                chat($"{heroTurn.name} a effectué une relance !");
                                break;
                            case "TURN_PHASE_ACTION_UPDATE":
                                var tpau = JsonConvert.DeserializeObject<TurnPhaseActionUpdate>(m.Content);
                                if (tpau.card == "CARD")
                                    chat($"{heroTurn.name} a activé {heroTurn.cards[tpau.type]}");
                                else if (tpau.card == "GENERIC")
                                    chat($"{heroTurn.name} a lancé une attaque de type {tpau.type}");
                                break;
                            case "TURN_PHASE_PLAYER"://maj fatigue & armure
                                var tpp = JsonConvert.DeserializeObject<TurnPhasePlayer>(m.Content);
                                heroes = tpp.heroes;
                                maj_pv();
                                maj_des();
                                break;
                            case "TURN_PHASE_PENDING_POSITION":
                                var tppp = JsonConvert.DeserializeObject<TurnPhasePendingPositions>(m.Content);
                                var heroChoosingPos = heroes.First(h => h.id == tppp.heroId);
                                if (heroChoosingPos.id != man.thisHero.id)
                                    chat($"{heroChoosingPos.name} choisit une position...");
                                else
                                {
                                    chat("Choisis une position :");
                                    man.availablePositions = tppp.positions;
                                    man.clicTrigger = true;
                                }
                                break;
                            case "TURN_PHASE_PENDING_POSITION_UPDATE":
                                var tpppu = JsonConvert.DeserializeObject<TurnPhasePendingPositionUpdate>(m.Content);
                                var heroUpdate = heroes.First(h => h.id == tpppu.heroId);
                                heroUpdate.position = tpppu.position; // maj pos
                                Vector3 destPositionPlayer = new Vector3(position_unity[heroUpdate.position.y][heroUpdate.position.x].x, position_unity[heroUpdate.position.y][heroUpdate.position.x].y, -1);
                                heroUpdate.realplayer.transform.position = new Vector3(
                                                      Mathf.Lerp(heroUpdate.realplayer.transform.position.x, destPositionPlayer.x, 0.2f),
                                                      Mathf.Lerp(heroUpdate.realplayer.transform.position.y, destPositionPlayer.y, 0.2f),
                                                      -1
                                                      );
                                break;
                            case "TURN_PHASE_PENDING_HERO": //choix cible
                                var tpph = JsonConvert.DeserializeObject<TurnPhasePendingHero>(m.Content);
                                var heroChoosing = heroes.First(h => h.id == tpph.heroId);
                                if (heroChoosing != man.thisHero)
                                    chat($"{heroChoosing.name} choisit une cible...");
                                else
                                {
                                    for (var j = 0; j < tpph.nbToChoose; j++)
                                    {
                                        chat("Choisis un héros :");
                                        man.clicTriggerFdp = true;
                                        man.availablePositions = heroes.Select(h => h.position).ToList();
                                        //tpph.heroesId.Remove(tpph.heroesId[indSelected]);
                                    }
                                }
                                break;
                            case "TURN_PHASE_PENDING_HERO_UPDATE":
                                var tpphu = JsonConvert.DeserializeObject<TurnPhasePendingHeroUpdate>(m.Content);
                                var heroCaster = heroes.First(h => h.id == tpphu.heroId);
                                var heroesTarget = tpphu.heroes.Select(h => heroes.First(hs => hs.id == h));
                                chat($"{heroCaster.name} cible {string.Join(", ", heroesTarget.Select(ht => ht.name))}");
                                break;
                        }

                    });
                }
            }
        }
    }
}
