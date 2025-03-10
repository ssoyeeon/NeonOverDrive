using UnityEngine;
namespace KartGame.KartSystems
{
    public partial class EngineAudio
    {
        
        [System.Serializable]
        public struct Stroke
        {
            // ��Ʈ��ũ�� ���� ����� Ŭ��
            public AudioClip clip;

            // ��Ʈ��ũ�� ���� ���� (0~1 ����)
            [Range(0, 1)]
            public float gain;

            // ����� ���� �����͸� �����ϴ� ���� ����
            internal float[] buffer;

            // ���� ���� ��� ��ġ
            internal int position;

            // ���� ��� ��ġ��
            internal void Reset() => position = 0;

            // ���� ��ġ�� ����� ������ ��ȯ�ϰ� ��ġ ����
            internal float Sample()
            {
                if (position < buffer.Length)
                {
                    var s = buffer[position];
                    position++;
                    return s * gain;
                }
                return 0;
            }

            // ��Ʈ��ũ �ʱ�ȭ �� ����� ������ �ε�
            internal void Init()
            {
                // Ŭ���� ���� ��� ����� �ִ� �����ĸ� �����Ͽ� ��ü
                // �ִ� ��� Ŭ���� ���� �����ͷ� ���� �ʱ�ȭ
                if (clip == null)
                {
                    buffer = new float[4096];
                    for (var i = 0; i < buffer.Length; i++)
                        buffer[i] = Mathf.Sin(i * (1f / 44100) * 440) + Random.Range(-1, 1) * 0.05f;
                }
                else
                {
                    buffer = new float[clip.samples];
                    clip.GetData(buffer, 0);
                }
            }
        }
    }
}