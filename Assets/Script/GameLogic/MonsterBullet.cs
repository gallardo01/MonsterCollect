using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MarchingBytes;

public class MonsterBullet : MonoBehaviour
{
    private MonsterData monsterData;
    private GameObject player;
    private bool type = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        type = false;
    }

    private void Update()
    {
        if (type)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, 0.07f);
        }
    }

    public void initData(MonsterData data, bool type)
    {
        monsterData = data;
        this.type = type;
        monsterData.Atk = monsterData.Atk / 2;
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
            collision.gameObject.GetComponent<PlayerController>().setPlayerHurt(monsterData, 2);
            EasyObjectPool.instance.ReturnObjectToPool(gameObject);
            gameObject.SetActive(false);
        }
    }
}
