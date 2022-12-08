using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public enum TaskStatus { Success, Failed, Running }
public abstract class BTBaseNode
{
    public WBlackboard blackboard;
    public MonoBehaviour executor;
    public virtual int maxChildCount { get; private set; } = 0;
    public BTBaseNode parent;
    public BTBaseNode[] children = Array.Empty<BTBaseNode>();
    public GUID guid = GUID.Generate();

    public virtual TaskStatus RunNode()
    {
        TaskStatus stat = Run();
        blackboard.lastExecutedNode = this;
        return stat;
    }
    public abstract TaskStatus Run();

    public virtual void Initialize()
    {
        for (int i = 0; i < children.Length; i++)
        {
            children[i].Initialize();
        }
    }
    
    public BTBaseNode Build()
    {
        Initialize();
        return this;
    }
    
    public BTBaseNode SetBlackboard(WBlackboard blackboard)
    {
        this.blackboard = blackboard;
        foreach (BTBaseNode child in children)
        {
            child.SetBlackboard(blackboard);
        }
        return this;
    }

    public BTBaseNode SetExecutor(MonoBehaviour executor)
    {
        this.executor = executor;
        foreach (BTBaseNode child in children)
        {
            child.SetExecutor(executor);
        }
        return this;
    }

    public BTBaseNode Append(BTBaseNode child)
    {
        if (children.Length < maxChildCount)
        {
            child.parent = this;
            child.SetBlackboard(blackboard);
            child.SetExecutor(executor);
            children = children.Append(child).ToArray();
        }
        else
        {
            Debug.LogError(GetType().FullName + " can only have " + maxChildCount + " child nodes.");
        }
        return this;
    }

    #if UNITY_EDITOR
    
    /*
     * All the below code is only used to generate a visual, it has been written in a very ugly and slow way
     */
    
    public string ToPlantUML() //WARNING THIS IS VERY SLOW!!!!!
    {
        string result = "@startuml\n";
        result += GetPlantUMLDefenitions();
        result += GetPlantUMLConnections();
        result += "\n@enduml";
        return result;
    }

    private string GetPlantUMLConnections()
    {
        string result = "";
        string myname =  guid.GetHashCode().ToString().Replace("-","0");
        bool isConditional = GetType() == typeof(ConditionalNode);
        bool isSequential = GetType() == typeof(SequenceNode);
        int index = 0;
        foreach (var child in children)
        {
            result += child.GetPlantUMLConnections();
            string childname = child.guid.GetHashCode().ToString().Replace("-","0");
            result += myname + " -down-|> " + childname + (isConditional ? index == 0 ? " : True" : " : False" : (isSequential ? " : " + (index + 1) : "")) + "\n";
            index++;
        }
        return result;
    }

    private HashSet<string> ignoredFields = new HashSet<string>()
    {
        "ignoredFields",
        "blackboard",
        "executor",
        "maxChildCount",
        "parent",
        "children",
        "guid",
        "onRun",
        "conditionEvaluator"
    };

    private string GetPlantUMLDefenitions()
    {
        string result = "";
        foreach (var child in children)
        {
            result += child.GetPlantUMLDefenitions();
        }
        result += "class " + guid.GetHashCode().ToString().Replace("-","0") + " {\n --" + GetType().Name.Replace("`1","") + "--\n";
        FieldInfo[] myFields = GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Default | BindingFlags.Static | BindingFlags.Instance);
        foreach (var fieldInfo in myFields)
        {
            if (!ignoredFields.Contains(fieldInfo.Name))
            {
                result += " +" + fieldInfo.Name + ":" + fieldInfo.GetValue(this) + "\n";
            }
        }
        result += "}\n";
        return result;
    }
    #endif
}
