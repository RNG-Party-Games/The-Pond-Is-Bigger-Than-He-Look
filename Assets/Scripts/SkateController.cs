using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkateController : MonoBehaviour
{
    public Transform target;
    public EnergyScript bar;
    public List<TrailRenderer> trails;
    public Fish fish;
    public static SkateController instance;
    Quaternion originalRotation;
    Rigidbody rb;
    Animator anim;
    float thrust = 100.0f, rough = 0.5f;
    float energy = 100.0f;
    int jumps, glides;
    const float max_energy = 100.0f;
    bool jumpActive = true, beingEaten;
    Current currentcurrent;
    Vector3 lastSafePlace;
    public List<Collider> waterObjects;
    // Start is called before the first frame update
    void Start()
    {
        if(instance == null)
        {
            instance = this;
        }
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        lastSafePlace = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        CheckFish();
        bar.SetEnergy(energy / 100.0f);
    }

    private void LateUpdate()
    {
        CheckRotation();
        CheckInput();
        CheckPhysics();
    }

    public void CheckRotation()
    {
        int layerMask = 1 << 9;
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        originalRotation = transform.rotation;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            target.position = hit.point;
            Vector3 targetVector = target.position - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(targetVector);
            if (Mathf.Abs(transform.rotation.eulerAngles.y - targetRotation.eulerAngles.y) > 1)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 10);
            }
            //transform.LookAt(target);
        }
        transform.eulerAngles = new Vector3(originalRotation.eulerAngles.x, transform.rotation.eulerAngles.y, originalRotation.eulerAngles.z);
    }

    public void CheckInput()
    {
        if (energy > 0)
        {
            if (jumpActive && !beingEaten)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    ++glides;
                    Debug.Log("Gliding with " + waterObjects.Count + " water objects.");
                    if(waterObjects.Count == 0)
                    {
                        rb.AddForce(transform.forward * thrust * rough, ForceMode.Impulse);
                    }
                    else
                    {
                        rb.AddForce(transform.forward * thrust, ForceMode.Impulse);
                    }
                    anim.Play("Skate");
                    energy -= 3;
                }

                if (Input.GetButtonDown("Jump"))
                {
                    Jump();
                }
            }
        }
    }

    public void CheckPhysics()
    {
        //if(groundObjects.Count > 0 && rb.velocity.magnitude < 1)
        //{
        //    transform.position = lastSafePlace;
        //    rb.velocity = Vector3.zero;
        //}
        if(currentcurrent != null)
        {
            rb.AddForce(currentcurrent.transform.forward * 160);
        }
    }

    public void Jump()
    {
        ++jumps;
        jumpActive = false;
        rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y + 80, rb.velocity.z);
        anim.Play("Jump");
        energy -= 5;
        SetTrails(false);
    }

    public void CheckFish()
    {
        Animator fishAnim = fish.GetComponent<Animator>();
        if (jumpActive && rb.velocity.magnitude < 5 && fishAnim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            fishAnim.Play("FishAppear");
        }
        else if (fishAnim.GetCurrentAnimatorStateInfo(0).IsName("FishAppear"))
        {
            if (!jumpActive || rb.velocity.magnitude > 5)
            {
                fishAnim.Play("FishGone");
            }
        }
    }

    public void Eat(float energyValue)
    {
        energy += energyValue;
        if(energy > max_energy)
        {
            energy = max_energy;
        }
    }

    public void SetTrails(bool b)
    {
        foreach(TrailRenderer t in trails)
        {
            t.enabled = b;
        }
    }

    public void Spit()
    {
        beingEaten = true;
        anim.Play("Eaten");
    }

    public void FinishSpit()
    {
        fish.GetComponent<Animator>().Play("FishGone");
        float x = Random.Range(-100, 100);
        float y = Random.Range(50, 100);
        float z = Random.Range(-100, 0);
        rb.velocity = new Vector3(rb.velocity.x + x, rb.velocity.y + y, rb.velocity.z + z);
        beingEaten = false;
        jumpActive = false;
    }

    public void OnCollisionEnter(Collision collision)
    {
        if(!jumpActive && (collision.collider.tag == "Water" || collision.collider.tag == "Ground"))
        {
            jumpActive = true;
            SetTrails(true);
            lastSafePlace = transform.position;
        }
        if(collision.collider.tag == "Water")
        {
            waterObjects.Add(collision.collider);
        }
    }

    public void OnCollisionExit(Collision collision)
    {
        if (collision.collider.tag == "Water")
        {
            waterObjects.Remove(collision.collider);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.tag == "WaterCurrent")
        {
            Debug.Log("CurrentEnter");
            currentcurrent = other.GetComponent<Current>();
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if(other.tag == "WaterCurrent")
        {
            Debug.Log("CurrentExit");
            currentcurrent = null;
        }
    }
}
