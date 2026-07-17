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

        displayRoutine = StartCoroutine(ShowMessageRoutine("GO!", 0.5f));
    }

    public void ShowFinished()
    {
        if (displayRoutine != null)
            StopCoroutine(displayRoutine);

        displayRoutine = StartCoroutine(ShowMessageRoutine("FINISHED!", 1f));
    }

    private IEnumerator ShowMessageRoutine(string message, float duration)
    {
        readyText.text = message;
        readyText.gameObject.SetActive(true);

        yield return new WaitForSeconds(duration);

        readyText.gameObject.SetActive(false);

        displayRoutine = null;
    }
}