/****************************************************
    文件：AgentSoccer.cs
    作者：Biu
    邮箱: 1024906432@qq.com
    功能：
*****************************************************/

using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;

public enum Team
{
    Blue = 0,
    Red = 1
}

public class AgentSoccer : Agent 
{
    public enum Position
    {
        Forward,//前锋
        Middle,//中腰
        Back,//后卫
        Goalkeeper,//守门员
        Generic
    }

    [HideInInspector]
    public Team team;
    float m_KickPower;
    float m_BallTouch;//接触球的奖励系数
    public Position position;

    //比例系数
    const float k_Power = 2000f;
    float m_Existential;    //球在场上
    float m_LateralSpeed;   //侧向速度
    float m_ForwardSpeed;   //前向速度

    //组件
    [HideInInspector]
    public Rigidbody agentRb;
    SoccerSettings m_SoccerSettings;
    BehaviorParameters m_BehaviorParameters;
    public Vector3 initialPos;
    public float rotSign;


}