using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ApplicationManager : MonoBehaviour {
	

	public void Quit () 
	{
		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#else
		Application.Quit();
		#endif
	}

    public void Play(){
        SceneManager.LoadScene("ProceduralRunner");
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("StartingMenu");
    }
}
