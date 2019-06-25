using Client.Models;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class over : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    public Manager man;
    public string type;
    public void OnPointerClick(PointerEventData eventData)
    {
        man.comm.send(JsonConvert.SerializeObject(new TurnPhaseAction(new Dictionary<string, string>()
                                            {
                                                {"card", "CARD"},
                                                {"type", type}
                                            })));
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        try
        {
            charge_sprite(man.thisHero.cards[type].requirements, new Image[] { man.cout_1, man.cout_2 });
            man.description_carte.text = man.thisHero.cards[type].description;
            man.nom_carte.text = man.thisHero.cards[type].name;
            man.panel_carte.SetActive(true);
        } catch (Exception) { }
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        try
        {
            man.panel_carte.SetActive(false);
        } catch (Exception) { }
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
