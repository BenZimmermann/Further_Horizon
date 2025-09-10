using UnityEditor;
using UnityEngine;

public class PlanetDefinitionCreator
{
    [MenuItem("Tools/Create Planet Definition")]
    public static void CreatePlanetDefinition()
    {
        PlanetDefinition asset = ScriptableObject.CreateInstance<PlanetDefinition>();
        AssetDatabase.CreateAsset(asset, "Assets/NewPlanetDefinition.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }
}
