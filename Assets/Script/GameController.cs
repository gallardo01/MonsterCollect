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
    public TextMeshProUGUI levelText;
    public GameObject bossController;

    private int exp = 0;
    private int playerLevel = 1;


    // Start is called before the first frame update
    void Start()
    {
        initInfo();
        playerLevel = PlayerController.Instance.getLevel();
        for (int i = 0; i < 12; i++)
        {
            addEnemy();
        }
        //addBoss();
        levelText.text = playerLevel.ToString();
        //btnQuit.onClick.AddListener(quitGame);
    }

    private void initInfo()
    {
        expBar.GetComponent<Slider>().value = 0;
    }

    private void addBoss()
    {
        string enemyType = "No." + (playerLevel/10 + 10);
        GameObject boss = Instantiate(Resources.Load("Prefabs/Pokemon/" + enemyType) as GameObject, bossController.transform);
        boss.transform.localPosition = new Vector3(0, 0, 0);
        boss.GetComponent<DragonBones.UnityArmatureComponent>().animation.Play("idle");
    }

    public void addEnemy()
    {
        int playerLv = PlayerController.Instance.getLevel();
        int chance = Random.Range(2, 14);
        int enemyId = 0;
        if (chance < 11)
        {
            enemyId = playerLv;
        }
        else
        {
            enemyId = playerLv + chance % 10;
            if(enemyId > ((playerLv - 1)/10 + 9))
            {
                enemyId = ((playerLv - 1) / 10) + 9;
            }
        }
        string enemyType = "Enemy" + enemyId;
        GameObject enemy = EasyObjectPool.instance.GetObjectFromPool(enemyType, transform.position, transform.rotation);
        enemy.GetComponent<MonsterController>().initData(enemyId);
    }
    public IEnumerator respawnEnemy()
    {
        yield return new WaitForSeconds(2f);
        int chance = Random.Range(1, 4);
        if(chance == 1)
        {
            addEnemy();
        } else if(chance == 2)
        {
            addEnemy();
            yield return new WaitForSeconds(1f);
            addEnemy();
        }
    }
    public void initEatMonster(int lv)
    {
        // 10 12 14 16 18 20 22 24 25
        exp += ((lv % 10) + 2)* 50;
        if (exp >= playerLevel % 10 * 1000)
        {
            playerLevel++;
            exp = 0;
            updateProgressBar(true);
        }
        else
        {
            updateProgressBar(false);
        }
        StartCoroutine(respawnEnemy());
    }
    private void updateColorText()
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("Enemy");
        foreach(GameObject go in gos)
        {
            go.GetComponent<MonsterController>().setColor();
        }
    }

    private void updateProgressBar(bool levelUp)
    {
        float progres = (float) exp / (float) (playerLevel % 10 * 1000);
        //expBar.GetComponent<Slider>().value = progres;
        StartCoroutine(animationprogressBar(expBar.GetComponent<Slider>().value, progres, levelUp));
        expText.text = exp + "/" + (playerLevel % 10 * 1000);
        if (levelUp)
        {
            levelText.text = playerLevel.ToString();
            PlayerController.Instance.gainLv(playerLevel);
            updateColorText();
        }
    }

    private IEnumerator animationprogressBar(float current, float last, bool lvUp)
    {
        if (lvUp)
        {
            while (current < 1)
            {
                current += 0.05f;
                if (current >= 1)
                {
                    current = 1;
                }
                expBar.GetComponent<Slider>().value = current;
                yield return new WaitForSeconds(0.03f);
            }
            current = 0f;
            expBar.GetComponent<Slider>().value = current;
        }
        while (current < last)
        {
            current += 0.05f;
            if(current >= last)
            {
                current = last;
            }
            expBar.GetComponent<Slider>().value = current;
            yield return new WaitForSeconds(0.03f);
        }
    }

    public int getLevel()
    {
        return playerLevel;
    }

    public void addParticle(GameObject obj, int index)
    {
        string par = "Particle" + index;
        GameObject particle = EasyObjectPool.instance.GetObjectFromPool(par, obj.transform.position, obj.transform.rotation);
        if (index == 2 || index == 4)
        {
            particle.transform.SetParent(obj.transform);
        }
        StartCoroutine(disableParticle(particle));
    }

    public void addExplosion(MyHeroes heroes, GameObject obj, int index)
    {
        string par = "Particle" + index;
        GameObject particle = EasyObjectPool.instance.GetObjectFromPool(par, obj.transform.position, obj.transform.rotation);
        particle.GetComponent<ExplosionController>().initData(heroes);
        StartCoroutine(disableParticle(particle));
    }

    IEnumerator disableParticle(GameObject obj)
    {
        yield return new WaitForSeconds(1f);
        EasyObjectPool.instance.ReturnObjectToPool(obj);
        obj.SetActive(false);
    }

    void quitGame()
    {
        controller.SetActive(false);
        canvas.SetActive(false);
        player.SetActive(false);

        result.gameObject.SetActive(true);
    }
}
