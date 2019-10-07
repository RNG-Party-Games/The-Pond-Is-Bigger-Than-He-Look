using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Scorecard : MonoBehaviour
{
    // Start is called before the first frame update
    public static Scorecard instance;
    public Image card;
    public TextMeshProUGUI[] dartText;
    public TextMeshProUGUI[] jumpText;
    public TextMeshProUGUI totalText;
    int[] darts, jumps;
    void Start()
    {
        if(instance == null)
        {
            instance = this;
        }
        darts = new int[5];
        jumps = new int[5];
        UpdateText();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Cancel"))
        {
            card.gameObject.SetActive(!card.gameObject.activeInHierarchy);
        }
    }

    public void SetDarts(int index, int number)
    {
        darts[index] = number;
        UpdateText();
    }
    public void SetJumps(int index, int number)
    {
        jumps[index] = number;
        UpdateText();
    }

    public void UpdateText()
    {
        int total = 0;
        for(int i = 0; i < 5; ++i)
        {
            dartText[i].text = "" + darts[i];
            jumpText[i].text = "" + jumps[i];
            total += darts[i] + jumps[i];
            if (darts[i] == 0)
            {
                dartText[i].text = "";
            }
            if (jumps[i] == 0)
            {
                jumpText[i].text = "";
            }
        }
        totalText.text = "Total: " + total;
    }

    public void Show()
    {
        card.gameObject.SetActive(true);
    }

    public void Hide()
    {
        card.gameObject.SetActive(false);
    }

    public bool GetState()
    {
        return card.gameObject.activeInHierarchy;
    }
}
