using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MarchingBytes;

public class FloatingText : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] TextMeshPro text;

    private void Awake()
    {
        text.gameObject.transform.position = new Vector3(Random.Range(0.2f, 0.5f), Random.Range(0.2f, 0.5f), 0f);
    }

    public void healPlayer(int amount)
    {
        text.text = "+" + amount.ToString();
        text.fontSize = 5;
        text.color = Color.green;
        StartCoroutine(disableFloating());
    }

    public void playerHealth(int dame)
    {
        text.text = dame.ToString();
        text.fontSize = 4;
        text.color = Color.red;
        StartCoroutine(disableFloating());
    }

    public void critHealth()
    {
        text.fontSize = 5;
        text.color = Color.yellow;
    }
    public void disableObject(int dame, int type)
    {
        int actualDame = Mathf.Abs(dame);
        if (dame > 0)
        {
            text.fontSize = 5;
            text.color = Color.yellow;
        }
        else
        {
            text.fontSize = 3;
            text.color = Color.white;
        }
        text.text = actualDame.ToString();
        StartCoroutine(disableFloating());
    }

    public void showGold(int gold)
    {
        text.text = "+" + gold.ToString();
        text.color = Color.yellow;
    }
    private IEnumerator disableFloating()
    {
        yield return new WaitForSeconds(1f);
        EasyObjectPool.instance.ReturnObjectToPool(gameObject);
        gameObject.SetActive(false);
    }
}
