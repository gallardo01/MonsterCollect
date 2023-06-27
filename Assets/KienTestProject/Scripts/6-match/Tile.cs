using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Transform[] skillType;
    public TextMeshPro life;

    public int posX { get; set; }
    public int posY { get; set; }
    public int type { get; set; }


    // Start is called before the first frame update
    void Start()
    {
        skillType[type].gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
