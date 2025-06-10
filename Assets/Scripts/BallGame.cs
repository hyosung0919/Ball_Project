using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallGame : MonoBehaviour
{
    public GameObject[] ballprefabs;                                                    //�� ������ �迭 ����

    public float[] ballSizes = { 0.5f, 0.7f, 0.9f, 1.1f, 1.3f, 1.5f, 1.7f, 1.9f };      //�� ũ�� ����

    public GameObject currentBall;                                                      //���� ����ִ� ��
    public int currentBallType;

    public float ballStartHeight = 6.0f;                                                //�� ���۽� ���� ����(�ν����Ϳ��� ���� ����)
    public float gameWidth = 5.0f;                                                      //������ �ʺ�
    public bool isGameOver = false;                                                     //���� ����
    public Camera maincamera;                                                           //ī�޶� ����(���콺 ��ġ ��ȯ�� �ʿ�)
}
