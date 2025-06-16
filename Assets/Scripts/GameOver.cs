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
                ballShooter.countdownText.gameObject.SetActive(false); // ����
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

        // ���⼭ �� ������ ��ٷ��� ȭ�鿡 "1"�� ��¥�� ���Դϴ�!
        yield return null;

        // �Ǵ� �� Ȯ���ϰ�
        yield return new WaitForSeconds(1f);

        // �� ���� 1�� ��ٸ��鼭 "1"�� ȭ�鿡 ����
        yield return new WaitForSeconds(1f);

        text.gameObject.SetActive(false);

        ballShooter.TriggerGameOver("���� ���� ������ 3�� �̻� �ӹ������ϴ�.");
    }
}
