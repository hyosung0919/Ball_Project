using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallShooter : MonoBehaviour
{
    [Header("�߻� ����")]
    public GameObject ballPrefab;           // ��ü ������
    public Transform startPosition;         // ���� ��ġ (��ü�� �ִ� ��)
    public float maxForce = 20f;            // �ִ� �߻��
    public float forceMultiplier = 5f;      // �� ����

    [Header("�巡�� ����")]
    public float maxDragDistance = 3f;      // �ִ� �巡�� �Ÿ�
    public LineRenderer aimLine;            // ���ؼ�
    public int trajectoryPoints = 30;       // ���� �� ����

    private GameObject currentBall;         // ���� ��� ���� ��ü
    private bool isDragging = false;        // �巡�� ������
    private Vector3 dragStartPos;           // �巡�� ���� ��ġ
    private Camera mainCamera;              // ���� ī�޶�

    void Start()
    {
        mainCamera = Camera.main;

        // ���ؼ� ����
        if (aimLine != null)
        {
            aimLine.positionCount = trajectoryPoints;
            aimLine.enabled = false;
        }

        // ù ��° ��ü ����
        CreateNewBall();
    }

    void ShowTrajectory(Vector3 worldDrag)
    {
        if (currentBall == null) return;

        Vector3 startPos = currentBall.transform.position;
        float launchForce = Mathf.Clamp(worldDrag.magnitude * forceMultiplier, 0f, maxForce);
        Vector3 launchVelocity = worldDrag.normalized * launchForce;

        // ������ ���� ���
        for (int i = 0; i < trajectoryPoints; i++)
        {
            float time = i * 0.1f;
            Vector3 point = CalculateTrajectoryPoint(startPos, launchVelocity, time);
            aimLine.SetPosition(i, point);
        }
    }

    Vector3 CalculateTrajectoryPoint(Vector3 startPos, Vector3 startVel, float time)
    {
        // ������ � ����: ��ġ = �ʱ���ġ + �ʱ�ӵ�*�ð� + 0.5*�߷�*�ð�^2
        return startPos + startVel * time + 0.5f * Physics.gravity * time * time;
    }

    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        if (currentBall == null) return;

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
        dragStartPos = Input.mousePosition;

        if (aimLine != null)
            aimLine.enabled = true;
    }

    void UpdateDrag()
    {
        // ���� ���콺 ��ġ�� ���� ��ġ�� ���� ���
        Vector3 currentMousePos = Input.mousePosition;
        Vector3 dragVector = dragStartPos - currentMousePos;

        // ȭ�� ��ǥ�� ���� ��ǥ�� ��ȯ
        Vector3 worldDrag = mainCamera.ScreenToWorldPoint(new Vector3(dragVector.x, dragVector.y, 10f));
        worldDrag = worldDrag - mainCamera.ScreenToWorldPoint(new Vector3(0, 0, 10f));

        // �巡�� �Ÿ� ����
        worldDrag = Vector3.ClampMagnitude(worldDrag, maxDragDistance);

        // ������ ���� ǥ��
        if (aimLine != null)
        {
            ShowTrajectory(worldDrag);
        }
    }

    void LaunchBall()
    {
        if (currentBall == null) return;

        // �߻� ���� �� �� ���
        Vector3 currentMousePos = Input.mousePosition;
        Vector3 dragVector = dragStartPos - currentMousePos;

        // ȭ�� ��ǥ�� ���� ��ǥ�� ��ȯ
        Vector3 worldDrag = mainCamera.ScreenToWorldPoint(new Vector3(dragVector.x, dragVector.y, 10f));
        worldDrag = worldDrag - mainCamera.ScreenToWorldPoint(new Vector3(0, 0, 10f));

        // �߻�� ���
        float launchForce = Mathf.Clamp(worldDrag.magnitude * forceMultiplier, 0f, maxForce);
        Vector3 launchDirection = worldDrag.normalized;

        // Rigidbody Ȱ��ȭ �� �߻�
        Rigidbody rb = currentBall.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false; // ���� Ȱ��ȭ
            rb.AddForce(launchDirection * launchForce, ForceMode.Impulse);
        }

        // �巡�� ����
        isDragging = false;
        if (aimLine != null)
            aimLine.enabled = false;

        // ��ü �ڵ� ���� �� �� ��ü ����
        Destroy(currentBall, 10f);
        currentBall = null;

        // ��� �� �� ��ü ����
        Invoke("CreateNewBall", 1f);
    }

    void CreateNewBall()
    {
        if (ballPrefab == null || startPosition == null)
            return;

        // �� ��ü ����
        currentBall = Instantiate(ballPrefab, startPosition.position, Quaternion.identity);

        // Rigidbody�� kinematic���� ���� (��� ����)
        Rigidbody rb = currentBall.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }
    }
}
