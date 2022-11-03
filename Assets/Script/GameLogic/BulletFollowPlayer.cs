using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MarchingBytes;

public class BulletFollowPlayer : MonoBehaviour
{
    private GameObject target;
    private int attack;
    private int crit;
    private int type;
    private int skill;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, .4f);
        } else
        {
            EasyObjectPool.instance.ReturnObjectToPool(gameObject);
            gameObject.SetActive(false);
        }
    }

    public void initBullet(int a, int c, int t, int s)
    {
        attack = a;
        crit = c;
        type = t;
        skill = s;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            GameController.Instance.addParticle(collision.gameObject, 1);
            EasyObjectPool.instance.ReturnObjectToPool(gameObject);
            gameObject.SetActive(false);
        }
        if (collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "Boss")
        {
            GameController.Instance.addParticle(collision.gameObject, type*10 + skill);
            EasyObjectPool.instance.ReturnObjectToPool(gameObject);
            gameObject.SetActive(false);
        }
    }



}
