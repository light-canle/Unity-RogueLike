sealed class Ring : Equippable
{
    public Ring()
    {
        EquipmentType = EquipmentType.Ring;
    }

    private void OnValidate()
    {
        if (gameObject.transform.parent)
        {
            gameObject.transform.parent.GetComponent<Equipment>().Ring = this;
        }
    }
}