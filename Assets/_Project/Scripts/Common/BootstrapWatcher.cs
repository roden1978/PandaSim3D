using Infrastructure;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BootstrapWatcher : MonoBehaviour
{
    void Start()
    {
        Bootstrapper bootstrapper = FindObjectOfType<Bootstrapper>();

        if (bootstrapper is null)
        {
            SceneManager.LoadScene("Bootstrap");
        }
        
        Destroy(this);
    }
}
