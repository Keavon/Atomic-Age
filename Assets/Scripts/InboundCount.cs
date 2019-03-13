using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InboundCount : MonoBehaviour
{

    private int inboundCount = 0;

    void Update()
    {
        Text count = this.GetComponent<Text>();
        inboundCount++;
        count.text = "Inbound: " + ((int) Mathf.Sqrt(inboundCount) / 4).ToString();
    }
}
