using StaticData;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Item", fileName = "New Item", order = 51)]
public class Item: ScriptableObject
{
    public ItemType Type;
    public string Name;
    public Sprite Sprite;
    public string Description;
    public int Price;
    public StuffSpecies StuffSpecies;
    
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(Name))
        {
            Name = name;
        }
    }
}
