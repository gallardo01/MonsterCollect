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
    private int skillDame;
    private int temp_circle = 0;
    private float speed = 3f;
    private int grass = 0;
    private int temp = 3;
    private void Start()
    {
        speed = 3f * Application.targetFrameRate / 60f;
    }

    void Update()
    {
        transform.position = target.position + new Vector3(speed * Mathf.Cos(Mathf.PI * ((Time.fixedTime)%3)/1.5f + Mathf.PI * temp_circle / temp), speed * Mathf.Sin(Mathf.PI * ((Time.fixedTime) % 3) / 1.5f + Mathf.PI * temp_circle / temp), 0);
    }

    public void initBullet(MyHeroes myHeroes, int skill, int dame, Transform enemy)
    {
        if (myHeroes.Type == 4)
        {
            temp = 4;
            grass = 1;
            speed = 4f;
            StartCoroutine(deactiveSelf());
        }
        temp_circle = skill;
        skillDame = dame;
        target = enemy;
        heroes = myHeroes;
    }

    IEnumerator deactiveSelf()
    {
        yield return new WaitForSeconds(3.5f);
        EasyObjectPool.instance.ReturnObjectToPool(gameObject);
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "Boss")
        {
            if (collision.gameObject.tag == "Enemy")
            {
                collision.gameObject.GetComponent<MonsterController>().enemyHurt(heroes, skillDame);
            }
            else
            {
                collision.gameObject.GetComponent<BossController>().enemyHurt(heroes, skillDame);
            }

            GameController.Instance.addParticleDefault(collision.gameObject, heroes.Type);
            if (grass == 1)
            {
                StopAllCoroutines();
                EasyObjectPool.instance.ReturnObjectToPool(gameObject);
                gameObject.SetActive(false);
            }
        }
    }

}
