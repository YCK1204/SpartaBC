using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasePlayerController : MonoBehaviour
{
    [HideInInspector]
    public Rigidbody2D rb;
    protected Animator _anim;

    private void Start()
    {
        Init();
    }
    protected virtual void Init()
    {
        rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
    }
}