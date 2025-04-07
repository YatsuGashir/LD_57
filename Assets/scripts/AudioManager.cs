using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Звуки")]
    [SerializeField] private List<Sound> sounds;

    [Header("Пул источников")]
    [SerializeField] private GameObject sourcePrefab;
    [SerializeField] private int poolSize = 10;
    //[SerializeField] private AudioMixerGroup mixerGroup;

    private Queue<AudioSource> audioPool = new Queue<AudioSource>();
    private Dictionary<string, Sound> soundDict = new Dictionary<string, Sound>();

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        InitPool();
        InitSounds();
    }

    private void InitPool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(sourcePrefab, transform);
            AudioSource src = obj.GetComponent<AudioSource>();
            //src.outputAudioMixerGroup = mixerGroup;
            src.playOnAwake = false;
            audioPool.Enqueue(src);
        }
    }

    private void InitSounds()
    {
        foreach (Sound s in sounds)
        {
            if (!soundDict.ContainsKey(s.name))
                soundDict.Add(s.name, s);
        }
    }

    public void Play(string soundName)
    {
        PlaySound(soundName, false); // Для 2D звука spatialBlend = 0
    }

    public void Play(string soundName, Vector3 position)
    {
        PlaySound(soundName, true, position); // Для 3D звука spatialBlend = 1, позиция звука
    }

    private void PlaySound(string soundName, bool is3D, Vector3? position = null)
    {
        if (!soundDict.ContainsKey(soundName))
        {
            Debug.LogWarning($"Звук '{soundName}' не найден!");
            return;
        }

        if (audioPool.Count == 0) return;

        Sound s = soundDict[soundName];
        AudioSource src = audioPool.Dequeue();

        src.clip = s.clip;
        src.volume = s.volume;
        src.loop = s.loop;

        // Настроим звук на 2D или 3D
        src.spatialBlend = is3D ? 1f : 0f;

        if (is3D && position.HasValue)
        {
            src.transform.position = position.Value; // Устанавливаем позицию для 3D звука
        }
        if (is3D)
        {
            src.minDistance = 3f; // Минимальное расстояние, на котором звук будет играть с полной громкостью
            src.maxDistance = 15f; // Максимальное расстояние, на котором звук будет слышен
            src.rolloffMode = AudioRolloffMode.Linear; // Тип убывания громкости
        }

        src.Play();

        StartCoroutine(ReturnToPool(src, s.clip.length));
    }
    /*public void Play(string soundName)
    {
        if (!soundDict.ContainsKey(soundName)) {
            Debug.LogWarning($"Звук '{soundName}' не найден!");
            return;
        }

        if (audioPool.Count == 0) return;

        Sound s = soundDict[soundName];
        AudioSource src = audioPool.Dequeue();

        src.clip = s.clip;
        src.volume = s.volume;
        src.loop = s.loop;
        src.Play();

        StartCoroutine(ReturnToPool(src, s.clip.length));
    }*/

    private System.Collections.IEnumerator ReturnToPool(AudioSource src, float delay)
    {
        yield return new WaitForSeconds(delay);
        src.Stop();
        src.clip = null;
        src.loop = false;
        audioPool.Enqueue(src);
    }
}
