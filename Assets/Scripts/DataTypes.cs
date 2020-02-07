using UnityEngine;

[CreateAssetMenu]
public class SubjectData : ScriptableObject
{
    public new string name;

    public classHour[] classes;
}

[CreateAssetMenu]
public class StudentData : ScriptableObject
{
    public new string name;
    public string[] classNames;
}

[CreateAssetMenu]
public class TeacherData : ScriptableObject
{
    public new string name;
    public string[] classNames;
}

