using UnityEngine;

public class AudioOneShot : MonoBehaviour
{
    AudioSource source;
    public AudioClip clip;
    // Use this for initialization
    void Start()
    {
        source = GetComponent<AudioSource>();
        if (source == null)
            source = gameObject.AddComponent<AudioSource>();
    }

    // Update is called once per frame
    public void Play()
    {
        source.PlayOneShot(clip);
    }
}
