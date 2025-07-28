using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMonobehaviour : MonoBehaviour
{
    IEnumerator InvokeAction(Action action, float time)
    {
        yield return new WaitForSeconds(time);
        action?.Invoke();
    }

    protected void Invoke(Action action, float time = 0)
    {
        StartCoroutine(InvokeAction(action, time));
    }
}
