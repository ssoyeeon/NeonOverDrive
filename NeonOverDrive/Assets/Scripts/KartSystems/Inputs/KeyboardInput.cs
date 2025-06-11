using UnityEngine;

namespace KartGame.KartSystems
{
    public class KeyboardInput : BaseInput
    {
        [Header("키보드 설정")]
        public KeyCode accelerateKey = KeyCode.W;        // 가속 (W)
        public KeyCode accelerateAltKey = KeyCode.UpArrow; // 가속 대체키 (↑)
        public KeyCode brakeKey = KeyCode.S;             // 브레이크 (S)
        public KeyCode brakeAltKey = KeyCode.DownArrow;  // 브레이크 대체키 (↓)
        public KeyCode leftKey = KeyCode.A;              // 좌회전 (A)
        public KeyCode leftAltKey = KeyCode.LeftArrow;   // 좌회전 대체키 (←)
        public KeyCode rightKey = KeyCode.D;             // 우회전 (D)
        public KeyCode rightAltKey = KeyCode.RightArrow; // 우회전 대체키 (→)
        public KeyCode driftKey = KeyCode.LeftShift;     // 드리프트
        public KeyCode driftAltKey = KeyCode.RightShift; // 드리프트 대체키
        public KeyCode boostKey = KeyCode.Space;         // 부스트

        [Header("드리프트 및 부스트 설정")]
        public float boostDuration = 2f;                 // 부스트 지속 시간
        public float boostForce = 2000f;                 // 부스트 힘
        public float driftSteerMultiplier = 1.5f;        // 드리프트 시 조향 배율

        private ArcadeKart kart;
        private bool isBoosting = false;
        private float currentBoostTime = 0f;
        private bool isDrifting = false;
        private Rigidbody rb;

        void Start()
        {
            kart = GetComponent<ArcadeKart>();
            rb = GetComponent<Rigidbody>();
        }

        void Update()
        {
            HandleBoost();
            HandleDrift();
        }

        void HandleBoost()
        {
            if ((Input.GetKeyDown(boostKey)) && !isBoosting)
            {
                isBoosting = true;
                currentBoostTime = 0f;

                var boostPowerup = new ArcadeKart.StatPowerup
                {
                    PowerUpID = "Boost",
                    MaxTime = boostDuration,
                    modifiers = new ArcadeKart.Stats
                    {
                        TopSpeed = 40f,
                        Acceleration = 150f,
                        Grip = 1f
                    }
                };

                kart.AddPowerup(boostPowerup);
                // 부스트 시 순간적인 가속도 부여
                rb.AddForce(transform.forward * boostForce);
            }

            if (isBoosting)
            {
                currentBoostTime += Time.deltaTime;
                if (currentBoostTime >= boostDuration)
                {
                    isBoosting = false;
                }
            }
        }

        void HandleDrift()
        {
            // 속도가 낮거나 드리프트 키를 놓으면 드리프트 종료
            if ((isDrifting && rb.velocity.magnitude < 5f) ||
                (isDrifting && !Input.GetKey(driftKey) && !Input.GetKey(driftAltKey)) ||
                (isDrifting && kart.GroundPercent < 0.5f))
            {
                isDrifting = false;
                return;
            }

            if (Input.GetKeyDown(driftKey) && !isDrifting
                && Vector3.Dot(rb.velocity, transform.forward) > 0
                && rb.velocity.magnitude > 14f
                && kart.GroundPercent > 0.5f)
            {
                isDrifting = true;
                var driftPowerup = new ArcadeKart.StatPowerup
                {
                    PowerUpID = "Drift",
                    MaxTime = 0.5f,
                    modifiers = new ArcadeKart.Stats
                    {
                        Grip = 0.1f,           // 그립력 증가
                        Steer = 0.5f,          // 조향 감소
                        TopSpeed = -5f,        // 드리프트 시 속도 약간 감소
                        Acceleration = -2f      // 가속도 약간 감소
                    }
                };
                kart.AddPowerup(driftPowerup);
            }

            if (isDrifting)
            {
                // 드리프트 중에는 현재 방향을 유지하면서 천천히 회전
                float turnForce = GetTurnInput();
                rb.MoveRotation(rb.rotation * Quaternion.Euler(0, turnForce * 2.0f, 0));
            }

            if (isDrifting && (Input.GetKeyUp(driftKey) || Input.GetKeyUp(driftAltKey)))
            {
                isDrifting = false;
                // 드리프트 종료 시 현재 속도 방향으로 가속
                rb.AddForce(rb.velocity.normalized * 1000f);
            }
        }

        // 좌우 키 입력을 -1 ~ 1 사이 값으로 변환
        private float GetTurnInput()
        {
            float turnInput = 0f;
            if (Input.GetKey(leftKey) || Input.GetKey(leftAltKey)) turnInput -= 1f;
            if (Input.GetKey(rightKey) || Input.GetKey(rightAltKey)) turnInput += 1f;
            return turnInput;
        }

        public override InputData GenerateInput()
        {
            return new InputData
            {
                Accelerate = Input.GetKey(accelerateKey) || Input.GetKey(accelerateAltKey),
                Brake = Input.GetKey(brakeKey) || Input.GetKey(brakeAltKey) || Input.GetKey(driftKey) || Input.GetKey(driftAltKey),
                TurnInput = GetTurnInput() * (isDrifting ? driftSteerMultiplier : 1f)
            };
        }
    }
}