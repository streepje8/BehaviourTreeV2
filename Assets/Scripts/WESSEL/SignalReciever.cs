using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SignalReciever : MonoBehaviour
{
    [HideInInspector] public static List<SignalReciever> recievers { get; private set; } = new List<SignalReciever>();
    private Dictionary<string, Action<MonoBehaviour>> callbacks = new Dictionary<string, Action<MonoBehaviour>>();

    public void OnEnable()
    {
        recievers.Add(this);
    }

    private void OnDisable()
    {
        recievers.Remove(this);
    }

    public void AddReciever(string name, Action<MonoBehaviour> callback)
    {
        if (callbacks.ContainsKey(name))
        {
            callbacks[name] = callback;
            Debug.LogWarning("Reciever callback got overwritten!", gameObject);
        }
        else
        {
            callbacks.Add(name,callback);
        }
    }

    public void Recieve(MonoBehaviour caller,string name)
    {
        if (callbacks.TryGetValue(name, out Action<MonoBehaviour> callback))
        {
            callback.Invoke(caller);
        }
    }
    
}
