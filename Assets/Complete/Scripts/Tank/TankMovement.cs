using UnityEngine;

namespace Complete
{
    public class TankMovement : MonoBehaviour
    {
        public int m_PlayerNumber = 1;              // Used to identify which tank belongs to which player.  This is set by this tank's manager.
        public float m_Speed = 2f;                 // How fast the tank moves forward and back.
        public float m_TurnSpeed = 180f;            // How fast the tank turns in degrees per second.
        public AudioSource m_MovementAudio;         // Reference to the audio source used to play engine sounds. NB: different to the shooting audio source.
        public AudioClip m_EngineIdling;            // Audio to play when the tank isn't moving.
        public AudioClip m_EngineDriving;           // Audio to play when the tank is moving.
        public float m_PitchRange = 0.2f;           // The amount by which the pitch of the engine noises can vary.


        private string m_MovementAxisName;          // The name of the input axis for moving forward and back.
        private string m_TurnAxisName;              // The name of the input axis for turning.
        private Rigidbody m_Rigidbody;              // Reference used to move the tank.
        private float m_MovementInputValue;         // The current value of the movement input.
        private float m_TurnInputValue;             // The current value of the turn input.
        private float m_OriginalPitch;              // The pitch of the audio source at the start of the scene.

        private float m_vertical;
        private float m_horizontal;
        private string m_JumpButton;

        public float rotationSpeed = 5;
        public float jumpSpeed = 500;
        private Vector3 inputVec;
        private Quaternion rotate;
        private Vector3 angle;
        private Vector3 target;
        private bool isTop = false;
        private bool moveStart = false;
        private Vector3 moveForward;

        private float x;
        private float z;
        private float dx;
        private float dz;

        private float dampSpeed = 4;

        private float gravity = 25;

        private void Awake()
        {
            m_Rigidbody = GetComponent<Rigidbody>();
        }


        private void OnEnable()
        {
            // When the tank is turned on, make sure it's not kinematic.
            m_Rigidbody.isKinematic = false;

            // Also reset the input values.
            m_MovementInputValue = 0f;
            m_TurnInputValue = 0f;
        }


        private void OnDisable()
        {
            // When the tank is turned off, set it to kinematic so it stops moving.
            m_Rigidbody.isKinematic = true;
        }


        private void Start()
        {
            // The axes names are based on player number.
            m_MovementAxisName = "Vertical" + m_PlayerNumber;
            m_TurnAxisName = "Horizontal" + m_PlayerNumber;

            m_JumpButton = "Fire" + m_PlayerNumber;

            // Store the original pitch of the audio source.
            m_OriginalPitch = m_MovementAudio.pitch;
            // rotate = new Quaternion(0,0.38f,0,0.92f);
            rotate = m_Rigidbody.rotation;
            angle = new Vector3(0, 0, 0);
            inputVec = new Vector3(0, 0, 0);
        }


        private void Update()
        {
            // Store the value of both input axes.
            m_MovementInputValue = Input.GetAxis(m_MovementAxisName);
            m_TurnInputValue = Input.GetAxis(m_TurnAxisName);

            inputVec.Set(-m_MovementInputValue, 0, m_TurnInputValue);


            if (Input.GetButtonDown(m_JumpButton))
            {
                if (m_PlayerNumber == 1)
                    Debug.Log("输出" + m_PlayerNumber + " :" + m_TurnInputValue);
                Jump();
                isTop = true;
            }
            else
            {
                if (m_Rigidbody.velocity.y <= 1 && isTop)
                {
                    isTop = false;
                    // m_Rigidbody.velocity = Vector3.down * (jumpSpeed / 100);
                    Debug.Log("okkkkkkk");
                }

            }

            m_vertical = Input.GetAxisRaw(m_MovementAxisName);
            m_horizontal = Input.GetAxisRaw(m_TurnAxisName);
            if (!moveStart && (Input.GetButton(m_MovementAxisName) || Input.GetButton(m_TurnAxisName)))
            {
                Debug.Log("moveStart :" + moveStart);
                moveStart = true;
                target = new Vector3(m_Rigidbody.transform.localPosition.x + m_vertical, m_Rigidbody.transform.localPosition.y, m_Rigidbody.transform.localPosition.z - m_horizontal);
            }

            if (m_vertical != 0)
            {
                if (dx * m_vertical < 0)
                    x = 0f;
                z = 0f;
                dx = m_vertical * dampSpeed * Time.deltaTime;
                dz = 0f;

            }

            if (m_horizontal != 0)
            {
                x = 0f;
                if (dz * m_horizontal > 0)
                    z = 0f;
                dx = 0f;
                dz = -m_horizontal * dampSpeed * Time.deltaTime;
            }
            if (m_vertical != 0 && m_horizontal != 0)
            {
                dx = 0f;
                dz = 0f;
            }



            EngineAudio();
        }


        private void EngineAudio()
        {
            // If there is no input (the tank is stationary)...
            if (Mathf.Abs(m_MovementInputValue) < 0.1f && Mathf.Abs(m_TurnInputValue) < 0.1f)
            {
                // ... and if the audio source is currently playing the driving clip...
                if (m_MovementAudio.clip == m_EngineDriving)
                {
                    // ... change the clip to idling and play it.
                    m_MovementAudio.clip = m_EngineIdling;
                    m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                    m_MovementAudio.Play();
                }
            }
            else
            {
                // Otherwise if the tank is moving and if the idling clip is currently playing...
                if (m_MovementAudio.clip == m_EngineIdling)
                {
                    // ... change the clip to driving and play.
                    m_MovementAudio.clip = m_EngineDriving;
                    m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                    m_MovementAudio.Play();
                }
            }
        }


        private void FixedUpdate()
        {
            if (m_PlayerNumber != 1)
                    return;
            //增加重力方法中的一种
            m_Rigidbody.AddForce(Vector3.down * gravity);

            // float angle = Vector3.Angle(new Vector3(m_MovementInputValue,0f,0f),new Vector3(0f,0f,m_TurnInputValue));
            // Vector3 angle = new Vector3(m_MovementInputValue, 0f, 0f) + new Vector3(0f, 0f, -m_TurnInputValue);
            // Vector3 angle = new Vector3(m_MovementInputValue, 0f, -m_TurnInputValue);
            angle.Set(m_MovementInputValue, 0f, -m_TurnInputValue);
            angle = Vector3.Normalize(angle);

            if (Mathf.Abs(x) < 1)
            {
                x += dx;
            }
            else
            {
                x = x / Mathf.Abs(x);
            }
            if (Mathf.Abs(z) < 1)
            {
                z += dz;
            }
            else
            {
                z = z / Mathf.Abs(z);
            }

            Debug.Log("x: " + x);
            moveForward.Set(x, 0, z);
            // Adjust the rigidbodies position and orientation in FixedUpdate.
            // MoveBlock(target);

            Move(moveForward);
            Turn(angle);
        }


        private void Move(Vector3 angle)
        {
            // Create a vector in the direction the tank is facing with a magnitude based on the input, speed and the time between frames.
            Vector3 movement = angle * m_Speed * Time.deltaTime;
            // Vector3 movement2 = Vector3.right * m_TurnInputValue * m_Speed * Time.deltaTime;


            // Apply this movement to the rigidbody's position.
            m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
            // Debug.Log("速度"+angle);
            // Vector3 movement2 = angle * m_Speed;
            // m_Rigidbody.velocity = angle; 
            

            // m_Rigidbody.transform.localPosition = Vector3.MoveTowards(m_Rigidbody.transform.localPosition,new Vector3(),m_Speed * Time.deltaTime);
        }

        private void MoveBlock(Vector3 angle)
        {

            m_Rigidbody.transform.localPosition = Vector3.MoveTowards(m_Rigidbody.transform.localPosition, angle, m_Speed * Time.deltaTime);

            Debug.Log(m_Speed * Time.deltaTime);
            if (Vector3.Distance(target, m_Rigidbody.transform.localPosition) <= m_Speed)
            {
                moveStart = false;
            }


        }


        private void Turn(Vector3 angle)
        {
            if (inputVec != Vector3.zero)
            {
                Quaternion quate = rotate * Quaternion.LookRotation(inputVec);
                // m_Rigidbody.transform.rotation = Quaternion.Slerp(m_Rigidbody.transform.rotation, quate, Time.deltaTime * rotationSpeed);
                m_Rigidbody.MoveRotation(Quaternion.Slerp(m_Rigidbody.transform.rotation, quate, Time.deltaTime * rotationSpeed));
            }
            // Determine the number of degrees to be turned based on the input, speed and time between frames.
            // float turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime;
            // Make this into a rotation in the y axis.
            // Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);

            // Apply this rotation to the rigidbody's rotation.
            // m_Rigidbody.MoveRotation(m_Rigidbody.rotation * turnRotation);

            // m_Rigidbody.MoveRotation(m_Rigidbody.rotation * turnRotation);
            // m_Rigidbody.rotation = Quaternion.FromToRotation(Quaternion.ToEulerAngles(m_Rigidbody.rotation),angle);
            // m_Rigidbody.MoveRotation(Quaternion.Slerp(m_Rigidbody.rotation,Quaternion.Euler(angle),Time.deltaTime));
        }

        private void Jump()
        {
            m_Rigidbody.velocity = Vector3.up * 0;
            Debug.Log("" + m_Rigidbody.velocity);

            m_Rigidbody.AddForce(Vector3.up * jumpSpeed);
        }

        void OnCollisionStay(Collision collision)
        {
            // Debug.Log("还在里面"+x+" z:"+z+" dx:"+dx+" dz:"+dz);
            // foreach (ContactPoint contact in collision.contacts)
            // {
            //     // print(contact.thisCollider.name + " hit " + contact.otherCollider.name);
            //     Debug.DrawRay(contact.point, contact.normal, Color.white);
            // }
            var parent = collision.gameObject.transform.parent;
            if (parent != null)
            {
                var pro = parent.gameObject.GetComponent<Property>();
                if (pro != null)
                {
                    var gb = pro.gameObject;
                    if (gb.tag != "LandGround")
                    {
                        ContactPoint contactPoint = collision.contacts[0];
                      
                        Debug.Log("出发");
                        //法线方向 朝上时 不进行速度重置
                        if (contactPoint.normal.y == 0)
                        {
                            x = 0;
                            z = 0;
                            dx = 0f;
                            dz = 0f;
                        }

                    }
                }
            }
        }
        private Vector3 m_preVelocity = Vector3.zero;//上一帧速度
        void OnCollisionEnter(Collision collision)
        {
          
            // foreach(var contacts in collision.contacts){
            //     Debug.DrawRay(contacts.point,contacts.normal,Color.green);
            // }
            // Debug.Log(collision.gameObject.GetComponentInParent<CompleteLevelArt>());
            // Debug.Log();
            // Debug.Log(collision.gameObject.transform.parent.gameObject.GetComponent<Property>());
            var parent = collision.gameObject.transform.parent;
            if (parent != null)
            {
                var pro = parent.gameObject.GetComponent<Property>();
                if (pro != null)
                {
                    var gb = pro.gameObject;
                    if (gb.tag != "LandGround")
                    {
                        ContactPoint contactPoint = collision.contacts[0];
                        // Vector3 newDir = Vector3.zero;
                        // Vector3 curDir = m_Rigidbody.transform.TransformDirection(Vector3.forward);
                        // Debug.Log("碰撞点:"+contactPoint);
                        // Debug.Log("法线："+contactPoint.normal);
                        // newDir = Vector3.Reflect(curDir, contactPoint.normal);
                        // Quaternion rotation = Quaternion.FromToRotation(Vector3.forward, newDir);
                        // m_Rigidbody.transform.rotation = rotation;
                        // m_Rigidbody.velocity = newDir.normalized * (m_preVelocity.x  / m_preVelocity.normalized.x);
                        //判断碰撞的方位
                        // if(collision.collider.collsionFlags)
                        Debug.Log("出发");
                        //法线方向 朝上时 不进行速度重置
                        if (contactPoint.normal.y == 0)
                        {
                            x = 0;
                            z = 0;
                            dx = 0f;
                            dz = 0f;
                        }

                    }
                }
            }
            // GameObject obj = collision.gameObject.transform.parent.gameObject.GetComponent<Property>().gameObject;
            // if (collision.gameObject.transform.parent.gameObject.tag != "LandGround")
            // {

            // }

        }

    }


}