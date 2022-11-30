using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MarchingBytes;

public class ExplosionController : MonoBehaviour
{
    private MyHeroes data;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void initData(MyHeroes d)
    {
        data = d;
    }


    IEnumerator returnToPool(float time)
    {
        yield return new WaitForSeconds(time);
        EasyObjectPool.instance.ReturnObjectToPool(gameObject);
        gameObject.SetActive(false);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.GetComponent<MonsterController>().enemyHurt(data);
            GameController.Instance.addParticle(collision.gameObject, 4);
            EasyObjectPool.instance.ReturnObjectToPool(gameObject);
            gameObject.SetActive(false);
        }
    }

}
