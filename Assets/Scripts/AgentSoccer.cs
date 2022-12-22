/****************************************************
    文件：AgentSoccer.cs
    作者：Biu
    邮箱: 1024906432@qq.com
    功能：训练的模型信息
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
    const float k_Power = 500f;
    float m_Existential;    //球在场上的时长
    float m_LateralSpeed;   //侧向速度
    float m_ForwardSpeed;   //前向速度

    //组件
    [HideInInspector]
    public Rigidbody agentRb;
    SoccerSettings m_SoccerSettings;
    BehaviorParameters m_BehaviorParameters;
    public Vector3 initialPos;
    public float rotSign;

    EnvironmentParameters m_ResetParams;

    public override void Initialize()
    {
        SoccerEnvController envController = GetComponentInParent<SoccerEnvController>();
        if(envController != null)
        {
            m_Existential = 1f / envController.MaxEnvironmentSteps;
        }
        else
        {
            m_Existential = 1f / MaxStep;
        }

        m_BehaviorParameters = gameObject.GetComponent<BehaviorParameters>();
        if(m_BehaviorParameters.TeamId == (int)Team.Blue)
        {
            team = Team.Blue;
            initialPos = new Vector3(transform.position.x, 1.15f, transform.position.z);
            rotSign = 1f;
        }
        else
        {
            team = Team.Red;
            initialPos = new Vector3(transform.position.x, 1.15f, transform.position.z);
            rotSign = -1f;
        }

        //各个球员的初始化速度
        if(position == Position.Forward)
        {
            m_LateralSpeed = 0.7f;
            m_ForwardSpeed = 1.3f;
        }
        else if (position == Position.Middle)
        {
            m_LateralSpeed = 1f;
            m_ForwardSpeed = 1f;
        }
        else if(position == Position.Back)
        {
            m_LateralSpeed = 1.1f;
            m_ForwardSpeed = 0.9f;
        }
        else if(position == Position.Goalkeeper)
        {
            m_LateralSpeed = 1.5f;
            m_ForwardSpeed = 0.5f;
        }
        else
        {
            m_LateralSpeed = 1f;
            m_ForwardSpeed = 1f;
        }

        m_SoccerSettings = FindObjectOfType<SoccerSettings>();
        agentRb = GetComponent<Rigidbody>();
        agentRb.maxAngularVelocity = 500f;

        m_ResetParams = Academy.Instance.EnvironmentParameters;
    }

    //运动逻辑
    public void MoveAgent(ActionSegment<int> act)
    {
        Vector3 dirToGo = Vector3.zero;
        Vector3 rotateDir = Vector3.zero;

        m_KickPower = 0f;

        var forwardAxis = act[0];
        var rightAxis = act[1];
        var rotateAxis = act[2];

        switch (forwardAxis)
        {
            case 1:
                dirToGo = transform.forward * m_ForwardSpeed;
                m_KickPower = 1f;
                break;
            case 2:
                dirToGo = transform.forward * -m_ForwardSpeed;
                break;
        }

        switch (rightAxis)
        {
            case 1:
                dirToGo = transform.right * m_LateralSpeed;
                break;
            case 2:
                dirToGo = transform.right * -m_LateralSpeed;
                break;
        }

        switch (rotateAxis)
        {
            case 1:
                rotateDir = transform.up * -1f;
                break;
            case 2:
                rotateDir = transform.up * 1f;
                break;
        }

        transform.Rotate(rotateDir, Time.deltaTime * 100f);

        agentRb.AddForce(dirToGo * m_SoccerSettings.agentRunSpeed,
            ForceMode.VelocityChange);
    }

    //每次接收到操作的update函数
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        if (position == Position.Goalkeeper)
        {
            AddReward(m_Existential);
        }
        else if(position == Position.Back)
        {
            AddReward(m_Existential);
        }
        else if (position == Position.Forward)
        {
            AddReward(-m_Existential);
        }
        else if (position == Position.Middle)
        {
            AddReward(-m_Existential);
        }

        MoveAgent(actionBuffers.DiscreteActions);
    }

    //操作测试
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        //forward
        if (Input.GetKey(KeyCode.W))
        {
            discreteActionsOut[0] = 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            discreteActionsOut[0] = 2;
        }
        //right
        if (Input.GetKey(KeyCode.A))
        {
            discreteActionsOut[1] = 2;
        }
        if (Input.GetKey(KeyCode.D))
        {
            discreteActionsOut[1] = 1;
        }
        //rotate
        if (Input.GetKey(KeyCode.E))
        {
            discreteActionsOut[2] = 1;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            discreteActionsOut[2] = 2;
        }
    }

    //计算踢球的力
    private void OnCollisionEnter(Collision c)
    {
        var force = k_Power * m_KickPower;
        
        if(c.gameObject.CompareTag("ball")) //踢到球
        {
            AddReward(.2f * m_BallTouch);
            var dir = c.contacts[0].point - transform.position;
            dir = dir.normalized;
            c.gameObject.GetComponent<Rigidbody>().AddForce(dir * force);
        }
        else if(c.gameObject.tag == transform.gameObject.tag)//碰到自己人
        {
            AddReward(-0.2f);
        }
        else if(c.gameObject.CompareTag("wall"))
        {
            AddReward(-0.2f);
        }
        else if(c.gameObject.CompareTag("redEndline"))
        {

        }
        else if(c.gameObject.CompareTag("blueEndline"))
        {

        }
    }
    public override void OnEpisodeBegin()
    {
        m_BallTouch = m_ResetParams.GetWithDefault("ball_touch", 0);
    }
}