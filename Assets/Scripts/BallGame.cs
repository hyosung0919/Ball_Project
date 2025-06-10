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

    public float ballTimer;                                                             //�� �ð� ������ ���� Ÿ�̸�

    public float gameHeight;                                                            //���� ���� ����

    void Start()
    {
        maincamera = Camera.main;                                                       //���� ī�޶� ���� ��������
        SpawnNewBall();                                                                 //���� ���� �� ù �� ����
        ballTimer = -3.0f;
        gameHeight = ballStartHeight + 0.5f;
    }

    void Update()
    {
        if (isGameOver) return;         //���� ������ ����

        if(ballTimer >= 0 )                             //Ÿ�̸��� �ð��� 0���� Ŭ ���
        {
            ballTimer -= Time.deltaTime;
        }

        if(ballTimer < 0 && ballTimer > -2)             //Ÿ�̸� �ð��� 0�� -2 ���̿� ������ �� �Լ��� ȣ�� �ϰ� �ٸ� �ð���� ������.
        {
            CheckGameOVer();
            SpawnNewBall();
            ballTimer = -3.0f;                          //Ÿ�̸� �ð��� -3���� ������.
        }

        if ( currentBall != null)       //���� ������ ���� ���� ���� ó��
        {
            Vector3 mousePosition = Input.mousePosition;                                //���콺 ��ġ�� �޾ƿ´�.
            Vector3 worldPostion = maincamera.ScreenToWorldPoint(mousePosition);        //���콺 ��ġ�� ���� ��ǥ�� ��ȯ

            Vector3 newPostion = currentBall.transform.position;                        //�� ��ġ ������Ʈ
            newPostion.x = worldPostion.x;

            float halfBallSize = ballSizes[currentBallType] / 2;
            if ( newPostion.x < -gameWidth / 2 + halfBallSize )
            {
                newPostion.x = -gameWidth / 2 + halfBallSize;
            }
            if (newPostion.x > gameWidth / 2 + halfBallSize)
            {
                newPostion.x = gameWidth / 2 + halfBallSize;
            }

            currentBall.transform.position = newPostion;                                //�� ��ǥ ����
        }

        if (Input.GetMouseButtonDown(0) && ballTimer == -3.0f)                          //���콺 �� Ŭ���ϸ� ���� ����Ʈ����. && Ÿ�̸� �ð��� -3.0f
        {
            DropBall();
        }
    }

    void SpawnNewBall()                 //�� ���� �Լ�
    {
        if (!isGameOver)                //���� ������ �ƴ� ���� �� �� ����
        {
            currentBallType = Random.Range(0,3);                //0 ~ 2 ������ ���� �� Ÿ��

            Vector3 mousePosition = Input.mousePosition;        //���콺 ��ġ�� �޾ƿ´�.
            Vector3 worldPosition = maincamera.ScreenToWorldPoint(mousePosition);       //���콺 ��ġ�� ���� ��ǥ�� ��ȯ

            Vector3 spawnPosion = new Vector3(worldPosition.x, ballStartHeight, 0);     //X ��ǥ�� ����ϰ� �������� ������ ��� �Ѵ�.

            float halfBallSize = ballSizes[currentBallType] / 2;

            //X �� ��ġ�� ���� ������ ����� �ʵ��� ����
            spawnPosion.x = Mathf.Clamp(spawnPosion.x, - gameWidth / 2 + halfBallSize, gameWidth / 2 - halfBallSize);

            currentBall = Instantiate(ballprefabs[currentBallType], spawnPosion, Quaternion.identity);                      //�� ����
            currentBall.transform.localScale = new Vector3(ballSizes[currentBallType], ballSizes[currentBallType], 1);      //�� ũ�� ����

            Rigidbody2D rb = currentBall.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.gravityScale = 0f;
            }
        }
    }

    void DropBall()
    {
        if(currentBall == null) return;

        Rigidbody2D rb = currentBall.GetComponent<Rigidbody2D>();
        if(rb != null)
        {
            rb.gravityScale = 1f;               //�߷��� ���� ������ ���� ��Ų��.

            currentBall = null;                 //���� ��� �ִ� �� ����

            ballTimer = 1.0f;                   //Ÿ�̸� �ʱ�ȭ
        }
    }

    public void MergeBalls(int ballType, Vector3 position)
    {
        if(ballType < ballprefabs.Length - 1)           //������ ����Ÿ���� �ƴ϶��
        {
            GameObject newBall = Instantiate(ballprefabs[ballType + 1], position, Quaternion.identity);
            newBall.transform.localScale = new Vector3(ballSizes[ballType + 1], ballSizes[ballType + 1], 1.0f);

            //���� �߰� ���� ���
        }
    }

    public void CheckGameOVer()                         //���� ���� üũ �Լ�
    {
        Ball[] allBalls = FindObjectsOfType<Ball>();    //Scene�� �ִ� ��� ���� ������Ʈ�� �پ��ִ� ������Ʈ�� �����´�. ���� ���ӿ����� ��� ����� ��

        float gameOverHeight = gameHeight;              //���� ���̺��� ���� ��ġ�� ������ �ִ��� Ȯ��

        for(int i = 0; i < allBalls.Length; i++)        //��� ������ �˻��Ѵ�.
        {
            if(allBalls[i] != null)
            {
                Rigidbody2D rb = allBalls[i].GetComponent<Rigidbody2D>();

                //������ ���� �����̰� ���� ��ġ�� �ִٸ�
                if (rb != null && rb.velocity.magnitude < 0.1f && allBalls[i].transform.position.y > gameOverHeight)
                {
                    isGameOver = true;
                    Debug.Log("���� ����");

                    break;
                }
            }
        }
    }
}
