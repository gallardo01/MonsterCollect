using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
	public Button btnNewGame;
	public Canvas mainMenu;
	public GameObject canvas;
	public GameObject player;
	public GameObject controller;

	void Start()
	{
		btnNewGame.onClick.AddListener(NewGame);
	}

	void NewGame()
	{
		controller.SetActive(true);
		canvas.SetActive(true);
		player.SetActive(true);

		mainMenu.gameObject.SetActive(false);

	}
}
