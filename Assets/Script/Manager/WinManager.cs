using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WinManager : MonoBehaviour
{
    public static WinManager Instance { get; private set; }
    public UnityEvent onGameWin;
    public UnityEvent onGameLose;
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    // Call to trigger win condition
    public void Win()
    {
        Debug.Log("WinManager: Player wins!");
        // change scene
        onGameWin?.Invoke();
    }

    // Call to trigger lose condition
    public void Lose()
    {
        Debug.Log("WinManager: Player loses!");
        //change scene
        onGameLose?.Invoke();
    }
}
