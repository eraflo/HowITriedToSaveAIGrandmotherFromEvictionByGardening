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
        StartCoroutine(FadeOut(nextDayOverlay, 2f));
    }

    public void DoNextDayTransition()
    {
        TimeOfDayManager.Instance.Paused = true;
        dayText.text = $"DAY {TimeOfDayManager.Instance.Day}";
        StartCoroutine(NextDayTransition());
    }

    private IEnumerator NextDayTransition()
    {
        yield return FadeIn(nextDayOverlay, 2f);
        yield return new WaitForSeconds(2f);
        TimeOfDayManager.Instance.Reset();
        yield return FadeOut(nextDayOverlay, 2f);
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
