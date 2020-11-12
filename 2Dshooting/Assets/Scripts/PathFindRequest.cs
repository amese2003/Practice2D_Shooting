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
    }

    private void Update()
    {
    }

    public void RequestPath(PathRequest _request)
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
    public Finish_CallBack callback;

    public PathResult(Vector3[] _path, bool _success, Finish_CallBack _callback)
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
    public Finish_CallBack callback;
    public int unitSize;

    public PathRequest(int _size, Vector3 _start, Vector3 _end, Finish_CallBack _callback)
    {
        pathStart = _start;
        pathEnd = _end;
        callback = _callback;
        unitSize = _size;
    }
}