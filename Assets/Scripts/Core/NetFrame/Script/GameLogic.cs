using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData
{
    public ulong MyID;
    public class PlayerData
    {
        
        public int Progress;
        public Vector3 Pos;
        public Quaternion Dir;

        public PlayerData()
        {
            Pos = Vector3.zero;
            Dir = Quaternion.identity;
        }
    }

    public Dictionary<ulong, PlayerData> Players = new Dictionary<ulong, PlayerData>();

    public void Reset()
    {
        Players.Clear();
    }
}

public class GameLogic
{

    public GameData Data = new GameData();

    public void Reset()
    {
        Data.Reset();
    }

    public GameData.PlayerData GetMyData()
    {
        return Data.Players[Data.MyID];
    }

    public void JoinRoom(ulong id)
    {
        Data.Players[id] = new GameData.PlayerData();
    }

    public void SetProgress(ulong id,int progress)
    {
        Data.Players[id].Progress = progress;
    }

    public void ProcessFrameData(pb.FrameData msg)
    {
        if (null == msg)
        {
            return;
        }
        if (null != msg.InputList)
        {
            foreach (var f in msg.InputList)
            {
                PlayerCmd(f);
            }
        }
    }

    public void PlayerCmd(pb.InputData cmd)
    {
        GameData.PlayerData data = null;
        if (!Data.Players.TryGetValue(cmd.Id, out data))
        {
            return;
        }
        var position = new Vector3(cmd.X,cmd.Y,cmd.Z);
        var dir = cmd.Sid;

        data.Pos = position;
        Debug.LogError(string.Format("CMDKEY player[{0}] = ", cmd.Id) + data.Pos);
        //if (InputDefined.Forward == dir)
        //{
        //    data.Pos = data.Pos + Vector3.forward;
        //} else if (InputDefined.Back == dir)
        //{
        //    data.Pos = data.Pos + Vector3.back;
        //}
        //else if (InputDefined.Left == dir)
        //{
        //    data.Pos = data.Pos + Vector3.left;
        //}
        //else if (InputDefined.Right == dir)
        //{
        //    data.Pos = data.Pos + Vector3.right;
        //}
        //else if (InputDefined.None == dir)
        //{
        //    data.Pos = Vector3.zero;
        //}
    }
}
