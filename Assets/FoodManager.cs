using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodManager : MonoBehaviour
{
    // Start is called before the first frame update
    public List<Food> food;
    public static FoodManager instance;
    void Start()
    {
        if(instance == null)
        {
            instance = this;
        }
        foreach(Transform child in transform)
        {
            food.Add(child.GetComponent<Food>());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Reset()
    {
        foreach(Food f in food)
        {
            f.gameObject.SetActive(true);
        }
    }
}
