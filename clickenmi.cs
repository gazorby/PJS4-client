using Client.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//display the cards of an ennemi when his panel (on the top left corner) is clicked.
public class clikenmi : MonoBehaviour, IPointerClickHandler
{
    public Manager man;
    public Hero h;
    public void OnPointerClick(PointerEventData eventData)
    {

        man.nom_joueur_panel_ennemi.text = h.name;
        charge_sprite(h.cards["Die.WEAPON"].requirements, new Image[] { man.cout_1_carte_ennemi_1, man.cout_2_carte_ennemi_1 });
        charge_sprite(h.cards["Die.MOUNT"].requirements, new Image[] { man.cout_1_carte_ennemi_2, man.cout_2_carte_ennemi_2 });
        charge_sprite(h.cards["Die.ARMOR"].requirements, new Image[] { man.cout_1_carte_ennemi_3, man.cout_2_carte_ennemi_3 });
        charge_sprite(h.cards["Die.SPELL"].requirements, new Image[] { man.cout_1_carte_ennemi_4, man.cout_2_carte_ennemi_4 });
        man.description_carte_ennemi_1.text = h.cards["Die.WEAPON"].description;
        man.description_carte_ennemi_2.text = h.cards["Die.MOUNT"].description;
        man.description_carte_ennemi_3.text = h.cards["Die.ARMOR"].description;
        man.description_carte_ennemi_4.text = h.cards["Die.SPELL"].description;
        man.nom_carte_ennemi_1.text = h.cards["Die.WEAPON"].name;
        man.nom_carte_ennemi_2.text = h.cards["Die.MOUNT"].name;
        man.nom_carte_ennemi_3.text = h.cards["Die.ARMOR"].name;
        man.nom_carte_ennemi_4.text = h.cards["Die.SPELL"].name;

        man.panel_cartes_ennemi.SetActive(true);
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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
