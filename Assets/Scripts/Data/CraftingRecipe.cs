using UnityEngine;

[CreateAssetMenu(fileName = "NewRecipe", menuName = "Crafting/Recipe")]
public class CraftingRecipe : ScriptableObject
{
    [Header("Crafting Output")] // Was raus kommt am ende
    public ItemDefinition outputItem;
    public int outputAmount = 1;

    [System.Serializable]
    public struct Ingredient // Was ma neistecke muss
    {
        public ItemDefinition item;
        public int amount;
    }
    [Header("Crafting Materials")] // damits gut ausschaut
    public Ingredient[] ingredients;
}
