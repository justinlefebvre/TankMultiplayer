using UnityEngine;
using UnityEngine.UI;


public class o_TankShooting : MonoBehaviour
{
    public int m_PlayerNumber = 1;              // Used to identify the different players.
    public Rigidbody m_Shell;                   // Prefab of the shell.
    public Transform m_FireTransform;           // A child of the tank where the shells are spawned.
    public Slider m_AimSlider;                  // A child of the tank that displays the current launch force.
    public AudioSource m_ShootingAudio;         // Reference to the audio source used to play the shooting audio. NB: different to the movement audio source.
    public AudioClip m_ChargingClip;            // Audio that plays when each shot is charging up.
    public AudioClip m_FireClip;                // Audio that plays when each shot is fired.
    public float m_MinLaunchForce = 20f;        // The force given to the shell if the fire button is not held.
    public float m_MaxLaunchForce = 70f;        // The force given to the shell if the fire button is held for the max charge time.
    public float m_MaxChargeTime = 0.75f;       // How long the shell can charge for before it is fired at max force.
    public Transform turret;                    //


    private string m_FireButton;                // The input axis that is used for launching shells.
    private float m_CurrentLaunchForce;         // The force that will be given to the shell when the fire button is released.
    private float m_ChargeSpeed;                // How fast the launch force increases, based on the max charge time.
    private bool m_Fired;                       // Whether or not the shell has been launched with this button press.
    private Vector3 targetFirePosition;


    private void OnEnable()
    {
        // When the tank is turned on, reset the launch force and the UI
        m_CurrentLaunchForce = m_MaxLaunchForce;
        m_AimSlider.value = m_MinLaunchForce;
    }


    private void Start ()
    {
        // The fire axis is based on the player number.
        m_FireButton = "Fire" + m_PlayerNumber;

        // The rate that the launch force charges up is the range of possible forces by the max charge time.
        m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime;
    }

    private void FixedUpdate() {
        Vector2 turnDir;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, Vector3.up);
        float distance = 0f;
        Vector3 hitPos = Vector3.zero;

        if(plane.Raycast(ray,out distance)) {
            hitPos = ray.GetPoint(distance) - transform.position;
        }

        turnDir = new Vector2(hitPos.x, hitPos.z);     



        RotateTurret(new Vector2(hitPos.x, hitPos.z));
    }

    private void Update ()
    {

        if (Input.mousePresent)
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            float hitDist;
            RaycastHit hit;
            int groundLayer = LayerMask.GetMask("Ground");
            Plane floor = new Plane(Vector3.up, 0);

            if(Physics.Raycast(mouseRay, out hit, float.PositiveInfinity, groundLayer))
            {
                targetFirePosition = hit.point;
            } else if(floor.Raycast(mouseRay, out hitDist))
            {
                targetFirePosition = mouseRay.GetPoint(hitDist);
            }

        }
        // The slider should have a default value of the minimum launch force.
        m_AimSlider.value = m_MinLaunchForce;
        // If the max force has been exceeded and the shell hasn't yet been launched...
        if (m_CurrentLaunchForce <= m_MinLaunchForce && !m_Fired)
        {
            // ... use the max force and launch the shell.
            m_CurrentLaunchForce = m_MinLaunchForce;
            Fire ();
        }
        // Otherwise, if the fire button has just started being pressed...
        else if (Input.GetButtonDown (m_FireButton))
        {
            // ... reset the fired flag and reset the launch force.
            m_Fired = false;
            m_CurrentLaunchForce = m_MaxLaunchForce;

            // Change the clip to the charging clip and start it playing.
            m_ShootingAudio.clip = m_ChargingClip;
            m_ShootingAudio.Play ();
        }
        // Otherwise, if the fire button is being held and the shell hasn't been launched yet...
        else if (Input.GetButton(m_FireButton) && !m_Fired)
        {
            // Increment the launch force and update the slider.
            m_CurrentLaunchForce -= m_ChargeSpeed * Time.deltaTime;

            float aimValue = m_Fired ? m_MaxLaunchForce : m_CurrentLaunchForce;
            m_AimSlider.value = m_MaxLaunchForce - aimValue + m_MinLaunchForce;
        }
        // Otherwise, if the fire button is released and the shell hasn't been launched yet...
        else if (Input.GetButtonUp (m_FireButton) && !m_Fired)
        {
            // ... launch the shell.
            Fire ();
        }
    }


    private void Fire ()
    {
        // Set the fired flag so only Fire is only called once.
        m_Fired = true;
        
        Vector3 fireVector = FiringLogic.CalculateFireVector(targetFirePosition, m_FireTransform.position, m_CurrentLaunchForce);

        int randSeed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);

        FireVisualShell(fireVector, m_FireTransform.position, randSeed);
        
        
        // Change the clip to the firing clip and play it.
        m_ShootingAudio.clip = m_FireClip;
        m_ShootingAudio.Play ();

        // Reset the launch force.  This is a precaution in case of missing button events.
        m_CurrentLaunchForce = m_MinLaunchForce;
    }

    private void  RotateTurret(Vector2 direction = default(Vector2)) {
        if (direction == Vector2.zero)
            return;

        int newRotation = (int)(Quaternion.LookRotation(new Vector3(direction.x, 0, direction.y)).eulerAngles.y);

        turret.rotation = Quaternion.Euler(0, newRotation, 0);
    }

    private void FireVisualShell(Vector3 shotVector, Vector3 position,int randSeed) {
        Rigidbody shellInstance = Instantiate<Rigidbody>(m_Shell);


        shellInstance.transform.position = position;
        shellInstance.velocity = shotVector;
    }


}
