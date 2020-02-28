using UnityEngine;

[CreateAssetMenu]
public class SubjectData : ScriptableObject
{
    public new string name;

    public activity[] classes;
}

[CreateAssetMenu]
public class StudentData : ScriptableObject
{
    public new string name;
    public string[] classNames;
    public activity[] facultyActivities;
    public personality per;
}

[CreateAssetMenu]
public class TeacherData : ScriptableObject
{
    public new string name;
    public string[] classNames;
    public activity[] facultyActivities;
    public personality per;
}

