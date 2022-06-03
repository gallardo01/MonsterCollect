using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MarchingBytes;

public class GameController : Singleton<GameController>
{

    // Start is called before the first frame update
    void Start()
    {
        addEnemy();
    }

    public void addEnemy()
    {
        int playerLv = 1;
        for (int i = 21; i <= 30; i++)
        {
            int enemyId = 1;
            string enemyType = "Enemy" + i;
            GameObject enemy = EasyObjectPool.instance.GetObjectFromPool(enemyType, transform.position, transform.rotation);
            enemy.GetComponent<MonsterController>().setupWaypoints(Random.Range(1, 17), i);
        }
    }

    public void addParticle(GameObject obj, int index)
    {
        string par = "Particle" + index;
        GameObject particle = EasyObjectPool.instance.GetObjectFromPool(par, obj.transform.position, obj.transform.rotation);
        if(index == 2)
        {
            particle.transform.SetParent(obj.transform);
        }
        StartCoroutine(disableParticle(particle));
    }

    IEnumerator disableParticle(GameObject obj)
    {
        yield return new WaitForSeconds(0.5f);
        //obj.SetActive(false);
        EasyObjectPool.instance.ReturnObjectToPool(obj);
    }
}
