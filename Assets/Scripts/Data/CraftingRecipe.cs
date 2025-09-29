using UnityEngine;

[CreateAssetMenu(fileName = "NewRecipe", menuName = "Crafting/Recipe")]
public class CraftingRecipe : ScriptableObject // Definiert ein Crafting-Rezept im Spiel  
{
    [Header("Crafting Output")]
    public ItemDefinition outputItem;   // Das hergestellte Item
    public int outputAmount = 1; // Menge des hergestellten Items

    [System.Serializable]
    public struct Ingredient 
    {
        public ItemDefinition item; // Das benötigte Item
        public int amount;          // Die benötigte Menge
    }
    [Header("Crafting Materials")] 
    public Ingredient[] ingredients; // Liste der benötigten Zutaten
}
