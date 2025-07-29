using UnityEngine;
using UnityEngine.SceneManagement;
public class NewMonoBehaviourScript1 : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
 public void LoadGameScene()
    {
        Debug.Log("Loading Dungeon Scene...");
      
        SceneManager.LoadScene("SampleScene"); 

       
    }

    // You might have other functions here for other buttons (e.g., Quit Game)
    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
        Application.Quit(); 
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Stops play mode in the editor
        #endif
    }
}
