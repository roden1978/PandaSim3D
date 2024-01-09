using UnityEngine;

namespace Infrastructure
{
    public class GameRunner : MonoBehaviour
    {
        [SerializeField] private Bootstrapper _bootstrapper;
        private void Awake()
        {
            Bootstrapper bootstrapper = FindObjectOfType<Bootstrapper>();
            if (bootstrapper == null)
                Instantiate(_bootstrapper);
        }
    }
}