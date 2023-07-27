using MarchingBytes;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class BulletRootController : MonoBehaviour
{
    MyHeroes heroes;
    int dame;
    bool isRoot = true;
    GameObject enemy;
    GameObject grass_2;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void initBullet(MyHeroes myHeroes, int dame)
    {
        isRoot = true;
        heroes = myHeroes;
        this.dame = dame;
        StartCoroutine(deactive());
    }

    IEnumerator deactive()
    {
        yield return new WaitForSeconds(5f);
        if (isRoot)
        {
            GameObject grass_2 = EasyObjectPool.instance.GetObjectFromPool("Particle_Grass_2", transform.position, transform.rotation);
            grass_2.GetComponent<ParticleSystem>().Play();
            EasyObjectPool.instance.ReturnObjectToPool(gameObject);
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy" && isRoot)
        {
            isRoot = false;
            Debug.Log("ROOT");
            collision.gameObject.GetComponent<MonsterController>().stopRunningBySecond(4f, gameObject.transform);
            grass_2 = EasyObjectPool.instance.GetObjectFromPool("Particle_Grass_3", transform.position, transform.rotation);
            grass_2.GetComponent<ParticleSystem>().Play();
            collision.gameObject.GetComponent<MonsterController>().enemyHurt(heroes, dame);
            GameController.Instance.addParticleDefault(collision.gameObject, heroes.Type);
            enemy = collision.gameObject;
            StartCoroutine(enemyHurtByStay());
        }
        else if (collision.gameObject.tag == "Boss" && isRoot)
        {
            isRoot = false;
            GameObject grass_2 = EasyObjectPool.instance.GetObjectFromPool("Particle_Grass_2", transform.position, transform.rotation);
            grass_2.GetComponent<ParticleSystem>().Play();
            StartCoroutine(hurtEnemyAround());
        }
    }

    IEnumerator enemyHurtByStay()
    {
        yield return new WaitForSeconds(1f);
        if(enemy.tag == "Enemy" && enemy.GetComponent<MonsterController>().getIsDead() == false) 
        {
            enemy.GetComponent<MonsterController>().enemyHurt(heroes, dame);
            GameController.Instance.addParticleDefault(enemy, heroes.Type);
        } else
        {
            GameObject grass_2 = EasyObjectPool.instance.GetObjectFromPool("Particle_Grass_2", transform.position, transform.rotation);
            grass_2.GetComponent<ParticleSystem>().Play();
            EasyObjectPool.instance.ReturnObjectToPool(gameObject);
            EasyObjectPool.instance.ReturnObjectToPool(grass_2);
            gameObject.SetActive(false);
        }
    }

    IEnumerator hurtEnemyAround()
    {
        yield return new WaitForSeconds(1f);
        EasyObjectPool.instance.getAllObjectInPosition(gameObject, 0.5f, heroes, dame);
        StartCoroutine(hurtEnemyAround());
    }

}
