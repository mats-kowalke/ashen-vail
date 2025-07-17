using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TimerController : MonoBehaviour
{

    public XPHandler xPHandler;

    public int initialTime;
    public TMP_Text text;
    private int currentTime;

    public void Start()
    {
        this.currentTime = this.initialTime;
        StartCoroutine(DecrementRoutine());
    }

    private IEnumerator DecrementRoutine()
    {
        while (this.currentTime > 0)
        {
            this.text.text = ConvertTime(this.currentTime);
            yield return new WaitForSeconds(1);
            this.currentTime--;
        }
        LevelKeeper.levelScore = this.xPHandler.currentXP;
        SceneManager.LoadScene(0);
    }

    public void DecrementTime(int amount)
    {
        this.currentTime -= amount;
        this.text.text = this.ConvertTime(this.currentTime);
    }

    private String ConvertTime(int seconds)
    {
        int minutes = seconds / 60;
        if (minutes == 0) return seconds.ToString();
        else return minutes.ToString() + ":" + (seconds - minutes * 60).ToString();
    }

    public void EndGame(InputAction.CallbackContext context)
    {
        Debug.Log("Escaping game");
        if (context.performed)
        {
            LevelKeeper.levelScore = this.xPHandler.currentXP;
            SceneManager.LoadScene(0);
        }
    }
}
