using UnityEngine;

namespace UI
{
    public abstract class Dialog : MonoBehaviour
    { 
        [SerializeField] protected Canvas _canvas;
        protected void Hide()
       {
           _canvas.gameObject.SetActive(false);
       }
    }
}
