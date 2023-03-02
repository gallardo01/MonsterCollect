using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Equipment : MonoBehaviour
{
    [SerializeField] GameObject item;
    [SerializeField] Image container;
    [SerializeField] Image image;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] Sprite defaultImage;

    public void InitData(ItemInventory item, Sprite sprite)
    {
        if (item.Type == 0) return;
        levelText.text = "Lv." + item.Level.ToString();
        image.sprite = sprite;
    }   
    
    public void SwitchToDefaultSprite()
    {
        image.sprite = defaultImage;        
    }
}
