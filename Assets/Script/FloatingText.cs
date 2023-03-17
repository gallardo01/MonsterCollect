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
    public void disableObject(int dame, int type)
    {
        int actualDame = Mathf.Abs(dame);
        if (dame > 0)
        {
            text.fontSize = 4;
            text.text = "<sprite=0>" + actualDame.ToString();
        }
        else
        {
            text.fontSize = 3;
            text.text = actualDame.ToString();
        }
        if (type < 0)
        {
            text.color = new Color(195f/255f, 195f / 255f, 195f / 255f);
        }
        else if (type > 0)
        {
            text.color = new Color(245f / 255f, 231f / 255f, 31f / 255f);
        }
        else
        {
            text.color = Color.white;
        }
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
