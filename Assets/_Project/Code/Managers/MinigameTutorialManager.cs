using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Responsibilities:
/// - Display Minigame Information
/// - Wait for Players to continue
/// - Load Minigame
/// </summary>

public class MinigameTutorialManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text controlsText;
    [SerializeField] private TMP_Text rulesText;
    [SerializeField] private Image previewImage;

    // Store Ready States
    private bool player1Ready;
    private bool player2Ready;
    private bool player3Ready;
    private bool player4Ready;
    
    private void Start()
    {        
        Debug.Log($"GameManager = {GameManager.Instance}"); 
        Debug.Log($"InputManager = {InputManager.Instance}"); 
        Debug.Log($"MinigameManager = {GameManager.Instance}");
        
        LoadMinigameData();
    }
    
    private void Update()
    {
        if (InputManager.Instance.Controls.GameSetup.Confirm.WasPressedThisFrame())
            StartMinigame();
    }

    private void LoadMinigameData()
    {
        Debug.Log($"GameManager = {GameManager.Instance}");
        
        if (GameManager.Instance.CurrentMinigame != null)
            Debug.Log($"Current Minigame = {GameManager.Instance.CurrentMinigame}");


        
        MinigameData data = GameManager.Instance.CurrentMinigame;

        titleText.text = data.MinigameName;
        controlsText.text = data.ControlsText;
        rulesText.text = data.RulesText;
        previewImage.sprite = data.PreviewImage;
    }
    
    private void StartMinigame()
    {
        MinigameData data = GameManager.Instance.CurrentMinigame;
        GameManager.Instance.ChangeGameState(GameState.Minigame);
        SceneManager.LoadScene(data.SceneName);
    }
}
