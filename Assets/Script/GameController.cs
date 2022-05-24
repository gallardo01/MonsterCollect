using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MarchingBytes;

public class GameController : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        addEnemy();
    }

    private void addEnemy()
    {
        int playerLv = 1; 
        for(int i = 21; i <= 30; i++)
        {
            int enemyId = 1;
            string enemyType = "Enemy" + i; 
            GameObject enemy = EasyObjectPool.instance.GetObjectFromPool(enemyType, transform.position, transform.rotation);
            enemy.GetComponent<MonsterController>().setupWaypoints(Random.Range(1,17), i);
        }

    }
}
