using System;
using UnityEngine;
namespace KartGame.KartSystems
{
    [DefaultExecutionOrder(100)]
    public class KartAnimation : MonoBehaviour
    {
        [Serializable]
        public class Wheel
        {
            [Tooltip("휠의 트랜스폼에 대한 참조입니다.")]
            public Transform wheelTransform;
            [Tooltip("휠의 WheelCollider에 대한 참조입니다.")]
            public WheelCollider wheelCollider;

            Quaternion m_SteerlessLocalRotation;
            public void Setup() => m_SteerlessLocalRotation = wheelTransform.localRotation;
            public void StoreDefaultRotation() => m_SteerlessLocalRotation = wheelTransform.localRotation; // 기본 회전값을 저장합니다.
            public void SetToDefaultRotation() => wheelTransform.localRotation = m_SteerlessLocalRotation; // 기본 회전값으로 설정합니다.
        }
        [Tooltip("어떤 카트를 참조할 것인지 설정합니다.")]
        public ArcadeKart kartController;
        [Space]
        [Tooltip("입력에 따른 조향 애니메이션의 감쇠 값입니다. 숫자가 클수록 감쇠가 적어집니다.")]
        public float steeringAnimationDamping = 10f;
        [Space]
        [Tooltip("조향 입력이 1 또는 -1일 때 앞바퀴가 기본 위치에서 회전할 수 있는 최대 각도(도)입니다.")]
        public float maxSteeringAngle;
        [Tooltip("카트의 왼쪽 앞바퀴에 대한 정보입니다.")]
        public Wheel frontLeftWheel;
        [Tooltip("카트의 오른쪽 앞바퀴에 대한 정보입니다.")]
        public Wheel frontRightWheel;
        [Tooltip("카트의 왼쪽 뒷바퀴에 대한 정보입니다.")]
        public Wheel rearLeftWheel;
        [Tooltip("카트의 오른쪽 뒷바퀴에 대한 정보입니다.")]
        public Wheel rearRightWheel;
        float m_SmoothedSteeringInput; // 부드럽게 처리된 조향 입력값
        void Start()
        {
            // 모든 바퀴를 초기화합니다.
            frontLeftWheel.Setup();
            frontRightWheel.Setup();
            rearLeftWheel.Setup();
            rearRightWheel.Setup();
        }
        void FixedUpdate()
        {
            // 현재 조향 입력값을 부드럽게 보간합니다.
            m_SmoothedSteeringInput = Mathf.MoveTowards(m_SmoothedSteeringInput, kartController.Input.TurnInput,
                steeringAnimationDamping * Time.deltaTime);
            // 앞바퀴 조향 처리
            float rotationAngle = m_SmoothedSteeringInput * maxSteeringAngle;
            frontLeftWheel.wheelCollider.steerAngle = rotationAngle;
            frontRightWheel.wheelCollider.steerAngle = rotationAngle;
            // WheelCollider에서 위치와 회전 업데이트
            UpdateWheelFromCollider(frontLeftWheel);
            UpdateWheelFromCollider(frontRightWheel);
            UpdateWheelFromCollider(rearLeftWheel);
            UpdateWheelFromCollider(rearRightWheel);
        }
        void LateUpdate()
        {
            // WheelCollider에서 위치와 회전 업데이트 (애니메이션을 위해 한번 더 실행)
            UpdateWheelFromCollider(frontLeftWheel);
            UpdateWheelFromCollider(frontRightWheel);
            UpdateWheelFromCollider(rearLeftWheel);
            UpdateWheelFromCollider(rearRightWheel);
        }
        void UpdateWheelFromCollider(Wheel wheel)
        {
            // 휠 콜라이더에서 월드 위치와 회전을 가져와 실제 휠 모델에 적용합니다.
            wheel.wheelCollider.GetWorldPose(out Vector3 position, out Quaternion rotation);
            wheel.wheelTransform.position = position;
            wheel.wheelTransform.rotation = rotation;
        }
    }
}