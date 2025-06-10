using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallGame : MonoBehaviour
{
    public GameObject[] ballprefabs;                                                    //공 프리팹 배열 선언

    public float[] ballSizes = { 0.5f, 0.7f, 0.9f, 1.1f, 1.3f, 1.5f, 1.7f, 1.9f };      //공 크기 선언

    public GameObject currentBall;                                                      //현재 들고있는 공
    public int currentBallType;

    public float ballStartHeight = 6.0f;                                                //공 시작시 높이 설정(인스펙터에서 조절 가능)
    public float gameWidth = 5.0f;                                                      //게임판 너비
    public bool isGameOver = false;                                                     //게임 상태
    public Camera maincamera;                                                           //카메라 참조(마우스 위치 변환에 필요)

    public float ballTimer;                                                             //잰 시간 설정을 위한 타이머

    public float gameHeight;                                                            //게임 종료 높이

    void Start()
    {
        maincamera = Camera.main;                                                       //메인 카메라 참조 가져오기
        SpawnNewBall();                                                                 //게임 시작 시 첫 공 생성
        ballTimer = -3.0f;
        gameHeight = ballStartHeight + 0.5f;
    }

    void Update()
    {
        if (isGameOver) return;         //게임 오버면 리턴

        if(ballTimer >= 0 )                             //타이머의 시간이 0보다 클 경우
        {
            ballTimer -= Time.deltaTime;
        }

        if(ballTimer < 0 && ballTimer > -2)             //타이머 시간이 0과 -2 사이에 있을때 잰 함수를 호출 하고 다른 시간대로 보낸다.
        {
            CheckGameOVer();
            SpawnNewBall();
            ballTimer = -3.0f;                          //타이머 시간을 -3으로 보낸다.
        }

        if ( currentBall != null)       //현재 생성된 공이 있을 때만 처리
        {
            Vector3 mousePosition = Input.mousePosition;                                //마우스 위치를 받아온다.
            Vector3 worldPostion = maincamera.ScreenToWorldPoint(mousePosition);        //마우스 위치를 월드 좌표로 변환

            Vector3 newPostion = currentBall.transform.position;                        //공 위치 업데이트
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

            currentBall.transform.position = newPostion;                                //공 좌표 갱신
        }

        if (Input.GetMouseButtonDown(0) && ballTimer == -3.0f)                          //마우스 좌 클릭하면 공을 떨어트린다. && 타이머 시간은 -3.0f
        {
            DropBall();
        }
    }

    void SpawnNewBall()                 //공 생성 함수
    {
        if (!isGameOver)                //게임 오버가 아닐 때만 새 공 생성
        {
            currentBallType = Random.Range(0,3);                //0 ~ 2 사이의 랜덤 공 타입

            Vector3 mousePosition = Input.mousePosition;        //마우스 위치를 받아온다.
            Vector3 worldPosition = maincamera.ScreenToWorldPoint(mousePosition);       //마우스 위치를 월드 좌표로 변환

            Vector3 spawnPosion = new Vector3(worldPosition.x, ballStartHeight, 0);     //X 좌표만 사용하고 나머지는 설정한 대로 한다.

            float halfBallSize = ballSizes[currentBallType] / 2;

            //X 의 위치가 게임 영역을 벗어나지 않도록 제한
            spawnPosion.x = Mathf.Clamp(spawnPosion.x, - gameWidth / 2 + halfBallSize, gameWidth / 2 - halfBallSize);

            currentBall = Instantiate(ballprefabs[currentBallType], spawnPosion, Quaternion.identity);                      //공 생성
            currentBall.transform.localScale = new Vector3(ballSizes[currentBallType], ballSizes[currentBallType], 1);      //공 크기 설정

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
            rb.gravityScale = 1f;               //중력을 원래 값으로 복구 시킨다.

            currentBall = null;                 //현재 들고 있는 공 해제

            ballTimer = 1.0f;                   //타이머 초기화
        }
    }

    public void MergeBalls(int ballType, Vector3 position)
    {
        if(ballType < ballprefabs.Length - 1)           //마지막 과일타입이 아니라면
        {
            GameObject newBall = Instantiate(ballprefabs[ballType + 1], position, Quaternion.identity);
            newBall.transform.localScale = new Vector3(ballSizes[ballType + 1], ballSizes[ballType + 1], 1.0f);

            //점수 추가 로직 등등
        }
    }

    public void CheckGameOVer()                         //게임 오버 체크 함수
    {
        Ball[] allBalls = FindObjectsOfType<Ball>();    //Scene에 있는 모든 과일 컴포넌트가 붙어있는 오브젝트를 가져온다. 작은 게임에서만 사용 비용이 쌤

        float gameOverHeight = gameHeight;              //일정 높이보다 높은 위치에 과일이 있는지 확인

        for(int i = 0; i < allBalls.Length; i++)        //모든 과일을 검사한다.
        {
            if(allBalls[i] != null)
            {
                Rigidbody2D rb = allBalls[i].GetComponent<Rigidbody2D>();

                //과일이 정지 상태이고 높은 위치에 있다면
                if (rb != null && rb.velocity.magnitude < 0.1f && allBalls[i].transform.position.y > gameOverHeight)
                {
                    isGameOver = true;
                    Debug.Log("게임 오버");

                    break;
                }
            }
        }
    }
}
