using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;


public class TaksList<T>
{
    private class Task
    {
        public T TaskType;
        public Action Enter { get; set; }
        public Func<bool> UpDate { get; set; }
        public Action Exit { get; set; }

        public Task(T taskType,Action enter,Func<bool> update,Action exit)
        {
            TaskType = taskType;
            Enter = enter;
            UpDate = update;
            Exit = exit;

        }

    }

    //��`���ꂽTask
    Dictionary<T, Task> DefaultTaskDictionary = new Dictionary<T, Task>();

    //�ǉ�����Ă�Task
    List<Task> StackedTask = new List<Task>();

    //���ݓ����Ă�Task
    Task MovingTask = null;

    //���ݓ����Ă�Task�̔ԍ�
    int MovingTaskNumber = 0;


    private bool IsEnd()
    {
        if(StackedTask.Count <= MovingTaskNumber)
        {
            return true;
        }

        return false;
    }

    public  void UpDateTask()
    {
        if (MovingTask == null)
        {
            MovingTask = StackedTask[MovingTaskNumber];
            MovingTask.Enter?.Invoke();

        }

        //upDate���Ă�
        bool IsEndTask = MovingTask.UpDate();

        //Update���I����Ă���
        if (IsEndTask)
        {
            //�����Ă���Task�̏I������
            MovingTask?.Exit();

            ++MovingTaskNumber;
           
            //���̐��Task���Ȃ���΃N���A����
            if (IsEnd())
            {
                MovingTask = null;
                MovingTaskNumber = 0;
                StackedTask.Clear();
            }

            MovingTask = StackedTask[MovingTaskNumber];

            MovingTask.Enter?.Invoke();

        }


    }

    
    public void IntializeTask(T t,Action enter,Func<bool> update,Action exit)
    {
        var task = new Task(t, enter, update, exit);
        DefaultTaskDictionary.Add(t,task);
    }

    public void AddTask(T t)
    {
        Task task = null;
        var ex = DefaultTaskDictionary.TryGetValue(t, out task);

        if (!ex)
        {
            return;
        }

        StackedTask.Add(task);
        
    }

}


