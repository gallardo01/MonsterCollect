using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textConversation;
    [SerializeField] Button boy;
    [SerializeField] Button girl;
    [SerializeField] Button pokemon1;
    [SerializeField] Button pokemon2;
    [SerializeField] Button pokemon3;

    private int step = 1;

    private static string[] conversation = { "",
        "Hello friends, My name is Professor Goodwin, Welcome to the Element Monster world",
        "There are a lot of mysterious pet called Element Monster",
        "First, tell me, Are you a boy or a girl?",
        "Welcome, I will give you a Element Monster to travel with you",
        "Can you pick one?",
        "Fire beat Grass, Grass beat Electric, Electric beat Water, Water beat Fire",
        "Nice choice, hope you have an excited adventure",
    };
    // Start is called before the first frame update
    void Start()
    {
        boy.onClick.AddListener(() => chooseBoy());
        girl.onClick.AddListener(() => chooseGirl());
        pokemon1.onClick.AddListener(() => choose1());
        pokemon2.onClick.AddListener(() => choose2());
        pokemon3.onClick.AddListener(() => choose3());
        StartCoroutine(tutorial());
    }

    IEnumerator tutorial()
    {
        textConversation.text = conversation[step];
        yield return new WaitForSeconds(4f);
        step++;
        textConversation.text = conversation[step];
        yield return new WaitForSeconds(3f);
        step++;
        textConversation.text = conversation[step];
        yield return new WaitForSeconds(3f);
        boy.gameObject.SetActive(true);
        girl.gameObject.SetActive(true);
    }

    IEnumerator pickOne()
    {
        textConversation.text = conversation[step];
        yield return new WaitForSeconds(3f);
        step++;
        textConversation.text = conversation[step];
        yield return new WaitForSeconds(3f);
        pokemon1.gameObject.SetActive(true);
        pokemon2.gameObject.SetActive(true);
        pokemon3.gameObject.SetActive(true);
        boy.gameObject.SetActive(false);
        girl.gameObject.SetActive(false);
    }

    private void chooseBoy()
    {
        step++;
        girl.gameObject.SetActive(false);
        boy.interactable = false;
        StartCoroutine(pickOne());
    }

    private void chooseGirl()
    {
        step++;
        boy.gameObject.SetActive(false);
        girl.interactable = false;
        StartCoroutine(pickOne());
    }
    IEnumerator finale()
    {
        step++;
        textConversation.text = conversation[step];
        yield return new WaitForSeconds(4f);
        step++;
        textConversation.text = conversation[step];
        yield return new WaitForSeconds(4f);
        // Load scene
        AsyncOperation loadLevelOp = SceneManager.LoadSceneAsync("UI");
        while (!loadLevelOp.isDone)
        {
            // update progress bar amount to loadLevelOp.progress
            yield return null;
        }
    }

    private void choose1()
    {
        pokemon2.gameObject.SetActive(false);
        pokemon3.gameObject.SetActive(false);
        pokemon1.interactable = false;
        HeroesDatabase.Instance.unlockHero(10);
        PlayerPrefs.SetInt("HeroesPick", 10);
        StartCoroutine(finale());
    }

    private void choose2()
    {
        pokemon1.gameObject.SetActive(false);
        pokemon3.gameObject.SetActive(false);
        pokemon2.interactable = false;
        HeroesDatabase.Instance.unlockHero(20);
        PlayerPrefs.SetInt("HeroesPick", 20);
        StartCoroutine(finale());
    }

    private void choose3()
    {
        pokemon1.gameObject.SetActive(false);
        pokemon2.gameObject.SetActive(false);
        pokemon3.interactable = false;
        HeroesDatabase.Instance.unlockHero(30);
        PlayerPrefs.SetInt("HeroesPick", 30);
        StartCoroutine(finale());
    }


}
