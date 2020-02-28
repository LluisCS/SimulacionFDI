using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeUI : MonoBehaviour
{
    private Text day, hour;
    void Start()
    {
        day = transform.Find("Day").GetComponent<Text>();
        hour = transform.Find("Hour").GetComponent<Text>();
        if (day == null || hour == null) {
            Debug.LogError("TimeUI gameobject missing text objects");
            Destroy(this);
        }
    }

    void Update()
    {
        day.text = DayTime.Instance().WeekDay().ToString();
        hour.text = DayTime.Instance().Hour().ToString("D2") + ":" + DayTime.Instance().Minute().ToString("D2");
    }
}
