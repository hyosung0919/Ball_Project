using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public int ballType;                                            //�� Ÿ��  int�� index ����

    public bool hasMered = false;                                   //���� ���������� Ȯ���ϴ� �÷���
    private float gameOverZoneTimer = 0f;
    private bool inGameOverZone = false;
    private BallShooter ballShooter;

    void Start()
    {
        ballShooter = FindObjectOfType<BallShooter>(); // BallShooter ���� ���
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasMered) return;

        Ball otherBall = collision.gameObject.GetComponent<Ball>();



        if (otherBall != null && !otherBall.hasMered && otherBall.ballType == ballType)
        {
            // �ִ� ũ���̸� �������� ����
            if (ballType >= ballShooter.ballPrefab.Length - 1)
            {
                return;
            }

            hasMered = true;
            otherBall.hasMered = true;

            Vector3 mergePosition = (transform.position + otherBall.transform.position) / 2f;

            BallShooter gameManager = FindObjectOfType<BallShooter>();
            if (gameManager != null)
            {
                gameManager.MergeBalls(ballType, mergePosition);
            }

            Destroy(otherBall.gameObject);
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("GameOver"))
        {
            inGameOverZone = true;
            gameOverZoneTimer = 0f; // ���� �� Ÿ�̸� ����
        }

        if (collision.CompareTag("Out"))
        {
            if (ballShooter != null)
            {
                ballShooter.DecreaseScore(15);
            }

            Destroy(gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("GameOver"))
        {
            inGameOverZone = false;
            gameOverZoneTimer = 0f;
        }
    }

    void Update()
    {
        if (inGameOverZone)
        {
            gameOverZoneTimer += Time.deltaTime;

            if (gameOverZoneTimer >= 3f)
            {
                if (ballShooter != null)
                {
                    ballShooter.TriggerGameOver("���� ���� ������ 3�� �̻� �ӹ������ϴ�.");
                }
            }
        }
    }
}