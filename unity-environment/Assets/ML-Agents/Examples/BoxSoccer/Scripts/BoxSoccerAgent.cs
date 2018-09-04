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

    private Rigidbody agentRigidBody;
    private Rigidbody ballRigidBody;
    private Rigidbody opponentRigidBody;
    // This exists so that each agent sees it's world in the same way: if an agent is attacking right-left on-screen
    // it should be be able to use the same policy as an agent attacking left-right.
    private float invertMult;


    public override void InitializeAgent()
    {
        agentRigidBody = GetComponent<Rigidbody>();
        ballRigidBody = ball.GetComponent<Rigidbody>();
        opponentRigidBody = opponent.GetComponent<Rigidbody>();
    }

    // Collect observations from the environment.
    // These observations will inform the decisions made by the agent.
    public override void CollectObservations()
    {
        AddVectorObs(invertMult * (transform.position.x - myArea.transform.position.x));
        AddVectorObs(invertMult * agentRigidBody.velocity.x);
    }

    // Given an int in [0... 4), take the appropriate action.
    // 0: no action, 1: move left, 2: move right; 3: jump.
    public override void AgentAction(float[] vectorAction, string textAction)
    {
        // `action` here is the int in [0... 4) which represents our action.
        // Other arguments to this function are for more complex environments than ours.
        int action = Mathf.FloorToInt(vectorAction[0]);
    }

    // Take the jump action.
    private void Jump()
    {
    }

    // Given an int that represents an action, determine which x-axis direction it moves.
    private static int GetDirection(int action)
    {
        int direction = 0;

        return direction;
    }
    
    // Take the move action in the specified direction.
    private void Move(int direction)
    {
    }

    public override void AgentReset()
    {
        invertMult = invertX ? -1f : 1f;

        transform.position = new Vector3(-invertMult * Random.Range(6f, 8f), -1.5f, 0f) + transform.parent.transform.position;
        agentRigidBody.velocity = new Vector3(0f, 0f, 0f);
        BoxSoccerArea area = myArea.GetComponent<BoxSoccerArea>();
        area.MatchReset();
    }
}
