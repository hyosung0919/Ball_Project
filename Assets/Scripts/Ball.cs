using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using Unity.VisualScripting;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public int ballType;                                            //공 타입 ( 0 : 탁구공 1: 야구공 2: 농구공 3: 축구공 ) int로 index 설정

    public bool hasMered = false;                                   //공이 합쳐졌는지 확인하는 플래그

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasMered)                                               //이미 합쳐진 공은 무시
            return;

        Ball otherBall = collision.gameObject.GetComponent<Ball>();             //다른 공과 충돌했는지 확인

        if (otherBall != null && !otherBall.hasMered && otherBall.ballType == ballType)     //충돌한 것이 공이고 타입이 같다면 (합쳐지지 않았을 경우)
        {
            hasMered = true;                //합쳤다고 표시
            otherBall.hasMered = true;

            Vector3 mergePosition = (transform.position + otherBall.transform.position) / 2f;       //두 공의 중간 위치 계산

            //게임 매니저에서 Merge 구현된 것을 호출
            BallGame gameManager = FindObjectOfType<BallGame>();
            if(gameManager != null)
            {
                gameManager.MergeBalls(ballType, mergePosition);            //함수를 실행하고 호출한다.
            }

            //공들 제거
            Destroy(otherBall.gameObject);
            Destroy(gameObject);
        }
    }
}
