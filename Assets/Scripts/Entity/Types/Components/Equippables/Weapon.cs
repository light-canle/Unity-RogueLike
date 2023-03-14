sealed class Weapon : Equippable
{
    public Weapon()
    {
        EquipmentType = EquipmentType.Weapon;
    }

    private void OnValidate()
    {
        if (gameObject.transform.parent)
        {
            gameObject.transform.parent.GetComponent<Equipment>().Weapon = this;
        }
    }
}