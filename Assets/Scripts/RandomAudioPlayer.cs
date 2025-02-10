using UnityEngine;

public class RandomAudioPlayer : MonoBehaviour
{
    public Vector2 pitchRange = new(0.8f, 1.2f);
    public Vector2 volumeRange = new(0.7f, 1.0f);
    public AudioClip[] clips;
    private AudioSource source;

    void OnEnable()
    {
        if (source == null)
        {
            source = GetComponent<AudioSource>();
        }
        source.clip = clips[Random.Range(0, clips.Length)];
        source.pitch = Random.Range(pitchRange.x, pitchRange.y);
        source.volume = Random.Range(volumeRange.x, volumeRange.y);
        source.loop = false;
        source.Play();
    }
}
