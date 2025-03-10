using UnityEngine;

namespace KartGame.KartSystems
{

    [RequireComponent(typeof(ArcadeKart))]
    public class KartBounce : MonoBehaviour
    {
      
        public bool BounceFlag { get; private set; }

        [Tooltip("충돌 시 카트에 적용할 충격량")]
        public float BounceFactor = 10f;
        [Tooltip("충돌 시 카트가 자신을 재정렬하는 속도. 값이 클수록 더 빠르게 정렬됨")]
        public float RotationSpeed = 3f;
        [Tooltip("이 게임오브젝트가 충돌해야 하는 레이어")]
        public LayerMask CollisionLayer;
        [Tooltip("카트가 얼마나 멀리 앞을 감지하여 튕겨야 하는가? 양수 값이어야 함")]
        public float RayDistance = 1f;
        [Tooltip("한 번 튕긴 후 카트가 언제 다시 튕길 수 있는지")]
        public float PauseTime = 0.5f;
        [Tooltip("더 큰 차량을 위해 레이 원점의 y 높이를 조정해야 하는가?")]
        public float HeightOffset;
        [Tooltip("튕김을 감지하기 위해 발사할 레이캐스트 수와 각도. DrawGizmos를 활성화하여 각도 선택 시 레이캐스트가 어떻게 보이는지 디버깅 가능")]
        public float[] Angles;

        [Tooltip("디버깅을 위해 기즈모를 그려야 하는가? 레이를 확인하는 데 도움이 됨")]
        public bool DrawGizmos;
        [Tooltip("카트가 벽과 충돌할 때 재생할 오디오 클립")]
        public AudioClip BounceSound;

        // 아케이드 카트 컴포넌트 참조
        ArcadeKart kart;
        // 다시 이동 가능한 시간
        float resumeTime;
        // 충돌 여부
        bool hasCollided;
        // 반사 벡터
        Vector3 reflectionVector;

        // 초기화
        void Start()
        {
            // 카트 컴포넌트 가져오기
            kart = GetComponent<ArcadeKart>();
        }

        // 매 프레임 업데이트
        void Update()
        {
            // 트리거 플래그 리셋
            if (BounceFlag)
            {
                BounceFlag = false;
            }

            // 레이캐스트 시작 위치 설정
            Vector3 origin = transform.position;
            origin.y += HeightOffset;

            // 모든 각도에 대해 레이캐스트 수행
            for (int i = 0; i < Angles.Length; i++)
            {
                // 각도에 따른 방향 계산
                Vector3 direction = GetDirectionFromAngle(Angles[i], Vector3.up, transform.forward);

                // 레이캐스트로 충돌 감지
                if (Physics.Raycast(origin, direction, out RaycastHit hit, RayDistance, CollisionLayer) && Time.time > resumeTime && !hasCollided && kart.LocalSpeed() > 0)
                {
                    // 히트 노멀이 위쪽을 향하면 튕기지 않음 (경사로 등)
                    if (Vector3.Dot(hit.normal, Vector3.up) > 0.2f)
                    {
                        return;
                    }

                    // 카트가 물체와 충돌하는 입사 벡터 계산
                    Vector3 incidentVector = hit.point - origin;

                    // 충돌의 입사 벡터를 사용하여 반사 벡터 계산
                    Vector3 hitNormal = hit.normal.normalized;
                    reflectionVector = incidentVector - 2 * Vector3.Dot(incidentVector, hitNormal) * hitNormal;
                    reflectionVector.y = 0;  // y축 반사는 무시 (높이 변화 방지)

                    // 속도 감소
                    kart.Rigidbody.velocity /= 2;

                    // 반사 벡터 방향으로 튕김 충격 적용
                    kart.Rigidbody.AddForce(reflectionVector.normalized * BounceFactor, ForceMode.Impulse);

                    // 차량이 충돌했음을 표시하고 재개 시간 설정
                    kart.SetCanMove(false);
                    BounceFlag = hasCollided = true;
                    resumeTime = Time.time + PauseTime;

                    // 튕김 소리 재생
                    if (BounceSound)
                    {
                        AudioUtility.CreateSFX(BounceSound, transform.position, AudioUtility.AudioGroups.Collision, 0f);
                    }
                    return;
                }
            }

            // 튕김 후 재개 시간 동안의 회전 처리
            if (Time.time < resumeTime)
            {
                // 반사 벡터 방향으로 카트 회전
                Vector3 targetPos = origin + reflectionVector;
                Vector3 direction = targetPos - origin;
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                kart.transform.rotation = Quaternion.Slerp(kart.transform.rotation, targetRotation, RotationSpeed * Time.deltaTime);
            }
        }

        // 모든 업데이트 후 처리
        void LateUpdate()
        {
            // 재개 시간이 지났고 충돌 상태였으면 이동 가능하게 설정
            if (Time.time > resumeTime && hasCollided)
            {
                kart.SetCanMove(true);
                hasCollided = false;
            }
        }

        // 기즈모 그리기 (편집기에서만 표시)
        void OnDrawGizmos()
        {
            if (DrawGizmos)
            {
                // 레이캐스트 시작 위치
                Vector3 origin = transform.position;
                origin.y += HeightOffset;

                // 전방 방향 표시
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(origin, origin + transform.forward);

                // 각 레이캐스트 방향 표시
                Gizmos.color = Color.red;
                for (int i = 0; i < Angles.Length; i++)
                {
                    var direction = GetDirectionFromAngle(Angles[i], Vector3.up, transform.forward);
                    Gizmos.DrawLine(origin, origin + (direction.normalized * RayDistance));
                }
            }
        }

        // 각도로부터 방향 벡터 계산
        Vector3 GetDirectionFromAngle(float degrees, Vector3 axis, Vector3 zerothDirection)
        {
            Quaternion rotation = Quaternion.AngleAxis(degrees, axis);
            return (rotation * zerothDirection);
        }
    }
}