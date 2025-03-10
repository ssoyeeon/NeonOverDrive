using UnityEngine;

namespace KartGame.KartSystems
{
    public partial class EngineAudio : MonoBehaviour
    {
        // 아케이드 카트 컴포넌트 참조
        public ArcadeKart arcadeKart;

        // 엔진 RPM 값 (0~1 사이)
        [Range(0, 1)]
        public float RPM;
        [Space]
        // 엔진의 최소 가능 RPM
        [Tooltip("엔진의 최소 가능 RPM")]
        public float minRPM = 900;
        // 엔진의 최대 가능 RPM
        [Tooltip("엔진의 최대 가능 RPM")]
        public float maxRPM = 10000;
        [Space]
        // 엔진 오디오에 랜덤성 추가
        [Tooltip("엔진 오디오의 랜덤성을 증가시킵니다")]
        public float lumpyCamFactor = 0.05f;
        [Space]
        // 최소 RPM일 때의 볼륨
        [Tooltip("최소 RPM일 때의 볼륨")]
        public float minVolume = 0.2f;
        // 최대 RPM일 때의 볼륨
        [Tooltip("최대 RPM일 때의 볼륨")]
        public float maxVolume = 1.2f;
        [Space]
        // 새로운 스트로크가 시작될 때 파형의 스무딩
        [Tooltip("새로운 스트로크가 시작될 때 파형의 스무딩")]
        public float strokeDamping = 0.1f;
        [Space]
        // 각 엔진 스트로크에 대한 오디오 구성
        [Tooltip("각 엔진 스트로크에 대한 오디오 구성")]
        public Stroke intake, compression, combustion, exhaust;
        // RPM을 RPM^3으로 맵핑
        [Tooltip("RPM을 RPM^3으로 맵핑")]
        public bool usePow = false;

        // 다음 스트로크 시간
        float m_NextStrokeTime;
        // 현재 시간
        float m_Time;
        // 샘플당 초 단위 시간
        float m_SecondsPerSample;
        // 현재 스트로크 인덱스
        int m_Stroke;
        // 랜덤 값 버퍼
        float[] m_RandomBuffer;
        // RPM 변화량
        float m_DeltaRPM;
        // 이전 프레임의 RPM
        float m_LastRPM;
        // 이전 오디오 샘플 값 (좌/우)
        float m_LastSampleL, m_LastSampleR;
        // 스무딩을 위한 댐퍼 값
        float m_Damper = 1f;
        // 오디오 볼륨
        float m_Volume = 1;

        // 초기화
        void Awake()
        {
            // 랜덤 값 버퍼 초기화
            m_RandomBuffer = new float[97];
            for (var i = 0; i < m_RandomBuffer.Length; i++)
                m_RandomBuffer[i] = Random.Range(-1, 1);

            // 각 스트로크 초기화
            intake.Init();
            compression.Init();
            combustion.Init();
            exhaust.Init();

            // 변수 초기화
            m_Stroke = 0;
            m_Time = 0;
            m_SecondsPerSample = 1f / AudioSettings.outputSampleRate;
        }

        // 매 프레임 업데이트
        void Update()
        {
            // 카트 속도에 따라 RPM 설정
            RPM = arcadeKart != null ? Mathf.Abs(arcadeKart.LocalSpeed()) : 0;
            m_DeltaRPM = RPM - m_LastRPM;

            // m_LastRPM의 움직임을 부드럽게 함
            m_LastRPM = Mathf.Lerp(m_LastRPM, RPM, Time.deltaTime * 100);

            // 타임스케일에 따라 볼륨 조정
            if (Time.timeScale < 1)
                m_Volume = 0;
            else
                m_Volume = 1;
        }

        // 오디오 필터 처리 (오디오 시스템이 호출)
        void OnAudioFilterRead(float[] data, int channels)
        {
            // 스테레오 채널만 처리
            if (channels != 2)
                return;

            // RPM 값 계산 (usePow가 true면 큐빅 스케일링)
            var r = usePow ? m_LastRPM * m_LastRPM * m_LastRPM : m_LastRPM;
            var gain = Mathf.Lerp(minVolume, maxVolume, r);

            // 4 스트로크당 한 번의 회전 (4 스트로크 엔진)
            var strokeDuration = 1f / ((Mathf.Lerp(minRPM, maxRPM, r) / 60f) * 2);

            // 오디오 데이터 처리
            for (var i = 0; i < data.Length; i += 2)
            {
                m_Time += m_SecondsPerSample;

                // "lumpy cam" 효과를 위한 작은 랜덤 값
                var rnd = m_RandomBuffer[i % 97] * lumpyCamFactor;

                // 다음 스트로크 시간이 되었는지 확인
                if (m_Time >= m_NextStrokeTime)
                {
                    // 현재 스트로크에 따라 리셋
                    switch (m_Stroke)
                    {
                        case 0:
                            intake.Reset();
                            break;
                        case 1:
                            compression.Reset();
                            break;
                        case 2:
                            combustion.Reset();
                            break;
                        case 3:
                            exhaust.Reset();
                            break;
                    }

                    // 스트로크 카운터 증가
                    m_Stroke++;
                    if (m_Stroke >= 4) m_Stroke = 0;

                    // RPM이 감소할 때 lumpy cam 요소를 적용한 다음 스트로크 시간
                    m_NextStrokeTime += strokeDuration + (strokeDuration * rnd * (m_DeltaRPM < 0 ? 1 : 0));

                    // 스트로크마다 댐핑 리셋 (클릭음 제거 및 스트로크 간 전환 개선)
                    m_Damper = 0;
                }

                // 좌우 채널 샘플 초기화
                var sampleL = 0f;
                var sampleR = 0f;

                // 4기통 엔진에서는 모든 스트로크가 동시에 재생됨
                switch (m_Stroke)
                {
                    case 0:
                        sampleL += intake.Sample() * rnd;
                        sampleR += compression.Sample();
                        sampleL += combustion.Sample();
                        sampleR += exhaust.Sample();
                        break;
                    case 1:
                        sampleR += intake.Sample();
                        sampleL += compression.Sample() * rnd;
                        sampleR += combustion.Sample();
                        sampleL += exhaust.Sample();
                        break;
                    case 2:
                        sampleR += intake.Sample();
                        sampleR += compression.Sample();
                        sampleL += combustion.Sample() * rnd;
                        sampleL += exhaust.Sample();
                        break;
                    case 3:
                        sampleL += intake.Sample();
                        sampleL += compression.Sample();
                        sampleR += combustion.Sample();
                        sampleR += exhaust.Sample() * rnd;
                        break;
                }

                // 댐퍼 값 증가
                m_Damper += strokeDamping;
                if (m_Damper > 1) m_Damper = 1;

                // 스트로크 간 샘플 스무딩
                sampleL = m_LastSampleL + (sampleL - m_LastSampleL) * m_Damper;
                sampleR = m_LastSampleR + (sampleR - m_LastSampleR) * m_Damper;
                sampleL = Mathf.Clamp(sampleL * gain, -1, 1);
                sampleR = Mathf.Clamp(sampleR * gain, -1, 1);

                // 오디오 채널 데이터에 샘플 추가 (약간의 크로스 피드 포함)
                data[i + 0] += sampleL + (sampleR * 0.75f);
                data[i + 0] *= m_Volume;
                data[i + 1] += sampleR + (sampleL * 0.75f);
                data[i + 1] *= m_Volume;

                // 이전 샘플 값 저장
                m_LastSampleL = sampleL;
                m_LastSampleR = sampleR;
            }
        }
    }
}