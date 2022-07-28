using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    // Start is called before the first frame update
   private int facingRight = 1;
    private float x = 0f, y = 0f;
    private float moveSpeed = 3f;
    private int waypointIndex = 0;
    private Vector2[] waypoints;

    private bool isMove = false;

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
        StartCoroutine(idleBehavior());
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

    public void flip()
    {
        facingRight = 1 - facingRight;
        transform.localScale = new Vector3(
                  transform.localScale.x * (-1),
                  transform.localScale.y,
                  transform.localScale.z);
    }

    
}
