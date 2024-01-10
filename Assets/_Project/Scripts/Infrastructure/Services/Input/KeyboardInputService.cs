using UnityEngine;

namespace Services.Input
{
    public class KeyboardInputService : IInputService
    {
        public Vector2 Horizontal { get; private set; }
        public Vector2 Vertical { get; private set; }

        public void Update()
        {
            Horizontal = UnityEngine.Input.GetKeyDown(KeyCode.A) 
                ? Vector2.left 
                : UnityEngine.Input.GetKeyDown(KeyCode.D) 
                    ? Vector2.right 
                    : Vector2.zero;
            
            Vertical = UnityEngine.Input.GetKeyDown(KeyCode.W) 
                ? Vector2.up 
                : UnityEngine.Input.GetKeyDown(KeyCode.S) 
                    ? Vector2.down 
                    : Vector2.zero;
        }
    }
}
