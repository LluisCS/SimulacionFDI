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
    public GameObject cameraPositions;
    private bool pause = false, freeCursor = true;
    private int positionIndex = 0;
    
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
            if (optionObject.activeSelf)
                toggleOptionsMenu();
            else
            {
                freeCursor = !freeCursor;
                UpdateCursor();
            }
        }
        else if (Input.GetButtonDown("Fire1"))
        {
            Agent ag;
            if (!freeCursor && followCam.Select(out ag))
            {
                freeCam.active = false;
                agUI.updateUI(ag);
            }
            
        }
        else if (Input.GetButtonDown("Fire2"))
        {
            followCam.Deselect();
            agUI.updateUI(null);
            if (!freeCursor)
                freeCam.active = true;
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
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            if (!freeCam.active)
                return;
            positionIndex++;
            if (positionIndex >= cameraPositions.transform.childCount)
                positionIndex = 0;
            Camera.main.transform.position = cameraPositions.transform.GetChild(positionIndex).transform.position;
            Camera.main.transform.rotation = cameraPositions.transform.GetChild(positionIndex).rotation;
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            if (!freeCam.active)
                return;
            positionIndex--;
            if (positionIndex < 0)
                positionIndex = cameraPositions.transform.childCount - 1;
            Camera.main.transform.position = cameraPositions.transform.GetChild(positionIndex).transform.position;
            Camera.main.transform.rotation = cameraPositions.transform.GetChild(positionIndex).rotation;
        }
    }
    
    public void toggleOptionsMenu()
    {

        if (optionObject == null)
            return;
        optionObject.SetActive(!optionObject.activeSelf);
        freeCursor = optionObject.activeSelf;
        UpdateCursor();
    }
    private void UpdateCursor()
    {
        if (freeCursor)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            freeCam.active = false;
            followCam.Deselect();
            //agUI.updateUI(null);
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            freeCam.active = true;
        }
    }
}
