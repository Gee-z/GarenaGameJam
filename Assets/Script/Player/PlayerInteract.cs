using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInteract : MonoBehaviour
{
    public bool inRange = false;
    public UnityEvent onInteract;
    public UnityEvent onEnter;
    public UnityEvent onExit;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Interactable"))
        {
            Debug.Log("In Range");
            inRange = true;
            onEnter?.Invoke();
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Interactable"))
        {
            Debug.Log("out Range");
            inRange = false;
            onExit?.Invoke();
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && inRange)
        {
            Debug.Log("Interacted");
            onInteract?.Invoke();
        }
    }
}
