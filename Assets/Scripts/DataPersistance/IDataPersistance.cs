// IDataPersistence.cs
public interface IDataPersistence
{
    /// <summary>
    /// Ziehe Daten AUS GameData IN dieses Objekt (beim Laden).
    /// </summary>
    void LoadData(GameData data);

    /// <summary>
    /// Schreibe Daten AUS diesem Objekt IN GameData (beim Speichern).
    /// </summary>
    void SaveData(GameData data);
}
