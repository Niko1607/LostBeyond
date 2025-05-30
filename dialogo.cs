using System.Collections;
using UnityEngine;
using TMPro;

public class dialogo : MonoBehaviour
{
    
    [SerializeField] private gameObject dialogueMark;
    [SerializeField] private gameObject dialoguepanel;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField, TextArea(4, 6)] private string[] dialogueLines;

    private float typingTime = 0.05f;
    private bool isPlayerInRange;
    private bool didDialogueStart;
    private int lineIndex;

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown("Fire1"))
        {
            if (!didDialogueStart)
            {
                StartDialogue();
            }
            else if (dialogueText.Text == dialogueLines[lineIndex])
            {
                NextDiaLogueLine();
            }
            else
            {
                StopAllCoroutines(); 
                dialogueText.text = dialogueLines[lineIndex]; 
            }
        }

    }

    private void StartDialogue()
    {
        didDialogueStart = true;
        dialoguepanel.SetActive(true);
        dialogueMark.SetActive(false);
        lineIndex = 0;
        Time.timeScale = 0f; 
        StartCoroutine(ShowLine());
    }

    private IEnumerator ShowLine()
    {
        dialogueText.text = string.Empty; 

        foreach (char ch in dialogueLines[lineIndex])
        {
            dialogueText.text += ch;
            yield return new WaitForSecondsRealtime(typingTime); 
        }
    }

    private void NextDiaLogueLine()
    {
        lineIndex++;
        if (lineIndex < dialogueLines.Length)
        {
            StartCoroutine(ShowLine());
        }
        else
        {
            didDialogueStart = false;
            dialoguepanel.SetActive(false);
            dialogueMark.SetActive(true);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerInRange = true;
            dialogueMark.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerInRange = false;
            dialogueMark.SetActive(false);
        }
    }    
}