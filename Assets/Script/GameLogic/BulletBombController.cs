using MarchingBytes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBombController : MonoBehaviour
{
    [SerializeField] GameObject bomb1;
    [SerializeField] GameObject bomb2;
    [SerializeField] GameObject bomb3;
    [SerializeField] GameObject VFX;
    private MyHeroes heroes;
    private int dame;
    private bool isTouching = true;
    private int touch = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void initBullet(MyHeroes myHeroes, int dame)
    {
        touch = 0;
        isTouching = true;
        bomb1.SetActive(touch == 0);
        bomb2.SetActive(touch == 1);
        bomb3.SetActive(touch == 2);
        VFX.GetComponent<ParticleSystem>().Play();
        this.dame = dame;
        heroes = myHeroes;
    }

    private void explosion(MyHeroes data, string particle)
    {
        GameController.Instance.addExplosionText(data, gameObject, dame, particle);
    }

    IEnumerator changeStatusTouching()
    {
        VFX.GetComponent<ParticleSystem>().Play();
        isTouching = false;
        bomb1.SetActive(touch == 0);
        bomb2.SetActive(touch == 1);
        bomb3.SetActive(touch == 2);
        if (touch == 3)
        {
            explosion(heroes, "Particle_Fire_3");
            EasyObjectPool.instance.ReturnObjectToPool(gameObject);
            gameObject.SetActive(false);
        }
        yield return new WaitForSeconds(0.5f);
        isTouching = true;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "Boss") && isTouching)
        {
            if (collision.gameObject.tag == "Enemy")
            {
                touch++;
                StartCoroutine(changeStatusTouching());
            }
            else
            {
                touch = 2;
                StartCoroutine(changeStatusTouching());
            }
        }
    }

}
