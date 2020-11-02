using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;

public delegate void Finish_CallBack(List<Node> path);

public class PathFindRequest : MonoBehaviour
{
    GridManager gridManager;
    Queue<PathRequest> pathFind_Request = new Queue<PathRequest>();

    bool pathfind_process = false;

    private void Awake()
    {
        if (gridManager == null) 
            gridManager = gameObject.GetComponent<GridManager>();
    }

    private void Update()
    {
        if (pathFind_Request.Count > 0 && !pathfind_process)
            StartCoroutine(Searching());
    }

    public void RequestPath(int size, Vector3 currPos, Vector3 targetPos, Finish_CallBack callback)
    {
        PathRequest request = new PathRequest(size, currPos, targetPos, callback);
        pathFind_Request.Enqueue(request);
    }

    private IEnumerator Searching()
    {
        pathfind_process = true;
        PathRequest request = pathFind_Request.Dequeue();
        List<Node> path = gridManager.Trace(request.pathStart, request.pathEnd, 1f);
        yield return new WaitUntil(() => path != null);
        request.callback(path);
        yield return new WaitForSeconds(0.1f);
        pathfind_process = false;
    }

    struct PathRequest
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
}

