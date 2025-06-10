using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using Unity.VisualScripting;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public int ballType;                                            //�� Ÿ�� ( 0 : Ź���� 1: �߱��� 2: �󱸰� 3: �౸�� ) int�� index ����

    public bool hasMered = false;                                   //���� ���������� Ȯ���ϴ� �÷���

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasMered)                                               //�̹� ������ ���� ����
            return;

        Ball otherBall = collision.gameObject.GetComponent<Ball>();             //�ٸ� ���� �浹�ߴ��� Ȯ��

        if (otherBall != null && !otherBall.hasMered && otherBall.ballType == ballType)     //�浹�� ���� ���̰� Ÿ���� ���ٸ� (�������� �ʾ��� ���)
        {
            hasMered = true;                //���ƴٰ� ǥ��
            otherBall.hasMered = true;

            Vector3 mergePosition = (transform.position + otherBall.transform.position) / 2f;       //�� ���� �߰� ��ġ ���

            //���� �Ŵ������� Merge ������ ���� ȣ��
            BallGame gameManager = FindObjectOfType<BallGame>();
            if(gameManager != null)
            {
                gameManager.MergeBalls(ballType, mergePosition);            //�Լ��� �����ϰ� ȣ���Ѵ�.
            }

            //���� ����
            Destroy(otherBall.gameObject);
            Destroy(gameObject);
        }
    }
}
