using System;
using System.Collections;
using System.Collections.Generic;
using pb;
using UnityEngine;
using Random = UnityEngine.Random;

public class GamePlay : MonoBehaviour 
{
    public GameObject Prefab;
    public GameObject OnlinePerfab;
    public List<GameObject> Objs;
    public Dictionary<ulong, GameObject> Players = new Dictionary<ulong, GameObject>();

    private uint NextEventFrame = 60;
    private List<GameObject> objs = new List<GameObject>();
    void Awake()
    {
        var data = Game.Instance.Logic.Data;
        foreach (var kv in data.Players)
        {
           
            if (data.MyID == kv.Key)
            {
                Players[kv.Key] = GameObject.Instantiate(Prefab, Vector3.zero, Quaternion.identity);
            }
            else
            {
                Players[kv.Key] = GameObject.Instantiate(OnlinePerfab, Vector3.zero, Quaternion.identity);
            }
            Players[kv.Key].name = kv.Key.ToString();
        }
    }

	// Use this for initialization
	void Start ()
	{
	    Game.Instance.Callback += TickFrame;

	}

    void OnDestroy()
    {
        Game.Instance.Callback -= TickFrame;
    }

    //C2S_InputMsg.Builder msg = pb.C2S_InputMsg.CreateBuilder();

    GameData gameData = Game.Instance.Logic.Data;
    void Update () {
	    
        if (gameData == null || gameData.Players == null)
        {
            return;
        }

	    foreach (var playerData in gameData.Players)
	    {
	        GameObject o = null;
            if (gameData.MyID != playerData.Key)
            {
                if (Players.TryGetValue(playerData.Key, out o))
                {
                    //o.transform.localPosition = Vector3.Lerp(o.transform.localPosition, (playerData.Value.Pos),Time.deltaTime * 20);
                    o.transform.localPosition = Vector3.Lerp(o.transform.localPosition, new Vector3(playerData.Value.Pos.x, o.transform.localPosition.y, playerData.Value.Pos.z), Time.deltaTime * 20);
                    //o.transform.localPosition = playerData.Value.Pos;
                }
            }
            else
            {
                if (Players.TryGetValue(playerData.Key, out o))
                {
                    var msg = pb.C2S_InputMsg.CreateBuilder();
                    msg.SetX(o.transform.position.x );
                    msg.SetY(o.transform.position.y );
                    msg.SetZ(o.transform.position.z );
                    Network.Instance.Send(pb.ID.MSG_Input, msg.Build());
                }
            }
            Debug.LogError(string.Format("player[{0}] = ",playerData.Key) + playerData.Value.Pos);
        }

        return;
        //if (Input.GetKeyDown(KeyCode.W))
        //{
        //    msg.SetSid(InputDefined.Forward);
        //    Network.Instance.Send(pb.ID.MSG_Input, msg.Build());
        //}
        //if (Input.GetKeyDown(KeyCode.S))
        //{
        //    msg.SetSid(InputDefined.Back);
        //    Network.Instance.Send(pb.ID.MSG_Input, msg.Build());
        //}
        //if (Input.GetKeyDown(KeyCode.A))
        //{
        //    msg.SetSid(InputDefined.Left);
        //    Network.Instance.Send(pb.ID.MSG_Input, msg.Build());
        //}
        //if (Input.GetKeyDown(KeyCode.D))
        //{
        //    msg.SetSid(InputDefined.Right);
        //    Network.Instance.Send(pb.ID.MSG_Input, msg.Build());
        //}
    }

    void TickFrame(uint a, GameData b)
    {
        if (a >= NextEventFrame)
        {

            //NextEventFrame = a + (uint)Random.Range(10, 40);
            //var x = Random.Range(-8, 8);
            //var z = Random.Range(-8, 8);
            //var o = GameObject.Instantiate(Objs[Random.Range(0, Objs.Count - 1)], new Vector3(x, 0, z), Quaternion.identity);
            //o.name = a.ToString();
            //objs.Add(o);
        }
        for (int i=0; i<objs.Count; )
        {
            var n = UInt64.Parse(objs[i].name);

            if (a>n + 60 )
            {
                GameObject.Destroy(objs[i]);
                objs.RemoveAt(i);
                continue;
            }
            i++;
        }
    }
}
