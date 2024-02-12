sealed class Sword : Equippable
{
    public Sword(int reinforce = 0)
    {
        EquipmentType = EquipmentType.Weapon;
        PowerBonus = 4;
        Reinforcement = reinforce;
    }

    private void OnValidate()
    {
        if (gameObject.transform.parent)
        {
            gameObject.transform.parent.GetComponent<Equipment>().Weapon = this;
        }
    }
}