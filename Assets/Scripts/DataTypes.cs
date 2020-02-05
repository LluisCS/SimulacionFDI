using UnityEngine;
[System.Serializable]
public enum personality { }

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

public struct subject
{
    public bool active;
    public string name;
    public int startHour, startMinute, endHour, endMinute;
    public GameObject room;
    public GameObject[] teachers;
    public GameObject[] students;
}