using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BossController : MonoBehaviour
{
    // Start is called before the first frame update
    private int facingRight = 1;
    private float x = 0f, y = 0f;
    public float moveSpeed = 1f;
    private int waypointIndex = 0;
    private Vector2[] waypoints;
    private int wayMove = 1;
    private BoxCollider2D boxCollider2D;


    private bool isMove = true;
    private bool isCast = false;

    private Vector3 playerLastPos;

    public int id;

    public TextMeshPro level;

    public Transform player;

    // boss 1
    private float rate;
    [SerializeField] private Transform bossTarget;

    void Awake()
    {
        boxCollider2D = gameObject.GetComponent<BoxCollider2D>();
        //boss 1
        rate = 2f;
        bossTarget.gameObject.SetActive(false);
    }

    void Start()
    {
        waypoints = new Vector2[]
        {
            new Vector2( 0, 0 ),
            new Vector2( 0, 0 ),
            new Vector2( 0, 0 ),
            new Vector2( 0, 0 ),
        };
        x = transform.position.x;
        y = transform.position.y;
        //init();
        initInfo();
        runAnimation(2);
        StartCoroutine(castSkill());
    }

    public int getLevel()
    {
        return id;
    }

    private void initInfo()
    {
        level.text = "Lv." + id.ToString();
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

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        if (isMove)
        {
            if(wayMove == 1)
            {
                transform.position = Vector2.MoveTowards(transform.position,
                player.position,
                (moveSpeed / 4 * 3) * Time.deltaTime);

                //if (transform.position.x == waypoints[waypointIndex].x && transform.position.y == waypoints[waypointIndex].y)
                //{
                //    isMove = false;
                //    GetComponent<DragonBones.UnityArmatureComponent>().animation.Play("idle");
                //    StartCoroutine(castSkill());
                //}

                if (transform.position.x < player.position.x && facingRight == 0)
                {
                    flip();
                }
                else if (transform.position.x > player.position.x && facingRight == 1)
                {
                    flip();
                }
            }
            else if (wayMove == 2)
            {
                boxCollider2D.enabled = false;
                transform.position = Vector2.MoveTowards(transform.position, new Vector3(transform.position.x, 20f, 0f), (moveSpeed * 30) * Time.deltaTime);
            }
            else if (wayMove == 3)
            {   
                boxCollider2D.enabled = true;
                transform.position = Vector2.MoveTowards(transform.position,
                        playerLastPos,
                        (moveSpeed * 50) * Time.deltaTime);
            }

        }

    }

    IEnumerator castSkill()
    {
        yield return new WaitForSeconds(2f);
        int chance = Random.Range(0, 10);
        if (chance <= rate && !isCast)
        {
            isCast = true; 
            //GetComponent<DragonBones.UnityArmatureComponent>().animation.Play("move");

            runAnimation(3);
            //waypointIndex = Random.Range(0, 4);
            //if (transform.position.x < waypoints[waypointIndex].x && facingRight == 0)
            //{
            //    flip();
            //}else if(transform.position.x > waypoints[waypointIndex].x && facingRight == 1)
            //{
            //    flip();
            //}

            
            StartCoroutine(runSkill());
        }

        StartCoroutine(castSkill());

    }

    IEnumerator runSkill()
    {
        wayMove = 2;

        yield return new WaitForSeconds(1f);
        playerLastPos = player.position;

        bossTarget.position =  new Vector3(playerLastPos.x, playerLastPos.y - 1f, playerLastPos.z);
        bossTarget.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        wayMove = 3;
        bossTarget.gameObject.SetActive(false);
        yield return new WaitForSeconds(1f);
        wayMove = 1;
        isCast = false;


    }

    void init()
    {
        if(Random.Range(0,4) % 2 == 0)
        {
            flip();
        }
        waypoints[0] = new Vector2(transform.position.x, transform.position.y);
        waypoints[1] = new Vector2(transform.position.x + Random.Range(-3f, 3f), transform.position.y + Random.Range(-5f, 5f));
        waypoints[2] = new Vector2(transform.position.x + Random.Range(-3f, 3f), transform.position.y + Random.Range(-5f, 5f));
        waypoints[3] = new Vector2(transform.position.x + Random.Range(-3f, 3f), transform.position.y + Random.Range(-5f, 5f));
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
            runAnimation(2);
        }
        else
        {
            runAnimation(1);
        }
    }


    IEnumerator disableObject()
    {
        gameObject.GetComponent<Collider2D>().enabled = false;
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
    }

}
