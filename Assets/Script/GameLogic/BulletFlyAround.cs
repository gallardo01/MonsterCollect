using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MarchingBytes;

public class BulletFlyAround : MonoBehaviour
{
    // time = 3

    // Update is called once per frame

    private Transform target;
    private MyHeroes heroes;
    private void Start()
    {
       


    }


    void Update()
    {
        transform.position = target.position + new Vector3(2.5f * Mathf.Cos(Mathf.PI * ((Time.fixedTime)%3)/1.5f), 2.5f * Mathf.Sin(Mathf.PI * ((Time.fixedTime) % 3) / 1.5f), 0);
    }

    public void initBullet(MyHeroes myHeroes, int skill, Transform enemy)
    {
        target = enemy;
        heroes = myHeroes;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.GetComponent<MonsterController>().enemyHurt(heroes);
            GameController.Instance.addParticle(collision.gameObject, 4);
        }
    }

}
