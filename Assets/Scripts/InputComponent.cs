using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputComponent : MonoBehaviour
{
    public GameObject pauseObject;
    private bool pause = false;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            pause = !pause;
            pauseObject.SetActive(pause);
            DayTime.Instance().togglePause();
            SimulationManager.Instance().togglePause();
        }
    }
}
