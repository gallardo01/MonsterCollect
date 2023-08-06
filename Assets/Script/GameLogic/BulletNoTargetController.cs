using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MarchingBytes;
// using VoxelBusters.EssentialKit.Editor.Android;

public class BulletNoTargetController : MonoBehaviour
{
    private MyHeroes heroes;
    [SerializeField] int id;
    private Transform target;
    private int skillDame;
    float timer = 0f;
    // Start is called before the first frame update

    private void Start()
    {

    }

    private void Update()
    {
    }

    public void initBullet(MyHeroes myHeroes, int skill, int dame, Transform enemy)
    {
        timer = Time.fixedTime;
        skillDame = dame;
        target = enemy;
        heroes = myHeroes;
    }
    public void initBullet(MyHeroes myHeroes, int dame)
    {
        heroes = myHeroes;
        skillDame = dame;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            EasyObjectPool.instance.ReturnObjectToPool(gameObject);
            gameObject.SetActive(false);
        }
        if ((collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "Boss") && id == 3)
        {
            GameController.Instance.addExplosion(heroes, gameObject, skillDame, 5);
            EasyObjectPool.instance.ReturnObjectToPool(gameObject);
            gameObject.SetActive(false);
        }
        else if ((collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "Boss") && id == 6)
        {
            timer = Time.fixedTime - timer;
            float multiple = 5 + skillDame * timer;
            skillDame = (int)multiple;
            GameController.Instance.addExplosionText(heroes, gameObject, skillDame, "Particle_Fire_3");
            EasyObjectPool.instance.ReturnObjectToPool(gameObject);
            gameObject.SetActive(false);
        }
        else if ((collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "Boss") && id == 7)
        {
            if (collision.gameObject.tag == "Enemy")
            {
                collision.gameObject.GetComponent<MonsterController>().enemyHurt(heroes, skillDame);
                GameController.Instance.addParticleDefault(collision.gameObject, heroes.Type);
            }
            else
            {
                collision.gameObject.GetComponent<BossController>().enemyHurt(heroes, skillDame);
                GameController.Instance.addParticleDefault(collision.gameObject, heroes.Type);
            }
        }
        else if ((collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "Boss") && id == 8)
        {
            if (collision.gameObject.tag == "Enemy")
            {
                collision.gameObject.GetComponent<MonsterController>().enemyHurt(heroes, skillDame);
                GameController.Instance.addParticleDefault(collision.gameObject, heroes.Type);
            }
            else
            {
                collision.gameObject.GetComponent<BossController>().enemyHurt(heroes, skillDame);
                GameController.Instance.addParticleDefault(collision.gameObject, heroes.Type);
            }
            EasyObjectPool.instance.ReturnObjectToPool(gameObject);
            gameObject.SetActive(false);
        }
    }
}