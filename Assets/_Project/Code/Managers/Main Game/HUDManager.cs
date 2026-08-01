using UnityEngine;

public class HUDManager : MonoBehaviour
{
    [SerializeField] private HUDPlayerPanel[] panels;

    private int nextPanelIndex;

    private void Awake()
    {
        foreach (HUDPlayerPanel panel in panels)
            panel.gameObject.SetActive(false);

        nextPanelIndex = 0; 
    }

    private void OnEnable()
    {
        GameEvents.OnGameStateChange += HandleGameStateChanged;
        
        GameEvents.OnGameSetupStarted += ResetHUD;
        GameEvents.OnBoardReady += RebuildHUD;

        GameEvents.OnTurnOrderConfirmed += HandleTurnOrderConfirmed;

        GameEvents.OnPropertyPurchased += HandleHUDUpdate;
        GameEvents.OnRentPaid += HandleHUDUpdate;

    }

    private void OnDisable()
    {
        GameEvents.OnGameStateChange -= HandleGameStateChanged;
        
        GameEvents.OnGameSetupStarted -= ResetHUD;
        GameEvents.OnBoardReady -= RebuildHUD;

        GameEvents.OnTurnOrderConfirmed -= HandleTurnOrderConfirmed;

        GameEvents.OnPropertyPurchased -= HandleHUDUpdate;
        GameEvents.OnRentPaid -= HandleHUDUpdate;

    }
    
    private void HandleGameStateChanged(GameState state)
    {
        if (state == GameState.GameEnd)
        {
            HideHUD();
        }
        else if (state == GameState.PlayerTurn)
        {
            RebuildHUD();
        }
    }
    
    private void HideHUD()
    {
        foreach (HUDPlayerPanel panel in panels)
        {
            if (panel == null)
                continue;

            panel.gameObject.SetActive(false);
        }
    }

    private void HandleTurnOrderConfirmed(Player player)
    {
        if (nextPanelIndex >= panels.Length)
            return;

        HUDPlayerPanel panel = panels[nextPanelIndex];

        panel.gameObject.SetActive(true);

        panel.SetPlayer(player);

        nextPanelIndex++;
    }

    private void HandleHUDUpdate(Player player, BoardSpace space)
    {
        RefreshHUD();
    }

    public void RefreshHUD()
    {
        foreach (HUDPlayerPanel panel in panels)
        {
            if (panel == null)
                continue;
            
            if (panel.gameObject.activeSelf)
                panel.Refresh();
        }
    }

    private void ResetHUD()
    {
        nextPanelIndex = 0;

        foreach (HUDPlayerPanel panel in panels)
        {
            if (panel == null)
                continue;
            
            panel.gameObject.SetActive(false);
        }
    }

    private void RebuildHUD()
    {
        if (GameManager.Instance.CurrentGameState != GameState.PlayerTurn)
            return;

        if (panels == null || panels.Length == 0)
            return;

        ResetHUD();

        foreach (Player player in GameManager.Instance.TurnOrder)
        {
            if (player == null)
                continue;

            HandleTurnOrderConfirmed(player);
        }

        RefreshHUD();
    }
}
