using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using TMPro;
//using UnityEditorInternal;
using UnityEngine.EventSystems;

public class BallShooter : MonoBehaviour
{
    [Header("�߻� ����")]
    public GameObject[] ballPrefab;           // ��ü ������ (2D Rigidbody�� CircleCollider2D �ʿ�)
    public float[] ballSizes = { 0.5f, 0.7f, 0.9f, 1.1f, 1.3f, 1.5f, 1.7f, 1.9f };      //�� ũ�� ����
    public Transform startPosition;         // ���� ��ġ (��ü�� �ִ� ��)
    public float maxForce = 20f;            // �ִ� �߻��
    public float forceMultiplier = 5f;      // �� ����
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

    [Header("�巡�� ����")]
    public float maxDragDistance = 3f;      // �ִ� �巡�� �Ÿ�
    public LineRenderer aimLine;            // ���ؼ�
    public int trajectoryPoints = 30;       // ���� �� ����

    public GameObject currentBall;         // ���� ��� ���� ��ü
    public int currentBallType;
    private bool isDragging = false;        // �巡�� ������
    private Vector2 dragStartPos;           // �巡�� ���� ��ġ (2D)
    private Camera mainCamera;              // ���� ī�޶�
                                            //���� ���� ����
    private bool isGameOver = false;                                                     //���� ����

    void Start()
    {
        mainCamera = Camera.main;

        audioSource = GetComponent<AudioSource>();

        // ���ؼ� ����
        if (aimLine != null)
        {
            aimLine.positionCount = trajectoryPoints;
            aimLine.enabled = false;
            aimLine.useWorldSpace = true;
        }

        // ù ��° ��ü ����
        CreateNewBall();
    }

    void ShowTrajectory(Vector2 worldDrag)
    {
        if (currentBall == null) return;

        Vector2 startPos = currentBall.transform.position;
        float launchForce = Mathf.Clamp(worldDrag.magnitude * forceMultiplier, 0f, maxForce);
        Vector2 launchVelocity = worldDrag.normalized * launchForce;

        // ������ ���� ��� (2D)
        for (int i = 0; i < trajectoryPoints; i++)
        {
            float time = i * 0.1f;
            Vector2 point = CalculateTrajectoryPoint(startPos, launchVelocity, time);
            aimLine.SetPosition(i, new Vector3(point.x, point.y, -1));
        }
    }

    Vector2 CalculateTrajectoryPoint(Vector2 startPos, Vector2 startVel, float time)
    {
        // 2D ������ � ����
        return startPos + startVel * time + 0.5f * Physics2D.gravity * time * time;
    }

    void Update()
    {
        if (isGameOver || isPaused) return;

        // UI ���� Ŭ�� ���̸� �Է� ����
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        HandleInput();
    }

    void HandleInput()
    {
        if (currentBall == null) return;

        Rigidbody2D rb = currentBall.GetComponent<Rigidbody2D>();

        // ���� ���� ��� ����(kineamtic)�� ���� �巡�� ���
        if (rb == null || !rb.isKinematic) return;

        // ���콺 Ŭ������ �巡�� ����
        if (Input.GetMouseButtonDown(0))
        {
            StartDrag();
        }

        // �巡�� ��
        if (Input.GetMouseButton(0) && isDragging)
        {
            UpdateDrag();
        }

        // ���콺 ���� �߻�
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
        // ���� ���콺 ��ġ�� ���� ��ǥ�� ��ȯ
        Vector2 currentMouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dragVector = dragStartPos - currentMouseWorldPos;

        // �巡�� �Ÿ� ����
        dragVector = Vector2.ClampMagnitude(dragVector, maxDragDistance);

        // ������ ���� ǥ��
        if (aimLine != null)
        {
            ShowTrajectory(dragVector);
        }
    }

    void LaunchBall()
    {
        if (currentBall == null) return;

        // �߻� ���� �� �� ���
        Vector2 currentMouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dragVector = dragStartPos - currentMouseWorldPos;

        // �߻�� ���
        float launchForce = Mathf.Clamp(dragVector.magnitude * forceMultiplier, 0f, maxForce);
        Vector2 launchDirection = dragVector.normalized;

        // Rigidbody2D Ȱ��ȭ �� �߻�
        Rigidbody2D rb = currentBall.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.isKinematic = false; // ���� Ȱ��ȭ
            rb.AddForce(launchDirection * launchForce, ForceMode2D.Impulse);
        }
        else
        {
            rb.isKinematic = true;
        }

        // �巡�� ����
        isDragging = false;
        if (aimLine != null)
            aimLine.enabled = false;

        // ��� �� �� ��ü ����
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

        if (!isGameOver)                //���� ������ �ƴ� ���� �� �� ����
        {
            currentBallType = Random.Range(0, 3);                //0 ~ 2 ������ ���� �� Ÿ��

            float halfBallSize = ballSizes[currentBallType] / 2;

            currentBall = Instantiate(ballPrefab[currentBallType]); //�� ����
            currentBall.transform.localScale = new Vector3(ballSizes[currentBallType], ballSizes[currentBallType], 1);      //�� ũ�� ����

            Rigidbody2D rb2 = currentBall.GetComponent<Rigidbody2D>();
        }

        // Rigidbody2D�� kinematic���� ���� (��� ����)
        Rigidbody2D rb = currentBall.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.isKinematic = true;
            // Z�� ȸ�� ���� (2D������ Z�� ȸ���� �ʿ�)
            rb.freezeRotation = true;
        }
    }
    public void CheckGameOVer()                         //���� ���� üũ �Լ�
    {
        Ball[] allBalls = FindObjectsOfType<Ball>();    //Scene�� �ִ� ��� ���� ������Ʈ�� �پ��ִ� ������Ʈ�� �����´�. ���� ���ӿ����� ��� ����� ��

        float gameOverHeight = gameHeight;              //���� ���̺��� ���� ��ġ�� ������ �ִ��� Ȯ��

        for (int i = 0; i < allBalls.Length; i++)        //��� ������ �˻��Ѵ�.
        {
            if (allBalls[i] != null)
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
    public void MergeBalls(int ballType, Vector3 position)
    {
        PlaySound(mergeSound);
        // �ִ� ũ�� ���̸� �������� �ʰ� ����
        if (ballType >= ballPrefab.Length - 1)
        {
            Debug.Log("�ִ� ũ���̹Ƿ� ���� �Ұ�");
            return;
        }

        // ���� ������ ���: ���ο� �� ����
        GameObject newBall = Instantiate(ballPrefab[ballType + 1], position, Quaternion.identity);
        newBall.transform.localScale = new Vector3(ballSizes[ballType + 1], ballSizes[ballType + 1], 1.0f);

        // ���� �߰�
        int addScore = (ballType + 1) * 10;
        score += addScore;
        UpdateScoreText();
    }
    void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "����: " + score.ToString();
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

        // ���� �ؽ�Ʈ ����
        if (resultText != null)
            resultText.text = $"���� ���� \n����: {score}��";

        // ���� ����
        Ranking.SaveNewScore(score);

        // �г��� 1�� �ڿ� ���
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

