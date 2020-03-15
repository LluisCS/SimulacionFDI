using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AgentUI : MonoBehaviour
{
    private Text buttonText, agentText;
    private Button button;
    private Transform desplegable;
    private Agent ag;
    private bool visible = false;
    void Start()
    {
        buttonText = transform.Find("Button").Find("Text").GetComponent<Text>();
        button = transform.Find("Button").GetComponent<Button>();
        desplegable = transform.Find("Desplegable");
        agentText = desplegable.GetComponentInChildren<Text>();
        if (buttonText == null || button == null || desplegable == null || agentText == null)
        {
            Debug.LogError("SubjectUI gameobject not correctly setup");
            Destroy(this);
        }
        else
        {
            button.onClick.AddListener(ButtonClick);
            buttonText.text = "SHOW";
            agentText.text = "Nothing selected.";
            desplegable.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (ag == null)
        {
            agentText.text = "Nothing selected.";
            return;
        }

        agentText.text = "Name: " + ag.name + "\nType: " + ag.state.type
            + "\nAction: " + ag.state.action
            + "\nPersonality: " + ag.state.per
            + "\nSimulation: " + ag.state.sim
            + "\nCurrent Subject: " + ag.currentSubject;
    }

    void ButtonClick()
    {
        visible = !visible;
        desplegable.gameObject.SetActive(visible);
        if (visible)
            buttonText.text = "HIDE";
        else
            buttonText.text = "SHOW";
    }

    public void updateUI(Agent agent)
    {
        ag = agent;
        if (agent == null)
        {
            agentText.text = "Nothing selected.";

        }
        else
        {
            agentText.text = "Name: " + agent.name + "\nType: " + agent.state.type
                + "\nAction: " + agent.state.action
                + "\nPersonality: " + agent.state.per
                + "\nSimulation: " + agent.state.sim
                + "\nCurrent Subject: " + agent.currentSubject;
        }
        agentText.enabled = false;
        agentText.enabled = true;

    }
}
