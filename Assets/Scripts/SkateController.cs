using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkateController : MonoBehaviour
{
    public Transform target;
    public EnergyScript bar;
    public List<TrailRenderer> trails;
    public List<Checkpoint> checkpoints;
    public CustomSFX SFX;
    public AudioClip dartSFX, jumpSFX, eatBing, eatBug, fishAppear, noEnergy, scoreGoal;
    public AudioSource holdSpace;
    int checkpointIndex = 0;
    public Fish fish;
    public Arc arc;
    public static SkateController instance;
    Quaternion originalRotation;
    Rigidbody rb;
    Animator anim;
    float thrust = 100.0f, rough = 0.5f;
    float energy = 50.0f;
    float jumppower = 200.0f;
    int jumps, glides;
    const float max_energy = 100.0f;
    bool jumpActive = true, beingEaten;
    Current currentcurrent;
    Vector3 lastSafePlace;
    List<Collider> waterObjects;
    List<Collider> groundObjects;
    bool jumpheld = false, finished = false;
    float jumpstart, gamestart;
    public float jump_energy = 20.0f;
    public float dart_energy = 5.0f;

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
        waterObjects = new List<Collider>();
        groundObjects = new List<Collider>();
        gamestart = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        CheckFish();
        bar.SetEnergy(energy / 100.0f);
    }

    private void LateUpdate()
    {
        CheckPhysics();
        CheckRotation();
        CheckInput();
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
        if (energy >= 2)
        {
            if (jumpActive && !beingEaten && !Scorecard.instance.GetState())
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Dart();
                }

                if (jumpActive && Input.GetButtonDown("Jump"))
                {
                    jumpheld = true;
                    holdSpace.Play();
                    jumpstart = Time.time;
                }
                if (jumpheld && Input.GetButtonUp("Jump"))
                {
                    jumpheld = false;
                    holdSpace.Stop();
                    Jump();
                }
            }
        }
        else if(energy < 2 && rb.velocity.magnitude < 0.3)
        {
            Reset();
            PlaySFX(noEnergy, 1);
        }
        if(Input.GetButtonUp("Jump"))
        {
            jumpheld = false;
            holdSpace.Stop();
        }
        if(jumpheld)
        {
            float power = CalculateJumpPower();
            arc.Calculate(power);
        }
    }

    public void CheckPhysics()
    {
        if(groundObjects.Count == 0 && waterObjects.Count == 0)
        {
            jumpActive = false;
        }
        if (currentcurrent != null)
        {
            rb.AddForce(currentcurrent.transform.forward * 160);
        }
    }

    float CalculateJumpPower()
    {
        float timediff = Time.time - jumpstart;
        if (timediff > 1)
        {
            timediff = 1;
        }
        float effectivejump = timediff * jumppower;
        return effectivejump;
    }

    public void Reset()
    {
        rb.velocity = Vector3.zero;
        checkpoints[checkpointIndex].Reset();
        transform.position = checkpoints[checkpointIndex].GetResetPoint().position;
        energy = 50;
        FoodManager.instance.Reset();
    }

    public void Dart()
    {
        checkpoints[checkpointIndex].Dart();
        ++glides;
        if (waterObjects.Count == 0)
        {
            rb.AddForce(transform.forward * thrust * rough, ForceMode.Impulse);
        }
        else
        {
            rb.AddForce(transform.forward * thrust, ForceMode.Impulse);
        }
        anim.Play("Skate");
        energy -= dart_energy;
        PlaySFX(dartSFX, 1);
    }

    public void Jump()
    {
        checkpoints[checkpointIndex].Jump();
        ++jumps;
        jumpActive = false;
        rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y + CalculateJumpPower(), rb.velocity.z);
        anim.Play("Jump");
        energy -= jump_energy;
        SetTrails(false);
        PlaySFX(jumpSFX, 1);
    }

    public void CheckFish()
    {
        Animator fishAnim = fish.GetComponent<Animator>();
        if (Time.time - gamestart > 30 && waterObjects.Count > 0 && jumpActive && rb.velocity.magnitude < 5 && fishAnim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
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
        PlaySFX(eatBug, 1);
        PlaySFX(eatBing, 1);
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

    public Vector3 GetVelocity()
    {
        return rb.velocity;
    }

    public void Spit()
    {
        PlaySFX(fishAppear, 1);
        beingEaten = true;
        anim.Play("Eaten");
    }

    public void FinishSpit()
    {
        fish.GetComponent<Animator>().Play("FishGone");
        float x = Random.Range(50, 100);
        float y = Random.Range(50, 100);
        float z = Random.Range(-100, 100);
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
            arc.Hide();
        }
        if(collision.collider.tag == "Water")
        {
            waterObjects.Add(collision.collider);
        }
        if (collision.collider.tag == "Ground")
        {
            groundObjects.Add(collision.collider);
        }
    }

    public void OnCollisionExit(Collision collision)
    {
        if (collision.collider.tag == "Water")
        {
            waterObjects.Remove(collision.collider);
        }
        if (collision.collider.tag == "Ground")
        {
            groundObjects.Remove(collision.collider);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.tag == "WaterCurrent")
        {
            Debug.Log("CurrentEnter");
            currentcurrent = other.GetComponent<Current>();
        }
        if(other.tag == "Checkpoint")
        {
            if(other.GetComponent<Checkpoint>().GetIndex() > checkpointIndex)
            {
                checkpointIndex = other.GetComponent<Checkpoint>().GetIndex();
            }
        }
        if (other.tag == "EndState" && !finished)
        {
            finished = true;
            PlaySFX(scoreGoal, 1);
            Scorecard.instance.Show();
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

    public void PlaySFX(AudioClip clip, float volume)
    {
        CustomSFX sfx = Instantiate(SFX);
        sfx.GetComponent<AudioSource>().clip = clip;
        sfx.GetComponent<AudioSource>().volume = volume;
        sfx.GetComponent<AudioSource>().Play();
        sfx.SetInvoke("Kill", clip.length);
    }

    public void HardReset()
    {
        rb.velocity = Vector3.zero;
        checkpointIndex = 0;
        transform.position = checkpoints[checkpointIndex].GetResetPoint().position;
        for (int i = 0; i < checkpoints.Count; ++i)
        {
            checkpoints[i].Reset();
        }
        energy = 50;
        FoodManager.instance.Reset();
    }
}
