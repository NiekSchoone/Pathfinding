using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PathRequestManager : MonoBehaviour
{
    //Defines a queue of requested paths.
    Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();

    //The current path that wants to be calculated and processed.
    PathRequest currentPathRequest;

    static PathRequestManager instance;

    Pathfinding pathFinding;

    bool isProcessingPath;

    void Awake()
    {
        instance = this;
        pathFinding = GetComponent<Pathfinding>();
    }

    /// <summary>
    /// Requests a path from a starting point to an end point and puts it in the queue of requested paths.
    /// </summary>
    /// <param name="pathStart"></param>
    /// <param name="pathEnd"></param>
    /// <param name="callback"></param>
    public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback)
    {
        PathRequest newRequest = new PathRequest(pathStart, pathEnd, callback);

        instance.pathRequestQueue.Enqueue(newRequest);
        instance.TryProcessNext();
    }
    
    /// <summary>
    /// The current processing path request has finished and will try to process the next path request in the queue.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="succes"></param>
    public void FinishedProcessingPath(Vector3[] path, bool succes)
    {
        currentPathRequest.callback(path, succes);
        isProcessingPath = false;
        TryProcessNext();
    }

    /// <summary>
    /// Will try to start processing the next path request in the queue.
    /// </summary>
    void TryProcessNext()
    {
        if(!isProcessingPath && pathRequestQueue.Count > 0)
        {
            currentPathRequest = pathRequestQueue.Dequeue();
            isProcessingPath = true;
            pathFinding.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd);
        }
    }
    
    struct PathRequest
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

}
