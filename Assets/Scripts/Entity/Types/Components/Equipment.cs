using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Actor))]
public class Equipment : MonoBehaviour
{
    [SerializeField] private Equippable weapon;
    [SerializeField] private Equippable armor;
    [SerializeField] private Equippable ring;

    public Equippable Weapon { get => weapon; set => weapon = value; }
    public Equippable Armor { get => armor; set => armor = value; }
    public Equippable Ring { get => ring; set => ring = value; }

    // 속성 보너스
    public int DefenseBonus()
    {
        int bonus = 0;

        if (weapon is not null && weapon.DefenseBonus > 0)
        {
            bonus += weapon.DefenseBonus;
        }

        if (armor is not null && armor.DefenseBonus > 0)
        {
            bonus += armor.DefenseBonus + armor.Reinforcement;
        }

        if (ring is not null && ring.DefenseBonus > 0)
        {
            if (ring.RingBonusType == RingBonusType.Defense){
                bonus += ring.Reinforcement;
            }

            bonus += ring.DefenseBonus;
        }
        return bonus;
    }

    public int PowerBonus()
    {
        int bonus = 0;

        if (weapon is not null && weapon.PowerBonus > 0)
        {
            bonus += weapon.PowerBonus + weapon.Reinforcement;
        }

        if (armor is not null && armor.PowerBonus > 0)
        {
            bonus += armor.PowerBonus;
        }

        if (ring is not null && ring.PowerBonus > 0)
        {
            if (ring.RingBonusType == RingBonusType.Power){
                bonus += ring.Reinforcement;
            }
            bonus += ring.PowerBonus;
        }
        return bonus;
    }

    public int EvasionBonus()
    {
        int bonus = 0;

        if (weapon is not null && weapon.EvasionBonus > 0)
        {
            bonus += weapon.EvasionBonus;
        }

        if (armor is not null && armor.EvasionBonus > 0)
        {
            bonus += armor.EvasionBonus;
        }

        if (ring is not null && ring.EvasionBonus > 0)
        {
            if (ring.RingBonusType == RingBonusType.Evasion){
                bonus += ring.Reinforcement;
            }

            bonus += ring.EvasionBonus;
        }
        return bonus;
    }

    public int AccuracyBonus()
    {
        int bonus = 0;

        if (weapon is not null && weapon.AccuracyBonus > 0)
        {
            bonus += weapon.AccuracyBonus;
        }

        if (armor is not null && armor.AccuracyBonus > 0)
        {
            bonus += armor.AccuracyBonus;
        }

        if (ring is not null && ring.AccuracyBonus > 0)
        {
            if (ring.RingBonusType == RingBonusType.Accuracy){
                bonus += ring.Reinforcement;
            }

            bonus += ring.AccuracyBonus;
        }
        return bonus;
    }

    public float FireResistanceBonus()
    {
        float bonus = 0;

        if (weapon is not null && weapon.FireResistanceBonus > 0)
        {
            bonus += weapon.FireResistanceBonus;
        }

        if (armor is not null && armor.FireResistanceBonus > 0)
        {
            bonus += armor.FireResistanceBonus;
        }

        if (ring is not null && ring.FireResistanceBonus > 0)
        {
            if (ring.RingBonusType == RingBonusType.FireResistance){
                bonus += ring.Reinforcement * 0.25f;
            }

            bonus += ring.FireResistanceBonus;
        }
        return bonus;
    }
    
    public float PoisonResistanceBonus()
    {
        float bonus = 0;

        if (weapon is not null && weapon.PoisonResistanceBonus > 0)
        {
            bonus += weapon.PoisonResistanceBonus;
        }

        if (armor is not null && armor.PoisonResistanceBonus > 0)
        {
            bonus += armor.PoisonResistanceBonus;
        }

        if (ring is not null && ring.PoisonResistanceBonus > 0)
        {
            if (ring.RingBonusType == RingBonusType.PoisonResistance){
                bonus += ring.Reinforcement * 0.25f;
            }

            bonus += ring.PoisonResistanceBonus;
        }
        return bonus;
    }

    public bool ItemIsEquipped(Item item)
    {
        if (item.Equippable is null)
        {
            return false;
        }

        return item.Equippable == weapon || item.Equippable == armor || item.Equippable == ring;
    }

    public void UnequipMessage(string name)
    {
        UIManager.instance.AddMessage($"당신은 {name}을(를) 장착 해제했다.", "#da8ee7");
    }

    public void EquipMessage(string name)
    {
        UIManager.instance.AddMessage($"당신은 {name}을(를) 장착했다.", "#a000c8");
    }

    public void EquipToSlot(string slot, Item item, bool addMessage)
    {
        Equippable currentItem;
        if (slot == "Weapon")
        {
            currentItem = weapon;
        }
        else if (slot == "Armor")
        {
            currentItem = armor;
        }
        else
        {
            currentItem = ring;
        }

        if (currentItem is not null)
        {
            UnequipFromSlot(slot, addMessage);
        }

        if (slot == "Weapon")
        {
            weapon = item.Equippable;
        }
        else if (slot == "Armor")
        {
            armor = item.Equippable;
        }
        else
        {
            ring = item.Equippable;
        }

        if (addMessage)
        {
            EquipMessage(item.name);
        }

        item.name = $"{item.name} (E)";
    }

    public void UnequipFromSlot(string slot, bool addMessage)
    {
        Equippable currentItem;
        if (slot == "Weapon")
        {
            currentItem = weapon;
        }
        else if (slot == "Armor")
        {
            currentItem = armor;
        }
        else
        {
            currentItem = ring;
        }

        currentItem.name = currentItem.name.Replace(" (E)", "");

        if (addMessage)
        {
            UnequipMessage(currentItem.name);
        }

        if (slot == "Weapon")
        {
            weapon = null;
        }
        else if (slot == "Armor")
        {
            armor = null;
        }
        else
        {
            ring = null;
        }
    }

    public void ToggleEquip(Item equippableItem, bool addMessage = true)
    {
        string slot;
        if (equippableItem.Equippable.EquipmentType == EquipmentType.Weapon)
        {
            slot = "Weapon";
        }
        else if (equippableItem.Equippable.EquipmentType == EquipmentType.Armor)
        {
            slot = "Armor";
        }
        else
        {
            slot = "Ring";
        }


        if (ItemIsEquipped(equippableItem))
        {
            UnequipFromSlot(slot, addMessage);
        }
        else
        {
            EquipToSlot(slot, equippableItem, addMessage);
        }
    }
}
