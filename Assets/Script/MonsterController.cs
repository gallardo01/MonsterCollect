using System.Collections;
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

    void Start()
    {
        collider = GetComponent<BoxCollider2D>();
        //gameObject.GetComponent<Collider2D>().enabled = true;
    }
    void FixedUpdate()
    {
        Move();
    }
    
    public void initData(int id)
    {
        monsterData = MonsterDatabase.Instance.fetchMonsterIndex(id);
        currentHp = monsterData.Hp;
        setText(id);
        setupWaypoints();
    }
    public MonsterData getLevel()
    {
        return monsterData;
    }

    public void triggerWaypoints()
    {
        wayMove = 2;
    }

    public void enemyHurt(MyHeroes heroes)
    {
        int dame = MathController.Instance.playerHitEnemy(heroes, monsterData);
        int actualDame = Mathf.Abs(dame);
        currentHp -= actualDame;
        if(currentHp <= 0)
        {
            isMove = false;
            setAction(2);
        }
        disableObject();
        string floatingText = "FloatingText";
        GameObject particle = EasyObjectPool.instance.GetObjectFromPool(floatingText, transform.position, transform.rotation);
        particle.GetComponent<FloatingText>().disableObject(dame);

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
        if (isMove)
        {
            // move theo waypoints
            if (wayMove == 1)
            {
                transform.position = Vector2.MoveTowards(transform.position,
                    waypoints[waypointIndex].transform.position,
                    (monsterData.Speed / 1200f) * Time.deltaTime);
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
                    (monsterData.Speed / 1200f) * Time.deltaTime);
                checkFlipPlayer();
            }
        }
    }

    IEnumerator idleBehavior()
    {
        yield return new WaitForSeconds(2f);
        int chance = Random.Range(0, 2);
        if (chance % 2 == 0)
        {
            isMove = true;
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
        level.text = "Lv." + lv.ToString();
        setColor();
    }

    public void setColor()
    {
        int playerLv = PlayerController.Instance.getLevel();
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

    public void setupWaypoints()
    {
        isMove = true;
        wayMove = 1;
        string route = "Route1";
        GameObject[] waypoints1 = GameObject.FindGameObjectsWithTag(route);
        System.Array.Sort(waypoints1, CompareObNames);
        waypoints = waypoints1;
        int pos = Random.Range(0, waypoints.Length);
        gameObject.SetActive(true);
        transform.position = waypoints[pos].transform.position;
        waypointIndex = Random.Range(0, waypoints.Length);
        playerPos = GameObject.FindGameObjectsWithTag("Player")[0].transform;
        checkFlip();
        runAnimation(2);
    }

    private void runAnimation(int pos)
    {
        //idle
        if (pos == 1)
        {
            if (monsterData.Id == 48)
            {
                GetComponent<DragonBones.UnityArmatureComponent>().animation.Play("IDLE");
            }
            else
            {
                GetComponent<DragonBones.UnityArmatureComponent>().animation.Play("idle");
            }
        }
        //move
        else if (pos == 2)
        {
            if (monsterData.Id == 33 || monsterData.Id == 47 || monsterData.Id == 63)
            {
                GetComponent<DragonBones.UnityArmatureComponent>().animation.Play("Move");
            }
            else
            {
                GetComponent<DragonBones.UnityArmatureComponent>().animation.Play("move");
            }
        }
        //attack
        else if (pos == 3)
        {
            if (monsterData.Id == 18 || monsterData.Id == 63)
            {
                GetComponent<DragonBones.UnityArmatureComponent>().animation.GotoAndPlayByTime("attacl", 0.5f, 1);
            }
            else
            {
                GetComponent<DragonBones.UnityArmatureComponent>().animation.GotoAndPlayByTime("attack", 0.5f, 1);
            }
            StartCoroutine(replayAnimation());
        }
        // die
        else if (pos == 4)
        {
            if (monsterData.Id == 29 || monsterData.Id == 63 || monsterData.Id == 63)
            {
                GetComponent<DragonBones.UnityArmatureComponent>().animation.GotoAndPlayByTime("newAnimation", 0.5f, 1);
            }
            else
            {
                GetComponent<DragonBones.UnityArmatureComponent>().animation.GotoAndPlayByTime("die", 1f, 1);
            }
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

    IEnumerator disableObject()
    {
        gameObject.GetComponent<Collider2D>().enabled = false;
        yield return new WaitForSeconds(1f);
        gameObject.GetComponent<Collider2D>().enabled = true;
        EasyObjectPool.instance.ReturnObjectToPool(gameObject);
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if (collision.gameObject.tag == "Bullet")
        //{
        //    Debug.Log("ABC");
        //    StartCoroutine(disableObject());
        //}
    }
}
