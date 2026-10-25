using UnityEngine;

[System.Serializable]
public class AudioClipData
{
    public string id;
    public AudioClip clip;

    public float volume = 1f;
    public float minPitch = 0.95f;
    public float maxPitch = 1.05f;
}