sealed class Armor : Equippable
{
    public Armor()
    {
        EquipmentType = EquipmentType.Armor;
    }

    private void OnValidate()
    {
        if (gameObject.transform.parent)
        {
            gameObject.transform.parent.GetComponent<Equipment>().Armor = this;
        }
    }
}