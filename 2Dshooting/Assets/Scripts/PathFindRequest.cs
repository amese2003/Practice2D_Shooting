using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public delegate void Finish_CallBack(Vector3[] path, bool success);

public class PathFindRequest : MonoBehaviour
{
    GridManager gridManager;
    Queue<PathResult> results = new Queue<PathResult>();

    static PathFindRequest instance;
    PathFinding pathFinding;

    private void Awake()
    {
        instance = this;
        if (gridManager == null) 
            gridManager = gameObject.GetComponent<GridManager>();

        pathFinding = GetComponent<PathFinding>();
    }

    private void Update()
    {
        if(results.Count > 0)
        {
            int itemInQueue = results.Count;
            lock (results)
            {
                for(int i = 0; i <itemInQueue; i++)
                {
                    PathResult result = results.Dequeue();
                    result.callback(result.path, result.success);
                }
            }
        }

    }

    public static void RequestPath(PathRequest _request)
    {
        ThreadStart threadStart = delegate ()
        {
            instance.pathFinding.FindPath(_request, instance.FinishedProcessingPath);
        };
        threadStart.Invoke();
    }

    public void FinishedProcessingPath(PathResult _result)
    {
        lock (results)
        {
            results.Enqueue(_result);
        }
    }
    

    
}

public struct PathResult
{
    public Vector3[] path;
    public bool success;
    public Action<Vector3[], bool> callback;

    public PathResult(Vector3[] _path, bool _success, Action<Vector3[], bool> _callback)
    {
        path = _path;
        success = _success;
        callback = _callback;
    }

}


public struct PathRequest
{
    public Vector3 pathStart;
    public Vector3 pathEnd;
    public Action<Vector3[], bool> callback;

    public PathRequest(Vector3 _start, Vector3 _end, Action<Vector3[], bool> _callback)
    {
        pathStart = _start;
        pathEnd = _end;
        callback = _callback;
    }
}