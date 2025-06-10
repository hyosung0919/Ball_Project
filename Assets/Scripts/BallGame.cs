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
}
