using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class InflatePricingText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI pricing;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void updatePricing(string text)
    {
        pricing.text = text;
    }
}
