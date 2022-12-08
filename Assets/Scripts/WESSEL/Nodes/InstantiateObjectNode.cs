using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateObjectNode : ActionNode
{
    private string whereToSave;
    private GameObject obj;
    private Vector3 position;
    private Quaternion rotation;
    
    public InstantiateObjectNode(string whereToSave, GameObject obj, Vector3 position = new Vector3(),
        Quaternion rotation = new Quaternion())
    {
        this.whereToSave = whereToSave;
        this.obj = obj;
        this.position = position;
        this.rotation = rotation;
        onRun = Execute;
    }

    public TaskStatus Execute()
    {
        GameObject instantiated = Object.Instantiate(obj, position, rotation);
        blackboard.SetVariable(whereToSave,instantiated);
        return TaskStatus.Success;
    }
}
