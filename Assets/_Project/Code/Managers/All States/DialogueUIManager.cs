using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueUIManager : MonoBehaviour
{
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private TMP_Text speakerName;

    private Queue<string> dialogueQueue = new();

    private bool dialogueActive;
    private bool ignoreConfirmThisFrame;

    public bool DialogueActive => dialogueActive;

    private void Awake()
    {
        dialoguePanel.SetActive(false);
        
    }

    private void OnEnable()
    {
        GameEvents.OnDialogueRequested += StartDialogue;

    }

    private void OnDisable()
    {
        GameEvents.OnDialogueRequested -= StartDialogue;

    }
    

    private void Update()
    {
        if (!dialogueActive)
            return;

        if (ignoreConfirmThisFrame)
        {
            ignoreConfirmThisFrame = false;
            return;
        }

        if (InputManager.Instance.Controls.GameSetup.Confirm.WasPressedThisFrame())
        {
            ShowNextMessage();
        }

    }

    public void StartDialogue(string speaker, string[] messages)
    {
        Debug.Log("StartDialogue()");
        
        dialogueQueue.Clear();

        foreach (string message in messages)
            dialogueQueue.Enqueue(message);

        speakerName.text = speaker;
        
        dialoguePanel.SetActive(true);

        dialogueActive = true;
        ignoreConfirmThisFrame = true;

        ShowNextMessage();
    }

    public void ShowNextMessage()
    {
        if (dialogueQueue.Count == 0)
        {
            EndDialogue();
            return;
        }

        dialogueText.text = dialogueQueue.Dequeue();
    }

    private void EndDialogue()
    {
        dialoguePanel.SetActive(false);

        dialogueActive = false;

        GameEvents.OnDialogueFinished?.Invoke();
    }
}
