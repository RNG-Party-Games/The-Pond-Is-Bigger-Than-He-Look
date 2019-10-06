using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkateController : MonoBehaviour
{
    public Transform target;
    public EnergyScript bar;
    public List<TrailRenderer> trails;
    Quaternion originalRotation;
    Rigidbody rb;
    float thrust = 80.0f;
    float energy = 100.0f;
    bool jumpActive = true;
    // Start is called before the first frame update
    void Start()
    {
        originalRotation = transform.rotation;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckRotation();
        CheckInput();
        bar.SetEnergy(energy / 100.0f);
    }

    public void CheckRotation()
    {
        int layerMask = 1 << 9;
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            Debug.Log("Layer 9");
            target.position = hit.point;
            transform.LookAt(target);
        }
        transform.eulerAngles = new Vector3(originalRotation.eulerAngles.x, transform.rotation.eulerAngles.y, originalRotation.eulerAngles.z);
    }

    public void CheckInput()
    {
        if (energy > 0)
        {
            if (jumpActive)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    rb.AddForce(transform.forward * thrust, ForceMode.Impulse);
                    GetComponent<Animator>().Play("Skate");
                    energy -= 10;
                }

                if (Input.GetButtonDown("Jump"))
                {
                    Jump();
                }
            }
        }
    }

    public void Jump()
    {
        jumpActive = false;
        rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y + 40, rb.velocity.z);
        GetComponent<Animator>().Play("Jump");
        energy -= 10;
        SetTrails(false);
    }

    public void SetTrails(bool b)
    {
        foreach(TrailRenderer t in trails)
        {
            t.enabled = b;
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if(!jumpActive && collision.collider.tag == "Water")
        {
            jumpActive = true;
            SetTrails(true);
        }
    }
}
