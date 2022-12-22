/****************************************************
    文件：SoccerEnvController.cs
    作者：Biu
    邮箱: 1024906432@qq.com
    功能：场景的信息
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
        m_BallStartPos = new Vector3(ball.transform.position.x, 0.9249992f, ball.transform.position.z);

        foreach(var item in AgentsList)
        {
            item.StrartPos = item.Agent.transform.position;
            item.StartRot= item.Agent.transform.rotation;
            item.Rb = item.Agent.GetComponent<Rigidbody>();
            if (item.Agent.team == Team.Blue)
            {
                m_BlueAgentGroup.RegisterAgent(item.Agent);
            }
            else
            {
                m_RedAgentGroup.RegisterAgent(item.Agent);
            }
        }
        ResetScene();
    }

    public void ResetScene()
    {
        m_ResetTimer = 0;

        foreach (var item in AgentsList)
        {
            var randomPosZ = Random.Range(-2f, 2f);
            var newStartPos = item.Agent.initialPos + new Vector3(0f, 0f, randomPosZ);
            var rot = item.Agent.rotSign * Random.Range(80.0f, 100.0f);
            var newRot = Quaternion.Euler(0, rot, 0);
            item.Agent.transform.SetPositionAndRotation(newStartPos, newRot);

            item.Rb.velocity = Vector3.zero;
            item.Rb.angularVelocity = Vector3.zero;
        }

        ResetBall();
    }

    public void ResetBall()
    {
        var randomPosX = Random.Range(-2.5f, 2.5f);
        var randomPosZ = Random.Range(-2.5f, 2.5f);

        ball.transform.position = m_BallStartPos + new Vector3(randomPosX, 0, randomPosZ);
        ballRb.velocity = Vector3.zero;
        ballRb.angularVelocity = Vector3.zero;
    }

    private void FixedUpdate() //超时
    {
        m_ResetTimer += 1;
        if (m_ResetTimer >= MaxEnvironmentSteps && MaxEnvironmentSteps > 0)
        {
            m_BlueAgentGroup.GroupEpisodeInterrupted();
            m_RedAgentGroup.GroupEpisodeInterrupted();
            ResetScene();
        }
    }

    // 全队奖励
    public void GoalTouched(Team scoredTeam)
    {
        float reward = 1 - (float)m_ResetTimer / MaxEnvironmentSteps;
        if (scoredTeam == Team.Blue)
        {
            m_BlueAgentGroup.AddGroupReward(reward);
            m_RedAgentGroup.AddGroupReward(-reward);
        }
        else if(scoredTeam == Team.Red)
        {
            m_RedAgentGroup.AddGroupReward(reward);
            m_BlueAgentGroup.AddGroupReward(-reward);
        }

        m_RedAgentGroup.EndGroupEpisode();
        m_BlueAgentGroup.EndGroupEpisode();
        ResetScene();

    }
}