sealed class LeatherArmor : Equippable
{
    public LeatherArmor(int reinforce = 0)
    {
        EquipmentType = EquipmentType.Armor;
        DefenseBonus = 1;
        Reinforcement = reinforce;
    }

    private void OnValidate()
    {
        if (gameObject.transform.parent)
        {
            gameObject.transform.parent.GetComponent<Equipment>().Armor = this;
        }
    }
}