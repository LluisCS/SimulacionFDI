using UnityEngine;

[CreateAssetMenu]
public class StudentData : ScriptableObject
{
    public new string name;
    public string[] classNames;
    public activity[] facultyActivities;
    public personality per;
    public bool autoLunchActivity = false;
}
