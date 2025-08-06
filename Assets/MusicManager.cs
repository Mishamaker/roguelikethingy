using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [Header("Audio Clips")]
    public AudioClip loadInSound;
    public AudioClip battleMusic;
    public AudioClip[] ambientMusicTracks;

    private AudioSource audioSource;
    private bool isInBattle = false;
    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            return;
        }
    }
    
    public void StartMusic()
    {
        if (loadInSound != null)
        {
            audioSource.PlayOneShot(loadInSound);
            Invoke("StartAmbientMusicLoopCoroutine", loadInSound.length);
        }
        else
        {
            StartAmbientMusicLoopCoroutine();
        }
    }
    
    private void StartAmbientMusicLoopCoroutine()
    {
        StartCoroutine(PlayAmbientMusicLoop());
    }

    IEnumerator PlayAmbientMusicLoop()
    {
        while (true)
        {
            if (!isInBattle)
            {
                if (ambientMusicTracks.Length > 0)
                {
                    int randomIndex = UnityEngine.Random.Range(0, ambientMusicTracks.Length);
                    audioSource.clip = ambientMusicTracks[randomIndex];
                    audioSource.Play();
                    yield return new WaitForSeconds(audioSource.clip.length);
                }
                else
                {
                    yield break;
                }
            }
            else
            {
                yield return new WaitForSeconds(1f);
            }
        }
    }

    public void SetBattleMusic(bool battleState)
    {
        if (isInBattle == battleState)
        {
            return;
        }

        isInBattle = battleState;

        audioSource.Stop();
        StopAllCoroutines(); 
        

        if (isInBattle)
        {
            if (battleMusic != null)
            {
                audioSource.clip = battleMusic;
                audioSource.loop = true;
                audioSource.Play();
            }
        }
        else
        {
            StartCoroutine(PlayAmbientMusicLoop());
        }
    }
}