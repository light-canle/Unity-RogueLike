sealed class ChainMail : Equippable
{
    public ChainMail(int reinforce = 0)
    {
        EquipmentType = EquipmentType.Armor;
        DefenseBonus = 3;
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
