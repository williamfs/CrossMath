using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    public float valueLookingFor;
    public bool correct;

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.GetComponent<Block>() != null)
        {
            other.GetComponent<Block>().inBlock = true;
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if(other.GetComponent<Block>() != null)
        {
            Debug.Log("in");
            if (other.GetComponent<Drag>().draged == false)
            {
                Debug.Log("Mouse up");
                other.transform.position = this.transform.position;
                if(other.GetComponent<Block>().value == valueLookingFor)
                {
                    other.GetComponent<SpriteRenderer>().color = Color.green;
                    correct = true;
                }
                else
                {
                    other.GetComponent<SpriteRenderer>().color = Color.red;
                    correct = false;
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.GetComponent<Block>() != null)
        {
            other.GetComponent<SpriteRenderer>().color = Color.white;
            other.GetComponent<Block>().inBlock = false;
        }
    }
}
