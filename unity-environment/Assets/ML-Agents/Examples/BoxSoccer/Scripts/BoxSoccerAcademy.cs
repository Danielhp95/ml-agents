using MLAgents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxSoccerAcademy : Academy {
    public float gravityFactor = 1.5f;

	// Use this for initialization
	void Start () {
        Physics.gravity *= gravityFactor;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
