using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BossController : MonoBehaviour
{
    // Start is called before the first frame update
    private int facingRight = 1;
    private float x = 0f, y = 0f;
    private float moveSpeed = 3f;
    private int waypointIndex = 0;
    private Vector2[] waypoints;

    private bool isMove = false;
    public int id;

    public TextMeshPro level;


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
        init();
        initInfo();
        StartCoroutine(idleBehavior());
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
                waypoints[waypointIndex],
                (moveSpeed / 4 * 3) * Time.deltaTime);

            if (transform.position.x == waypoints[waypointIndex].x && transform.position.y == waypoints[waypointIndex].y)
            {
                isMove = false;
                GetComponent<DragonBones.UnityArmatureComponent>().animation.Play("idle");
                StartCoroutine(idleBehavior());
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
            GetComponent<DragonBones.UnityArmatureComponent>().animation.Play("move");
            waypointIndex = Random.Range(0, 4);
            if (transform.position.x < waypoints[waypointIndex].x && facingRight == 0)
            {
                flip();
            }else if(transform.position.x > waypoints[waypointIndex].x && facingRight == 1)
            {
                flip();
            }
        }
        else
        {
            StartCoroutine(idleBehavior());
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
            if (id == 48)
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
            if (id == 33 || id == 47 || id == 63)
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
            if (id == 18 || id == 63)
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
            if (id == 29 || id == 63 || id == 63)
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
        gameObject.SetActive(false);
    }

}
