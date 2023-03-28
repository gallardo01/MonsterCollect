﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MarchingBytes;
public class MonsterController : MonoBehaviour
{
    // Start is called before the first frame update
    public TextMeshPro level;
    private GameObject[] waypoints;
    private int waypointIndex = 0;
    private bool isMove = true;
    private int wayMove = 1;
    private int facingRight = 1;
    private Transform playerPos;
    private BoxCollider2D collider;
    private MonsterData monsterData;
    private int currentHp;
    private bool isDead = false;

    void Start()
    {
        collider = GetComponent<BoxCollider2D>();
        //gameObject.GetComponent<Collider2D>().enabled = true;
        StartCoroutine(stopIdle());
    }
    void FixedUpdate()
    {
        Move();
    }

    void OnEnable()
    {
        isMove = true;
        StartCoroutine(stopIdle());
    }

    IEnumerator stopIdle()
    {
        yield return new WaitForSeconds(Random.Range(5f, 9f));
        wayMove = 2;
    }
    public void initDataWaypoints(int id, int way)
    {
        isDead = false;
        monsterData = MonsterDatabase.Instance.fetchMonsterIndex(id);
        currentHp = monsterData.Hp;
        setText(id);
        setupWaypoints(way);
    }

    public void initData(int id, bool useWaypoint)
    {
        isDead = false;
        monsterData = MonsterDatabase.Instance.fetchMonsterIndex(id);
        wayMove = 2;

        currentHp = monsterData.Hp;
        setText(id);
        isMove = true;
        playerPos = GameObject.FindGameObjectsWithTag("Player")[0].transform;
        runAnimation(2);
    }

    public bool isFullHp()
    {
        return currentHp == monsterData.Hp;
    }

    public bool getIsDead()
    {
        return isDead;
    }
    public MonsterData getLevel()
    {
        return monsterData;
    }

    public void triggerWaypoints()
    {
        wayMove = 2;
    }

    public void enemyHurt(MyHeroes heroes, int damePercent)
    {
        if (!isDead)
        {
            int dame = MathController.Instance.playerHitEnemy(heroes, monsterData, damePercent);
            int type = MathController.Instance.getTypeValue(heroes, monsterData);
            int actualDame = Mathf.Abs(dame);
            currentHp -= actualDame;
            if (currentHp <= 0)
            {
                //GameController.Instance.initEatMonster(heroes.Level);
                // drop item
                dropItemController(monsterData.Id);
                isMove = false;
                setAction(2);
                isDead = true;
            }
            disableObject();
            string floatingText = "FloatingText";
            GameObject floatText = EasyObjectPool.instance.GetObjectFromPool(floatingText, transform.position, transform.rotation);
            floatText.GetComponent<FloatingText>().disableObject(dame, type);
        }
    }

    private void dropItemController(int monsterLv)
    {
        //drop exp
        GameObject expObj = EasyObjectPool.instance.GetObjectFromPool("Exp", transform.position, transform.rotation);
        expObj.GetComponent<ItemDropController>().setExp(returnExpGet(monsterLv));
        // drop gold
        if (Random.Range(0, 100) < 10)
        {
            GameObject goldObj = EasyObjectPool.instance.GetObjectFromPool("Gold", transform.position * 1.05f, transform.rotation);
            goldObj.GetComponent<ItemDropController>().setGold(Random.Range(10 + monsterLv * 2, 10 + monsterLv * 5));
        }
        // drop hp
        if (Random.Range(0, 100) < 5)
        {
            GameObject hpObj = EasyObjectPool.instance.GetObjectFromPool("Hp", transform.position * 1.05f, transform.rotation);
            hpObj.GetComponent<ItemDropController>().setHp(Random.Range(10, 31));
        }
        // drop magnet
        if (Random.Range(0, 100) < 5)
        {
            GameObject hpObj = EasyObjectPool.instance.GetObjectFromPool("Magnet", transform.position * 1.05f, transform.rotation);
            hpObj.GetComponent<ItemDropController>().setMagnet();
        }
        // drop item
        if (Random.Range(0, 100) < 1)
        {
            GameObject hpObj = EasyObjectPool.instance.GetObjectFromPool("Item", transform.position * 1.05f, transform.rotation);
            hpObj.GetComponent<ItemDropController>().setItem();
        }
    }

    private int returnExpGet(int lv)
    {
        // 10 12 14 16 18 20 22 24 25
        return ((lv % 10) + 2) * 350 * (PlayerController.Instance.getBonusPoints(9) + 100) / 100;
    }


    public int getCurrentHp()
    {
        return currentHp;
    }

    public void setAction(int action)
    {
        // attack back
        if (action == 1)
        {
            runAnimation(3);
            isMove = false;
            StartCoroutine(setMove());
        }
        else if (action == 2) // dead
        {
            runAnimation(4);
            isMove = false;
        }
    }

    IEnumerator setMove()
    {
        yield return new WaitForSeconds(1f);
        isMove = true;
    }

    private void Move()
    {
        if (isMove && !isDead)
        {
            // move theo waypoints
            if (wayMove == 1)
            {
                transform.position = Vector2.MoveTowards(transform.position,
                    waypoints[waypointIndex].transform.position,
                    (monsterData.Speed / 800f) * Time.deltaTime);
                if (transform.position.x == waypoints[waypointIndex].transform.position.x && transform.position.y == waypoints[waypointIndex].transform.position.y)
                {
                    isMove = false;
                    runAnimation(1);
                    StartCoroutine(idleBehavior());
                }
            }
            // move theo position
            else if (wayMove == 2)
            {
                transform.position = Vector2.MoveTowards(transform.position,
                    playerPos.position,
                    (monsterData.Speed / 1000f) * Time.deltaTime);
                checkFlipPlayer();
            }
        }
    }
    // Skill Monster

    private void skillGainSpeed()
    {
        StartCoroutine(skillSpeed());
    }

    IEnumerator skillSpeed()
    {
        int cacheSpeed = monsterData.Speed;
        yield return new WaitForSeconds(4f);
        monsterData.Speed *= 3/2;
    }
    private void skillGainArmour()
    {
        StartCoroutine(skillArmour());
    }
    IEnumerator skillArmour()
    {
        int cacheArmour = monsterData.Armour;
        yield return new WaitForSeconds(4f);
        monsterData.Armour += 10000;
        level.text = "<sprite=3>";
        yield return new WaitForSeconds(2f);
        level.text = " <sprite=" + (monsterData.Type + 10) + "> Lv." + monsterData.Id.ToString();
        monsterData.Armour -= 10000;
        StartCoroutine(skillArmour());
    }

    IEnumerator idleBehavior()
    {
        yield return new WaitForSeconds(2f);
        int chance = Random.Range(0, 2);
        if (chance % 2 == 0)
        {
            runAnimation(2);
            waypointIndex = Random.Range(0, 25);
            checkFlip();
        }
        else
        {
            StartCoroutine(idleBehavior());
        }
    }

    private void checkFlip()
    {
        if (transform.position.x < waypoints[waypointIndex].transform.position.x && facingRight == 0)
        {
            flip();
        }
        else if (transform.position.x > waypoints[waypointIndex].transform.position.x && facingRight == 1)
        {
            flip();
        }
    }

    private void checkFlipPlayer()
    {
        if (transform.position.x < playerPos.transform.position.x && facingRight == 0)
        {
            flip();
        }
        else if (transform.position.x > playerPos.transform.position.x && facingRight == 1)
        {
            flip();
        }
    }

    private void setText(int lv)
    {
        level.text = " <sprite=" + (monsterData.Type+10) +  "> Lv." + lv.ToString();
        setColor();
    }

    public void setColor()
    {
        int playerLv = GameController.Instance.getEnemyLv();
        if (monsterData.Id <= playerLv)
        {
            level.color = Color.white;
        }
        else if (monsterData.Id <= playerLv + 1)
        {
            level.color = Color.green;
        }
        else if (monsterData.Id <= playerLv + 2)
        {
            level.color = Color.yellow;
        }
        else
        {
            level.color = Color.red;
        }
    }

    public void setupWaypoints(int way)
    {
        isMove = true;
        wayMove = 1;
        string route = "Route1";
        GameObject[] waypoints1 = GameObject.FindGameObjectsWithTag(route);
        System.Array.Sort(waypoints1, CompareObNames);
        waypoints = waypoints1;
        //int pos = Random.Range(0, waypoints.Length);
        int pos = way;
        gameObject.SetActive(true);
        transform.position = waypoints[pos].transform.position;
        waypointIndex = Random.Range(0, waypoints.Length);
        playerPos = GameObject.FindGameObjectsWithTag("Player")[0].transform;
        checkFlip();
        runAnimation(2);
    }

    private void runAnimation(int pos)
    {
      

        //fix stupid animation 

        //idle
        if (pos == 1)
        {
            GetComponent<DragonBones.UnityArmatureComponent>().animation.Play("idle");         
        }
        //move
        else if (pos == 2)
        {
   
            GetComponent<DragonBones.UnityArmatureComponent>().animation.Play("move");
            
        }
        //attack
        else if (pos == 3)
        {      
            GetComponent<DragonBones.UnityArmatureComponent>().animation.GotoAndPlayByTime("attack", 0.5f, 1);            
            StartCoroutine(replayAnimation());
        }
        // die
        else if (pos == 4)
        {
            GetComponent<DragonBones.UnityArmatureComponent>().animation.GotoAndPlayByTime("die", 1f, 1);
            StartCoroutine(disableObject());
        }
    }

    IEnumerator replayAnimation()
    {
        yield return new WaitForSeconds(0.5f);
        if (isMove)
        {
            runAnimation(1);
        }
        else
        {
            runAnimation(2);
        }
    }

    IEnumerator returnObjectToPool(float timer, GameObject obj)
    {
        yield return new WaitForSeconds(timer);
        EasyObjectPool.instance.ReturnObjectToPool(obj);
        obj.SetActive(false);
    }


    IEnumerator disableObject()
    {
        gameObject.GetComponent<Collider2D>().enabled = false;
        yield return new WaitForSeconds(2f);
        EasyObjectPool.instance.ReturnObjectToPool(gameObject);
        gameObject.GetComponent<Collider2D>().enabled = true;
        gameObject.SetActive(false);
    }

    private int CompareObNames(GameObject x, GameObject y)
    {
        return x.name.CompareTo(y.name);
    }

    private void flip()
    {
        facingRight = 1 - facingRight;
        Vector3 newScale = gameObject.transform.localScale;
        newScale.x *= -1;
        Vector3 newScale2 = level.gameObject.transform.localScale;
        newScale2.x *= -1;
        level.gameObject.transform.localScale = newScale2;
        gameObject.transform.localScale = newScale;
    }

    public void stopRunning()
    {
        StartCoroutine(stopRunningSecond(0.5f));
    }

    IEnumerator stopRunningSecond(float timer)
    {
        int s = monsterData.Speed;
        monsterData.Speed = 0;
        yield return new WaitForSeconds(timer);
        monsterData.Speed = s;
    }
}
