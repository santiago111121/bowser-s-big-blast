using UnityEngine;

public class AudioCrossfade : MonoBehaviour
{
    [Header("Clips de Audio")]
    public AudioClip audio1;
    public AudioClip audio2;

    [Header("Configuración de Crossfade")]
    public float fadeDuration = 2f; // Duración del crossfade inicial

    [Header("Controladores de Volumen")]
    [Range(0f, 1f)] public float volumenAudio1 = 1f;
    [Range(0f, 1f)] public float volumenAudio2 = 1f;

    private AudioSource audioSource1;
    private AudioSource audioSource2;

    void Start()
    {
        audioSource1 = gameObject.AddComponent<AudioSource>();
        audioSource2 = gameObject.AddComponent<AudioSource>();

        audioSource1.clip = audio1;
        audioSource2.clip = audio2;

        audioSource1.loop = false;
        audioSource2.loop = true; // Audio 2 en loop continuo

        audioSource1.volume = volumenAudio1;
        audioSource2.volume = 0f; // Iniciar el segundo audio en silencio

        audioSource1.Play();
        Invoke(nameof(StartCrossfade), audio1.length - fadeDuration);
    }

    void Update()
    {
        // Permite ajustar los volúmenes en tiempo real desde el Inspector
        if (audioSource1.isPlaying) audioSource1.volume = volumenAudio1;
        if (audioSource2.isPlaying) audioSource2.volume = volumenAudio2;
    }

    void StartCrossfade()
    {
        StartCoroutine(CrossfadeToSecondAudio());
    }

    System.Collections.IEnumerator CrossfadeToSecondAudio()
    {
        float timer = 0f;
        audioSource2.Play();

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / fadeDuration;
            audioSource2.volume = volumenAudio2 * progress;
            audioSource1.volume = volumenAudio1 * (1 - progress);
            yield return null;
        }

        audioSource1.Stop();
        audioSource2.volume = volumenAudio2;
    }
}
