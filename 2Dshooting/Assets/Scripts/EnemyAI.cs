using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    MasterCharacter character;

    IEnumerator current_Movement;

    const float minPathUpdateTime = .2f;
    const float pathUpdateMoveThreshold = .5f;

    public float turnDst = 5f;
    public float stoppingDst = 10;
    public Transform target;

    Path path;

    private void Awake()
    {
        if (character == null)
            character = gameObject.GetComponent<MasterCharacter>();
    }

    private void Start()
    {
        StartCoroutine(UpdatePath());
    }

    public void OnPathFound(Vector3[] _waypoints, bool _pathSuccessful)
    {
        if (_pathSuccessful)
        {
            path = new Path(_waypoints, transform.position, turnDst, stoppingDst);
            StopCoroutine(FollowPath());
            StartCoroutine(FollowPath());
        }
    }

    IEnumerator UpdatePath()
    {
        if (Time.timeSinceLevelLoad < .3f)
            yield return new WaitForSeconds(.3f);

        PathFindRequest.RequestPath(new PathRequest(transform.position, target.transform.position, OnPathFound));

        float sqrMoveThreshole = pathUpdateMoveThreshold * pathUpdateMoveThreshold;
        Vector3 targetPosOld = target.position;

        while (true)
        {
            yield return new WaitForSeconds(minPathUpdateTime);
            if((target.position - targetPosOld).sqrMagnitude > sqrMoveThreshole)
            {
                PathFindRequest.RequestPath(new PathRequest(transform.position, target.transform.position, OnPathFound));
                targetPosOld = target.position;
            }
        }

    }
    IEnumerator FollowPath()
    {
        bool followingPath = true;
        int pathIndex = 0;
        //transform.LookAt(path.lookPoints[0]);

        float speedPercent = 1;

        while (true)
        {
            Vector2 pos2D = new Vector2(transform.position.x, transform.position.y);

            if (pathIndex < path.turnBoundaries.Length)
            {

                while (path.turnBoundaries[pathIndex].HasCrossedLine(pos2D))
                {
                    if (pathIndex == path.finishLineIndex || Mathf.Abs(target.transform.position.x - transform.position.x) < 3f)
                    {
                        followingPath = false;
                        break;
                    }
                    else
                        pathIndex++;
                }

                if (followingPath)
                {
                    if (pathIndex >= path.slowDownIndex && stoppingDst > 0)
                    {
                        speedPercent = Mathf.Clamp01(path.turnBoundaries[path.finishLineIndex].DistanceFromPoint(pos2D / stoppingDst));
                        if (speedPercent < 0.01f)
                            followingPath = false;
                    }
                    //Quaternion targetRotation = Quaternion.LookRotation(path.lookPoints[pathIndex] - transform.position);
                    //transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime);

                    Vector3 scale = target.position.x - transform.position.x < 0 ? new Vector3(1, 1, 1) : new Vector3(-1, 1, 1);
                    transform.localScale = scale;
                    Vector2 dir = transform.localScale.x > 0 ? Vector2.left : Vector2.right;
                    

                    transform.Translate(dir * Time.deltaTime * character.moveSpeed);
                }
            }
            yield return null;
        }

    }

    public void OnDrawGizmos()
    {
        if (path != null)
        {
            path.DrawWithGizmos();
        }
    }

    private float easeInQuad(float start, float end, float value)
    {
        return Mathf.Lerp(start, end, value);
    }
}
