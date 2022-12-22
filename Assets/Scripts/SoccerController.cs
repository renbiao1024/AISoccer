/****************************************************
    文件：SoccerController.cs
    作者：Biu
    邮箱: 1024906432@qq.com
    功能：传递足球状态
*****************************************************/

using UnityEngine;

public class SoccerController : MonoBehaviour 
{
    public GameObject area;

    [HideInInspector]
    public SoccerEnvController envController;

    public Collision lastAgent;

    private void Start()
    {
        envController = area.GetComponent<SoccerEnvController>();
    }

    private void OnCollisionEnter(Collision col)
    {
        //TODO:不同碰撞事件
        if (col.gameObject.CompareTag("redGoal"))//进了红队球门
        {
            envController.GoalTouched(Team.Blue);
        } 
        else if (col.gameObject.CompareTag("blueGoal")) 
        {
            envController.GoalTouched(Team.Red);
        }
        else if(col.gameObject.CompareTag("wall"))
        {
            lastAgent.gameObject.GetComponent<AgentSoccer>().AddReward(-0.2f);
        }
        else if(col.gameObject.CompareTag("redEndline"))
        {
            if(lastAgent.gameObject.CompareTag("redAgent"))
            {
                
            }
        }


    }
}