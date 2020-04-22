using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputComponent : MonoBehaviour
{
    public GameObject pauseObject;
    public FireAlarm fireAlarm;
    public AgentUI agUI;
    private FreeCamera freeCam;
    private FollowCamera followCam;
    private bool pause = false, lockCam = false, freeCursor = true;
    
    void Start()
    {
        freeCam = Camera.main.transform.GetComponent<FreeCamera>();
        followCam = Camera.main.transform.GetComponent<FollowCamera>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            pause = !pause;
            pauseObject.SetActive(pause);
            DayTime.Instance().togglePause();
            SimulationManager.Instance().togglePause();
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            freeCursor = !freeCursor;
            if (freeCursor)
            {
                Cursor.lockState = CursorLockMode.None;
                freeCam.active = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                freeCam.active = !lockCam;
            }
        }
        else if (Input.GetButton("Fire1"))
        {
            Agent ag;
            lockCam = followCam.select(out ag);
            if (lockCam)
            {
                freeCam.active = false;

            }
            agUI.updateUI(ag);
        }
        else if (Input.GetButton("Fire2"))
        {
            followCam.deselect();
            if (Cursor.lockState == CursorLockMode.Locked)
                freeCam.active = true;
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            followCam.changeTargetSimulation(simulation.virus);
        }
        else if (Input.GetKeyDown(KeyCode.O))
        {
            if (fireAlarm != null)
                fireAlarm.toggleAllActive();
        }
    }
        
}
