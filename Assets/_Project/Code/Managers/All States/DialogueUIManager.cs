using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class DialogueUIManager : Singleton<DialogueUIManager>
{
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private GameObject dialogueTextObject;
    [SerializeField] private GameObject speakerNameObject;
    
    private TMP_Text dialogueText;
    private TMP_Text speakerName;
    
    private GameObject dialoguePanelInstance;
    private GameObject dialogueTextInstance;
    private GameObject speakerNameInstance;
    
    private GameObject dialogueBoxCanvas;
    

    private Queue<string> dialogueQueue = new();

    private bool dialogueActive;
    private bool ignoreConfirmThisFrame;

    public bool DialogueActive => dialogueActive;

    protected override void Awake()
    {
        base.Awake();

        dialogueBoxCanvas = FindFirstObjectByType<DialogueBoxCanvas>().GameObject();

        dialoguePanel = Resources.Load<GameObject>("IndexedPrefabs/DialogueBoxPanel");
        dialogueTextObject = Resources.Load<GameObject>("IndexedPrefabs/DialogueText");
        speakerNameObject = Resources.Load<GameObject>("IndexedPrefabs/CharacterName");

        if (dialogueBoxCanvas == null)
            return;
        
        dialoguePanelInstance = Instantiate(dialoguePanel, dialogueBoxCanvas.transform);
        dialogueTextInstance = Instantiate(dialogueTextObject, dialoguePanelInstance.transform);
        speakerNameInstance = Instantiate(speakerNameObject, dialoguePanelInstance.transform);
        
        
        
        dialogueText = dialogueTextInstance.GetComponent<TMP_Text>();
        speakerName = speakerNameInstance.GetComponent<TMP_Text>();
        
        //DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        
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
        dialogueQueue.Clear();

        foreach (string message in messages)
            dialogueQueue.Enqueue(message);

        if(speakerName != null)speakerName.text = speaker;
        
        
        dialoguePanelInstance.SetActive(true);

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
        dialoguePanelInstance.SetActive(false);

        dialogueActive = false;

        GameEvents.OnDialogueFinished?.Invoke();
    }
}
