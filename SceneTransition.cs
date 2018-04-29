using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class SceneTransition {

	public string START = "_start";
	public string MAIN_MENU = "MainMenu";
	public string OVERWORLD_SCENE = "overworld";
	public string MEMORY1_SCENE = "Scenes/MemoryScenes/MemoryScene1";
	public string TO_OVERWORLD = "toOverworld";
	public string TO_MEMORY1 = "toMemory";

	public bool CheckScene(string sceneName)
	{
		//Debug.Log (SceneManager.GetActiveScene ().buildIndex);
		if (SceneManager.GetActiveScene ().name == sceneName) 
		{
			return true;
		}
			
		return false;
	}

	public void TransitionScene(string sceneName)
	{
		SceneManager.LoadScene (sceneName);
	}

}
