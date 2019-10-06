using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    public Transform toFollow;
    Vector3 diff;
    // Start is called before the first frame update
    void Start()
    {
        diff = transform.position - toFollow.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = toFollow.position + diff;
    }
}
