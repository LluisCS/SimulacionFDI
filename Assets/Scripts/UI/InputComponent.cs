using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputComponent : MonoBehaviour
{
    public GameObject pauseObject, optionObject;
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
            SimulationManager.Instance().TogglePause();
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
            lockCam = followCam.Select(out ag);
            if (lockCam)
            {
                freeCam.active = false;
                agUI.updateUI(ag);
            }
            
        }
        else if (Input.GetButton("Fire2"))
        {
            followCam.Deselect();
            if (Cursor.lockState == CursorLockMode.Locked)
                freeCam.active = true;
            agUI.updateUI(null);
        }
        else if (Input.GetKeyDown(KeyCode.U))
        {
            SimulationManager.Instance().infectionInfo.toggleProtocol();
        }
        else if (Input.GetKeyDown(KeyCode.I))
        {
            LogSystem.Instance().Log("Disease infection started");
            SimulationManager.Instance().feedbackUI.ShowFeedback("Disease infection started");
            if (!followCam.ChangeTargetSimulation(simulation.infection))
                SimulationManager.Instance().startSimulationEvent(simulation.infection);            
        }
        else if (Input.GetKeyDown(KeyCode.O))
        {
            LogSystem.Instance().Log("Zombie infection started");
            SimulationManager.Instance().feedbackUI.ShowFeedback("Zombie infection started");
            if (!followCam.ChangeTargetSimulation(simulation.zombie))
                SimulationManager.Instance().startSimulationEvent(simulation.zombie);
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            SimulationManager.Instance().feedbackUI.ShowFeedback("Fire started");
            LogSystem.Instance().Log("Fire started");
            if (fireAlarm != null)
                fireAlarm.ToggleAllActive();
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            toggleOptionsMenu();
        }
    }
    
    public void toggleOptionsMenu()
    {
        if(optionObject != null)
            optionObject.SetActive(!optionObject.activeSelf);
    }
}
