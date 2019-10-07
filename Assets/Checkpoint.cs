using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    // Start is called before the first frame update
    public int index;
    public Transform resetPoint;
    int darts = 0, jumps = 0;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int GetIndex()
    {
        return index;
    }

    public Transform GetResetPoint()
    {
        return resetPoint;
    }

    public void Reset()
    {
        UpdateScorecard();
    }

    public int GetDarts()
    {
        return darts;
    }

    public int GetJumps()
    {
        return jumps;
    }

    public void Dart()
    {
        darts++;
        UpdateScorecard();
    }

    public void Jump()
    {
        jumps++;
        UpdateScorecard();
    }

    public void UpdateScorecard()
    {
        Scorecard.instance.SetDarts(index, darts);
        Scorecard.instance.SetJumps(index, jumps);
    }
}
