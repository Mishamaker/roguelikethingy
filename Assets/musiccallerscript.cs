using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    void Start()
    {
        // Make sure the MusicManager exists before trying to call it.
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.StartMusic();
        }
        else
        {
            Debug.LogWarning("MusicManager instance not found. Music will not play.");
        }
    }
}