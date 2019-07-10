using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public float value;

    public Vector3 orPosition;

    public bool inBlock;


    void Start()
    {
        orPosition = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(value);
    }
}
