using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using TMPro;
//using UnityEditorInternal;
using UnityEngine.EventSystems;

public class BallShooter : MonoBehaviour
{
    [Header("발사 설정")]
    public GameObject[] ballPrefab;           // 구체 프리팹 (2D Rigidbody와 CircleCollider2D 필요)
    public float[] ballSizes = { 0.5f, 0.7f, 0.9f, 1.1f, 1.3f, 1.5f, 1.7f, 1.9f };      //공 크기 선언
    public Transform startPosition;         // 시작 위치 (구체가 있는 곳)
    public float maxForce = 20f;            // 최대 발사력
    public float forceMultiplier = 5f;      // 힘 배율
    public float gameHeight = 0.5f;
    public TMP_Text scoreText;
    private int score = 0;
    public TMP_Text countdownText;
    public TMP_Text resultText;
    public GameObject gameOverPanel;
    public GameObject pausePanel;
    private bool isPaused = false;
    public AudioClip shootSound;
    public AudioClip mergeSound;
    private AudioSource audioSource;

    [Header("드래그 설정")]
    public float maxDragDistance = 3f;      // 최대 드래그 거리
    public LineRenderer aimLine;            // 조준선
    public int trajectoryPoints = 30;       // 궤적 점 개수

    public GameObject currentBall;         // 현재 대기 중인 구체
    public int currentBallType;
    private bool isDragging = false;        // 드래그 중인지
    private Vector2 dragStartPos;           // 드래그 시작 위치 (2D)
    private Camera mainCamera;              // 메인 카메라
                                            //게임 종료 높이
    private bool isGameOver = false;                                                     //게임 상태

    void Start()
    {
        mainCamera = Camera.main;

        audioSource = GetComponent<AudioSource>();

        // 조준선 설정
        if (aimLine != null)
        {
            aimLine.positionCount = trajectoryPoints;
            aimLine.enabled = false;
            aimLine.useWorldSpace = true;
        }

        // 첫 번째 구체 생성
        CreateNewBall();
    }

    void ShowTrajectory(Vector2 worldDrag)
    {
        if (currentBall == null) return;

        Vector2 startPos = currentBall.transform.position;
        float launchForce = Mathf.Clamp(worldDrag.magnitude * forceMultiplier, 0f, maxForce);
        Vector2 launchVelocity = worldDrag.normalized * launchForce;

        // 포물선 궤적 계산 (2D)
        for (int i = 0; i < trajectoryPoints; i++)
        {
            float time = i * 0.1f;
            Vector2 point = CalculateTrajectoryPoint(startPos, launchVelocity, time);
            aimLine.SetPosition(i, new Vector3(point.x, point.y, -1));
        }
    }

    Vector2 CalculateTrajectoryPoint(Vector2 startPos, Vector2 startVel, float time)
    {
        // 2D 포물선 운동 공식
        return startPos + startVel * time + 0.5f * Physics2D.gravity * time * time;
    }

    void Update()
    {
        if (isGameOver || isPaused) return;

        // UI 위를 클릭 중이면 입력 무시
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        HandleInput();
    }

    void HandleInput()
    {
        if (currentBall == null) return;

        Rigidbody2D rb = currentBall.GetComponent<Rigidbody2D>();

        // 현재 공이 대기 상태(kineamtic)일 때만 드래그 허용
        if (rb == null || !rb.isKinematic) return;

        // 마우스 클릭으로 드래그 시작
        if (Input.GetMouseButtonDown(0))
        {
            StartDrag();
        }

        // 드래그 중
        if (Input.GetMouseButton(0) && isDragging)
        {
            UpdateDrag();
        }

        // 마우스 떼면 발사
        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            LaunchBall();
        }
    }

    void StartDrag()
    {
        isDragging = true;
        dragStartPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        if (aimLine != null)
            aimLine.enabled = true;
    }

    void UpdateDrag()
    {
        // 현재 마우스 위치를 월드 좌표로 변환
        Vector2 currentMouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dragVector = dragStartPos - currentMouseWorldPos;

        // 드래그 거리 제한
        dragVector = Vector2.ClampMagnitude(dragVector, maxDragDistance);

        // 포물선 궤적 표시
        if (aimLine != null)
        {
            ShowTrajectory(dragVector);
        }
    }

    void LaunchBall()
    {
        if (currentBall == null) return;

        // 발사 방향 및 힘 계산
        Vector2 currentMouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dragVector = dragStartPos - currentMouseWorldPos;

        // 발사력 계산
        float launchForce = Mathf.Clamp(dragVector.magnitude * forceMultiplier, 0f, maxForce);
        Vector2 launchDirection = dragVector.normalized;

        // Rigidbody2D 활성화 및 발사
        Rigidbody2D rb = currentBall.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.isKinematic = false; // 물리 활성화
            rb.AddForce(launchDirection * launchForce, ForceMode2D.Impulse);
        }
        else
        {
            rb.isKinematic = true;
        }

        // 드래그 종료
        isDragging = false;
        if (aimLine != null)
            aimLine.enabled = false;

        // 잠시 후 새 구체 생성
        Invoke("CreateNewBall", 1f);
        if (shootSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(shootSound);
        }
    }

    void CreateNewBall()
    {
        if (ballPrefab == null || startPosition == null)
            return;

        if (!isGameOver)                //게임 오버가 아닐 때만 새 공 생성
        {
            currentBallType = Random.Range(0, 3);                //0 ~ 2 사이의 랜덤 공 타입

            float halfBallSize = ballSizes[currentBallType] / 2;

            currentBall = Instantiate(ballPrefab[currentBallType]); //공 생성
            currentBall.transform.localScale = new Vector3(ballSizes[currentBallType], ballSizes[currentBallType], 1);      //공 크기 설정

            Rigidbody2D rb2 = currentBall.GetComponent<Rigidbody2D>();
        }

        // Rigidbody2D를 kinematic으로 설정 (대기 상태)
        Rigidbody2D rb = currentBall.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.isKinematic = true;
            // Z축 회전 고정 (2D에서는 Z축 회전만 필요)
            rb.freezeRotation = true;
        }
    }
    public void CheckGameOVer()                         //게임 오버 체크 함수
    {
        Ball[] allBalls = FindObjectsOfType<Ball>();    //Scene에 있는 모든 과일 컴포넌트가 붙어있는 오브젝트를 가져온다. 작은 게임에서만 사용 비용이 쌤

        float gameOverHeight = gameHeight;              //일정 높이보다 높은 위치에 과일이 있는지 확인

        for (int i = 0; i < allBalls.Length; i++)        //모든 과일을 검사한다.
        {
            if (allBalls[i] != null)
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
    public void MergeBalls(int ballType, Vector3 position)
    {
        PlaySound(mergeSound);
        // 최대 크기 공이면 병합하지 않고 종료
        if (ballType >= ballPrefab.Length - 1)
        {
            Debug.Log("최대 크기이므로 병합 불가");
            return;
        }

        // 병합 가능한 경우: 새로운 공 생성
        GameObject newBall = Instantiate(ballPrefab[ballType + 1], position, Quaternion.identity);
        newBall.transform.localScale = new Vector3(ballSizes[ballType + 1], ballSizes[ballType + 1], 1.0f);

        // 점수 추가
        int addScore = (ballType + 1) * 10;
        score += addScore;
        UpdateScoreText();
    }
    void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "점수: " + score.ToString();
        }
    }
    public void DecreaseScore(int amount)
    {
        score -= amount;
        if (score < 0) score = 0;
        UpdateScoreText();
    }
    public void TriggerGameOver(string reason)
    {
        if (isGameOver) return;

        isGameOver = true;

        // 점수 텍스트 설정
        if (resultText != null)
            resultText.text = $"게임 종료 \n점수: {score}점";

        // 점수 저장
        Ranking.SaveNewScore(score);

        // 패널은 1초 뒤에 띄움
        StartCoroutine(ShowGameOverPanelWithDelay(1f));
    }
    private IEnumerator ShowGameOverPanelWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }
    public void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }
    public void PauseGame()
    {
        Time.timeScale = 0f;
        isPaused = true;
        if (pausePanel != null)
            pausePanel.SetActive(true);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        isPaused = false;
        if (pausePanel != null)
            pausePanel.SetActive(false);
    }
    public void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
            audioSource.PlayOneShot(clip);
    }
}

