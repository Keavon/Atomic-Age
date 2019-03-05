using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiquidControl : MonoBehaviour {

    public static LiquidControl instance;

    public GameObject liquidPrefab;

    public int partitions = 50;
    public float maxDistanceToGround = 0.4f;
    public float liquidHeightOffGround = 0.03f;
    public float liquidDepthBelowGround = 0.06f;
    public float maxLedgePaint = 0.4f;
    public float minLedgeHeight = 0.1f;
    public float maxWidth = 2f;

    private GameObject currentLiquid;
    private List<GameObject> liquids = new List<GameObject>();


    public void Create(Vector3 position)
    {
        GameObject liquid = Instantiate(liquidPrefab, new Vector3(0, 0, -1), Quaternion.identity);
        liquid.GetComponent<LiquidBehavior>().Create(position);
        currentLiquid = liquid;
        liquids.Add(liquid);
    }

    public void Redraw(Vector3 position)
    {
        currentLiquid.GetComponent<LiquidBehavior>().UpdateMesh(position);
    }

}
