using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reinforcement : Consumable
{
    public override bool Activate(Actor consumer)
    {
        //consumer.GetComponent<Player>().ToggleSelectMenu();
        Consume(consumer);
        UIManager.instance.ToggleSelectMenu(consumer);
        UIManager.instance.AddMessage("강화할 장비를 선택하세요.", "#00ff00");
        return true;
    }
}
