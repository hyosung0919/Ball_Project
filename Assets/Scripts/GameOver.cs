using System.Collections;
using TMPro;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    private BallShooter ballShooter;
    private Coroutine countdownCoroutine;

    private void Start()
    {
        ballShooter = FindObjectOfType<BallShooter>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ball"))
        {
            if (countdownCoroutine == null)
                countdownCoroutine = StartCoroutine(StartCountdown());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ball"))
        {
            if (countdownCoroutine != null)
            {
                StopCoroutine(countdownCoroutine);
                countdownCoroutine = null;
                ballShooter.countdownText.gameObject.SetActive(false); // 숨김
            }
        }
    }

    private IEnumerator StartCountdown()
    {
        TMP_Text text = ballShooter.countdownText;

        yield return new WaitForSeconds(1f);

        text.gameObject.SetActive(true);

        text.text = "3";
        yield return new WaitForSeconds(1f);

        text.text = "2";
        yield return new WaitForSeconds(1f);

        text.text = "1";

        // 여기서 한 프레임 기다려야 화면에 "1"이 진짜로 보입니다!
        yield return null;

        // 또는 더 확실하게
        yield return new WaitForSeconds(1f);

        // 그 다음 1초 기다리면서 "1"을 화면에 유지
        yield return new WaitForSeconds(1f);

        text.gameObject.SetActive(false);

        ballShooter.TriggerGameOver("공이 위험 구역에 3초 이상 머물렀습니다.");
    }
}
