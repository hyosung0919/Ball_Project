using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallShooter : MonoBehaviour
{
    [Header("발사 설정")]
    public GameObject ballPrefab;           // 구체 프리팹
    public Transform startPosition;         // 시작 위치 (구체가 있는 곳)
    public float maxForce = 20f;            // 최대 발사력
    public float forceMultiplier = 5f;      // 힘 배율

    [Header("드래그 설정")]
    public float maxDragDistance = 3f;      // 최대 드래그 거리
    public LineRenderer aimLine;            // 조준선
    public int trajectoryPoints = 30;       // 궤적 점 개수

    private GameObject currentBall;         // 현재 대기 중인 구체
    private bool isDragging = false;        // 드래그 중인지
    private Vector3 dragStartPos;           // 드래그 시작 위치
    private Camera mainCamera;              // 메인 카메라

    void Start()
    {
        mainCamera = Camera.main;

        // 조준선 설정
        if (aimLine != null)
        {
            aimLine.positionCount = trajectoryPoints;
            aimLine.enabled = false;
        }

        // 첫 번째 구체 생성
        CreateNewBall();
    }

    void ShowTrajectory(Vector3 worldDrag)
    {
        if (currentBall == null) return;

        Vector3 startPos = currentBall.transform.position;
        float launchForce = Mathf.Clamp(worldDrag.magnitude * forceMultiplier, 0f, maxForce);
        Vector3 launchVelocity = worldDrag.normalized * launchForce;

        // 포물선 궤적 계산
        for (int i = 0; i < trajectoryPoints; i++)
        {
            float time = i * 0.1f;
            Vector3 point = CalculateTrajectoryPoint(startPos, launchVelocity, time);
            aimLine.SetPosition(i, point);
        }
    }

    Vector3 CalculateTrajectoryPoint(Vector3 startPos, Vector3 startVel, float time)
    {
        // 포물선 운동 공식: 위치 = 초기위치 + 초기속도*시간 + 0.5*중력*시간^2
        return startPos + startVel * time + 0.5f * Physics.gravity * time * time;
    }

    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        if (currentBall == null) return;

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
        dragStartPos = Input.mousePosition;

        if (aimLine != null)
            aimLine.enabled = true;
    }

    void UpdateDrag()
    {
        // 현재 마우스 위치와 시작 위치의 차이 계산
        Vector3 currentMousePos = Input.mousePosition;
        Vector3 dragVector = dragStartPos - currentMousePos;

        // 화면 좌표를 월드 좌표로 변환
        Vector3 worldDrag = mainCamera.ScreenToWorldPoint(new Vector3(dragVector.x, dragVector.y, 10f));
        worldDrag = worldDrag - mainCamera.ScreenToWorldPoint(new Vector3(0, 0, 10f));

        // 드래그 거리 제한
        worldDrag = Vector3.ClampMagnitude(worldDrag, maxDragDistance);

        // 포물선 궤적 표시
        if (aimLine != null)
        {
            ShowTrajectory(worldDrag);
        }
    }

    void LaunchBall()
    {
        if (currentBall == null) return;

        // 발사 방향 및 힘 계산
        Vector3 currentMousePos = Input.mousePosition;
        Vector3 dragVector = dragStartPos - currentMousePos;

        // 화면 좌표를 월드 좌표로 변환
        Vector3 worldDrag = mainCamera.ScreenToWorldPoint(new Vector3(dragVector.x, dragVector.y, 10f));
        worldDrag = worldDrag - mainCamera.ScreenToWorldPoint(new Vector3(0, 0, 10f));

        // 발사력 계산
        float launchForce = Mathf.Clamp(worldDrag.magnitude * forceMultiplier, 0f, maxForce);
        Vector3 launchDirection = worldDrag.normalized;

        // Rigidbody 활성화 및 발사
        Rigidbody rb = currentBall.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false; // 물리 활성화
            rb.AddForce(launchDirection * launchForce, ForceMode.Impulse);
        }

        // 드래그 종료
        isDragging = false;
        if (aimLine != null)
            aimLine.enabled = false;

        // 구체 자동 제거 및 새 구체 생성
        Destroy(currentBall, 10f);
        currentBall = null;

        // 잠시 후 새 구체 생성
        Invoke("CreateNewBall", 1f);
    }

    void CreateNewBall()
    {
        if (ballPrefab == null || startPosition == null)
            return;

        // 새 구체 생성
        currentBall = Instantiate(ballPrefab, startPosition.position, Quaternion.identity);

        // Rigidbody를 kinematic으로 설정 (대기 상태)
        Rigidbody rb = currentBall.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }
    }
}
