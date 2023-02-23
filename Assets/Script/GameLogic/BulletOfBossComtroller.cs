using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MarchingBytes;


public class BulletOfBossComtroller : MonoBehaviour
{

    private float speed = 5f;
    private Transform target;
    private Vector3 direction;

    
    // Start is called before the first frame update
  

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            transform.position += direction * speed *Time.deltaTime;
        }
    }

    public void initBullet(Transform Target, Vector3 Direction)
    {
        target = Target;

        direction = Direction;
        //direction = Vector3.Normalize(target.position -Source.position);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            GameController.Instance.addParticle(collision.gameObject, 1);
            EasyObjectPool.instance.ReturnObjectToPool(gameObject);
            gameObject.SetActive(false);
        }

        if (collision.gameObject.tag == "Player")
        {
            GameController.Instance.addParticle(collision.gameObject, 1);
            EasyObjectPool.instance.ReturnObjectToPool(gameObject);
            gameObject.SetActive(false);
        }
    }


    IEnumerator returnToPool(float time)
    {
        yield return new WaitForSeconds(time);
        EasyObjectPool.instance.ReturnObjectToPool(gameObject);
        gameObject.SetActive(false);
    }

    
}
