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

    public GameController deadPanel;
    public Button revive;
    public Button cancelRevive;

    private int exp = 0;
    private int playerLevel = 1;
    private bool isSpawn = true;

    private int enemyLv = 1;
    private int countEnemy = 1;

    private int goldAward = 0;
    private List<ItemInventory> itemAward = new List<ItemInventory>();

    // Start is called before the first frame update
    void Start()
    {
        // get stage
        enemyLv = 1;
        initInfo();
        playerLevel = PlayerController.Instance.getLevel();
        //addBoss();
        levelText.text = playerLevel.ToString();
        //btnQuit.onClick.AddListener(quitGame);
        StartCoroutine(addEnemyFirstScene());
    }

    public IEnumerator addEnemyFirstScene()
    {
        addEnemy();
        addEnemy();

        addEnemy();
        addEnemy();

        addEnemy();

        yield return new WaitForSeconds(1f);
        StartCoroutine(respawnEnemyDuringTime());
    }

    public int getEnemyLv()
    {
        return enemyLv;
    }
    private IEnumerator respawnEnemyDuringTime()
    {
        yield return new WaitForSeconds(1f);
        if (isSpawn)
        {
            addEnemy();
            if(countEnemy == 20)
            {
                enemyLv++;
                countEnemy = 0;
            }
        }
        if(enemyLv % 10 == 0)
        {

        } else
        {
            StartCoroutine(respawnEnemyDuringTime());
        }
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

    public void setSpawn(bool logic)
    {
        isSpawn = logic;
    }
    public void addEnemy()
    {
        if (isSpawn)
        {
            countEnemy++;
            int chance = Random.Range(0, 14);
            int enemyId;
            if (chance < 11)
            {
                enemyId = enemyLv;
            }
            else
            {
                enemyId = enemyLv + chance % 10;
                if (enemyId % 10 == 0)
                {
                    enemyId -= 1;
                }
            }
            string enemyType = "Enemy" + enemyId;
            GameObject enemy = EasyObjectPool.instance.GetObjectFromPool(enemyType, transform.position, transform.rotation);
            enemy.GetComponent<MonsterController>().initData(enemyId);
        }
    }
    public IEnumerator respawnEnemy()
    {
        yield return new WaitForSeconds(2f);
        int chance = Random.Range(1, 4);
        if(chance == 1)
        {
            addEnemy();
        }
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

    public void gainExpChar(int num)
    {
        exp += num;
        updateProgressBar(exp >= playerLevel * 1000);
    }

    private void updateProgressBar(bool levelUp)
    {
        //expBar.GetComponent<Slider>().value = progres;
        if (levelUp)
        {
            exp -= playerLevel * 1000;
            playerLevel++;
            expText.text = exp + "/" + (playerLevel * 1000);
            levelText.text = playerLevel.ToString();
            PlayerController.Instance.gainLv(playerLevel);
            updateColorText();
        }
        expText.text = exp + "/" + (playerLevel * 1000);
        float progres = (float)exp / (float)(playerLevel * 1000);
        StartCoroutine(animationprogressBar(expBar.GetComponent<Slider>().value, progres, levelUp));
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

    public void addAwardToInventory(int gold, ItemInventory item)
    {
        goldAward += gold;
        itemAward.Add(item);
    }
}
