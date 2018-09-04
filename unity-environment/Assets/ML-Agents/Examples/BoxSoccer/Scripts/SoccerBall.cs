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

    // Check collisions in order to add appropriate rewards to the agents
    // and in order to know when to reset the scenario.
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("iWall"))
        {
        }
    }

    private void Reset()
    {
        agentA.Done();
        agentB.Done();
        area.MatchReset();
    }
}
