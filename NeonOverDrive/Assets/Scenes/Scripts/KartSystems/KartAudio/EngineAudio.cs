using UnityEngine;

namespace KartGame.KartSystems
{
    public partial class EngineAudio : MonoBehaviour
    {
        // �����̵� īƮ ������Ʈ ����
        public ArcadeKart arcadeKart;

        // ���� RPM �� (0~1 ����)
        [Range(0, 1)]
        public float RPM;
        [Space]
        // ������ �ּ� ���� RPM
        [Tooltip("������ �ּ� ���� RPM")]
        public float minRPM = 900;
        // ������ �ִ� ���� RPM
        [Tooltip("������ �ִ� ���� RPM")]
        public float maxRPM = 10000;
        [Space]
        // ���� ������� ������ �߰�
        [Tooltip("���� ������� �������� ������ŵ�ϴ�")]
        public float lumpyCamFactor = 0.05f;
        [Space]
        // �ּ� RPM�� ���� ����
        [Tooltip("�ּ� RPM�� ���� ����")]
        public float minVolume = 0.2f;
        // �ִ� RPM�� ���� ����
        [Tooltip("�ִ� RPM�� ���� ����")]
        public float maxVolume = 1.2f;
        [Space]
        // ���ο� ��Ʈ��ũ�� ���۵� �� ������ ������
        [Tooltip("���ο� ��Ʈ��ũ�� ���۵� �� ������ ������")]
        public float strokeDamping = 0.1f;
        [Space]
        // �� ���� ��Ʈ��ũ�� ���� ����� ����
        [Tooltip("�� ���� ��Ʈ��ũ�� ���� ����� ����")]
        public Stroke intake, compression, combustion, exhaust;
        // RPM�� RPM^3���� ����
        [Tooltip("RPM�� RPM^3���� ����")]
        public bool usePow = false;

        // ���� ��Ʈ��ũ �ð�
        float m_NextStrokeTime;
        // ���� �ð�
        float m_Time;
        // ���ô� �� ���� �ð�
        float m_SecondsPerSample;
        // ���� ��Ʈ��ũ �ε���
        int m_Stroke;
        // ���� �� ����
        float[] m_RandomBuffer;
        // RPM ��ȭ��
        float m_DeltaRPM;
        // ���� �������� RPM
        float m_LastRPM;
        // ���� ����� ���� �� (��/��)
        float m_LastSampleL, m_LastSampleR;
        // �������� ���� ���� ��
        float m_Damper = 1f;
        // ����� ����
        float m_Volume = 1;

        // �ʱ�ȭ
        void Awake()
        {
            // ���� �� ���� �ʱ�ȭ
            m_RandomBuffer = new float[97];
            for (var i = 0; i < m_RandomBuffer.Length; i++)
                m_RandomBuffer[i] = Random.Range(-1, 1);

            // �� ��Ʈ��ũ �ʱ�ȭ
            intake.Init();
            compression.Init();
            combustion.Init();
            exhaust.Init();

            // ���� �ʱ�ȭ
            m_Stroke = 0;
            m_Time = 0;
            m_SecondsPerSample = 1f / AudioSettings.outputSampleRate;
        }

        // �� ������ ������Ʈ
        void Update()
        {
            // īƮ �ӵ��� ���� RPM ����
            RPM = arcadeKart != null ? Mathf.Abs(arcadeKart.LocalSpeed()) : 0;
            m_DeltaRPM = RPM - m_LastRPM;

            // m_LastRPM�� �������� �ε巴�� ��
            m_LastRPM = Mathf.Lerp(m_LastRPM, RPM, Time.deltaTime * 100);

            // Ÿ�ӽ����Ͽ� ���� ���� ����
            if (Time.timeScale < 1)
                m_Volume = 0;
            else
                m_Volume = 1;
        }

        // ����� ���� ó�� (����� �ý����� ȣ��)
        void OnAudioFilterRead(float[] data, int channels)
        {
            // ���׷��� ä�θ� ó��
            if (channels != 2)
                return;

            // RPM �� ��� (usePow�� true�� ť�� �����ϸ�)
            var r = usePow ? m_LastRPM * m_LastRPM * m_LastRPM : m_LastRPM;
            var gain = Mathf.Lerp(minVolume, maxVolume, r);

            // 4 ��Ʈ��ũ�� �� ���� ȸ�� (4 ��Ʈ��ũ ����)
            var strokeDuration = 1f / ((Mathf.Lerp(minRPM, maxRPM, r) / 60f) * 2);

            // ����� ������ ó��
            for (var i = 0; i < data.Length; i += 2)
            {
                m_Time += m_SecondsPerSample;

                // "lumpy cam" ȿ���� ���� ���� ���� ��
                var rnd = m_RandomBuffer[i % 97] * lumpyCamFactor;

                // ���� ��Ʈ��ũ �ð��� �Ǿ����� Ȯ��
                if (m_Time >= m_NextStrokeTime)
                {
                    // ���� ��Ʈ��ũ�� ���� ����
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

                    // ��Ʈ��ũ ī���� ����
                    m_Stroke++;
                    if (m_Stroke >= 4) m_Stroke = 0;

                    // RPM�� ������ �� lumpy cam ��Ҹ� ������ ���� ��Ʈ��ũ �ð�
                    m_NextStrokeTime += strokeDuration + (strokeDuration * rnd * (m_DeltaRPM < 0 ? 1 : 0));

                    // ��Ʈ��ũ���� ���� ���� (Ŭ���� ���� �� ��Ʈ��ũ �� ��ȯ ����)
                    m_Damper = 0;
                }

                // �¿� ä�� ���� �ʱ�ȭ
                var sampleL = 0f;
                var sampleR = 0f;

                // 4���� ���������� ��� ��Ʈ��ũ�� ���ÿ� �����
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

                // ���� �� ����
                m_Damper += strokeDamping;
                if (m_Damper > 1) m_Damper = 1;

                // ��Ʈ��ũ �� ���� ������
                sampleL = m_LastSampleL + (sampleL - m_LastSampleL) * m_Damper;
                sampleR = m_LastSampleR + (sampleR - m_LastSampleR) * m_Damper;
                sampleL = Mathf.Clamp(sampleL * gain, -1, 1);
                sampleR = Mathf.Clamp(sampleR * gain, -1, 1);

                // ����� ä�� �����Ϳ� ���� �߰� (�ణ�� ũ�ν� �ǵ� ����)
                data[i + 0] += sampleL + (sampleR * 0.75f);
                data[i + 0] *= m_Volume;
                data[i + 1] += sampleR + (sampleL * 0.75f);
                data[i + 1] *= m_Volume;

                // ���� ���� �� ����
                m_LastSampleL = sampleL;
                m_LastSampleR = sampleR;
            }
        }
    }
}