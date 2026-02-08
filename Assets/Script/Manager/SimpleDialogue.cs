using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class SimpleDialogue : MonoBehaviour
{
    public TMP_Text textBox;
    public List<string> lines = new List<string>();

    public float letterDelay = 0.03f;

    int currentIndex = 0;
    Coroutine typingRoutine;
    bool isTyping;

    void Awake()
    {
        textBox.text = "";
    }
    public void ShowNext()
    {
        if (isTyping)
        {
            StopCoroutine(typingRoutine);
            textBox.text = lines[currentIndex];
            isTyping = false;
            return;
        }
        if(typingRoutine != null)
            StopCoroutine(typingRoutine);
        if (currentIndex >= lines.Count)
        {
            EndDialogue();
            return;
        }

        typingRoutine = StartCoroutine(TypeLine(lines[currentIndex]));
        currentIndex++;
    }

    IEnumerator TypeLine(string line)
    {
        isTyping = true;
        textBox.text = "";

        for (int i = 0; i < line.Length; i++)
        {
            textBox.text += line[i];
            yield return new WaitForSeconds(letterDelay);
        }

        isTyping = false;
        yield return new WaitForSeconds(2.5f);
        textBox.text = "";
    }

    void EndDialogue()
    {
        textBox.text = "";
        currentIndex = 0;
    }
}
