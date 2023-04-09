﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MarchingBytes;

public class MonsterBullet : MonoBehaviour
{
    private MonsterData monsterData;
    private GameObject player;
    private bool type = false;
    private int percent = 0;
    private float speed = 0.07f;
    // Start is called before the first frame update
    void Start()
    {
        speed = 0.07f * Application.targetFrameRate / 60f;
    }

    private void OnEnable()
    {
        type = false;
    }

    private void Update()
    {
        if (type)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, speed);
            if(transform.position.x == player.transform.position.x && transform.position.y == player.transform.position.y)
            {
                EasyObjectPool.instance.ReturnObjectToPool(gameObject);
                gameObject.SetActive(false);
            }
        }
    }

    public void initData(MonsterData data, bool type, int percent)
    {
        this.percent = percent;
        monsterData = data;
        this.type = type;
        if (type)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            EasyObjectPool.instance.ReturnObjectToPool(gameObject);
            gameObject.SetActive(false);
        }

        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerController>().setPlayerHurt(monsterData, percent);
            EasyObjectPool.instance.ReturnObjectToPool(gameObject);
            gameObject.SetActive(false);
        }
    }
}
