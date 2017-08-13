using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class TankShooting : NetworkBehaviour
{
    public int m_PlayerNumber = 1;            // Used to identify the different players.
    public Rigidbody m_Shell;                 // Prefab of the shell.
    public Transform m_FireTransform;         // A child of the tank where the shells are spawned.
    public Slider m_AimSlider;                // A child of the tank that displays the current launch force.
    public AudioSource m_ShootingAudio;       // Reference to the audio source used to play the shooting audio. NB: different to the movement audio source.
    public AudioClip m_ChargingClip;          // Audio that plays when each shot is charging up.
    public AudioClip m_FireClip;              // Audio that plays when each shot is fired.
    public float m_MinLaunchForce = 20f;      // The force given to the shell if the fire button is not held.
    public float m_MaxLaunchForce = 100f;      // The force given to the shell if the fire button is held for the max charge time.
    public float m_MaxChargeTime = 0.75f;     // How long the shell can charge for before it is fired at max force.
    public Transform turret;                  // Rotating the turret

    [SyncVar]
    public int m_localID;

    [HideInInspector]
    [SyncVar(hook = "OnTurretRotation")]
    public int turretRotation;

    private string m_FireButton;            // The input axis that is used for launching shells.
    private Rigidbody m_Rigidbody;          // Reference to the rigidbody component.
    [SyncVar]
    private float m_LaunchForce;     // The force that will be given to the shell when the fire button is released.
    [SyncVar]
    private float m_ChargeSpeed;            // How fast the launch force increases, based on the max charge time.
    private bool m_Fired;                   // Whether or not the shell has been launched with this button press.
    private bool m_wasFired;
    private float nextRotate;
    private Vector3 targetFirePosition;

    private float waitSeconds = 5f;

    private void Awake()
    {
        // Set up the references.
        m_Rigidbody = GetComponent<Rigidbody>();
    }


    private void Start()
    {
        // The fire axis is based on the player number.
        m_FireButton = "Fire" + (m_localID + 1);        
    }

    [ClientCallback]
    private void Update()
    {
        if (!isLocalPlayer)
            return;
        
    }
    
    private void FixedUpdate()
    {
        if (!isLocalPlayer)
        {
            OnTurretRotation(turretRotation);
            return;
        }
        Rotate();

        if (Input.GetButtonDown(m_FireButton))
        {
            // ... launch the shell.
            Fire();
        }
    }

    private void Rotate()
    {
        Vector2 turnDir;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, Vector3.up);
        float distance = 0f;
        Vector3 hitPos = Vector3.zero;

        if (plane.Raycast(ray, out distance))
        {
            hitPos = ray.GetPoint(distance) - transform.position;
        }

        turnDir = new Vector2(hitPos.x, hitPos.z);
        RotateTurret(new Vector2(hitPos.x, hitPos.z));
    }

    private void RotateTurret(Vector2 direction = default(Vector2))
    {
        if (direction == Vector2.zero)
            return;

        int newRotation = (int)(Quaternion.LookRotation(new Vector3(direction.x, 0, direction.y)).eulerAngles.y);
        if(Time.time >= nextRotate && (Mathf.Abs(Mathf.DeltaAngle(turretRotation,newRotation)) > 5))
        {
            nextRotate = Time.time + 0.1f;
            turretRotation = newRotation;
            CmdRotateTurret(newRotation);
        }

        turret.rotation = Quaternion.Euler(0, newRotation, 0);
    }



    [Command]
    void CmdRotateTurret(int value)
    {
        turretRotation = value;
    }

    private void Fire()
    {
        int randSeed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        // Change the clip to the firing clip and play it.
        m_ShootingAudio.clip = m_FireClip;
        m_ShootingAudio.Play();
        
        CmdFire(m_LaunchForce, m_FireTransform.forward, m_FireTransform.position, m_FireTransform.rotation);

        // Reset the launch force.  This is a precaution in case of missing button events.
        
        
    }

    [Command]
    private void CmdFire(float launchForce, Vector3 forward, Vector3 position, Quaternion rotation)
    {
        // Create an instance of the shell and store a reference to it's rigidbody.
        Rigidbody shellInstance =
             Instantiate(m_Shell, position, rotation) as Rigidbody;

        // Set the shell's velocity to this velocity.
        shellInstance.velocity = forward * launchForce;

        NetworkServer.Spawn(shellInstance.gameObject);
    }

    void OnTurretRotation(int value)
    {
        if (isLocalPlayer) return;
        turretRotation = value;
        turret.rotation = Quaternion.Euler(0, turretRotation, 0);
    }

    // This is used by the game manager to reset the tank.
    public void SetDefaults()
    {
        m_LaunchForce = 20f;
        m_AimSlider.value = 0f;
    }
}