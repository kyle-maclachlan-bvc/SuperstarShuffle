using System.Collections;
using TMPro;
using UnityEngine;

public class MinigameStartUI : MonoBehaviour
{
    [SerializeField] private TMP_Text readyText;

    private Coroutine displayRoutine;

    private void Awake()
    {
        readyText.gameObject.SetActive(false);
    }

    public void ShowReady()
    {
        if (displayRoutine != null)
            StopCoroutine(displayRoutine);

        readyText.text = "READY?";
        readyText.gameObject.SetActive(true);
    }

    public void ShowGo()
    {
        if (displayRoutine != null)
            StopCoroutine(displayRoutine);

        displayRoutine = StartCoroutine(ShowGoRoutine());
    }

    private IEnumerator ShowGoRoutine()
    {
        readyText.text = "GO!";

        yield return new WaitForSeconds(0.5f);

        readyText.gameObject.SetActive(false);

        displayRoutine = null;
    }
}