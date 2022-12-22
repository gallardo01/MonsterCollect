﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MarchingBytes;

public class BulletController : MonoBehaviour
{
    private MyHeroes heroes;
    [SerializeField] int id;
    private Transform target;
    // Start is called before the first frame update

    private void Start()
    {
       
    }

    void Update()
    {
        if (target != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, .035f);
        } else
        {
            returnToPool(0f);
        }
    }

    void OnDisable()
    {
    }

    void OnEnable()
    {
        if (id == 1)
        {
            StartCoroutine(disableCollider());
        }
    }

    IEnumerator disableCollider()
    {
        gameObject.GetComponent<Collider2D>().enabled = false;
        yield return new WaitForSeconds(1f);
        gameObject.GetComponent<Collider2D>().enabled = true;
    }
    IEnumerator thunder_2()
    {
        yield return new WaitForSeconds(1f);
        StartCoroutine(explosion(heroes));
        StartCoroutine(returnToPool(1.3f));
    }

    public void initBullet(MyHeroes myHeroes, int skill, Transform enemy)
    {
        target = enemy;
        heroes = myHeroes;

        if (skill == 1)
        {
            StartCoroutine(thunder_2());
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Wall" && id != 1)
        {
            GameController.Instance.addParticle(collision.gameObject, 1);
            EasyObjectPool.instance.ReturnObjectToPool(gameObject);
            gameObject.SetActive(false);
        }
        if (collision.gameObject.tag == "Enemy" && id != 1 && id != 12)
        {
            collision.gameObject.GetComponent<MonsterController>().enemyHurt(heroes);
            GameController.Instance.addParticle(collision.gameObject, 1);
            EasyObjectPool.instance.ReturnObjectToPool(gameObject);
            gameObject.SetActive(false);
        }
        if (collision.gameObject.tag == "Enemy" && id == 12)
        {
            collision.gameObject.GetComponent<MonsterController>().enemyHurt(heroes);
            GameController.Instance.addParticle(collision.gameObject, 1);
            returnToPool(1f);
        }
        if (collision.gameObject.tag == "Enemy" && id == 1)
        {
            //collision.gameObject.GetComponent<MonsterController>().stopRunning();
        }
    }
    
    IEnumerator explosion(MyHeroes data)
    {
        yield return new WaitForSeconds(0);
        GameController.Instance.addExplosion(data, gameObject, 3);
    }

    IEnumerator returnToPool(float time)
    {
        yield return new WaitForSeconds(time);
        EasyObjectPool.instance.ReturnObjectToPool(gameObject);
        gameObject.SetActive(false);
    }
}
