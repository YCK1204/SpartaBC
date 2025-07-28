using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZepController : BasePlayerController
{
    int _walkCount = 0;
    int WalkCount
    {
        get { return _walkCount; }
        set
        {
            _walkCount = value;
            _anim.SetBool("IsWalking", _walkCount > 0);
        }
    }
    bool _isRunning = false;
    bool IsRunning
    {
        get { return _isRunning; }
        set
        {
            if (_isRunning == value)
                return;
            _isRunning = value;
            _anim.SetBool("IsRunning", _isRunning);
            if (_isRunning)
            {
                Speed *= 2; // Increase speed when running
            }
            else
            {
                Speed /= 2; // Normal speed when not running
            }
        }
    }
    int _animDirection = 0; // 0: down, 1: left, right, 2: up
    int AnimDirection
    {
        get { return _animDirection; }
        set
        {
            if (_animDirection == value)
                return;
            _animDirection = value;
            _anim.SetInteger("Direction", _animDirection);
        }
    }
    Vector2 _direction = Vector2.zero;
    Vector2 Direction
    {
        get { return _direction; }
        set
        {
            _direction = value;
            if (_direction == Vector2.zero)
            {
                WalkCount--;
                return;
            }

            WalkCount++;
            if (_direction == Vector2.down)
                AnimDirection = 0;
            else if (_direction == Vector2.left)
            {
                AnimDirection = 1;
                _spriteRenderer.flipX = false;
            }
            else if (_direction == Vector2.right)
            {
                AnimDirection = 1;
                _spriteRenderer.flipX = true;
            }
            else if (_direction == Vector2.up)
                AnimDirection = 2;
        }
    }
    SpriteRenderer _spriteRenderer;
    protected override void Init()
    {
        base.Init();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }
    void Update()
    {
        HandleInput();
        Move();
    }
    [SerializeField]
    float Speed;
    void Move()
    {
        var powerX = Input.GetAxis("Horizontal");
        var powerY = Input.GetAxis("Vertical");
        Vector3 nextPos = transform.position + Speed * Time.deltaTime * new Vector3(powerX, powerY, 0);
        transform.position = nextPos;
    }
    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            Direction = Vector2.left;
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            Direction = Vector2.right;
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            Direction = Vector2.up;
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            Direction = Vector2.down;

        if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.A))
            Direction = Vector2.zero;
        if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.D))
            Direction = Vector2.zero;
        if (Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.W))
            Direction = Vector2.zero;
        if (Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.S))
            Direction = Vector2.zero;

        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
            IsRunning = true;
        else if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))
            IsRunning = false;
    }
}
