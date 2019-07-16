using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject triggerHolder;
    public GameObject winCanvas;

    void Update()
    {
        check();
    }

    public bool check()
    {
        foreach (Transform child in triggerHolder.GetComponentsInChildren<Transform>())
        {
			Trigger trigger = child.GetComponent<Trigger>();
            if(trigger.correct == false)
            {
                Debug.Log("All false");
                return false;
            }
        }
        winCanvas.SetActive(true);
        Debug.Log("All true");
        return true;
    }
}
