using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using DataManagement.Managers;
using DataManagement.Serializers.ValidationExtension;

public class DataManager : MonoBehaviour
{
    [Header("Managers")]
    [Tooltip("List of managers that must save their content when Save method is invoked")]
    [SerializeField] private List<BaseManager> onSaveExecuteList;
    [Tooltip("List of managers that must reset their content when Reset method is invoked")]
    [SerializeField] private List<BaseManager> onResetExecuteList;

    [Header("Debug Tools")]
    [SerializeField] bool resetAllOnStart;


    private void Start()
    {
        if (resetAllOnStart)
        {
            foreach(BaseManager manager in onSaveExecuteList)
            {
                manager.ResetData();
            }
            foreach (BaseManager manager in onResetExecuteList)
            {
                manager.ResetData();
            }
        }
    }

    public void SaveManagers()
    {
        foreach (BaseManager serializer in onSaveExecuteList)
        {
            serializer.SaveData();
        }
    }

    public void ResetManagers()
    {
        foreach (BaseManager serializer in onResetExecuteList)
        {
            serializer.ResetData();
        }
    }
}