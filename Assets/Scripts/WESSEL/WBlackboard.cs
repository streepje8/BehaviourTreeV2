using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// A struct which the editor can display
/// </summary>
/// <typeparam name="T">The type it has to store</typeparam>
[Serializable]
public struct Variable<T>
{
    public string name;
    public T value;
}

/// <summary>
/// A struct which the editor can display
/// </summary>
/// <typeparam name="T">The type it has to store</typeparam>
[Serializable]
public struct Reference<T> where T : Object
{
    public string name;
    public T value;
}

/// <summary>
/// Used in the nodes so they can be more abstract
/// </summary>
public enum BlackboardOperationType
{
    WriteVariable,
    ReadVariable,
    WriteReference,
    ReadReference
}

/// <summary>
/// My version of the blackboard class
/// </summary>
[Serializable]
public class WBlackboard
{
    public List<Reference<Object>> references = new List<Reference<Object>>();
    
    [HideInInspector]public List<Variable<System.Object>> variables = new List<Variable<System.Object>>();
    [HideInInspector] public Dictionary<string, System.Object> realtimeVariables = new Dictionary<string, System.Object>();
    [HideInInspector] public Dictionary<string, Object> realtimeReferences = new Dictionary<string, Object>();
    [HideInInspector] public BTBaseNode lastExecutedNode = null;

    /// <summary>
    /// Builds the editor set values in to the realtime dictonaries
    /// </summary>
    public void BuildToDictionary() //We have to store tuples because unity has no editor for dictionaries
    {
        variables.ForEach(x => realtimeVariables.Add(x.name,x.value));
        references.ForEach(x => realtimeReferences.Add(x.name,x.value));
    }

    /// <summary>
    /// Set a variable on the blackboard
    /// </summary>
    /// <param name="name">The variable name</param>
    /// <param name="value">The value to store</param>
    /// <returns>If the variable was overwritten</returns>
    public bool SetVariable(string name, object value)
    {
        if (realtimeVariables.ContainsKey(name))
        {
            realtimeVariables[name] = value;
            return true;
        }
        else
        {
            realtimeVariables.Add(name,value);
            return true;
        }
    }
    
    /// <summary>
    /// Set a reference on the blackboard
    /// </summary>
    /// <param name="name">The reference name</param>
    /// <param name="value">The reference to store</param>
    /// <returns>If the reference was overwritten</returns>
    public bool SetReference(string name, Object value)
    {
        if (realtimeReferences.ContainsKey(name))
        {
            realtimeReferences[name] = value;
            return true;
        }
        else
        {
            realtimeReferences.Add(name,value);
            return true;
        }
    }

    /// <summary>
    /// Tries to get a value from the blackboard
    /// </summary>
    /// <param name="name">The variable name</param>
    /// <param name="value">The value which was retrieved</param>
    /// <typeparam name="T">The type of the variable</typeparam>
    /// <returns>If the variable was resolved</returns>
    public bool TryGetVariable<T>(string name, out T value)
    {
        if (realtimeVariables.TryGetValue(name, out object val))
        {
            if (val.GetType() == typeof(T) || val.GetType().IsSubclassOf(typeof(T)))
            {
                value = (T)val;
                return true;
            }
        }
        value = default;
        return false;
    }
    
    /// <summary>
    /// Tries to get a reference from the blackboard
    /// </summary>
    /// <param name="name">The reference name</param>
    /// <param name="value">The reference which was retrieved</param>
    /// <typeparam name="T">The type of the reference</typeparam>
    /// <returns>If the reference was resolved</returns>
    public bool TryGetReference<T>(string name, out T value) where T : Object
    {
        if (realtimeReferences.TryGetValue(name, out Object val))
        {
            if (val.GetType() == typeof(T) || val.GetType().IsSubclassOf(typeof(T)))
            {
                value = (T)val;
                return true;
            }
        }
        value = default;
        return false;
    }

}
