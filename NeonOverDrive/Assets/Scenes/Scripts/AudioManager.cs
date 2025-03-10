using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

// 게임 내 오디오를 관리하는 클래스
public class AudioManager : MonoBehaviour
{
    // 오디오 믹서 참조
    public AudioMixer audioMixer;

    // 효과음이 재생 완료 후 자동으로 제거되도록 보장하는 함수
    public void EnsureSFXDestruction(AudioSource source)
    {
        StartCoroutine("DelayedSFXDestruction", source);
    }

    // 효과음 재생이 완료될 때까지 대기한 후 게임 오브젝트를 제거하는 코루틴
    private IEnumerator DelayedSFXDestruction(AudioSource source)
    {
        // 오디오 소스의 재생이 끝날 때까지 대기
        while (source.isPlaying)
        {
            yield return null;
        }

        // 효과음 재생이 완료되면 게임 오브젝트 제거
        GameObject.Destroy(source.gameObject);
    }
}