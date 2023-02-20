using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MarchingBytes;

public class ExplosionController : MonoBehaviour
{
    private MyHeroes data;
    public int id;
    private int skillDame;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void initData(MyHeroes d, int dame)
    {
        skillDame = dame;
        data = d;
    }


    IEnumerator returnToPool(float time)
    {
        yield return new WaitForSeconds(time);
        EasyObjectPool.instance.ReturnObjectToPool(gameObject);
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "Boss")
        {
            if (collision.gameObject.tag == "Enemy")
            {
                collision.gameObject.GetComponent<MonsterController>().enemyHurt(data, skillDame);
            }
            else
            {
                collision.gameObject.GetComponent<BossController>().enemyHurt(data, skillDame);
            }
            if (id != 3)
            {
                GameController.Instance.addParticle(collision.gameObject, 4);
            }
        }
    }

    void OnEnable()
    {
        //StartCoroutine(returnToPool(0.3f));
    }


}
