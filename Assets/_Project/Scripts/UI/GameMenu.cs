using UnityEngine;

namespace UI
{
    public class GameMenu : MonoBehaviour
    {
        [field: SerializeField] public Pause Pause { get; private set; }
        [field: SerializeField] public Died Died { get; private set; }
        public void Construct()
        {
            
        }
        
        
    }
}
