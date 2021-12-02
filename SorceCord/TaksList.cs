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

    //定義されたTask
    Dictionary<T, Task> DefaultTaskDictionary = new Dictionary<T, Task>();

    //追加されてるTask
    List<Task> StackedTask = new List<Task>();

    //現在動いてるTask
    Task MovingTask = null;

    //現在動いてるTaskの番号
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

        //upDateを呼ぶ
        bool IsEndTask = MovingTask.UpDate();

        //Updateが終わってたら
        if (IsEndTask)
        {
            //動いているTaskの終了処理
            MovingTask?.Exit();

            ++MovingTaskNumber;
           
            //この先にTaskがなければクリアする
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


