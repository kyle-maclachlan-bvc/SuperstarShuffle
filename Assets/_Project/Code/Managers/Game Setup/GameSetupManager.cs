using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameSetupManager : MonoBehaviour
{
    [SerializeField] private TurnManager turnManager;

    private StateMachine setupStateMachine;
    public StateMachine StateMachine => setupStateMachine;

    private void Awake()
    {
        setupStateMachine = new StateMachine();
    }
    
    private void OnEnable()
    {
        GameEvents.OnDialogueFinished += HandleDialogueFinished;
    }

    private void OnDisable()
    {
        GameEvents.OnDialogueFinished -= HandleDialogueFinished;
    }
    
    private void Update()
    {
        setupStateMachine.Update();
        
        if (GameManager.Instance.CurrentGameState != GameState.GameSetup)
            return;
    }
    private void HandleDialogueFinished()
    {
        setupStateMachine.CurrentState?.DialogueFinished();
    }
    
    public void StartGameSetup()
    {
        foreach (Player player in GameManager.Instance.Players)
        {
            player.Data.CurrentSpaceIndex = player.Data.StartingSpaceIndex;
            player.Data.Coins = 0;
            player.Data.OwnedSpaceIndices.Clear();
        }
        
        GameEvents.OnGameSetupStarted?.Invoke();

        setupStateMachine.ChangeState(new SetupIntroState(this));
    }

    public void EnterRollState()
    {
        setupStateMachine.ChangeState(new SetupRollState(this));
    }

    public void EnterPresentationState()
    {
        setupStateMachine.ChangeState(new SetupPresentationState(this));
    }
    

    public void EnterStartingCoinsState()
    {
        setupStateMachine.ChangeState(new SetupStartingCoinsState(this));
    }

    public void BeginGameplay()
    {
        Debug.Log("Begin Gameplay");
        GameEvents.OnGameStateRequested?.Invoke(GameState.PlayerTurn);
    }
}
