using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerHandler : MonoBehaviour
{
    [SerializeField]
    UnityEvent EnterEvent;
    [SerializeField]
    UnityEvent ExitEvent;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "Player")
            EnterEvent?.Invoke();
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.name == "Player")
            ExitEvent?.Invoke();
    }
}
