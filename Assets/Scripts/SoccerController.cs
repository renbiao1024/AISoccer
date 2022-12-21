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

    public string redGoalTag;
    public string blueGoalTag;

    private void Start()
    {
        envController = area.GetComponent<SoccerEnvController>();
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag(redGoalTag))//进了红队球门
        {
            //envController.GoalTouched(Team.Blue);
        } 
        if (col.gameObject.CompareTag(blueGoalTag)) 
        {
            //envController.GoalTouched(Team.Purple);
        }
    }
}