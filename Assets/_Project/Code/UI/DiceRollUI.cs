using System.Collections;
using TMPro;
using UnityEngine;

public class DiceRollUI : MonoBehaviour
{
    [SerializeField] private TMP_Text rollText;
    [SerializeField] private Canvas canvas;

    private Camera mainCamera;
    private Player player;

    private void Awake()
    {
        mainCamera = Camera.main;
        player = GetComponentInParent<Player>();
        canvas.enabled = false;
    }

    private void OnEnable()
    {
        GameEvents.OnDiceRolled += HandleDiceRoll;
    }

    private void OnDisable()
    {
        GameEvents.OnDiceRolled -= HandleDiceRoll;
    }

    private void LateUpdate()
    {
        if (!canvas.enabled)
            return;

        canvas.transform.forward = mainCamera.transform.forward;
    }

    private void HandleDiceRoll(Player rolledPlayer, int value)
    {
        if (rolledPlayer != player)
            return;
        
        ShowRoll(value);
    }

    public void ShowRoll(int value)
    {
        StopAllCoroutines();
        
        rollText.text = value.ToString();
        canvas.enabled = true;

        StartCoroutine(HideRoutine());
    }

    private IEnumerator HideRoutine()
    {
        yield return new WaitForSeconds(1.5f);
        canvas.enabled = false;
    }
}
