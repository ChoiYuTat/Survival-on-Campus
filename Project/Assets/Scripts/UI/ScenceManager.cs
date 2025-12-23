using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenceManager : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadSceneAsync(sceneName);
    }   
}
