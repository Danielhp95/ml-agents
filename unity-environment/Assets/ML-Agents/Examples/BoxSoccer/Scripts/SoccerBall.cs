using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoccerBall : MonoBehaviour
{
    public GameObject areaObject;
    public int lastAgentHit;

    private BoxSoccerArea area;
    private BoxSoccerAgent agentA;
    private BoxSoccerAgent agentB;

    // Use this for initialization
    void Start()
    {
        area = areaObject.GetComponent<BoxSoccerArea>();
        agentA = area.agentA.GetComponent<BoxSoccerAgent>();
        agentB = area.agentB.GetComponent<BoxSoccerAgent>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("iWall"))
        {
            if (collision.gameObject.name == "goalB")
            {
                agentA.AddReward(-1f);
                agentB.AddReward(1f);
                Reset();
            }
            else if (collision.gameObject.name == "goalA")
            {
                agentA.AddReward(1f);
                agentB.AddReward(-1f);
                Reset();
            }
        }

        if (collision.gameObject.CompareTag("agent"))
        {
            lastAgentHit = collision.gameObject.name == "BoxA" ? 0 : 1;
            if(lastAgentHit == 0)
            {
                agentA.AddReward(0.05f);
            } else
            {
                agentB.AddReward(0.05f);
            }
        }
    }

    private void Reset()
    {
        agentA.Done();
        agentB.Done();
        area.MatchReset();
    }
}
