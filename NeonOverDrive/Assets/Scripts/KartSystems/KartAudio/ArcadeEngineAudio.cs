using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace KartGame.KartSystems
{
    /// <summary>
    /// This class produces audio for various states of the vehicle's movement.
    /// </summary>
    public class ArcadeEngineAudio : MonoBehaviour
    {
        // 카트가 시작할 때 재생되는 오디오 소스
        [Tooltip("카트가 시작할 때 재생할 오디오 클립은 무엇인가요?")]
        public AudioSource StartSound;

        // 카트가 정지해 있을 때 재생되는 오디오 소스
        [Tooltip("카트가 아무것도 하지 않을 때 재생할 오디오 클립은 무엇인가요?")]
        public AudioSource IdleSound;

        // 카트가 움직일 때 재생되는 오디오 소스
        [Tooltip("카트가 움직일 때 재생할 오디오 클립은 무엇인가요?")]
        public AudioSource RunningSound;

        // 카트가 드리프트할 때 재생되는 오디오 소스
        [Tooltip("카트가 드리프트할 때 재생할 오디오 클립은 무엇인가요?")]
        public AudioSource Drift;

        // 최대 속도에서 주행 소리의 최대 볼륨
        [Tooltip("최대 속도에서 주행 소리의 최대 볼륨")]
        [Range(0.1f, 1.0f)] public float RunningSoundMaxVolume = 1.0f;

        // 최대 속도에서 주행 소리의 최대 피치
        [Tooltip("최대 속도에서 주행 소리의 최대 피치")]
        [Range(0.1f, 2.0f)] public float RunningSoundMaxPitch = 1.0f;

        // 카트가 후진할 때 재생되는 오디오 소스
        [Tooltip("카트가 후진할 때 재생할 오디오 클립은 무엇인가요?")]
        public AudioSource ReverseSound;

        // 최대 후진 속도에서 후진 소리의 최대 볼륨
        [Tooltip("최대 후진 속도에서 후진 소리의 최대 볼륨")]
        [Range(0.1f, 1.0f)] public float ReverseSoundMaxVolume = 0.5f;

        // 최대 후진 속도에서 후진 소리의 최대 피치
        [Tooltip("최대 후진 속도에서 후진 소리의 최대 피치")]
        [Range(0.1f, 2.0f)] public float ReverseSoundMaxPitch = 0.6f;

        // 카트 컨트롤러 참조
        ArcadeKart arcadeKart;

        // 초기화 시 부모 오브젝트에서 ArcadeKart 컴포넌트 찾기
        void Awake()
        {
            arcadeKart = GetComponentInParent<ArcadeKart>();
        }

        // 매 프레임마다 카트 상태에 따라 오디오 업데이트
        void Update()
        {
            // 카트 속도 초기화
            float kartSpeed = 0.0f;

            // 카트 컨트롤러가 유효하면 속도 및 드리프트 상태 가져오기
            if (arcadeKart != null)
            {
                kartSpeed = arcadeKart.LocalSpeed();
                Drift.volume = arcadeKart.IsDrifting && arcadeKart.GroundPercent > 0.0f ? arcadeKart.Rigidbody.velocity.magnitude / arcadeKart.GetMaxSpeed() : 0.0f;
            }

            // 정지 시 소리 볼륨 설정
            IdleSound.volume = Mathf.Lerp(0.6f, 0.0f, kartSpeed * 4);

            // 후진 중인 경우
            if (kartSpeed < 0.0f)
            {
                // 전진 소리 끄고 후진 소리 활성화
                RunningSound.volume = 0.0f;
                ReverseSound.volume = Mathf.Lerp(0.1f, ReverseSoundMaxVolume, -kartSpeed * 1.2f);
                ReverseSound.pitch = Mathf.Lerp(0.1f, ReverseSoundMaxPitch, -kartSpeed + (Mathf.Sin(Time.time) * .1f));
            }
            else
            {
                // 전진 중인 경우
                // 후진 소리 끄고 전진 소리 활성화
                ReverseSound.volume = 0.0f;
                RunningSound.volume = Mathf.Lerp(0.1f, RunningSoundMaxVolume, kartSpeed * 1.2f);
                RunningSound.pitch = Mathf.Lerp(0.3f, RunningSoundMaxPitch, kartSpeed + (Mathf.Sin(Time.time) * .1f));
            }
        }
    }
}