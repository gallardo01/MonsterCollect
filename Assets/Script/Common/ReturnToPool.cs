using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MarchingBytes;

public class ReturnToPool : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        StartCoroutine(returnToPool());
    }

    IEnumerator returnToPool()
    {
        yield return new WaitForSeconds(1f);
        EasyObjectPool.instance.ReturnObjectToPool(gameObject);
        gameObject.SetActive(false);
    }
}
