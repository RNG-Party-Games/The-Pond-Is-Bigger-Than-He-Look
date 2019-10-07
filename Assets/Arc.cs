using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arc : MonoBehaviour
{
    // Start is called before the first frame update
    LineRenderer lr;
    Vector3 velocity;
    int line_resolution = 20;
    void Start()
    {
        lr = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Calculate(float jump_velocity)
    {

        velocity = SkateController.instance.GetVelocity();
        float airtime = -(2 * jump_velocity) / Physics.gravity.y;
        airtime += 0.2f;
        Vector3[] positions = new Vector3[line_resolution];
        float time_passed = 0;
        for (int i = 0; i < line_resolution; ++i)
        {
            time_passed = i * (airtime / (float)line_resolution);
            float x = SkateController.instance.transform.position.x + velocity.x * time_passed;
            float y = SkateController.instance.transform.position.y + jump_velocity * time_passed + (0.5f * Physics.gravity.y) * time_passed * time_passed;
            Debug.Log("time passed = " + time_passed);
            Debug.Log(SkateController.instance.transform.position.y + " + " + jump_velocity * time_passed + " + " + (0.5f * Physics.gravity.y) * time_passed * time_passed + " = " + y);
            float z = SkateController.instance.transform.position.z + velocity.z * time_passed;
            positions[i] = new Vector3(x, y, z);
        }
        lr.positionCount = positions.Length;
        lr.SetPositions(positions);
    }
    public void Hide()
    {
        lr.positionCount = 0;
    }
}