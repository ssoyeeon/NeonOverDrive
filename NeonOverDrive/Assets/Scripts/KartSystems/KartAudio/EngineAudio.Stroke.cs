using UnityEngine;
namespace KartGame.KartSystems
{
    public partial class EngineAudio
    {
        
        [System.Serializable]
        public struct Stroke
        {
            // 스트로크에 사용될 오디오 클립
            public AudioClip clip;

            // 스트로크의 볼륨 게인 (0~1 사이)
            [Range(0, 1)]
            public float gain;

            // 오디오 샘플 데이터를 저장하는 내부 버퍼
            internal float[] buffer;

            // 현재 버퍼 재생 위치
            internal int position;

            // 버퍼 재생 위치를
            internal void Reset() => position = 0;

            // 현재 위치의 오디오 샘플을 반환하고 위치 증가
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

            // 스트로크 초기화 및 오디오 데이터 로드
            internal void Init()
            {
                // 클립이 없는 경우 노이즈가 있는 사인파를 생성하여 대체
                // 있는 경우 클립의 샘플 데이터로 버퍼 초기화
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