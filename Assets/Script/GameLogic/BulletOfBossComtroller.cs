﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MarchingBytes;

public class BulletOfBossComtroller : MonoBehaviour
{

    private float speed = 5f;
    private Transform target;
    private Vector3 direction;
    private int type = 0;
    private MonsterData monsterData;

    const string AC_SCALE_B7 = "is_trigger_scale";
    const string AC_EXPLOSION_B10 = "is_trigger_explosion";


    private Vector3[] dir = {
        new Vector3(1, 0.6f, 0),
        new Vector3(1, -0.6f, 0),
        new Vector3(0, -1, 0),
        new Vector3(-1, -0.6f, 0),
        new Vector3(-1, 0.6f, 0),
        new Vector3(0, 1, 0),
    };



    // Start is called before the first frame update


    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            transform.position += direction * speed *Time.deltaTime;
        }
    }

    public void initBullet(Transform Target, Vector3 Direction, float Speed, int type, MonsterData monsterData)
    {
        target = Target;
        speed = Speed;
        direction = Direction;
        this.type = type;
        this.monsterData = monsterData;
        //direction = Vector3.Normalize(target.position -Source.position);

        if (type == 4 ) // bubble
        {
            StartCoroutine(runAnimScaleBubble(1f));

            StartCoroutine(returnToPool(10f));

        }else if (type == 5 )
        {
            StartCoroutine(runAnimExplosion(1f));
            StartCoroutine(returnToPool(5f));

        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            GameController.Instance.addParticle(collision.gameObject, 1);
            EasyObjectPool.instance.ReturnObjectToPool(gameObject);
            gameObject.SetActive(false);
        }

        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerController>().setPlayerHurt(monsterData, type);

            if (type == 1) // root bullet
            {
                speed = 0f;
                transform.position = collision.transform.position;

                StartCoroutine(returnToPool(1f));

            }
            else if (type == 2) // ice spike
            {


            }
            else if (type == 3) // slow wind
            {
                speed = 0f;
                StartCoroutine(returnToPool(3f));

            }
            else if (type == 4) // bubble 
            {
                transform.position = collision.transform.position;
                StartCoroutine(returnToPool(1f));

            }
            else if (type == 5) // explosion 
            {
                StartCoroutine(runAnimExplosion(1f));
                StartCoroutine(returnToPool(5f));

            }
            else
            {
                GameController.Instance.addParticle(collision.gameObject, 1);
                EasyObjectPool.instance.ReturnObjectToPool(gameObject);
                gameObject.SetActive(false);
            }
        }

        if (collision.gameObject.tag == "Boss" || collision.gameObject.tag == "Player" || collision.gameObject.tag == "Bullet")
        {
            if (type == 2) // ice spike
            {
                int ran = Random.Range(-30, 0);

                for (int i = 0; i < 6; i++)
                {
                    GameObject iceFrag = EasyObjectPool.instance.GetObjectFromPool("Bullet_ice_fragment", transform.position, transform.rotation);
                    //dir[i] = Quaternion.Euler(ran, ran, 0) * dir[i];

                    Vector3 temp = new Vector3(dir[i].x * Mathf.Cos(ran) + dir[i].y * Mathf.Sin(ran), -dir[i].x * Mathf.Sin(ran) + dir[i].y * Mathf.Cos(ran), 0);
                    //Debug.Log(Vector3.Normalize(temp));

                    Vector3 direction = Vector3.Normalize(temp);
                    iceFrag.GetComponent<BulletOfBossComtroller>().initBullet(target, direction, 5f, 0, monsterData);

                    float angle = Mathf.Atan2(-dir[i].x * Mathf.Sin(ran) + dir[i].y * Mathf.Cos(ran), dir[i].x * Mathf.Cos(ran) + dir[i].y * Mathf.Sin(ran));
                    iceFrag.transform.Rotate(0, 0, angle * 180 /Mathf.PI);
                }

                GameController.Instance.addParticle(collision.gameObject, 1);
                EasyObjectPool.instance.ReturnObjectToPool(gameObject);
                gameObject.SetActive(false);

                
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {   
            if (type == 3) // slow wind
            {
                collision.gameObject.GetComponent<PlayerController>().setPlayerNormal();
            }

        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            GameController.Instance.addParticle(collision.gameObject, 1);
            EasyObjectPool.instance.ReturnObjectToPool(gameObject);
            gameObject.SetActive(false);
        }

        if (collision.gameObject.tag == "Bullet")
        {
            if (type == 2) // ice spike
            {
                int ran = Random.Range(-30, 0);

                for (int i = 0; i < 6; i++)
                {
                    GameObject iceFrag = EasyObjectPool.instance.GetObjectFromPool("Bullet_ice_fragment", transform.position, transform.rotation);
                    //dir[i] = Quaternion.Euler(ran, ran, 0) * dir[i];

                    Vector3 temp = new Vector3(dir[i].x * Mathf.Cos(ran) + dir[i].y * Mathf.Sin(ran), -dir[i].x * Mathf.Sin(ran) + dir[i].y * Mathf.Cos(ran), 0);
                    //Debug.Log(Vector3.Normalize(temp));

                    Vector3 direction = Vector3.Normalize(temp);
                    iceFrag.GetComponent<BulletOfBossComtroller>().initBullet(target, direction, 5f, 0, monsterData);

                    float angle = Mathf.Atan2(-dir[i].x * Mathf.Sin(ran) + dir[i].y * Mathf.Cos(ran), dir[i].x * Mathf.Cos(ran) + dir[i].y * Mathf.Sin(ran));
                    iceFrag.transform.Rotate(0, 0, angle * 180 / Mathf.PI);
                }

                GameController.Instance.addParticle(collision.gameObject, 1);
                EasyObjectPool.instance.ReturnObjectToPool(gameObject);
                gameObject.SetActive(false);
            }
        }
    }




    IEnumerator returnToPool(float time)
    {
        yield return new WaitForSeconds(time);

        if (type == 4)
        {
            gameObject.GetComponent<Animator>().SetBool(AC_SCALE_B7, false);

        }
        else if (type == 5)
        {
            gameObject.GetComponent<Animator>().SetBool(AC_EXPLOSION_B10, false);

        }
        EasyObjectPool.instance.ReturnObjectToPool(gameObject);
        gameObject.SetActive(false);

        
    }

    IEnumerator runAnimExplosion(float time)
    {
        yield return new WaitForSeconds(time);
        gameObject.GetComponent<Animator>().SetBool(AC_EXPLOSION_B10, true);
        speed = 0f;
    }

    IEnumerator runAnimScaleBubble(float time)
    {
        yield return new WaitForSeconds(time);
        gameObject.GetComponent<Animator>().SetBool(AC_SCALE_B7, true);
        speed = 0f;
    }

}
