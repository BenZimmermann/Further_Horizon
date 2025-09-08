// InventoryPanelDataBridge.cs
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Beispiel-Brücke: Liest/Schreibt die Items des aktuellen Levels aus/in GameData,
/// und zeigt sie in TMPs an. Passe die Referenzen an deine Hierarchie an.
/// </summary>
public class InventoryPanelDataBridge : MonoBehaviour, IDataPersistence
{
    [Header("Anzeige")]
    [SerializeField] private TextMeshProUGUI line1; // displayName | collected | dummy1
    [SerializeField] private TextMeshProUGUI line2; // displayName | collected | dummy2

    private LevelSaveData currentLevelData;

    public void LoadData(GameData data)
    {
        // Szene = Level: finde Leveleintrag zur aktiven Szene
        string scene = SceneManager.GetActiveScene().name;
        currentLevelData = data.levels.FirstOrDefault(l => l.sceneName == scene);

        if (currentLevelData == null)
        {
            if (line1) line1.text = $"Kein Level-Datensatz für Szene '{scene}' gefunden.";
            if (line2) line2.text = $"Kein Level-Datensatz für Szene '{scene}' gefunden.";
            return;
        }

        // Anzeige aktualisieren, oben pflicht items unten commen items
        var req = currentLevelData.items.Where(i => i.isRequired).ToList();
        var col = currentLevelData.items.Where(i => !i.isRequired).ToList();

        var sb1 = new StringBuilder();
        sb1.AppendLine($"Level {currentLevelData.levelIndex} ({currentLevelData.sceneName}) — Pflicht/Sammel (Dummy1)");
        sb1.AppendLine("Pflicht-Items:");
        sb1.AppendLine(req.Count == 0 ? "  — Keine —" : string.Join("\n", req.Select(i => $"  • {i.displayName} | {i.collected} | {i.dummy1}")));
        sb1.AppendLine("\nSammel-Items:");
        sb1.AppendLine(col.Count == 0 ? "  — Keine —" : string.Join("\n", col.Select(i => $"  • {i.displayName} | {i.collected} | {i.dummy1}")));
        if (line1) line1.text = sb1.ToString();

        var sb2 = new StringBuilder();
        sb2.AppendLine($"Level {currentLevelData.levelIndex} ({currentLevelData.sceneName}) — Pflicht/Sammel (Dummy2)");
        sb2.AppendLine("Pflicht-Items:");
        sb2.AppendLine(req.Count == 0 ? "  — Keine —" : string.Join("\n", req.Select(i => $"  • {i.displayName} | {i.collected} | {i.dummy2}")));
        sb2.AppendLine("\nSammel-Items:");
        sb2.AppendLine(col.Count == 0 ? "  — Keine —" : string.Join("\n", col.Select(i => $"  • {i.displayName} | {i.collected} | {i.dummy2}")));
        if (line2) line2.text = sb2.ToString();
    }

    public void SaveData(GameData data)
    {
        // Hier würdest du UI→Daten zurückschreiben, falls du veränderbare Eingaben in der UI hast.
        // (pseudo): currentLevelData.items[...].collected = <aus UI>;
        data.lastSceneName = SceneManager.GetActiveScene().name;
    }
}
