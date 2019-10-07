using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomSFX : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetInvoke(string method, float time) {
        Invoke(method, time);
    }

    public void Kill() {
        Destroy(gameObject);
    }
}
