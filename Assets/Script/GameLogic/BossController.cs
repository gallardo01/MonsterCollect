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

    private bool isMove = true;
    public int id;

    public TextMeshPro level;

    public Transform player;


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

    }

    IEnumerator castSkill()
    {
        yield return new WaitForSeconds(2f);
        int chance = Random.Range(0, 10);
        if (chance <= 4)
        {
            Debug.Log("dam");
            isMove = false;
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

            isMove = true;

        }
        else
        {
            StartCoroutine(castSkill());
        }
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
