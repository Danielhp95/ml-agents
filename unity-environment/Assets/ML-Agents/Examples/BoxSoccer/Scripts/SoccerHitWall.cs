using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoccerHitWall : MonoBehaviour
{
    public GameObject areaObject;
    public int lastAgentHit;

    private TennisArea area;
    private BoxSoccerAgent agentA;
    private BoxSoccerAgent agentB;

    // Use this for initialization
    void Start()
    {
        area = areaObject.GetComponent<TennisArea>();
        agentA = area.agentA.GetComponent<BoxSoccerAgent>();
        agentB = area.agentB.GetComponent<BoxSoccerAgent>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "over")
        {
            if (lastAgentHit == 0)
            {
                agentA.AddReward(0.1f);
            }
            else
            {
                agentB.AddReward(0.1f);
            }
            lastAgentHit = 0;

        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("iWall"))
        {
            if (collision.gameObject.name == "goalA")
            {
                agentA.AddReward(-1f);
                agentB.AddReward(1f);
                Reset();
            }
            else if (collision.gameObject.name == "goalB")
            {
                agentA.AddReward(1f);
                agentB.AddReward(-1f);
                Reset();
            }
        }

        if (collision.gameObject.CompareTag("agent"))
        {
            lastAgentHit = collision.gameObject.name == "AgentA" ? 0 : 1;
        }
    }

    private void Reset()
    {
        agentA.Done();
        agentB.Done();
        area.MatchReset();
    }
}
