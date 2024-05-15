using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toggler : MonoBehaviour
{
    public GameObject obj;

    public void Toggle()
    {
        obj.SetActive(!obj.activeInHierarchy);
    }
}
