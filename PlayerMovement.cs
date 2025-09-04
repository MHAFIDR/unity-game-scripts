using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController cr;
    private Animator animator;

    [Header("Movement Settings")]
    public float speed = 6f;
    public float gravity = -15f;

    [Header("Look Settings")]
    public float lookSpeed = 150f;
    public Transform pivotLookAt;
    public Transform kamera;
    public Transform lookAt;

    [Header("Ground Check Settings")]
    public LayerMask groundMask;

    private Vector3 velocity;
    public bool isGrounded;
    private float pivotAimX;
    
    void Start()
    {
        cr = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        animator.applyRootMotion = false;
        
        // Force initial ground position
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, groundMask))
        {
            float capsuleBottomOffset = cr.height / 2f + cr.center.y;
            transform.position = new Vector3(transform.position.x, hit.point.y + capsuleBottomOffset, transform.position.z);
        }
    }

    void Update()
    {
        if (GameManager.instance.isUiOpen)
        {
            animator.SetFloat("Horizontal", 0f);
            animator.SetFloat("Vertical", 0f);
            return;
        }

        // Use built-in ground detection
        isGrounded = cr.isGrounded;
        


        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; 
        }

        float ws = Input.GetAxis("Vertical");
        float ad = Input.GetAxis("Horizontal");
        
        Vector3 move = transform.right * ad + transform.forward * ws;

        velocity.x = move.x * speed;
        velocity.z = move.z * speed;
        
        velocity.y += gravity * Time.deltaTime;

        cr.Move(velocity * Time.deltaTime);

        animator.SetFloat("Horizontal", ad);
        animator.SetFloat("Vertical", ws);
        
        look();
    }

    void look()
    {
        float MouseX = Input.GetAxis("Mouse X");
        float MouseY = Input.GetAxis("Mouse Y");

        transform.eulerAngles += new Vector3(0, MouseX * lookSpeed * Time.deltaTime, 0);

        pivotAimX += -MouseY * lookSpeed * Time.deltaTime;
        pivotAimX = Mathf.Clamp(pivotAimX, -30, 30);
        pivotLookAt.localEulerAngles = new Vector3(pivotAimX, 0, 0);
        kamera.LookAt(lookAt);
    }
}