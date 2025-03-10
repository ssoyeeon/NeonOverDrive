using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

// 오디오 재생과 관련된 유틸리티 기능을 제공하는 클래스
public class AudioUtility
{
    // AudioManager의 인스턴스 참조를 저장
    static AudioManager m_AudioManager;

    // 게임 내에서 사용되는 오디오 그룹을 정의하는 열거형
    public enum AudioGroups
    {
        Collision,  // 충돌 효과음
        Pickup,     // 아이템 획득 효과음
        HUDVictory, // 승리 UI 효과음
        HUDObjective // 목표 달성 UI 효과음
    }

    // 지정된 위치에 효과음을 생성하고 재생하는 함수
    public static void CreateSFX(AudioClip clip, Vector3 position, AudioGroups audioGroup, float spatialBlend, float rolloffDistanceMin = 1f)
    {
        // 효과음을 재생할 새 게임 오브젝트 생성
        GameObject impactSFXInstance = new GameObject("SFX_" + clip.name);
        impactSFXInstance.transform.position = position;

        // 오디오 소스 컴포넌트 추가 및 설정
        AudioSource source = impactSFXInstance.AddComponent<AudioSource>();
        source.clip = clip;
        source.spatialBlend = spatialBlend;
        source.minDistance = rolloffDistanceMin;
        source.Play();

        // 오디오 그룹 설정
        source.outputAudioMixerGroup = GetAudioGroup(audioGroup);

        // 효과음 재생 후 자동 제거 설정
        m_AudioManager.EnsureSFXDestruction(source);
    }

    // 지정된 오디오 그룹에 대한 AudioMixerGroup을 반환하는 함수
    public static AudioMixerGroup GetAudioGroup(AudioGroups group)
    {
        // AudioManager가 아직 설정되지 않은 경우 찾기
        if (m_AudioManager == null)
            m_AudioManager = GameObject.FindObjectOfType<AudioManager>();

        // 지정된 그룹 이름과 일치하는 오디오 믹서 그룹 찾기
        var groups = m_AudioManager.audioMixer.FindMatchingGroups(group.ToString());
        if (groups.Length > 0)
            return groups[0];

        // 일치하는 그룹을 찾지 못한 경우 경고 로그 출력
        Debug.LogWarning("Didn't find audio group for " + group.ToString());
        return null;
    }

    // 마스터 볼륨을 설정하는 함수
    public static void SetMasterVolume(float value)
    {
        // AudioManager가 아직 설정되지 않은 경우 찾기
        if (m_AudioManager == null)
            m_AudioManager = GameObject.FindObjectOfType<AudioManager>();

        // 0 이하의 값 처리 (로그 스케일에서 -무한대를 방지)
        if (value <= 0)
            value = 0.001f;

        // 선형 볼륨 값을 데시벨로 변환
        float valueInDB = Mathf.Log10(value) * 20;

        // 오디오 믹서에 볼륨 설정
        m_AudioManager.audioMixer.SetFloat("MasterVolume", valueInDB);
    }

    // 현재 설정된 마스터 볼륨을 반환하는 함수
    public static float GetMasterVolume()
    {
        // AudioManager가 아직 설정되지 않은 경우 찾기
        if (m_AudioManager == null)
            m_AudioManager = GameObject.FindObjectOfType<AudioManager>();

        // 오디오 믹서에서 데시벨 볼륨 값 가져오기
        m_AudioManager.audioMixer.GetFloat("MasterVolume", out var valueInDB);

        // 데시벨 값을 선형 스케일로 변환하여 반환
        return Mathf.Pow(10f, valueInDB / 20.0f);
    }
}