using UnityEngine;

public enum RingBonusType{
    None,
    Health,
    Power,
    Defense,
    Evasion,
    Accuracy,
    FireResistance,
    PoisonResistance,
}

public enum EnchantType{
    None,
    Fire,
    Electricity,
    Ice,
}

[RequireComponent(typeof(Item))]
public class Equippable : MonoBehaviour
{
    [SerializeField] private EquipmentType equipmentType;
    [SerializeField] private RingBonusType? ringBonusType;
    [SerializeField] private EnchantType? enchantType;
    [SerializeField] private int powerBonus = 0;
    [SerializeField] private int defenseBonus = 0;
    [SerializeField] private int evasionBonus = 0, accuracyBonus = 0;
    [SerializeField] private float fireResistanceBonus = 0.0f, poisonResistanceBonus = 0.0f;
    [SerializeField] private int reinforcement = 0;
    
    public EquipmentType EquipmentType { get => equipmentType; set => equipmentType = value; }
    public RingBonusType? RingBonusType { get => ringBonusType; set => ringBonusType = value; }
    public EnchantType? EnchantType { get => enchantType; set => enchantType = value; }
    public int PowerBonus { get => powerBonus; set => powerBonus = value;}
    public int DefenseBonus { get => defenseBonus; set => defenseBonus = value; }
    public int EvasionBonus { get => evasionBonus; set => evasionBonus = value; }
    public int AccuracyBonus { get => accuracyBonus; set => accuracyBonus = value; }
    public float FireResistanceBonus
    {
        get => fireResistanceBonus;
        set => fireResistanceBonus = value;
    }
    public float PoisonResistanceBonus
    {
        get => poisonResistanceBonus;
        set => poisonResistanceBonus = value;
    }
    public int Reinforcement { get => reinforcement; set => reinforcement = value; }
}
