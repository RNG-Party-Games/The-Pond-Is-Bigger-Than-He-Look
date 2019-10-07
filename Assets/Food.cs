using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    public float energyValue;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Delete()
    {
        Destroy(this.gameObject);
    }
    
    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "FoodSphere")
        {
            other.GetComponent<FoodSphere>().Eat(energyValue);
            GetComponent<Animator>().Play("Eat");
        }

    }
}
