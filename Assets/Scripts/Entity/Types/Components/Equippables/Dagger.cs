sealed class Dagger : Equippable
{
    public Dagger(int reinforce = 0)
    {
        EquipmentType = EquipmentType.Weapon;
        PowerBonus = 2;
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
