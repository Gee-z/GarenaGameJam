using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinManager : MonoBehaviour
{
    public static WinManager Instance { get; private set; }

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
    }

    // Call to trigger lose condition
    public void Lose()
    {
        Debug.Log("WinManager: Player loses!");
        //change scene
    }
}
