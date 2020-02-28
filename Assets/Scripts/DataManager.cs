using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public GameObject agentPrefab;
    [HideInInspector]
    public GameObject studentParent, teacherParent;
    public SubjectData[] subjectsData;
    public TeacherData[] teachersData;
    public StudentData[] studentsData;
    void Awake()
    {
        
    }

    void Update()
    {
        
    }

    public SubjectSchedule generateSchedule(LayoutManager LManager) {
        SubjectSchedule schedule = new SubjectSchedule();

        foreach (var sub in subjectsData)
        {
            SubjectInfo info = new SubjectInfo(sub.name);
            foreach (var hour in sub.classes)
            {
                Subject s = new Subject( hour.startHour, hour.startMinute, hour.durationHours, hour.durationMinutes, LManager.getRoom(hour.roomName),info);
                schedule.days[(int)hour.day].Add(s);
                info.hours.Add(s);
            }
            schedule.subjectInfos.Add(sub.name, info);
        }

        teacherParent = new GameObject("teacherParent");
        foreach (var teacher in teachersData)
        {
            GameObject agentObj = Instantiate(agentPrefab);
            agentObj.transform.SetParent(teacherParent.transform);
            Agent agentComp = agentObj.GetComponent<Agent>();
            agentComp.state.type = agentType.teacher;
            agentComp.name = teacher.name;
            if (agentComp == null)
                Debug.LogError("Agent prefab missing agent component.");
            else
            {
                foreach (var s in teacher.classNames)
                {
                    SubjectInfo tmp;
                    if (schedule.subjectInfos.TryGetValue(s, out tmp))
                    {
                        tmp.teachers.Add(agentComp);
                        agentComp.subjects.Add(s, tmp);
                    }
                    else
                        Debug.LogWarning("Subject with name " + s + " not found.");
            }
                foreach (var a in teacher.facultyActivities)
                {
                    agentComp.activities.Add(a);
                }
            }
        }
        studentParent = new GameObject("studentParent");
        foreach (var student in studentsData)
        {
            GameObject agentObj = Instantiate(agentPrefab);
            agentObj.transform.SetParent(studentParent.transform);
            Agent agentComp = agentObj.GetComponent<Agent>();
            agentComp.name = student.name;
            if (agentComp == null)
                Debug.LogError("Agent prefab missing agent component.");
            else
            {
                foreach (var s in student.classNames)
                {
                    SubjectInfo tmp;
                    if (schedule.subjectInfos.TryGetValue(s, out tmp))
                    {
                        tmp.students.Add(agentComp);
                        agentComp.subjects.Add(s, tmp);
                    }
                    else
                        Debug.LogWarning("Subject with name " + s + " not found.");
                }
                foreach (var a in student.facultyActivities)
                {
                    agentComp.activities.Add(a);
                }
            }
        }
        return schedule;
    }

    void generateRandomStudents() { }
    void generateRandomTeachers() { }
    //void generateRandomSubjects() { }
}
