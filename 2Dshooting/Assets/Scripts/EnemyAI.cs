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

        Finish_CallBack callBack = TraceEnemy_CallBack;
        GameManager.Instance.pathReq.RequestPath(character.unitHeight, gameObject.transform.position, target.transform.position,  callBack);
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
        float timer = 0;
        
        while(true)
        {
            if(Vector2.Distance(wayPoint, gameObject.transform.position)  == 0)
            {
                nodeNum++;
                if (nodeNum >= path.Count)
                    yield break;
                wayPoint = path[nodeNum].worldPos;
            }

            transform.position = wayPoint;

            //Vector2.MoveTowards(transform.position, wayPoint, character.moveSpeed * Time.deltaTime);
            yield return new WaitForSeconds(0.01f);
        }
    }

}
