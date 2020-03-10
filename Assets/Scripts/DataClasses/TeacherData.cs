using UnityEngine;

[CreateAssetMenu]
public class TeacherData : ScriptableObject
{
    public new string name;
    public string[] classNames;
    public activity[] facultyActivities;
    public personality per;
    public bool autoLunchActivity = false;
}
