using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubjectUI : MonoBehaviour
{
    private Text buttonText;
    private Button button;
    private Transform desplegable;
    private bool visible = false;
    public GameObject prefab;
    void Start()
    {
        buttonText = transform.Find("Button").Find("Text").GetComponent<Text>();
        button = transform.Find("Button").GetComponent<Button>();
        desplegable = transform.Find("Desplegable");
        if (buttonText == null || button == null || desplegable == null || prefab == null)
        {
            Debug.LogError("SubjectUI gameobject not correctly setup");
            Destroy(this);
        }
        else
        {
            button.onClick.AddListener(ButtonClick);
            buttonText.text = "SHOW";
            desplegable.gameObject.SetActive(false);
            foreach (Transform child in desplegable)
            {
                Destroy(child.gameObject);
            }
        }
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

    public void UpdateUI(List<Subject> activeSubjects)
    {
        int n = activeSubjects.Count;
        int m = desplegable.childCount;
        while (n > m)
        {
            GameObject g = Instantiate(prefab);
            g.transform.SetParent(desplegable);
            m++;
        }
        while (m > n)
        {
            Destroy(desplegable.GetChild(m - 1).gameObject);
            m--;
        }
        int i = 0;
        while (i < n)
        {
            Text t = desplegable.GetChild(i).GetComponent<Text>();
            t.text = activeSubjects[i].info.name + " - " + activeSubjects[i].room.name;
            i++;
        }
        
    }
}
