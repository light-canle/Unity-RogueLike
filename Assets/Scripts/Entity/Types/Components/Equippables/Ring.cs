sealed class Ring : Equippable
{
    public Ring(int reinforce = 0)
    {
        EquipmentType = EquipmentType.Ring;
        Reinforcement = reinforce;
    }

    private void OnValidate()
    {
        if (gameObject.transform.parent)
        {
            gameObject.transform.parent.GetComponent<Equipment>().Ring = this;
        }
    }
}