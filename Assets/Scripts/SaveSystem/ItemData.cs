using System;

[Serializable]
public class ItemData
{
    public string itemId;       // eindeutige ID
    public string displayName;  // Name für die UI
    public bool isRequired;     // Pflicht-Item fürs Level?
    public int collected;       // Wie viele wurden gesammelt?
    public int maxCount;        // Maximalanzahl (1 = Pflicht, 100 = Sammel-Item)

    public int dummy1;
    public int dummy2;
}
