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

        if (IsDueDay())
        {
            if (!GameManager.Instance.TakeRent())
            {
                dayText.text = "<size=50>BANKRUPT</size>\n<size=12>GRANDMA NOW LIVES ON THE STREET :(</size>";
                GameManager.Instance.SetPlayerPosition(PlayerPosition.Canvas);
                StartCoroutine(FadeIn(nextDayOverlay, 2f));
                return;
            }
        }
        
        dayText.text = $"DAY {TimeOfDayManager.Instance.Day}\n<size=20>100€ DUE BY DAY {NextDueDay()}</size>\n<size=20>BALANCE: {GameManager.Instance.Money}€</size>";
        StartCoroutine(NextDayTransition());
    }
    
    private bool IsDueDay()
    {
        var day = TimeOfDayManager.Instance.Day;
        return day % 7 == 0;
    }

    private int NextDueDay()
    {
        var day = TimeOfDayManager.Instance.Day;
        return day + (7 - day % 7);
    }

    private IEnumerator InitialTransition()
    {
        yield return new WaitForSeconds(5f);
        yield return FadeOut(nextDayOverlay, 2f);
        GameManager.Instance.SetPlayerPosition(PlayerPosition.House);
    }

    private IEnumerator NextDayTransition()
    {
        GameManager.Instance.SetPlayerPosition(PlayerPosition.Canvas);
        yield return FadeIn(nextDayOverlay, 2f);
        yield return new WaitForSeconds(2f);
        TimeOfDayManager.Instance.Reset();
        yield return FadeOut(nextDayOverlay, 2f);
        GameManager.Instance.SetPlayerPosition(PlayerPosition.House);
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
