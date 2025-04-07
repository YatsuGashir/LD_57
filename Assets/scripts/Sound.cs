using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;               // имя звука (ключ)
    public AudioClip clip;            // сам звук
    public float volume = 1f;
    public bool loop = false;
}