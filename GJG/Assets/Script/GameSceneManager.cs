// by Ceren Birsu YILMAZ
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    // Loads the next scene in the build order.
    public void StartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    // Quits the application.
    public void QuitGame()
    {
        Application.Quit();
        
    }

    // Restarts the current scene by reloading it.
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
