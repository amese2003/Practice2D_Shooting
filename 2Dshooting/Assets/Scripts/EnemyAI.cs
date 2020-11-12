using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    MasterCharacter character;

    IEnumerator current_Movement;

    private void Awake()
    {
        if (character == null)
            character = gameObject.GetComponent<MasterCharacter>();
    }

    private void Start()
    {
        TraceEnemy();
    }


    private void TraceEnemy()
    {
        int unitLayer = gameObject.layer;
        MasterCharacter target = unitLayer == 10 ? GameManager.Instance.player : null;

        if (target == null)
            return;

        //Finish_CallBack callBack = TraceEnemy_CallBack;
        //GameManager.Instance.pathReq.RequestPath(character.unitHeight, gameObject.transform.position, target.transform.position,  callBack);
        Debug.Log("xhd");
    }

    private void TraceEnemy_CallBack(List<Node> path)
    {
        if (current_Movement != null)
            StopCoroutine(current_Movement);

        current_Movement = MoveToDest(path);
        StartCoroutine(current_Movement);
    }

    IEnumerator MoveToDest(List<Node> path)
    {
        int nodeNum = 0;
        Vector2 wayPoint = path[0].worldPos;
        float percent = 0f;
        float max = max = Mathf.Abs(path[path.Count - 1].worldPos.x - gameObject.transform.position.x) / character.moveSpeed; ;
        float time = 0f;
        
        while(true)
        {
            if(wayPoint.x == gameObject.transform.position.x || percent > .8f)
            {
                nodeNum++;
                percent = 0;
                if (nodeNum >= path.Count)
                    yield break;

                
                wayPoint = path[nodeNum].worldPos;                            
            }

            float x = easeInQuad(gameObject.transform.position.x, wayPoint.x, percent);
            gameObject.transform.position = new Vector2(x, gameObject.transform.position.y);
            time += Time.deltaTime;
            percent = time / max;
            //transform.position = wayPoint;

            //float newPos = gameObject.transform.position.x + (wayPoint.x - gameObject.transform.position.x * character.moveSpeed * Time.deltaTime);
            //Vector2.MoveTowards(transform.position, wayPoint, character.moveSpeed);

            // percent += Time.deltaTime;
            //transform.position = new Vector2(newPos, transform.position.y);
            yield return null;
        }
    }

    private float easeInQuad(float start, float end, float value)
    {
        return Mathf.Lerp(start, end, value);
    }
}
