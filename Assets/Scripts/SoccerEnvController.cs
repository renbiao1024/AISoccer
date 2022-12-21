/****************************************************
    文件：SoccerEnvController.cs
    作者：Biu
    邮箱: 1024906432@qq.com
    功能：
*****************************************************/

using UnityEngine;
using Unity.MLAgents;
using System.Collections.Generic;

public class SoccerEnvController : MonoBehaviour 
{
    [System.Serializable]
    public class PlayerInfo
    {
        public AgentSoccer Agent;
        [HideInInspector]
        public Vector3 StrartPos;
        [HideInInspector]
        public Quaternion StartRot;
        [HideInInspector]
        public Rigidbody Rb;
    }

    //最大训练次数
    [Tooltip("Max Environment Steps")] public int MaxEnvironmentSteps = 25000;

    //边界列表
    public List<GameObject> BoundsList = new List<GameObject>();

    // 获胜切换材质
    public Material redWinMat;
    public Material blueWinMat;

    //足球信息
    public GameObject ball;
    [HideInInspector]
    public Rigidbody ballRb;
    Vector3 m_BallStartPos;
    private SoccerSettings m_SoccerSettings;

    //Agent信息
    public List<PlayerInfo> AgentsList = new List<PlayerInfo>();
    private SimpleMultiAgentGroup m_BlueAgentGroup;
    private SimpleMultiAgentGroup m_RedAgentGroup;

    private int m_ResetTimer;

    private void Start()
    {
        //初始化设置
        m_SoccerSettings = FindObjectOfType<SoccerSettings>();
        m_BlueAgentGroup = new SimpleMultiAgentGroup();
        m_RedAgentGroup= new SimpleMultiAgentGroup();
        ballRb = ball.GetComponent<Rigidbody>();
        m_BallStartPos = new Vector3(ball.transform.position.x, ball.transform.position.y, ball.transform.position.z);

        foreach(var item in AgentsList)
        {
            item.StrartPos = item.Agent.transform.position;
            item.StartRot= item.Agent.transform.rotation;

            if (item.Agent.team == Team.Blue)
            {
                m_BlueAgentGroup.RegisterAgent(item.Agent);
            }
            else
            {
                m_RedAgentGroup.RegisterAgent(item.Agent);
            }
        }
    }
}