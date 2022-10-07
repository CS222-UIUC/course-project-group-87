using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseInteract : MonoBehaviour
{
    private Camera m;
    
    // Start is called before the first frame update
    void Start()
    {
        m = Camera.main;
        Debug.Log(m.name);

    }

    void NetworkEnable()
    {
        m = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 loc = m.transform.position;
        transform.LookAt(loc);
    }
}
