using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class BoxSoccerAgent : Agent
{
    [Header("Specific to BoxSooccer")]
    public GameObject ball;
    public GameObject myArea;
    public GameObject opponent;
    public bool invertX;
    public float jumpForce;
    public float groundMovementForce;
    public float maxSpeed;

    private Rigidbody agentRb;
    private Rigidbody ballRb;
    private float invertMult;


    public override void InitializeAgent()
    {
        agentRb = GetComponent<Rigidbody>();
        ballRb = ball.GetComponent<Rigidbody>();
    }

    public override void CollectObservations()
    {
        AddVectorObs(invertMult * (transform.position.x - myArea.transform.position.x));
        AddVectorObs(transform.position.y - myArea.transform.position.y);
        AddVectorObs(invertMult * agentRb.velocity.x);
        AddVectorObs(agentRb.velocity.y);

        AddVectorObs(invertMult * (ball.transform.position.x - myArea.transform.position.x));
        AddVectorObs(ball.transform.position.y - myArea.transform.position.y);
        AddVectorObs(invertMult * ballRb.velocity.x);
        AddVectorObs(ballRb.velocity.y);
    }


    public override void AgentAction(float[] vectorAction, string textAction)
    {
        int action = Mathf.FloorToInt(vectorAction[0]);
     
        int direction = GetDirection(action);
        if (action >= 4)
        {
            jump();
        }

        Vector3 directionVector = new Vector3(direction, 0, 0);
        if (System.Math.Abs( agentRb.velocity.x) < maxSpeed )
        {
            agentRb.AddForce(directionVector * groundMovementForce, ForceMode.VelocityChange);
        }
    }

    private static int GetDirection(int action)
    {
        int direction = 0;
        if (action % 3 == 2)
        {
            direction = 1;
        }
        else if (action % 3 == 0)
        {
            direction = -1;
        }

        return direction;
    }

    private void jump()
    {
        if (transform.position.y <= -2.75)
        {
            agentRb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
        }
    }

    public override void AgentReset()
    {
        invertMult = invertX ? -1f : 1f;

        transform.position = new Vector3(-invertMult * Random.Range(6f, 8f), -1.5f, 0f) + transform.parent.transform.position;
        agentRb.velocity = new Vector3(0f, 0f, 0f);
    }
}
