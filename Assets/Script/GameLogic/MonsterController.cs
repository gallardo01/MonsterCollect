using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MarchingBytes;
public class MonsterController : MonoBehaviour
{
    // Start is called before the first frame update
    public TextMeshPro level;
    private Vector2 waypoints;
    private bool isMove = true;
    private int wayMove = 1;
    private int facingRight = 1;
    private Transform playerPos;
    private MonsterData monsterData;
    private int currentHp;
    private bool isDead = false;
    private float slowRate = 1f;
    private bool raiseArmour = false;
    private int idData = 0;

    void Start()
    {
        playerPos = PlayerController.Instance.returnObj();
    }
    void FixedUpdate()
    {
        Move();
    }

    private void Awake()
    {
        //waypoints.transform.position = new Vector3(Random.Range(-8f, 8f), Random.Range(-6f, 8f), 0f);
    }

    void OnEnable()
    {
        waypoints = new Vector2(Random.Range(-8f, 8f), Random.Range(-6f, 8f));
        isMove = true;
        StartCoroutine(stopIdle());
    }

    IEnumerator stopIdle()
    {
        yield return new WaitForSeconds(Random.Range(10f, 20f));
        wayMove = 2;
    }
    public void initDataWaypoints(int id, int way)
    {
        isDead = false;
        monsterData = MonsterDatabase.Instance.fetchMonsterIndex(id);
        abilityMonster();
        currentHp = monsterData.Hp;
        setText(id);
        idData = way;
        setupWaypoints();
    }
    public void initData(int id, bool useWaypoint)
    {
        //transform.position = new Vector3(Random.Range(-8f, 8f), Random.Range(-6f, 8f), 0f);
        isDead = false;
        monsterData = MonsterDatabase.Instance.fetchMonsterIndex(id);
        currentHp = monsterData.Hp;
        setText(id);
        isMove = true;
        wayMove = 1;
        runAnimation(2);
    }
    public int getIdData()
    {
        return idData;
    }
    public void setupWaypoints()
    {
        isMove = true;
        wayMove = 1;
        checkFlip();
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
            if(heroes == null)
            {
                heroes = PlayerController.Instance.getRealData();
            }
            int dame = MathController.Instance.playerHitEnemy(heroes, monsterData, damePercent);
            int type = MathController.Instance.getTypeValue(heroes, monsterData);
            if (raiseArmour)
            {
                dame = dame * 4 / 10;
            }
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
            hpObj.GetComponent<ItemDropController>().setHp(Random.Range(15, 31));
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
        int exp = 125;
        if(lv > 1 && lv < 9)
        {
            exp = 200 + (lv - 2) * 40;
        } else if(lv == 9)
        {
            exp = 500;
        }
        return (exp * (PlayerController.Instance.getBonusPoints(9) + 100) / 100);
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
                    waypoints,
                    (monsterData.Speed * slowRate / 800f) * Time.deltaTime);
                if (transform.position.x == waypoints.x && transform.position.y == waypoints.y)
                {
                    isMove = false;
                    StartCoroutine(idleBehavior());
                }
            }
            // move theo position
            else if (wayMove == 2)
            {
                transform.position = Vector2.MoveTowards(transform.position,
                    playerPos.position,
                    (monsterData.Speed * slowRate / 1000f) * Time.deltaTime);
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
        yield return new WaitForSeconds(4f);
        slowRate = 2f;
        yield return new WaitForSeconds(3f);
        slowRate = 1f;
        StartCoroutine(skillSpeed());
    }
    private void skillGainArmour()
    {
        StartCoroutine(skillArmour());
    }
    IEnumerator skillArmour()
    {
        yield return new WaitForSeconds(4f);
        raiseArmour = true;
        level.text = "<sprite=3>";
        yield return new WaitForSeconds(3f);
        level.text = " <sprite=" + (monsterData.Type + 10) + "> Lv." + monsterData.Id.ToString();
        StartCoroutine(skillArmour());
    }

    IEnumerator idleBehavior()
    {
        runAnimation(1);
        yield return new WaitForSeconds(1f);
        int chance = Random.Range(0, 2);
        if (chance % 2 == 0)
        {
            waypoints = new Vector2(Random.Range(-8f, 8f), Random.Range(-6f, 8f));
            isMove = true;
            runAnimation(2);
            checkFlip();
        }
        else
        {
            StartCoroutine(idleBehavior());
        }
    }

    private void checkFlip()
    {
        if (transform.position.x < waypoints.x && facingRight == 0)
        {
            flip();
        }
        else if (transform.position.x > waypoints.x && facingRight == 1)
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
    private void runAnimation(int pos)
    {
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
        if (monsterData.Id == 3)
        {
            GameObject bullet = EasyObjectPool.instance.GetObjectFromPool("MonsterBullet_1", transform.position, transform.rotation);
            Vector2 force = playerPos.position - transform.position;
            force = force.normalized;
            bullet.GetComponent<MonsterBullet>().initData(monsterData, false, 50);
            bullet.GetComponent<Rigidbody2D>().AddForce(force * 300);
            float angle = calAngle(playerPos, force);
            bullet.transform.Rotate(0, 0, angle + 90);
        }
        else if (monsterData.Id == 13)
        {
            GameObject bullet = EasyObjectPool.instance.GetObjectFromPool("MonsterBullet_2", transform.position, transform.rotation);
            Vector2 force = playerPos.position - transform.position;
            force = force.normalized;
            bullet.GetComponent<MonsterBullet>().initData(monsterData, false, 50);
            bullet.GetComponent<Rigidbody2D>().AddForce(force * 300);
            float angle = calAngle(playerPos, force);
            bullet.transform.Rotate(0, 0, angle + 90);
        }
        else if (monsterData.Id == 7)
        {
            GameObject bullet = EasyObjectPool.instance.GetObjectFromPool("MonsterBullet_3", transform.position, transform.rotation);
            Vector2 force = playerPos.position - transform.position;
            force = force.normalized;
            bullet.GetComponent<MonsterBullet>().initData(monsterData, true, 30);
            float angle = calAngle(playerPos, force);
            bullet.transform.Rotate(0, 0, angle + 90);
        }
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

    public void stopRunningBySecond(float timer, Transform pos)
    {
        transform.position = pos.position;
        StartCoroutine(stopRunningSecond(timer));
    }


    public void stopRunning()
    {
        StartCoroutine(stopRunningSecond(0.5f));
    }

    IEnumerator stopRunningSecond(float timer)
    {
        slowRate = 0;
        yield return new WaitForSeconds(timer);
        slowRate = 1;
    }

    private void abilityMonster()
    {
        if(monsterData.Id == 5)
        {
            skillGainArmour();
        } else if(monsterData.Id == 9)
        {
            skillGainSpeed();
        }
    }
    private float calAngle(Transform en, Vector2 vector)
    {
        Vector2 cur = new Vector2(-1, 0);
        float y = gameObject.transform.position.y;
        float n = en.transform.position.y;
        float angle = 0;

        if (y <= n)
            angle = 2 * AngleTo(cur, vector);
        else
            angle = -2 * AngleTo(cur, vector);

        return angle;
    }
    private float AngleTo(Vector2 pos, Vector2 target)
    {
        Vector2 diference;
        if (target.x > pos.x)
            diference = target - pos;
        else
            diference = pos - target;
        return Vector2.Angle(Vector2.right, diference);
    }

}
