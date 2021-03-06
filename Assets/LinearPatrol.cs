using UnityEngine;
using System.Collections;

public class LinearPatrol : MonoBehaviour {
	//destination relative to the drone
	public float xDistance, yDistance, velocity;
	public bool active;
	private Vector2 _destination, _home, _goal;
	// Use this for initialization
	void Start () {
		active = true;
		_goal = _destination = new Vector2(transform.position.x + xDistance, transform.position.y + yDistance);
		_home = (Vector2)transform.position;
        transform.right = _goal - (Vector2)transform.position;
    }
	
	/* This moves the object from left to right
     * so the object will be patrolling from left to right 
     */
	void Update () {
		if(active){
			//we aren't at our destination
			if((Vector2)transform.position != _goal){
				transform.position = Vector2.MoveTowards(transform.position, _goal, Time.smoothDeltaTime * velocity);	
            }
			else{
				changeDirection();
			}
		}
	}

	private void changeDirection(){
        if (VecEqual((Vector2)transform.position, _destination)){
			_goal = _home;
		}
		if(VecEqual((Vector2)transform.position, _home)){
			_goal = _destination;
		}
		//Debug.Log(_goal);
        transform.right = _goal - (Vector2)transform.position;
    }

	//calculate the magnitude of the distance to find their similarity. 0 being perfect
	//used to provide a margin of wiggle room for the patrolling enemies
	public bool VecEqual(Vector2 a, Vector2 b){
		return Vector2.SqrMagnitude(a - b) < .00001;
	}
		

}
