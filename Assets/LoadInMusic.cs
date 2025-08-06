using UnityEngine;

public class LoadInMusic : MonoBehaviour
{
    public AudioClip loadInMusic;
        private AudioSource audioSource;
    // Start is
    public void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    //  called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource.clip = loadInMusic;
                audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
