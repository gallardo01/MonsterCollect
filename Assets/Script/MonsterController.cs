using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MonsterController : MonoBehaviour
{
    // Start is called before the first frame update
    public int id;
    public TextMeshPro level;
    public int speed;
    private GameObject[] waypoints;
    private int waypointIndex = 0;
    private bool isMove = true;
    private int facingRight = 1;
    private int monsterLv = 1;
    
    void Start()
    {
        gameObject.GetComponent<Collider2D>().enabled = true;
        setText(id);
    }

    void Update()
    {
        Move();
    }

    public int getLevel()
    {
        return id;
    }

    public void setAction(int action)
    {
        // attack back
        if(action == 1)
        {
            runAnimation(3);
            isMove = false;
            StartCoroutine(setMove());
        }
        else if(action == 2) // dead
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
            transform.position = Vector2.MoveTowards(transform.position,
                waypoints[waypointIndex].transform.position,
                (speed / 80) * Time.deltaTime);
            if (transform.position.x == waypoints[waypointIndex].transform.position.x && transform.position.y == waypoints[waypointIndex].transform.position.y)
            {
                isMove = false;
                runAnimation(1);
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
            runAnimation(2);
            //waypointIndex = Random.Range(0, waypoints.Length);
            waypointIndex++;
            if(waypointIndex == waypoints.Length)
            {
                waypointIndex = 0;
            }
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
    private int getId()
    {
        return id;
    }

    private void setText(int lv)
    {
        monsterLv = lv;
        level.text = "Lv." + lv.ToString();
        setColor();
    }

    public void setColor()
    {
        int playerLv = PlayerController.Instance.getLevel();
        if (monsterLv <= playerLv)
        {
            level.color = Color.white;
        }
        else if(monsterLv <= playerLv+1)
        {
            level.color = Color.green;
        } else if(monsterLv <= playerLv+2)
        {
            level.color = Color.yellow;
        } else
        {
            level.color = Color.red;
        }
    }

    public void setupWaypoints(int num, int enemyId)
    {
        id = enemyId;
        string route = "Route" + num.ToString();
        GameObject[] waypoints1 = GameObject.FindGameObjectsWithTag(route);
        System.Array.Sort(waypoints1, CompareObNames);
        waypoints = waypoints1;
        int pos = Random.Range(0, waypoints.Length);
        gameObject.SetActive(true);
        transform.position = waypoints[pos].transform.position;
        checkFlip();
        runAnimation(2);
    }

    private void runAnimation(int pos)
    {
        //idle
        if(pos == 1)
        {
            if(id == 48)
            {
                GetComponent<DragonBones.UnityArmatureComponent>().animation.Play("IDLE");
            } else
            {
                GetComponent<DragonBones.UnityArmatureComponent>().animation.Play("idle");
            }
        }
        //move
        else if(pos == 2)
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
        else if(pos == 3)
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
        else if(pos == 4)
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

    }
}
