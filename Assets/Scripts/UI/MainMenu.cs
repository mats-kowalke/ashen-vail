using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public TMP_Text scoreText;

    public void Start()
    {
        this.scoreText.text = "Current high score: " + LevelKeeper.levelScore;
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}