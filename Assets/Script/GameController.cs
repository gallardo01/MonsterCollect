using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MarchingBytes;
using TMPro;

public class GameController : Singleton<GameController>
{
    public Button btnQuit;

    public Canvas result;
    public GameObject canvas;
    public GameObject player;
    public GameObject controller;

    public GameObject expBar;
    public TextMeshProUGUI expText;


    // Start is called before the first frame update
    void Start()
    {
        addEnemy();
        //btnQuit.onClick.AddListener(quitGame);
    }

    public void gainExp()
    {

    }

    public void addEnemy()
    {
        int playerLv = PlayerPrefs.GetInt("Map")*10;
        for (int i = 0; i < 7; i++)
        {
            int enemyId = playerLv + 1;
            string enemyType = "Enemy" + enemyId;
            GameObject enemy = EasyObjectPool.instance.GetObjectFromPool(enemyType, transform.position, transform.rotation);
            enemy.GetComponent<MonsterController>().setupWaypoints(Random.Range(1, 17), enemyId);
        }
        for (int i = 2; i < 5; i++)
        {
            int enemyId = playerLv + i;
            string enemyType = "Enemy" + enemyId;
            GameObject enemy = EasyObjectPool.instance.GetObjectFromPool(enemyType, transform.position, transform.rotation);
            enemy.GetComponent<MonsterController>().setupWaypoints(Random.Range(1, 17), enemyId);
        }
    }

    //public IEnumerator respawnEnemy()
    //{

    //}

    public void addParticle(GameObject obj, int index)
    {
        string par = "Particle" + index;
        GameObject particle = EasyObjectPool.instance.GetObjectFromPool(par, obj.transform.position, obj.transform.rotation);
        if(index == 2)
        {
            particle.transform.SetParent(obj.transform);
        }
        StartCoroutine(disableParticle(particle));
    }

    IEnumerator disableParticle(GameObject obj)
    {
        yield return new WaitForSeconds(0.5f);
        //obj.SetActive(false);
        EasyObjectPool.instance.ReturnObjectToPool(obj);
    }

    void quitGame()
    {
        controller.SetActive(false);
        canvas.SetActive(false);
        player.SetActive(false);

        result.gameObject.SetActive(true);
    }
}
