using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdController : BasePlayerController
{
    [Header("Movement Settings")]
    [SerializeField] float JumpForce = 5f;
    
    [Header("Rotation Settings")]
    [SerializeField] float initialRotationBoost = 300f;    // 클릭 시 즉각 회전력
    [SerializeField] float rotationGravity = 180f;         // 하향 회전 중력값 (각속도/초)
    [SerializeField] float maxUpwardAngle = 45f;           // 최대 상향 각도
    [SerializeField] float maxDownwardAngle = -45f;        // 최대 하향 각도
    
    // Private variables

    private float rotationVelocity = 0f;                   // 현재 회전 속도
    private void Update()
    {
        if (!Manager.Game.IsGameRunning)
            return;
        HandleInput();
        UpdateRotation();
    }
    
    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // 점프 처리
            rb.velocity = Vector2.zero;
            rb.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
            
            // 즉각적인 상향 회전력 추가
            rotationVelocity = initialRotationBoost;
        }
    }
    
    private void UpdateRotation()
    {
        // 중력처럼 지속적으로 하향 회전력 추가
        rotationVelocity -= rotationGravity * Time.deltaTime;
        
        // 현재 Z축 회전값 가져오기 (-180 ~ 180 범위로 정규화)
        float currentZRotation = transform.eulerAngles.z;
        if (currentZRotation > 180f) 
            currentZRotation -= 360f;
        
        // 회전 속도를 실제 회전에 적용
        float newRotation = currentZRotation + (rotationVelocity * Time.deltaTime);
        
        // 회전 각도 제한
        newRotation = Mathf.Clamp(newRotation, maxDownwardAngle, maxUpwardAngle);
        
        // 제한된 각도로 회전 적용
        transform.rotation = Quaternion.Euler(0, 0, newRotation);
        
        // 각도 제한에 도달했을 때 회전 속도 조정
        if (newRotation >= maxUpwardAngle && rotationVelocity > 0f)
        {
            rotationVelocity = 0f; // 위쪽 제한에 도달하면 상향 회전 속도 제거
        }
        else if (newRotation <= maxDownwardAngle && rotationVelocity < 0f)
        {
            rotationVelocity = 0f; // 아래쪽 제한에 도달하면 하향 회전 속도 제거
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!Manager.Game.IsGameRunning)
            return;
        _anim.SetBool("IsDead", true);
        Manager.Game.EndGame();
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Pass")
        {
            var manager = Manager.Game as BirdGameManager;
            manager.CountPass();
        }
    }
}