using MarchingBytes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyControllerKien : MonoBehaviour
{
    public float speed = 10;
    public GameObject physic;

    private Rigidbody2D rb;
    private int direction = 1;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(castSkill());
    }
    public void Update()
    {
        Vector3 tempVect = new Vector3(direction, 0, 0);
        tempVect = tempVect.normalized * speed * Time.deltaTime;
        rb.MovePosition(rb.transform.position + tempVect);

        Vector3 tempScale = new Vector3(-direction, 1, 1);
        transform.localScale = tempScale;

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("danh player");
        }
        if (collision.gameObject.tag == "Wall")
        {
            direction = -direction;

        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            Debug.Log("bi ban");
        }
    }

    IEnumerator castSkill()
    {
        yield return new WaitForSeconds(Random.Range(5, 10));
        direction = -direction;
        StartCoroutine(castSkill());

    }
}
