using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class GameManager : MonoBehaviour {

	private static GameManager instance;

	public static GameManager Instance
	{
		get 
		{
			if (instance == null) 
			{
				GameObject go = new GameObject ("GameManager");
				go.AddComponent<GameManager>();
			}

			return instance;
		}
	}

	public Transform mainMenu;
	public Transform optionsMenu;
	public Transform pauseMenu;
	public Transform optionsPauseMenu;
	public Transform canvas;
	public Toggle toggle;
	public Toggle optionsToggle;
	public Vector3 spawn { get; set; }
	public float inverted { get; set; }
	public bool start { get; set; }
	public bool paused { get; set; }
	public string currentScene { get; set; }
	private bool cursorLock;

	SceneTransition transition = new SceneTransition();
	private AudioSource audio;

	void Awake()
	{
		instance = this;
		paused = false;
		start = true;
		cursorLock = false;
		spawn = new Vector3 (163.6463f, 1f, 332.86f);
		audio = GetComponent<AudioSource> ();
		DontDestroyOnLoad (gameObject);

		if (transition.CheckScene (transition.START)) 
		{
			transition.TransitionScene (transition.MAIN_MENU);
		}
	}

	public void Update()
	{
		if (transition.CheckScene (transition.MAIN_MENU)) {
			canvas.gameObject.SetActive (true);
		} 
		else 
		{
			canvas.gameObject.SetActive (false);
		}
			
	}

	public void PlayGame()
	{
		ToggleInvert ();
		transition.TransitionScene (transition.OVERWORLD_SCENE);
		audio.Play ();
		cursorLock = true;

		if (toggle.isOn) {
			optionsToggle.isOn = true;
			toggle.isOn = false;
		} 
		else 
		{
			optionsToggle.isOn = false;
		}

	}

	public void PauseGame()
	{
		if (pauseMenu.gameObject.activeInHierarchy == false) 
		{
			pauseMenu.gameObject.SetActive (true);
			Time.timeScale = 0;
			paused = true;
			cursorLock = false;
		} 
		else 
		{
			pauseMenu.gameObject.SetActive (false);
			Time.timeScale = 1;
			paused = false;
			cursorLock = true;
		}
	}

	public void QuitGame()
	{
		Application.Quit ();
	}

	public void OptionsMenu(bool clicked)
	{
		if (clicked) 
		{
			optionsMenu.gameObject.SetActive (clicked);
			mainMenu.gameObject.SetActive (false);
		} 
		else 
		{
			optionsMenu.gameObject.SetActive (clicked);
			mainMenu.gameObject.SetActive (true);
		}
	}

	public void OptionsPauseMenu(bool clicked)
	{
		if (clicked) 
		{
			optionsPauseMenu.gameObject.SetActive (clicked);
			pauseMenu.gameObject.SetActive (false);
		} 
		else 
		{
			optionsPauseMenu.gameObject.SetActive (clicked);
			pauseMenu.gameObject.SetActive (true);
		}
	}

	public void ToMainMenu()
	{
		audio.Stop ();
		transition.TransitionScene (transition.MAIN_MENU);
		PauseGame ();
		cursorLock = false;

		if (optionsToggle.isOn) {
			toggle.isOn = true;
		} 
		else 
		{
			toggle.isOn = false;
		}
	}

	public void ToggleInvert()
	{
		if (toggle.isOn || optionsToggle.isOn) 
		{
			inverted = -1.0f;
		} 
		else 
		{
			inverted = 1.0f;
		}
			
	}

	public void CursorLock()
	{
		if (cursorLock)
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
		else
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
	}
}
