using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Player : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Vector3 input;
    [SerializeField] private Vector3 movement;
    [SerializeField] private float speed;
    [SerializeField] private float maxSpeed;
    [SerializeField] private SphereCollider collider;
    [SerializeField] private float currentVelocity;
    public int objsCollected = 0;

    [SerializeField] private CinemachineFreeLook cineCam;
    private float xInput, yInput;

    [SerializeField] private float playerSize = 1;

    List<GameObject> collection;

    public Rigidbody rb { get { return _rb; } set { _rb = value; } }

    private void Awake()
    {
        playerInput = new PlayerInput();
    }
    private void OnEnable()
    {
        playerInput.Enable();
    }

    private void OnDisable()
    {
        playerInput.Disable();
    }
    // Start is called before the first frame update
    void Start()
    {
        collection = new List<GameObject>();
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        //collider.radius = rb.mass;
        calInput();
        Vector3 cameraForwardDirection = cameraTransform.forward;
        Vector3 directionToMove = Vector3.Scale(cameraForwardDirection, (Vector3.right + Vector3.forward));

        movement = (input.x * cameraTransform.right) + (input.z * directionToMove);
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    private void calInput()
    {
        xInput = playerInput.Player.Movement.ReadValue<Vector2>().x;
        yInput = playerInput.Player.Movement.ReadValue<Vector2>().y;
        input = new Vector3(xInput, 0, yInput);
    }
    private void FixedUpdate()
    {
        currentVelocity = rb.velocity.magnitude;
        if(xInput == 0 && yInput == 0)
        {
            if(rb.velocity.magnitude > 0.5)
            {
                float newX = Mathf.Lerp(rb.velocity.x, 0, 0.01f);
                float newZ = Mathf.Lerp(rb.velocity.z, 0, 0.01f);
                rb.velocity = new Vector3(newX, rb.velocity.y, newZ);
            }
            else
            {
                rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, 0.1f);
            }
            if (rb.velocity.magnitude < 0.2f)
            {
                rb.velocity = Vector3.zero;
            }
        }
        if(rb.velocity.magnitude < maxSpeed)
        {
            rb.velocity += (movement * speed * Time.fixedDeltaTime);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Finish"))
        {
            Application.Quit();
        }
        if (collision.gameObject.CompareTag("Prop"))
        {
            if (collision.gameObject.GetComponent<Prop>().PropSize < playerSize)
            {
                collection.Add(collision.gameObject);
                Destroy(collision.gameObject.GetComponent<Rigidbody>());
                AddToPlayer(collision.gameObject);
                objsCollected++;
                collision.transform.parent = transform;
            }
        }
        
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Prop"))
        {
            if (collision.gameObject.GetComponent<Prop>().PropSize < playerSize)
            {
                collection.Add(collision.gameObject);
                Destroy(collision.gameObject.GetComponent<Rigidbody>());
                AddToPlayer(collision.gameObject);
                
                collision.transform.parent = transform;
            }
        }
    }
    private void AddToPlayer(GameObject obj)
    {
        cineCam.m_Orbits[0].m_Radius += obj.gameObject.GetComponent<Prop>().CamAddRadius;
        cineCam.m_Orbits[1].m_Radius += obj.gameObject.GetComponent<Prop>().CamAddRadius;
        cineCam.m_Orbits[2].m_Radius += obj.gameObject.GetComponent<Prop>().CamAddRadius;
        collider.radius += obj.gameObject.GetComponent<Prop>().PropAddradius;
        _rb.mass += obj.gameObject.GetComponent<Prop>().PropAddMass;
        playerSize += obj.gameObject.GetComponent<Prop>().PropAddSize;
    }

    
}
