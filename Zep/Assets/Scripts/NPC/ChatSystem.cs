using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChatSystem : MonoBehaviour
{
    Queue<string> _sentences = new Queue<string>();
    [SerializeField]
    TextMeshPro _text;
    [SerializeField]
    GameObject ChatBox;
    public void OnDialogue(string[] lines)
    {
        foreach (string line in lines)
            _sentences.Enqueue(line);
        _text.GetComponent<MeshRenderer>().sortingOrder = 51; // Set the text to be on top of other UI elements
        StartCoroutine(DialogueFlow());
    }
    IEnumerator DialogueFlow()
    {
        while (_sentences.Count > 0)
        {
            string sentence = _sentences.Dequeue();
            _text.text = sentence;
            
            float width = _text.preferredWidth;
            float height = _text.preferredHeight;
            
            Transform transform = ChatBox.GetComponent<Transform>();
            transform.localScale = new Vector2(width + .3f, height + .15f);
            
            yield return new WaitForSeconds(3f); // Wait for 3 seconds before showing the next sentence
        }
        Destroy(gameObject); // Destroy the chat system after the dialogue is complete
    }
}
