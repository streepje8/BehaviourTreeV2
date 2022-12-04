using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public struct Variable<T>
{
    public string name;
    public T value;
}

[Serializable]
public struct Reference<T> where T : Object
{
    public string name;
    public T value;
}

[Serializable]
public class WBlackboard
{
    public List<Variable<System.Object>> variables = new List<Variable<System.Object>>();
    public List<Reference<Object>> references = new List<Reference<Object>>();
    [HideInInspector] public Dictionary<string, System.Object> realtimeVariables = new Dictionary<string, System.Object>();
    [HideInInspector] public Dictionary<string, Object> realtimeReferences = new Dictionary<string, Object>();

    public void BuildToDictionary() //We have to store tuples because unity has no editor for dictionaries
    {
        variables.ForEach(x => realtimeVariables.Add(x.name,x.value));
        references.ForEach(x => realtimeReferences.Add(x.name,x.value));
    }
}
