using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;

public class WinnerAnnouncementUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text winnerText;

    [Header("Animation")]
    [SerializeField] private float popDuration = 0.35f;
    [SerializeField] private float displayDuration = 2.5f;
    [SerializeField] private float popScale = 1.15f;

    private void Awake()
    {
        panel.SetActive(false);
    }

    public IEnumerator Show(MinigamePlacement placement)
    {
        panel.SetActive(true);

        winnerText.text = BuildWinnerString(placement);

        panel.transform.localScale = Vector3.zero;

        float timer = 0f;

        while (timer < popDuration)
        {
            timer += Time.deltaTime;

            float t = timer / popDuration;

            float scale;

            if (t < .5f)
            {
                scale = Mathf.Lerp(0f, popScale, t * 2f);
            }
            else
            {
                scale = Mathf.Lerp(popScale, 1f, (t - .5f) * 2f);
            }

            panel.transform.localScale = Vector3.one * scale;

            yield return null;
        }

        panel.transform.localScale = Vector3.one;

        yield return new WaitForSeconds(displayDuration);

        panel.SetActive(false);
    }

    private string BuildWinnerString(MinigamePlacement placement)
    {
        if (placement.Players.Count == 4)
            return "Everyone Wins!";

        StringBuilder builder = new();

        for (int i = 0; i < placement.Players.Count; i++)
        {
            if (i > 0)
            {
                if (i == placement.Players.Count - 1)
                    builder.Append(" & ");
                else
                    builder.Append(", ");
            }

            builder.Append(placement.Players[i].PlayerName);
        }

        builder.Append(placement.Players.Count == 1 ? " Wins!" : " Win!");

        return builder.ToString();
    }
}
