using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadSetupScene()
    {
        SceneManager.LoadScene("SetupScene");
    }
}
