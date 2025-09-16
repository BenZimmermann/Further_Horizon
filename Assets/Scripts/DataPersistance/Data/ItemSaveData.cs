using System;

[Serializable]
public class ItemSaveData
{
    public string itemId;   // Referenz zu ItemDefinition.itemId
    public int collected;   // wie viele der Spieler davon besitzt

    // Optional erweiterbar, z. B.:
    // public bool isEquipped;
    // public int durability;

}
