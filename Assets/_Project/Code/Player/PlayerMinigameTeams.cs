using UnityEngine;

public class PlayerMinigameTeams : MonoBehaviour
{
    [SerializeField] private MinigameTeams currentTeam = MinigameTeams.None;

    public MinigameTeams CurrentTeam
    {
        get => currentTeam;
        set => currentTeam = value;
    }
}
