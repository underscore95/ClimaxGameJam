using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string _gameScene;

    public void Play()
    {
        SceneManager.LoadScene(_gameScene, LoadSceneMode.Single);
    }

    public void Exit()
    {
        Application.Quit();
    }
}