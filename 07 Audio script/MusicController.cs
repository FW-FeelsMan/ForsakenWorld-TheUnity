using System.Collections;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    float targetVolume = 0.1f;
    float fadeInDuration = 5f;
    float fadeOutDuration = 2f;
    public AudioSource[] audioSources;

    void Start()
    {
        audioSources = GetComponents<AudioSource>(); 

        foreach (var audioSource in audioSources)
        {
            audioSource.volume = 0f;
            audioSource.Play();
        }

        StartCoroutine(IncreaseVolume(fadeInDuration, targetVolume));

    }

    public void DecreaseVolumeStart()
    {
        foreach (var audioSource in audioSources)
        {
            StartCoroutine(DecreaseVolume(audioSource, fadeOutDuration, 0f));
        }
    }


    IEnumerator IncreaseVolume(float duration, float targetVolume)
    {
        foreach (var audioSource in audioSources)
        {
            float startVolume = 0f;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                audioSource.volume = Mathf.Lerp(startVolume, targetVolume, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            audioSource.volume = targetVolume;
        }
    }

    IEnumerator DecreaseVolume(AudioSource audioSource, float duration, float targetVolume)
    {
        float startVolume = audioSource.volume;
        float elapsedTime = 0f;
        float startTime = Time.time; 

        while (elapsedTime < duration)
        {
            float normalizedTime = (Time.time - startTime) / duration; 
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, normalizedTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        audioSource.volume = targetVolume;
        audioSource.Stop();
    }
}
