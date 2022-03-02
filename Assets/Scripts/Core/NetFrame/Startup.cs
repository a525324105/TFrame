using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Startup : MonoBehaviour
{

    public GameObject p;

    public static GameObject GameIns;

    void Start()
    {
        if (null == GameIns)
        {
            GameIns = GameObject.Instantiate(p);
        }
        GameIns.GetComponent<Game>().Reset();
    }
}
