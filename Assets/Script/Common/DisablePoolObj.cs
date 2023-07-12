using MarchingBytes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisablePoolObj : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void OnEnable()
    {
        StartCoroutine(disableParticle());
    }

    IEnumerator disableParticle()
    {
        yield return new WaitForSeconds(1f);
        EasyObjectPool.instance.ReturnObjectToPool(gameObject);
        gameObject.SetActive(false);
    }

}
