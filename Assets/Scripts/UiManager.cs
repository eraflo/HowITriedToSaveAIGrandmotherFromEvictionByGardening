using System.Collections;
using TMPro;
using UnityEngine;
using Utils;

public class UiManager : SingletonMonoBehaviour<UiManager>
{
    [SerializeField] private CanvasGroup nextDayOverlay;
    [SerializeField] private TextMeshProUGUI dayText;

    private void Start()
    {
        GameManager.Instance.SetPlayerPosition(PlayerPosition.Canvas);
        StartCoroutine(InitialTransition());
    }

    public void DoNextDayTransition()
    {
        TimeOfDayManager.Instance.Paused = true;
        dayText.text = $"DAY {TimeOfDayManager.Instance.Day}\n<size=20>100€ DUE BY DAY {NextDueDay()}</size>";
        StartCoroutine(NextDayTransition());
    }

    private int NextDueDay()
    {
        var day = TimeOfDayManager.Instance.Day;
        return day + (7 - day % 7);
    }

    private IEnumerator InitialTransition()
    {
        yield return FadeOut(nextDayOverlay, 5f);
        GameManager.Instance.SetPlayerPosition(PlayerPosition.House);
    }

    private IEnumerator NextDayTransition()
    {
        GameManager.Instance.SetPlayerPosition(PlayerPosition.Canvas);
        yield return FadeIn(nextDayOverlay, 2f);
        yield return new WaitForSeconds(2f);
        TimeOfDayManager.Instance.Reset();
        yield return FadeOut(nextDayOverlay, 2f);
        GameManager.Instance.SetPlayerPosition(PlayerPosition.Market);
    }

    private IEnumerator FadeIn(CanvasGroup canvas, float duration)
    {
        var elapsedTime = 0f;

        while (canvas.alpha < 1f)
        {
            canvas.alpha = elapsedTime / duration;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator FadeOut(CanvasGroup canvas, float duration)
    {
        var elapsedTime = 0f;

        while (canvas.alpha > 0f)
        {
            canvas.alpha = 1 - elapsedTime / duration;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
