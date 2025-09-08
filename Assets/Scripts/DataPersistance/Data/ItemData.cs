using System;

/// <summary>
/// Ein Item im Level (Pflicht oder sammelbar), hier einfach die item daten eintragen
/// alle infos zu jedem Item in unserem spiel hier hinterlegen
/// später kann man aussortieren was man anzeigen lassen will 
/// so mein grundgedanke
/// </summary>
[Serializable]
public class ItemData
{
    public string itemId;        // eindeutige ID
    public string displayName;   // Anzeigename
    public bool isRequired;      // Pflicht-Item oder sammel item als abfrage gedacht
    public int collected;        // aktuell als zähler gedacht
    public int maxCount;         // Limit für die items 
    // optional: deine bereits vorhandenen Zusatzfelder (dummy1, dummy2, …)
    public string dummy1; // hab ich zum testn angelegt
    public string dummy2; // das auch zum testn

    // für später falls wir das wollen, könnte man auch die positionen abspeichern
    /*
    public position = new float[3];
    position[0] = item.transform.position.x;
    position[1] = item.transform.position.x;
    position[2] = item.transform.position.x;
    */
    // So in der art könnte man das eventuell anlegen -> zum merken Yusuf

}
