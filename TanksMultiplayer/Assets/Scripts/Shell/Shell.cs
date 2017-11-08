using UnityEngine;



[RequireComponent(typeof(Rigidbody))]
//This class defines the behaviour of all shells in the game.
public class Shell : MonoBehaviour
{
	//Variables for controlling shell bounciness - how many times to bounce, force decay per bounce, and bounce direction
	[SerializeField]
	protected int m_Bounces = 0;
	[SerializeField]
	protected float m_BounceForceDecay = 1.05f;
	[SerializeField]
	protected Vector3 m_BounceAdditionalForce = Vector3.up;

	//Minimum height that a shell can be in world coordinates before self-destructing.
	[SerializeField]
	protected float m_MinY = -1;

	//Modifier for shell velocity
	[SerializeField]
	protected float m_SpeedModifier = 1;

	//Modifier for the shell's spin rate.
	[SerializeField]
	protected float m_Spin = 720;

	//Number of physics ticks that the shell ignores the tempignore collider.
	[SerializeField]
	protected int m_IgnoreColliderFixedFrames = 2;

	//internal reference to ignore collider and ignore ticks.
	private Collider m_TempIgnoreCollider;
	private int m_TempIgnoreColliderTime = 2;

	//Has this shell exploded?
	private bool m_Exploded;

	//The unique index of the player who fired this projectile for score purposes
	private int m_OwningPlayerId = -1;

	//Random seed for spawning debris.
	private int m_RandSeed;

	//The current rotation of the shell.
	private float m_CurrentSpinRot;

	//Internal reference to this shell's rigidbody
	private Rigidbody m_CachedRigidBody;

	//Internal list of trail renderers attached to this shell
	private TrailRenderer[] m_ShellTrails;

	//Accessor for speed modifier
	public float speedModifier
	{
		get { return m_SpeedModifier; }
	}


	public int owningPlayerId
	{
		get { return m_OwningPlayerId; }
	}

	private void Awake()
	{
		m_CachedRigidBody = GetComponent<Rigidbody>();
		m_Exploded = false;

		//Scan all children to find all trailrenderer objects attached to this shell.
		m_ShellTrails = GetComponentsInChildren<TrailRenderer>();
	}

	public void Setup(int owningPlayerId, Collider ignoreCollider, int seed)
	{
		this.m_OwningPlayerId = owningPlayerId;

		if (ignoreCollider != null)
		{
			// Ignore collisions temporarily
			Physics.IgnoreCollision(GetComponent<Collider>(), ignoreCollider, true);
			m_TempIgnoreCollider = ignoreCollider;
			m_TempIgnoreColliderTime = m_IgnoreColliderFixedFrames;
		}

		m_RandSeed = seed;

		// If we have a speed modifier, add an extra constant gravitational force to us
		if (m_SpeedModifier != 1)
		{
			ConstantForce force = gameObject.AddComponent<ConstantForce>();
			force.force = Physics.gravity * m_CachedRigidBody.mass * (m_SpeedModifier - 1);
		}

		transform.forward = m_CachedRigidBody.velocity;
	}

	private void FixedUpdate()
	{
		//If we have an ignore collider, deplete the count and cancel our collision ignorance when it's zero.
		if (m_TempIgnoreCollider != null)
		{
			m_TempIgnoreColliderTime--;
			if (m_TempIgnoreColliderTime <= 0)
			{
				Physics.IgnoreCollision(GetComponent<Collider>(), m_TempIgnoreCollider, false);
				m_TempIgnoreCollider = null;
			}
		}
	}
			
	// Face towards our movement direction
	private void Update()
	{
		transform.forward = m_CachedRigidBody.velocity;

		// Spin the projectile
		m_CurrentSpinRot += m_Spin * Time.deltaTime * m_CachedRigidBody.velocity.magnitude;
		transform.Rotate(Vector3.forward, m_CurrentSpinRot, Space.Self);

		// Enforce minimum y, in case we go out of bounds
		if (transform.position.y <= m_MinY)
		{
			Destroy(gameObject);
		}
		else
		{
			// Reset this. We can set it to true during bounces to stop multiple colliders hitting it per frame
			m_Exploded = false;
		}
	}
			
	// Create explosions on collision
	private void OnCollisionEnter(Collision c)
	{
		if (m_Exploded)
		{
			return;
		}

		m_Exploded = true;
	}

	private void OnDestroy()
	{
		//On destruction, decouple any trail objects that still exist from this object. 
		//This allows them to fizzle out naturally and self-destruct instead of popping out of existence when at full length.
		for(int i = 0; i < m_ShellTrails.Length; i++)
		{
			TrailRenderer trail = m_ShellTrails[i];
			if (trail != null)
			{
				trail.transform.SetParent(null);
				trail.autodestruct = true;
			}
		}
	}
}
