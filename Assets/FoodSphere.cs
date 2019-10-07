using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSphere : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Eat(float energy)
    {
        SkateController.instance.Eat(energy);
    }
}
