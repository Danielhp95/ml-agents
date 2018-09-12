using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class AgentSoccer : Agent
{

    public enum Team
    {
        red, blue
    }
    public enum AgentRole
    {
        striker, goalie
    }
    public Team team;
    public AgentRole agentRole;
    float kickPower;
    int playerIndex;
    public SoccerFieldArea area;
    [HideInInspector]
    public Rigidbody agentRB;
    SoccerAcademy academy;
    Renderer agentRenderer;
    RayPerception rayPer;

    public float moveSpeed = 1f;
    public float rotationSpeed = 1f;
    public float strikerStrafeSpeed = 1f;

    public void ChooseRandomTeam()
    {
        team = (Team)Random.Range(0, 2);
        agentRenderer.material = team == Team.red ? academy.redMaterial : academy.blueMaterial;
    }

    public void JoinRedTeam(AgentRole role)
    {
        agentRole = role;
        team = Team.red;
        agentRenderer.material = academy.redMaterial;
    }

    public void JoinBlueTeam(AgentRole role)
    {
        agentRole = role;
        team = Team.blue;
        agentRenderer.material = academy.blueMaterial;
    }

    public override void InitializeAgent()
    {
        base.InitializeAgent();
        agentRenderer = GetComponent<Renderer>();
        rayPer = GetComponent<RayPerception>();
        academy = FindObjectOfType<SoccerAcademy>();
        PlayerState playerState = new PlayerState();
        playerState.agentRB = GetComponent<Rigidbody>();
        agentRB = GetComponent<Rigidbody>();
        agentRB.maxAngularVelocity = 500;
        playerState.startingPos = transform.position;
        playerState.agentScript = this;
        area.playerStates.Add(playerState);
        playerIndex = area.playerStates.IndexOf(playerState);
        playerState.playerIndex = playerIndex;
    }

    // Here you define your agent's eyes: what do you allow your agent to see.
    // Here we will use raytracing
    public override void CollectObservations()
    {
        float rayDistance = 20f;
        float[] rayAngles = { 0f, 45f, 90f, 135f, 180f, 110f, 70f };
        string[] detectableObjects;
        if (team == Team.red)
        {
            detectableObjects = new string[] { "ball" };
        }
        else
        {
            detectableObjects = new string[] { "ball" };
        }
        AddVectorObs(rayPer.Perceive(rayDistance, rayAngles, detectableObjects, 0f, 0f));
        AddVectorObs(rayPer.Perceive(rayDistance, rayAngles, detectableObjects, 1f, 0f));
    }
    
    public void MoveAgent(int action)
    {


        // Goalies and Strikers have slightly different action spaces.
        if (agentRole == AgentRole.goalie)
        {

        }
        else
        {

        }
    }

    private void TakeGoalieAction (int action)
    {

    }

    private void TakeStrikerAction (int action)
    {

    }

    private Vector3 GetTranslation(int action)
    {
        return new Vector3();
    }

    private Vector3 GetRotation(int action)
    {
        return new Vector3();
    }
    
    // If the agent is moving forward set the kick force to 1f else, set it to 0f
    private void IsKicking(int action)
    {

    }

    // Given an int in [-1... 6), take the appropriate action.
    // For strikers
    // -1: no action, 0: move forward, 1: move backward, 2: move left; 3: move right, 4: rotate left, 5: rotate right
    // For Goalies
    // -1: no action, 0: move forward, 1: move backward, 2: move left; 3: move right.
    //
    // We will also add some rewards here eventually.
    public override void AgentAction(float[] vectorAction, string textAction)
    {
        // Our action is received as an array of floats for more complicated scenarios
        // All we care about here is the first action as an int, however
        int action = Mathf.FloorToInt(vectorAction[0]);

    }

    /// <summary>
    /// Used to provide a "kick" to the ball.
    /// </summary>
    void OnCollisionEnter(Collision c)
    {
        float force = 2000f * kickPower;
        if (c.gameObject.tag == "ball")
        {
            Vector3 dir = c.contacts[0].point - transform.position;
            dir = dir.normalized;
            c.gameObject.GetComponent<Rigidbody>().AddForce(dir * force);
        }
    }


    public override void AgentReset()
    {
        if (academy.randomizePlayersTeamForTraining)
        {
            ChooseRandomTeam();
        }

        if (team == Team.red)
        {
            JoinRedTeam(agentRole);
            transform.rotation = Quaternion.Euler(0f, -90f, 0f);
        }
        else
        {
            JoinBlueTeam(agentRole);
            transform.rotation = Quaternion.Euler(0f, 90f, 0f);
        }
        transform.position = area.GetRandomSpawnPos(team.ToString(),
                                                    agentRole.ToString());
        agentRB.velocity = Vector3.zero;
        agentRB.angularVelocity = Vector3.zero;
        area.ResetBall();
    }

    public override void AgentOnDone()
    {

    }
}
