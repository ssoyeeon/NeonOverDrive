using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.VFX;

namespace KartGame.KartSystems
{
    // 아케이드 스타일의 카트 물리 및 제어를 담당하는 클래스
    public class ArcadeKart : MonoBehaviour
    {
        // 파워업으로 인한 스탯 변경을 추적하는 클래스
        [System.Serializable]
        public class StatPowerup
        {
            public ArcadeKart.Stats modifiers; // 스탯 수정자
            public string PowerUpID;           // 파워업 식별자
            public float ElapsedTime;          // 경과 시간
            public float MaxTime;              // 최대 지속 시간
        }

        // 카트의 모든 물리적 특성을 정의하는 구조체
        [System.Serializable]
        public struct Stats
        {
            [Header("Movement Settings")]
            [Min(0.001f), Tooltip("전진할 때 도달할 수 있는 최대 속도")]
            public float TopSpeed;

            [Tooltip("카트가 최대 속도에 도달하는 속도")]
            public float Acceleration;

            [Min(0.001f), Tooltip("후진할 때 도달할 수 있는 최대 속도")]
            public float ReverseSpeed;

            [Tooltip("후진할 때 카트가 최대 속도에 도달하는 속도")]
            public float ReverseAcceleration;

            [Tooltip("카트가 0에서 가속을 시작하는 속도. 높은 값은 더 빨리 가속함을 의미")]
            [Range(0.2f, 1)]
            public float AccelerationCurve;

            [Tooltip("브레이크를 밟을 때 카트가 감속하는 속도")]
            public float Braking;

            [Tooltip("입력이 없을 때 카트가 완전히 정지할 때까지 걸리는 시간")]
            public float CoastingDrag;

            [Range(0.0f, 1.0f)]
            [Tooltip("카트의 좌우 마찰력 정도")]
            public float Grip;

            [Tooltip("카트가 좌우로 얼마나 빠르게 회전할 수 있는지")]
            public float Steer;

            [Tooltip("카트가 공중에 있을 때 추가되는 중력")]
            public float AddedGravity;

            // 파워업을 위한 스탯 더하기 연산자 오버로딩
            public static Stats operator +(Stats a, Stats b)
            {
                return new Stats
                {
                    Acceleration = a.Acceleration + b.Acceleration,
                    AccelerationCurve = a.AccelerationCurve + b.AccelerationCurve,
                    Braking = a.Braking + b.Braking,
                    CoastingDrag = a.CoastingDrag + b.CoastingDrag,
                    AddedGravity = a.AddedGravity + b.AddedGravity,
                    Grip = a.Grip + b.Grip,
                    ReverseAcceleration = a.ReverseAcceleration + b.ReverseAcceleration,
                    ReverseSpeed = a.ReverseSpeed + b.ReverseSpeed,
                    TopSpeed = a.TopSpeed + b.TopSpeed,
                    Steer = a.Steer + b.Steer,
                };
            }
        }

        // 주요 속성들
        public Rigidbody Rigidbody { get; private set; }    // 카트의 물리 컴포넌트
        public InputData Input { get; private set; }    // 입력 데이터
        public float AirPercent { get; private set; }    // 공중에 있는 정도 (0-1)
        public float GroundPercent { get; private set; }    // 지면에 있는 정도 (0-1)

        // 카트의 기본 스탯 설정
        public ArcadeKart.Stats baseStats = new ArcadeKart.Stats
        {
            TopSpeed = 20f,
            Acceleration = 5f,
            AccelerationCurve = 4f,
            Braking = 10f,
            ReverseAcceleration = 5f,
            ReverseSpeed = 5f,
            Steer = 5f,
            CoastingDrag = 4f,
            Grip = .95f,
            AddedGravity = 1f,
        };

        [Header("Vehicle Visual")]
        public List<GameObject> m_VisualWheels;    // 시각적 휠 오브젝트들

        [Header("Vehicle Physics")]
        [Tooltip("카트 질량의 위치를 결정하는 트랜스폼")]
        public Transform CenterOfMass;

        [Range(0.0f, 20.0f), Tooltip("공중에서 카트를 재정렬하는 데 사용되는 계수. 숫자가 클수록 카트가 수평면을 따라 더 빨리 재조정됨")]
        public float AirborneReorientationCoefficient = 3.0f;

        [Header("Drifting")]
        [Range(0.01f, 1.0f), Tooltip("드리프트 시 그립 값")]
        public float DriftGrip = 0.4f;
        [Range(0.0f, 10.0f), Tooltip("카트가 드리프트할 때 추가 조향")]
        public float DriftAdditionalSteer = 5.0f;
        [Range(1.0f, 30.0f), Tooltip("각도가 클수록 완전한 그립을 다시 얻기가 더 쉬움")]
        public float MinAngleToFinishDrift = 10.0f;
        [Range(0.01f, 0.99f), Tooltip("완전한 그립으로 돌아가기 위한 최소 속도 비율")]
        public float MinSpeedPercentToFinishDrift = 0.5f;
        [Range(1.0f, 20.0f), Tooltip("값이 클수록 드리프트 조향을 제어하기가 더 쉬움")]
        public float DriftControl = 10.0f;
        [Range(0.0f, 20.0f), Tooltip("값이 낮을수록 조향으로 제어하지 않고도 드리프트가 더 오래 지속됨")]
        public float DriftDampening = 10.0f;

        [Header("VFX")]
        [Tooltip("드리프트할 때 휠에 표시될 VFX")]
        public ParticleSystem DriftSparkVFX;
        [Range(0.0f, 0.2f), Tooltip("VFX를 측면으로 이동시키는 오프셋")]
        public float DriftSparkHorizontalOffset = 0.1f;
        [Range(0.0f, 90.0f), Tooltip("VFX를 회전시키는 각도")]
        public float DriftSparkRotation = 17.0f;
        [Tooltip("드리프트할 때 휠에 표시될 VFX")]
        public GameObject DriftTrailPrefab;
        [Range(-0.1f, 0.1f), Tooltip("트레일을 위아래로 이동시켜 지면 위에 있도록 하는 수직 오프셋")]
        public float DriftTrailVerticalOffset;
        [Tooltip("점프 후 착륙 시 생성될 VFX")]
        public GameObject JumpVFX;
        [Tooltip("카트의 노즐에 생성되는 VFX")]
        public GameObject NozzleVFX;
        [Tooltip("카트 노즐 위치 목록")]
        public List<Transform> Nozzles;

        [Header("Suspensions")]
        [Tooltip("카트 본체와 휠 사이의 최대 가능 확장 거리")]
        [Range(0.0f, 1.0f)]
        public float SuspensionHeight = 0.2f;
        [Range(10.0f, 100000.0f), Tooltip("값이 클수록 서스펜션이 더 딱딱해짐")]
        public float SuspensionSpring = 20000.0f;
        [Range(0.0f, 5000.0f), Tooltip("값이 클수록 카트가 더 빨리 안정화됨")]
        public float SuspensionDamp = 500.0f;
        [Tooltip("카트 본체에 상대적인 휠 위치를 조정하는 수직 오프셋")]
        [Range(-1.0f, 1.0f)]
        public float WheelsPositionVerticalOffset = 0.0f;

        [Header("Physical Wheels")]
        [Tooltip("카트 휠의 물리적 표현")]
        public WheelCollider FrontLeftWheel;
        public WheelCollider FrontRightWheel;
        public WheelCollider RearLeftWheel;
        public WheelCollider RearRightWheel;

        [Tooltip("휠이 감지할 레이어")]
        public LayerMask GroundLayers = Physics.DefaultRaycastLayers;

        // 카트를 제어할 수 있는 입력 소스
        IInput[] m_Inputs;

        // 상수 정의
        const float k_NullInput = 0.01f;
        const float k_NullSpeed = 0.01f;
        Vector3 m_VerticalReference = Vector3.up;

        // 드리프트 관련 변수
        public bool WantsToDrift { get; private set; } = false;  // 드리프트 요청 여부
        public bool IsDrifting { get; private set; } = false;    // 실제 드리프트 중인지 여부
        float m_CurrentGrip = 1.0f;                              // 현재 그립 값
        float m_DriftTurningPower = 0.0f;                        // 드리프트 회전력
        float m_PreviousGroundPercent = 1.0f;                    // 이전 프레임의 지면 접촉 비율
        readonly List<(GameObject trailRoot, WheelCollider wheel, TrailRenderer trail)> m_DriftTrailInstances = new List<(GameObject, WheelCollider, TrailRenderer)>();     // 드리프트 트레일 인스턴스
        readonly List<(WheelCollider wheel, float horizontalOffset, float rotation, ParticleSystem sparks)> m_DriftSparkInstances = new List<(WheelCollider, float, float, ParticleSystem)>(); // 드리프트 스파크 인스턴스

        // 카트가 움직일 수 있는지 여부
        bool m_CanMove = true;
        List<StatPowerup> m_ActivePowerupList = new List<StatPowerup>();  // 활성화된 파워업 목록
        ArcadeKart.Stats m_FinalStats;                                    // 파워업 적용 후 최종 스탯

        Quaternion m_LastValidRotation;                                   // 마지막 유효 회전
        Vector3 m_LastValidPosition;                                      // 마지막 유효 위치
        Vector3 m_LastCollisionNormal;                                    // 마지막 충돌 노멀
        bool m_HasCollision;                                              // 충돌 발생 여부
        bool m_InAir = false;                                             // 공중에 있는지 여부

        // 파워업 추가 메서드
        public void AddPowerup(StatPowerup statPowerup) => m_ActivePowerupList.Add(statPowerup);

        // 카트 이동 가능 여부 설정
        public void SetCanMove(bool move) => m_CanMove = move;

        // 최대 속도 반환
        public float GetMaxSpeed() => Mathf.Max(m_FinalStats.TopSpeed, m_FinalStats.ReverseSpeed);

        // 드리프트 VFX 활성화/비활성화
        private void ActivateDriftVFX(bool active)
        {
            // 스파크 효과 처리
            foreach (var vfx in m_DriftSparkInstances)
            {
                if (active && vfx.wheel.GetGroundHit(out WheelHit hit))
                {
                    if (!vfx.sparks.isPlaying)
                        vfx.sparks.Play();
                }
                else
                {
                    if (vfx.sparks.isPlaying)
                        vfx.sparks.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                }

            }

            // 트레일 효과 처리
            foreach (var trail in m_DriftTrailInstances)
                trail.Item3.emitting = active && trail.wheel.GetGroundHit(out WheelHit hit);
        }

        // 드리프트 VFX 방향 업데이트
        private void UpdateDriftVFXOrientation()
        {
            // 스파크 위치 및 회전 업데이트
            foreach (var vfx in m_DriftSparkInstances)
            {
                vfx.sparks.transform.position = vfx.wheel.transform.position - (vfx.wheel.radius * Vector3.up) + (DriftTrailVerticalOffset * Vector3.up) + (transform.right * vfx.horizontalOffset);
                vfx.sparks.transform.rotation = transform.rotation * Quaternion.Euler(0.0f, 0.0f, vfx.rotation);
            }

            // 트레일 위치 및 회전 업데이트
            foreach (var trail in m_DriftTrailInstances)
            {
                trail.trailRoot.transform.position = trail.wheel.transform.position - (trail.wheel.radius * Vector3.up) + (DriftTrailVerticalOffset * Vector3.up);
                trail.trailRoot.transform.rotation = transform.rotation;
            }
        }

        // 서스펜션 파라미터 업데이트
        void UpdateSuspensionParams(WheelCollider wheel)
        {
            // 서스펜션 거리 및 중심 설정
            wheel.suspensionDistance = SuspensionHeight;
            wheel.center = new Vector3(0.0f, WheelsPositionVerticalOffset, 0.0f);

            // 서스펜션 스프링 설정
            JointSpring spring = wheel.suspensionSpring;
            spring.spring = SuspensionSpring;
            spring.damper = SuspensionDamp;
            wheel.suspensionSpring = spring;
        }

        // 초기화
        void Awake()
        {
            // 컴포넌트 및 입력 참조 가져오기
            Rigidbody = GetComponent<Rigidbody>();
            m_Inputs = GetComponents<IInput>();

            // 모든 휠의 서스펜션 파라미터 업데이트
            UpdateSuspensionParams(FrontLeftWheel);
            UpdateSuspensionParams(FrontRightWheel);
            UpdateSuspensionParams(RearLeftWheel);
            UpdateSuspensionParams(RearRightWheel);

            // 초기 그립 설정
            m_CurrentGrip = baseStats.Grip;

            // 드리프트 스파크 추가
            if (DriftSparkVFX != null)
            {
                AddSparkToWheel(RearLeftWheel, -DriftSparkHorizontalOffset, -DriftSparkRotation);
                AddSparkToWheel(RearRightWheel, DriftSparkHorizontalOffset, DriftSparkRotation);
            }

            // 드리프트 트레일 추가
            if (DriftTrailPrefab != null)
            {
                AddTrailToWheel(RearLeftWheel);
                AddTrailToWheel(RearRightWheel);
            }

            // 노즐 VFX 추가
            if (NozzleVFX != null)
            {
                foreach (var nozzle in Nozzles)
                {
                    Instantiate(NozzleVFX, nozzle, false);
                }
            }
        }

        // 휠에 트레일 추가
        void AddTrailToWheel(WheelCollider wheel)
        {
            GameObject trailRoot = Instantiate(DriftTrailPrefab, gameObject.transform, false);
            TrailRenderer trail = trailRoot.GetComponentInChildren<TrailRenderer>();
            trail.emitting = false;
            m_DriftTrailInstances.Add((trailRoot, wheel, trail));
        }

        // 휠에 스파크 추가
        void AddSparkToWheel(WheelCollider wheel, float horizontalOffset, float rotation)
        {
            GameObject vfx = Instantiate(DriftSparkVFX.gameObject, wheel.transform, false);
            ParticleSystem spark = vfx.GetComponent<ParticleSystem>();
            spark.Stop();
            m_DriftSparkInstances.Add((wheel, horizontalOffset, -rotation, spark));
        }

        // 물리 업데이트
        void FixedUpdate()
        {
            // 모든 휠의 서스펜션 파라미터 업데이트
            UpdateSuspensionParams(FrontLeftWheel);
            UpdateSuspensionParams(FrontRightWheel);
            UpdateSuspensionParams(RearLeftWheel);
            UpdateSuspensionParams(RearRightWheel);

            // 입력 수집
            GatherInputs();

            // 파워업 적용하여 최종 스탯 계산
            TickPowerups();

            // 물리 속성 적용
            Rigidbody.centerOfMass = transform.InverseTransformPoint(CenterOfMass.position);

            // 지면에 닿은 휠 수 계산
            int groundedCount = 0;
            if (FrontLeftWheel.isGrounded && FrontLeftWheel.GetGroundHit(out WheelHit hit))
                groundedCount++;
            if (FrontRightWheel.isGrounded && FrontRightWheel.GetGroundHit(out hit))
                groundedCount++;
            if (RearLeftWheel.isGrounded && RearLeftWheel.GetGroundHit(out hit))
                groundedCount++;
            if (RearRightWheel.isGrounded && RearRightWheel.GetGroundHit(out hit))
                groundedCount++;

            // 지면 접촉 비율 및 공중 비율 계산
            GroundPercent = (float)groundedCount / 4.0f;
            AirPercent = 1 - GroundPercent;

            // 차량 물리 적용
            if (m_CanMove)
            {
                MoveVehicle(Input.Accelerate, Input.Brake, Input.TurnInput);
            }
            GroundAirbourne();

            // 이전 지면 접촉 비율 저장
            m_PreviousGroundPercent = GroundPercent;

            // 드리프트 VFX 방향 업데이트
            UpdateDriftVFXOrientation();
        }

        // 입력 수집
        void GatherInputs()
        {
            // 입력 초기화
            Input = new InputData();
            WantsToDrift = false;

            // 모든 입력 소스에서 0이 아닌 입력 수집
            for (int i = 0; i < m_Inputs.Length; i++)
            {
                Input = m_Inputs[i].GenerateInput();
                WantsToDrift = Input.Brake && Vector3.Dot(Rigidbody.velocity, transform.forward) > 0.0f;
            }
        }

        void TickPowerups()
        {
            // 지속시간이 만료된 모든 파워업 제거
            m_ActivePowerupList.RemoveAll((p) => { return p.ElapsedTime > p.MaxTime; });

            // 파워업 계산 전 초기화
            var powerups = new Stats();

            // 모든 활성 파워업 효과 합산
            for (int i = 0; i < m_ActivePowerupList.Count; i++)
            {
                var p = m_ActivePowerupList[i];

                // 경과 시간 추가
                p.ElapsedTime += Time.fixedDeltaTime;

                // 파워업 효과 합산
                powerups += p.modifiers;
            }

            // 기본 스탯에 파워업 효과를 더해 최종 스탯 계산
            m_FinalStats = baseStats + powerups;

            // 최종 스탯 값 제한 (그립은 0~1 사이로 제한)
            m_FinalStats.Grip = Mathf.Clamp(m_FinalStats.Grip, 0, 1);
        }

        // 공중에서의 동작 처리
        void GroundAirbourne()
        {
            // 공중에 있을 때 더 빨리 떨어지도록 중력 가속
            if (AirPercent >= 1)
            {
                Rigidbody.velocity += Physics.gravity * Time.fixedDeltaTime * m_FinalStats.AddedGravity;
            }
        }

        // 카트 회전 초기화 (x축과 z축 회전을 0으로 설정)
        public void Reset()
        {
            Vector3 euler = transform.rotation.eulerAngles;
            euler.x = euler.z = 0f;
            transform.rotation = Quaternion.Euler(euler);
        }

        // 카트의 로컬 속도 계산 (0~1 사이의 정규화된 값)
        public float LocalSpeed()
        {
            if (m_CanMove)
            {
                // 현재 속도와 전방 벡터 사이의 내적
                float dot = Vector3.Dot(transform.forward, Rigidbody.velocity);
                if (Mathf.Abs(dot) > 0.1f)
                {
                    float speed = Rigidbody.velocity.magnitude;
                    // 전진 또는 후진에 따라 속도 정규화
                    return dot < 0 ? -(speed / m_FinalStats.ReverseSpeed) : (speed / m_FinalStats.TopSpeed);
                }
                return 0f;
            }
            else
            {
                // 레이스 시작 카운트다운 대기 중 사운드 재생용 값
                return Input.Accelerate ? 1.0f : 0.0f;
            }
        }

        // 충돌 시작 시 처리
        void OnCollisionEnter(Collision collision) => m_HasCollision = true;

        // 충돌 종료 시 처리
        void OnCollisionExit(Collision collision) => m_HasCollision = false;

        // 충돌 지속 중 처리
        void OnCollisionStay(Collision collision)
        {
            // 충돌 상태 설정
            m_HasCollision = true;
            m_LastCollisionNormal = Vector3.zero;
            float dot = -1.0f;

            // 모든 접촉점 중 가장 위쪽을 향하는 노멀 찾기
            foreach (var contact in collision.contacts)
            {
                if (Vector3.Dot(contact.normal, Vector3.up) > dot)
                    m_LastCollisionNormal = contact.normal;
            }
        }

        // 차량 이동 및 드리프트 처리
        void MoveVehicle(bool accelerate, bool brake, float turnInput)
        {
            // 가속 입력 계산 (가속 - 제동)
            float accelInput = (accelerate ? 1.0f : 0.0f) - (brake ? 1.0f : 0.0f);

            // 수동 가속 곡선 계수
            float accelerationCurveCoeff = 5;
            Vector3 localVel = transform.InverseTransformVector(Rigidbody.velocity);

            // 가속 및 속도 방향 확인
            bool accelDirectionIsFwd = accelInput >= 0;
            bool localVelDirectionIsFwd = localVel.z >= 0;

            // 현재 이동 방향에 따른 최대 속도 및 가속도 설정
            float maxSpeed = localVelDirectionIsFwd ? m_FinalStats.TopSpeed : m_FinalStats.ReverseSpeed;
            float accelPower = accelDirectionIsFwd ? m_FinalStats.Acceleration : m_FinalStats.ReverseAcceleration;

            // 현재 속도와 관련된 가속 곡선 계산
            float currentSpeed = Rigidbody.velocity.magnitude;
            float accelRampT = currentSpeed / maxSpeed;
            float multipliedAccelerationCurve = m_FinalStats.AccelerationCurve * accelerationCurveCoeff;
            float accelRamp = Mathf.Lerp(multipliedAccelerationCurve, 1, accelRampT * accelRampT);

            // 제동 중인지 확인
            bool isBraking = (localVelDirectionIsFwd && brake) || (!localVelDirectionIsFwd && accelerate);

            // 제동 중이면 제동 가속도 사용, 아니면 일반 가속도 사용
            float finalAccelPower = isBraking ? m_FinalStats.Braking : accelPower;

            // 최종 가속도 계산
            float finalAcceleration = finalAccelPower * accelRamp;

            // 회전력 계산 (드리프트 중이면 드리프트 회전력, 아니면 조향 입력)
            float turningPower = IsDrifting ? m_DriftTurningPower : turnInput * m_FinalStats.Steer;

            // 회전 각도 및 전방 벡터 계산
            Quaternion turnAngle = Quaternion.AngleAxis(turningPower, transform.up);
            Vector3 fwd = turnAngle * transform.forward;

            // 이동 벡터 계산 (지면에 닿아 있거나 충돌 중일 때만 적용)
            Vector3 movement = fwd * accelInput * finalAcceleration * ((m_HasCollision || GroundPercent > 0.0f) ? 1.0f : 0.0f);

            // 최대 속도 초과 확인
            bool wasOverMaxSpeed = currentSpeed >= maxSpeed;

            // 최대 속도 초과 시 가속 불가
            if (wasOverMaxSpeed && !isBraking)
                movement *= 0.0f;

            // 새 속도 계산
            Vector3 newVelocity = Rigidbody.velocity + movement * Time.fixedDeltaTime;
            newVelocity.y = Rigidbody.velocity.y;  // 수직 속도는 유지

            // 지면에 있고 최대 속도를 초과하지 않은 경우 최대 속도 제한
            if (GroundPercent > 0.0f && !wasOverMaxSpeed)
            {
                newVelocity = Vector3.ClampMagnitude(newVelocity, maxSpeed);
            }

            // 가속 버튼을 누르지 않을 때 감속 (관성 드래그)
            if (Mathf.Abs(accelInput) < k_NullInput && GroundPercent > 0.0f)
            {
                newVelocity = Vector3.MoveTowards(newVelocity, new Vector3(0, Rigidbody.velocity.y, 0), Time.fixedDeltaTime * m_FinalStats.CoastingDrag);
            }

            // 최종 속도 적용
            Rigidbody.velocity = newVelocity;

            // 드리프트 처리
            if (GroundPercent > 0.0f)
            {
                // 공중에서 착지한 경우 VFX 생성
                if (m_InAir)
                {
                    m_InAir = false;
                    Instantiate(JumpVFX, transform.position, Quaternion.identity);
                }

                // 각속도 계수 설정
                float angularVelocitySteering = 0.4f;
                float angularVelocitySmoothSpeed = 20f;

                // 후진 중이고 후진 버튼을 누른 경우 회전 반전
                if (!localVelDirectionIsFwd && !accelDirectionIsFwd)
                    angularVelocitySteering *= -1.0f;

                // 현재 각속도 가져오기
                var angularVel = Rigidbody.angularVelocity;

                // Y축 각속도 목표치로 조정
                angularVel.y = Mathf.MoveTowards(angularVel.y, turningPower * angularVelocitySteering, Time.fixedDeltaTime * angularVelocitySmoothSpeed);

                // 각속도 적용
                Rigidbody.angularVelocity = angularVel;

                // 속도 조향 계수
                float velocitySteering = 25f;

                // 카트가 착지할 때 속도 방향과 전방이 일치하지 않으면 드리프트 시작
                if (GroundPercent >= 0.0f && m_PreviousGroundPercent < 0.1f)
                {
                    Vector3 flattenVelocity = Vector3.ProjectOnPlane(Rigidbody.velocity, m_VerticalReference).normalized;
                    if (Vector3.Dot(flattenVelocity, transform.forward * Mathf.Sign(accelInput)) < Mathf.Cos(MinAngleToFinishDrift * Mathf.Deg2Rad))
                    {
                        IsDrifting = true;
                        m_CurrentGrip = DriftGrip;
                        m_DriftTurningPower = 0.0f;
                    }
                }

                // 드리프트 관리
                if (!IsDrifting)
                {
                    // 드리프트 시작 조건: 드리프트 요청 또는 제동 중이고, 속도가 충분함
                    if ((WantsToDrift || isBraking) && currentSpeed > maxSpeed * MinSpeedPercentToFinishDrift)
                    {
                        IsDrifting = true;
                        m_DriftTurningPower = turningPower + (Mathf.Sign(turningPower) * DriftAdditionalSteer);
                        m_CurrentGrip = DriftGrip;

                        ActivateDriftVFX(true);  // 드리프트 시각 효과 활성화
                    }
                }

                // 드리프트 중인 경우 처리
                if (IsDrifting)
                {
                    // 조향 입력이 없을 때 자연 감쇠
                    float turnInputAbs = Mathf.Abs(turnInput);
                    if (turnInputAbs < k_NullInput)
                        m_DriftTurningPower = Mathf.MoveTowards(m_DriftTurningPower, 0.0f, Mathf.Clamp01(DriftDampening * Time.fixedDeltaTime));

                    // 입력에 따라 드리프트 회전력 업데이트
                    float driftMaxSteerValue = m_FinalStats.Steer + DriftAdditionalSteer;
                    m_DriftTurningPower = Mathf.Clamp(m_DriftTurningPower + (turnInput * Mathf.Clamp01(DriftControl * Time.fixedDeltaTime)), -driftMaxSteerValue, driftMaxSteerValue);

                    // 속도 방향과 카트 전방이 일치하는지 확인
                    bool facingVelocity = Vector3.Dot(Rigidbody.velocity.normalized, transform.forward * Mathf.Sign(accelInput)) > Mathf.Cos(MinAngleToFinishDrift * Mathf.Deg2Rad);

                    // 드리프트 종료 가능 여부 확인
                    bool canEndDrift = true;
                    if (isBraking)
                        canEndDrift = false;
                    else if (!facingVelocity)
                        canEndDrift = false;
                    else if (turnInputAbs >= k_NullInput && currentSpeed > maxSpeed * MinSpeedPercentToFinishDrift)
                        canEndDrift = false;

                    // 드리프트 종료 조건이 충족되면 드리프트 종료
                    if (canEndDrift || currentSpeed < k_NullSpeed)
                    {
                        // 입력 없음, 카트가 속도 방향과 일치 => 드리프트 중지
                        IsDrifting = false;
                        m_CurrentGrip = m_FinalStats.Grip;
                    }
                }

                // 현재 그립 및 조향 값에 따라 속도 벡터 회전
                Rigidbody.velocity = Quaternion.AngleAxis(turningPower * Mathf.Sign(localVel.z) * velocitySteering * m_CurrentGrip * Time.fixedDeltaTime, transform.up) * Rigidbody.velocity;
            }
            else
            {
                // 지면에 없으면 공중 상태로 설정
                m_InAir = true;
            }

            // 유효한 위치 확인
            bool validPosition = false;

            // 레이캐스트로 지면 확인
            if (Physics.Raycast(transform.position + (transform.up * 0.1f), -transform.up, out RaycastHit hit, 3.0f, 1 << 9 | 1 << 10 | 1 << 11)) // 레이어: 지면(9) / 환경(10) / 트랙(11)
            {
                // 충돌 노멀과 지면 노멀 중 더 위쪽을 향하는 것을 기준으로 수직 참조 벡터 보간
                Vector3 lerpVector = (m_HasCollision && m_LastCollisionNormal.y > hit.normal.y) ? m_LastCollisionNormal : hit.normal;
                m_VerticalReference = Vector3.Slerp(m_VerticalReference, lerpVector, Mathf.Clamp01(AirborneReorientationCoefficient * Time.fixedDeltaTime * (GroundPercent > 0.0f ? 10.0f : 1.0f)));  // 지면에 있으면 더 빠르게 보간
            }
            else
            {
                // 레이캐스트 히트가 없으면 충돌 노멀 또는 기본 상향 벡터 사용
                Vector3 lerpVector = (m_HasCollision && m_LastCollisionNormal.y > 0.0f) ? m_LastCollisionNormal : Vector3.up;
                m_VerticalReference = Vector3.Slerp(m_VerticalReference, lerpVector, Mathf.Clamp01(AirborneReorientationCoefficient * Time.fixedDeltaTime));
            }

            // 유효한 위치 조건: 충분히 지면에 있고, 충돌 없고, 수직 참조가 거의 수직
            validPosition = GroundPercent > 0.7f && !m_HasCollision && Vector3.Dot(m_VerticalReference, Vector3.up) > 0.9f;

            // 공중 또는 반쯤 지면에 있을 때 처리
            if (GroundPercent < 0.7f)
            {
                // Y축 각속도 유지하면서 나머지는 감쇠
                Rigidbody.angularVelocity = new Vector3(0.0f, Rigidbody.angularVelocity.y * 0.98f, 0.0f);

                // 수직 참조 기준으로 전방 방향 조정
                Vector3 finalOrientationDirection = Vector3.ProjectOnPlane(transform.forward, m_VerticalReference);
                finalOrientationDirection.Normalize();

                // 방향이 유효한 경우 회전 보간
                if (finalOrientationDirection.sqrMagnitude > 0.0f)
                {
                    Rigidbody.MoveRotation(Quaternion.Lerp(Rigidbody.rotation, Quaternion.LookRotation(finalOrientationDirection, m_VerticalReference), Mathf.Clamp01(AirborneReorientationCoefficient * Time.fixedDeltaTime)));
                }
            }
            else if (validPosition)
            {
                // 유효한 위치면 마지막 유효 위치/회전 저장
                m_LastValidPosition = transform.position;
                m_LastValidRotation.eulerAngles = new Vector3(0.0f, transform.rotation.y, 0.0f);
            }

            // 드리프트 VFX 활성화 (드리프트 중이고 지면에 있을 때)
            ActivateDriftVFX(IsDrifting && GroundPercent > 0.0f);
        }
    }
}
