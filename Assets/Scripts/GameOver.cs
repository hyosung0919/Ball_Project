using System.Collections;
using TMPro;
using UnityEngine;

public class DangerZoneChecker : MonoBehaviour
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
                ballShooter.countdownText.gameObject.SetActive(false); // ¼û±è
            }
        }
    }

    private IEnumerator StartCountdown()
    {
        TMP_Text text = ballShooter.countdownText;
        text.gameObject.SetActive(true);

        text.text = "3";
        yield return new WaitForSeconds(1f);
        text.text = "2";
        yield return new WaitForSeconds(1f);
        text.text = "1";
        yield return new WaitForSeconds(1f);

        text.gameObject.SetActive(false);

        ballShooter.TriggerGameOver("°øÀÌ ¼±¿¡ 3ÃÊ ÀÌ»ó ¸Ó¹°·¶½À´Ï´Ù.");
    }
}
