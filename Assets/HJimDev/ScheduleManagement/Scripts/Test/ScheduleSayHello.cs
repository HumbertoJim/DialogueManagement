using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

using ScheduleManagement.Managers;
using ScheduleManagement.Schedulables;

public class ScheduleSayHello : ScheduleManager
{
    [Header("Test")]
    [SerializeField] Text messageLabel;
    [SerializeField] Text timeLabel;
    [SerializeField] float time = 5;

    float timeCounter;

    private void Start()
    {
        Schedulable schedulable = new Schedulable(time, function: () => { messageLabel.text = "Hello!"; });
        
        Scheduler.Add("SayHello", schedulable);
        Scheduler.Start("SayHello");

        timeCounter = 0;
    }

    protected override void Update()
    {
        base.Update();

        timeCounter += Time.deltaTime;
        timeLabel.text = Math.Round(timeCounter, 2).ToString();
    }
}
