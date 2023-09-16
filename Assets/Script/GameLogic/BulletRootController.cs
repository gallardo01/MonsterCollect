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
    GameObject grass_2;
    int count = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void initBullet(MyHeroes myHeroes, int dame)
    {
        GameObject par = EasyObjectPool.instance.GetObjectFromPool("Particle_Grass_3", transform.position, transform.rotation);
        par.GetComponent<ParticleSystem>().Play();
        count = 0;
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
            GameObject par = EasyObjectPool.instance.GetObjectFromPool("Particle_Grass_3", transform.position, transform.rotation);
            par.GetComponent<ParticleSystem>().Play();
            StopAllCoroutines();
            EasyObjectPool.instance.ReturnObjectToPool(gameObject);
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            if (isRoot)
            {
                StartCoroutine(deactiveAfterRoot());
                grass_2 = EasyObjectPool.instance.GetObjectFromPool("Particle_Grass_2", transform.position, transform.rotation);
                grass_2.GetComponent<ParticleSystem>().Play();
            }
            isRoot = false;
            collision.gameObject.GetComponent<MonsterController>().stopRunningBySecond(4f, gameObject.transform);
            collision.gameObject.GetComponent<MonsterController>().enemyHurt(heroes, dame);
            GameController.Instance.addParticleDefault(collision.gameObject, heroes.Type);
            StartCoroutine(enemyHurtByStay(collision.gameObject));
        }
        else if (collision.gameObject.tag == "Boss" && isRoot)
        {
            isRoot = false;
            GameObject grass_2 = EasyObjectPool.instance.GetObjectFromPool("Particle_Grass_2", transform.position, transform.rotation);
            grass_2.GetComponent<ParticleSystem>().Play();
            StartCoroutine(hurtEnemyAround());
        }
    }

    IEnumerator deactiveAfterRoot()
    {
        yield return new WaitForSeconds(4.1f);
        GameObject par = EasyObjectPool.instance.GetObjectFromPool("Particle_Grass_3", transform.position, transform.rotation);
        par.GetComponent<ParticleSystem>().Play();
        EasyObjectPool.instance.ReturnObjectToPool(gameObject);
        EasyObjectPool.instance.ReturnObjectToPool(grass_2);
        grass_2.SetActive(false);
        gameObject.SetActive(false);
    }

    IEnumerator enemyHurtByStay(GameObject enemy)
    {
        count++;
        yield return new WaitForSeconds(1f);
        if(enemy.tag == "Enemy" && enemy.GetComponent<MonsterController>().getIsDead() == false && count < 4) 
        {
            enemy.GetComponent<MonsterController>().enemyHurt(heroes, dame);
            GameController.Instance.addParticleDefault(enemy, heroes.Type);
            StartCoroutine(enemyHurtByStay(enemy));
        }
    }

    IEnumerator hurtEnemyAround()
    {
        count++;
        yield return new WaitForSeconds(1f);
        EasyObjectPool.instance.getAllObjectInPosition(gameObject, 0.5f, heroes, dame);
        StartCoroutine(hurtEnemyAround());
        if (count > 4)
        {
            GameObject par = EasyObjectPool.instance.GetObjectFromPool("Particle_Grass_3", transform.position, transform.rotation);
            par.GetComponent<ParticleSystem>().Play();
            StopAllCoroutines();
            EasyObjectPool.instance.ReturnObjectToPool(gameObject);
            if (grass_2.gameObject != null)
            {
                EasyObjectPool.instance.ReturnObjectToPool(grass_2);
            }
            grass_2.SetActive(false);
            gameObject.SetActive(false);
        }
    }

}
