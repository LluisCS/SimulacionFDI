using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class FeedbackUI : MonoBehaviour
{
    private double timer = 0;
    private bool active = false;
    public double disableTime = 2.0;
    private Text textUI;

    void Start()
    {
        textUI = GetComponent<Text>();
        transform.parent.gameObject.SetActive(false);
    }

    public void ShowFeedback(string text)
    {
        transform.parent.gameObject.SetActive(true);
        active = true;
        timer = Time.time + disableTime;
        textUI.text = text.ToUpper();
    }

    void Update()
    {
        if (active && timer < Time.time)
        {
            transform.parent.gameObject.SetActive(false);
            active = false;
        }
    }
}
