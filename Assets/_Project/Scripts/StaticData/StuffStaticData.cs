using UnityEngine;

namespace StaticData
{
    [CreateAssetMenu(fileName = "New Stuff Static Data", menuName = "StaticData/StuffStaticData")]
    public class StuffStaticData : ScriptableObject
    {
        public StuffSpecies StuffSpecies;
    }
    
    public enum StuffSpecies
    {
        None = 0,
        Decor = 1,
        Meal = 2,
        Cloths = 3,
        Stuff = 4,
        Toys = 5,
    }
}