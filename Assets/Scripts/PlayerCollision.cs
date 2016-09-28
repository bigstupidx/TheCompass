//============================
//Amy Becerra
//Task Description: Create a collision function for the player that deals damage to the player when they collide with an asteroid and the asteroid has a particular velocity
//Last edited : 9/27/16
//============================

using UnityEngine;
using System.Collections;

public class PlayerCollision : MonoBehaviour 
{

	//Declaration of Variables
	private GameObject _asteroidInput;
	private bool _playerDamaged = false;
	private Rigidbody2D _asteroidRigidbody;
	private float _asteroidVelocity = 0f;
	private float _asteroidMinimum = 0f; //Minimum velocity of asteroid to deal damage to player, can be changed later
	private float _tempHealth = 100f;
	private float _asteroidDamageForce = 0f;
	const float _ASTEROIDFORCECONSTANT = 2f; //Can change later

	void Start () 
	{
		
	}

	void Update () 
	{
		
	}

	//Function that grabs velocity from the asteroid object and stores it in a var, applies damage to player if velocity is high enough, calculates a force, and subtracts that force from player health
	void OnCollisionEnter2D(Collision2D col)
	{
		if (col.gameObject.name == "AsteroidPlaceholder")
		{
			_asteroidInput = col.gameObject;
			_asteroidRigidbody = _asteroidInput.GetComponent<Rigidbody2D>( );
			_asteroidVelocity = _asteroidRigidbody.velocity.magnitude;
			if (_asteroidVelocity > _asteroidMinimum) //temporarily set to if the asteroid is moving, it deals damage automatically
			{
				_playerDamaged = true;
				_asteroidDamageForce = _asteroidVelocity * _ASTEROIDFORCECONSTANT; //Need requirements for damage 
				_tempHealth = _tempHealth - _asteroidDamageForce;
				print (_playerDamaged);
				print ("Velocity= " + _asteroidVelocity);
				print ("Player health= " + _tempHealth);
			}
		}
	}
}