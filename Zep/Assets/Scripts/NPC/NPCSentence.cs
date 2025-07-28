using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class NPCSentence : MonoBehaviour
{
    [SerializeField]
    string[] Sentences;
    [SerializeField]
    GameObject ChatBoxPrefab;
    [SerializeField]
    Transform ChatPoint;
    [SerializeField]
    float LoopTime = 1.5f;
    [SerializeField]
    UnityEvent OnClickEvent;
    private void Start()
    {
        StartCoroutine(LoopDialogue());
    }
    IEnumerator LoopDialogue()
    {
        GameObject go = null;
        while (true)
        {
            yield return new WaitForSeconds(LoopTime);
            if (go == null)
            {
                go = Instantiate(ChatBoxPrefab, ChatPoint);
                go.GetComponent<ChatSystem>().OnDialogue(Sentences);
            }
        }
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            CheckMouseClick();
    }

    void CheckMouseClick()
    {
        Camera currentCamera = Camera.main;
        if (currentCamera == null)
            currentCamera = FindObjectOfType<Camera>();

        Vector3 mouseScreenPos = Input.mousePosition;
        Vector2 mouseWorldPos = currentCamera.ScreenToWorldPoint(mouseScreenPos);

        // 콜라이더 Bounds로 클릭 감지 (가장 확실한 방법)
        Collider2D selfCollider = GetComponent<Collider2D>();
        if (selfCollider != null && selfCollider.bounds.Contains(mouseWorldPos))
            OnClickEvent.Invoke();
    }
}
