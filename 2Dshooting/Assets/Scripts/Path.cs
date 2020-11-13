using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.TerrainAPI;
using UnityEngine;

public class Path
{
    public readonly Vector3[] lookPoints;
    public readonly Line[] turnBoundaries;
    public readonly int finishLineIndex;
    public readonly int slowDownIndex;

    public Path(Vector3[] _wayPoints, Vector3 _startPos, float _turnDst, float stoppingDst)
    {
        lookPoints = _wayPoints;
        turnBoundaries = new Line[lookPoints.Length];
        finishLineIndex = turnBoundaries.Length - 1;

        Vector2 previousPoint = _startPos;
        for(int i = 0; i < lookPoints.Length; i++)
        {
            Vector2 currentPoint = lookPoints[i];
            Vector2 dirToCurrentPoint = (currentPoint - previousPoint).normalized;
            Vector2 turnBoundaryPoint = (i == finishLineIndex) ? currentPoint : currentPoint - dirToCurrentPoint * _turnDst;
            turnBoundaries[i] = new Line(turnBoundaryPoint, previousPoint - dirToCurrentPoint * _turnDst);
            previousPoint = turnBoundaryPoint;
        }

        float dstFromEndPoint = 0;
        for(int i = lookPoints.Length - 1; i> 0; i--)
        {
            dstFromEndPoint += Vector3.Distance(lookPoints[i], lookPoints[i - 1]);
            if(dstFromEndPoint > stoppingDst)
            {
                slowDownIndex = i;
                break;
            }
        }
    }

    public void DrawWithGizmos()
    {
        Gizmos.color = Color.black;
        foreach (Vector3 p in lookPoints)
            Gizmos.DrawCube(p + Vector3.up, Vector3.one);

        Gizmos.color = Color.white;
        foreach (Line line in turnBoundaries)
            line.DrawWithGizmos(10);
    }

    
}
