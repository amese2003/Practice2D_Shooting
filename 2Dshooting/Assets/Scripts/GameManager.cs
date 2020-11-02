using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GridManager gridManager;
    public PathFindRequest pathReq;

    public MasterCharacter player;
    public MasterCharacter enemy;

    public Vector3 playerPos
    {
        get { return player.gameObject.transform.position; }
    }

    public Vector3 enemyPos
    {
        get { return enemy.gameObject.transform.Find("GroundCheck").position; }
    }

    private void Awake()
    {
        Instance = this;
        
    }

    private void OnDisable()
    {
        Instance = null;
    }


}
