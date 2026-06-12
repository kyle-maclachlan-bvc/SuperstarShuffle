using UnityEngine;

[CreateAssetMenu(menuName = "Minigames/Minigame Data")]
public class MinigameData : ScriptableObject
{
    [Header("Identification")]
    [SerializeField] private string minigameName;

    [Header("Tutorial")]
    [TextArea]
    [SerializeField] private string controlsText;
    [TextArea]
    [SerializeField] private string rulesText;
    [SerializeField] private Sprite previewImage;

    [Header("Scene")]
    [SerializeField] private string sceneName;
    
    [Header("Minigame Type")]
    [SerializeField] private MinigameTypes minigameType;
    
    public string MinigameName => minigameName;
    public string ControlsText => controlsText;
    public string RulesText => rulesText;
    public Sprite PreviewImage => previewImage;
    public string SceneName => sceneName;
}
