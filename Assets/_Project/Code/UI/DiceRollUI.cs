using TMPro;
using UnityEngine;
using System.Collections;

public class DiceRollUI : MonoBehaviour
{
    [SerializeField] private TMP_Text rollText;
    [SerializeField] private Canvas canvas;

    private Camera mainCamera;
    private Player player;
    
    private Coroutine pulseRoutine;
    private Vector3 originalScale;

    private void Awake()
    {
        mainCamera = Camera.main;
        player = GetComponentInParent<Player>();
        canvas.enabled = false;
        
        originalScale = rollText.transform.localScale;
    }

    private void OnEnable()
    {
        GameEvents.OnDiceRolled += HandleDiceRoll;
        GameEvents.OnTurnOrderConfirmed += HandleTurnOrderConfirmed;
        GameEvents.OnMovementStepCompleted += HandleMovementStep;
        GameEvents.OnMovementFinished += HandleMovementFinished;
    }

    private void OnDisable()
    {
        GameEvents.OnDiceRolled -= HandleDiceRoll;
        GameEvents.OnTurnOrderConfirmed -= HandleTurnOrderConfirmed;
        GameEvents.OnMovementStepCompleted -= HandleMovementStep;
        GameEvents.OnMovementFinished -= HandleMovementFinished;
    }

    private void LateUpdate()
    {
        if (!canvas.enabled)
            return;

        if (mainCamera == null)
            mainCamera = Camera.main;

        if (mainCamera == null)
            return;

        canvas.transform.forward = mainCamera.transform.forward;
    }

    private void HandleDiceRoll(Player rolledPlayer, int value)
    {
        if (rolledPlayer != player)
            return;
        
        ShowRoll(value);
    }

    private void HandleMovementStep(Player movedPlayer, int remainingSpaces)
    {
        if (movedPlayer != player)
            return;

        if (remainingSpaces <= 0)
        {
            canvas.enabled = false;
            return;
        }
        
        rollText.text = remainingSpaces.ToString();
    }

    private void HandleMovementFinished(Player movedPlayer)
    {
        if (movedPlayer != player)
            return;
        
        canvas.enabled = false;
    }

    private void HandleTurnOrderConfirmed(Player confirmedPlayer)
    {
        if (confirmedPlayer != player)
            return;

        canvas.enabled = false;
    }

    public void ShowRoll(int value)
    {
        rollText.text = value.ToString();
        canvas.enabled = true;
    }

    public void StartPulse()
    {
        if (pulseRoutine != null)
            StopCoroutine(pulseRoutine);

        pulseRoutine = StartCoroutine(PulseRoutine());
    }

    public void StopPulse()
    {
        if (pulseRoutine != null)
            StopCoroutine(pulseRoutine);
        
        rollText.transform.localScale = originalScale;
    }

    public IEnumerator PulseRoutine()
    {
        while (true)
        {
            float t = 0f;

            while (t < 1f)
            {
                t += Time.deltaTime * 4f;
                
                float scale = Mathf.Lerp(1f, 1.25f, Mathf.PingPong(t, 0.5f) * 2f);

                rollText.transform.localScale = originalScale * scale;
                
                yield return null;
            }
        }
        
    }

    public void ShowValue(int value)
    {
        rollText.text = value.ToString();
        canvas.enabled = true;
    }

    public void Hide()
    {
        canvas.enabled = false;
    }
}
