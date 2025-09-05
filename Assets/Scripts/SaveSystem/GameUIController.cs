using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Text;
using TMPro;

public class GameUIController : MonoBehaviour
{
    [Header("TMP Felder")]
    [SerializeField] private TextMeshProUGUI line1;
    [SerializeField] private TextMeshProUGUI line2;
    [SerializeField] private TextMeshProUGUI line3;
    [SerializeField] private TextMeshProUGUI line4;
    [SerializeField] private TextMeshProUGUI line5;
    [SerializeField] private TextMeshProUGUI line6;
    [SerializeField] private TextMeshProUGUI line7;
    [SerializeField] private TextMeshProUGUI line8;

    private GameSaveData saveData; //Behälter 

    private void Start()
    {
        // Beim Start versuchen zu laden, sonst neuen Save anlegen
        if (!SaveSystem.Load(out saveData))
        {
            saveData = CreateDefaultSave(); // neuen Spielstand bauen
            SaveSystem.Save(saveData); // und sofort als Datei anlegen
        }

        RefreshUI();
    }
    public void OnClick_Save()
    {
        SaveSystem.Save(saveData);
    }
    public void OnClick_Load() 
    {
        if (SaveSystem.Load(out saveData))
            RefreshUI();
    }

    // UI mit Daten befüllen
    private void RefreshUI()
    {
        // Sicherheits-Checks
        if (saveData == null || saveData.levels == null || saveData.levels.Count == 0)
        {
            if (line1) line1.text = "Keine Leveldaten.";
            if (line2) line2.text = "Keine Leveldaten.";
            return;
        }
        // Szene = Level: aktueller Szenenname bestimmt Level
        string currentScene = SceneManager.GetActiveScene().name;

        // Suche Level nach Szenenname
        var candidates = saveData.levels.Where(l => l.sceneName == currentScene).ToList();
        LevelSaveData level = null;

        if (candidates.Count == 0)
        {
            // Kein passender Szenenname gefunden → harte, sichtbare Rückmeldung
            if (line1) line1.text = $"Kein Level-Datensatz für Szene '{currentScene}' gefunden.";
            if (line2) line2.text = $"Kein Level-Datensatz für Szene '{currentScene}' gefunden.";
            return;
        }
        else if (candidates.Count > 1)
        {
            // WARNUNG: doppelte Szenennamen → Unity kann die eindeutig nicht unterscheiden.
            // Es wird der erste Treffer verwendet.
            level = candidates[0];
        }
        else
        {
            level = candidates[0];
        }

        // Items laden und gruppieren
        var items = level.items ?? new System.Collections.Generic.List<ItemData>();
        var required    = items.Where(i => i.isRequired).ToList();
        var collectable = items.Where(i => !i.isRequired).ToList();

        // ---------- line1: displayName | collected | dummy1 ----------
        var sb1 = new StringBuilder(256);
        sb1.AppendLine($"Level {level.levelIndex} ({level.sceneName}) — Abgeschlossen: {level.isCompleted}");
        sb1.AppendLine();
        sb1.AppendLine("Pflicht-Items:");
        if (required.Count == 0) sb1.AppendLine("  — Keine —");
        else foreach (var it in required) sb1.AppendLine($"  • {it.displayName} | {it.collected} | {it.dummy1}");

        sb1.AppendLine();
        sb1.AppendLine("Sammel-Items:");
        if (collectable.Count == 0) sb1.AppendLine("  — Keine —");
        else foreach (var it in collectable) sb1.AppendLine($"  • {it.displayName} | {it.collected} | {it.dummy1}");

        if (line1) line1.text = sb1.ToString();

        // ---------- line2: displayName | collected | dummy2 ----------
        var sb2 = new StringBuilder(256);
        sb2.AppendLine($"Level {level.levelIndex} ({level.sceneName}) — Abgeschlossen: {level.isCompleted}");
        sb2.AppendLine();
        sb2.AppendLine("Pflicht-Items:");
        if (required.Count == 0) sb2.AppendLine("  — Keine —");
        else foreach (var it in required) sb2.AppendLine($"  • {it.displayName} | {it.collected} | {it.dummy2}");

        sb2.AppendLine();
        sb2.AppendLine("Sammel-Items:");
        if (collectable.Count == 0) sb2.AppendLine("  — Keine —");
        else foreach (var it in collectable) sb2.AppendLine($"  • {it.displayName} | {it.collected} | {it.dummy2}");

        if (line2) line2.text = sb2.ToString();
    }


    // Default Datei anlegen, damit später keine fehler passieren
    private GameSaveData CreateDefaultSave()
    {
        var g = new GameSaveData(); // variable als zwischenspeicher für Gesamtspielstand
        // Jetzt jeden Planeten einzeln anlegen
        // Planet 1
        var p1 = new LevelSaveData(1, "Inventory");      // Szenenname 1
        g.levels.Add(p1);       // … Items hinzufügen …

        // Planet 2
        var p2 = new LevelSaveData(2, "Cryovista");  // Szenenname 2
        g.levels.Add(p2);       // … Items hinzufügen …

        // Planet 3
        var p3 = new LevelSaveData(3, "Singara");      // szenenname 3
        g.levels.Add(p3);       // … Items hinzufügen …

        return g;
    }
}
