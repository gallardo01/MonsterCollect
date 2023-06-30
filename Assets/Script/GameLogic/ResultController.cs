using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class ResultController : MonoBehaviour
{
	public Button btnNewGame;
	public Button btnMainMenu;

	public Canvas result;

	public Canvas mainMenu;
	public GameObject canvas;
	public GameObject player;
	public GameObject controller;

	public TextMeshProUGUI score;
	void Start()
	{
		btnNewGame.onClick.AddListener(NewGame);
		btnMainMenu.onClick.AddListener(MainMenu);

		score.text = Random.Range(10, 1000).ToString();
	}

	void NewGame()
    {
		controller.SetActive(true);
		canvas.SetActive(true);
		player.SetActive(true);

		result.gameObject.SetActive(false);


	}

	void MainMenu()
    {
		mainMenu.gameObject.SetActive(true);

		result.gameObject.SetActive(false);

	}


}
