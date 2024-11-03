using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private GameController gameController;
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public int maxJumps = 2;
    public float jumpCooldown;
    public float airMuliplayer;
    public float dashForce = 20f;
    public float dashCooldown = 1f;
    public float groundDrag;

    private Rigidbody rb;
    private int currentJumps = 0;
    private bool isGrounded = true;
    private float lastDashTime = -10;
    private Vector3 moveDirection;
    private float moveX;
    private float moveZ;
    
    private bool readyToJump = true;
    [SerializeField] private Transform orientation;
    private ObjectData playerData;
    private bool isDashing = false;
    [SerializeField] GameObject cameraHolder;
    private void Start()
    {
        playerData = transform.GetChild(0).GetComponent<ObjectData>();
        rb = transform.GetChild(0).GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (!playerData.isDead)
        {
            isGrounded = Physics.Raycast(rb.transform.position, Vector3.down, 2 * 0.5f + 0.2f);

            if (!isDashing)
            {
                MovePlayer();
                SpeedControl();
            }

            Dash();

            if (isGrounded)
            {
                rb.drag = groundDrag;
                currentJumps = 0;
            }
            else
                rb.drag = 0;
        }
        else
        {
            cameraHolder.GetComponent<MoveCamera>().enabled = false;
            cameraHolder.GetComponent<PlayerCam>().enabled = false;
            gameController.GameOver();
        }
    }

    private void MovePlayer()
    {
        moveX = Input.GetAxis("Horizontal");
        moveZ = Input.GetAxis("Vertical");
        moveDirection = orientation.forward * moveZ + orientation.right * moveX;

        if (isGrounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else if (!isGrounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        if (Input.GetKeyDown(KeyCode.Space) && readyToJump && isGrounded || (currentJumps < maxJumps - 1 
            && Input.GetKeyDown(KeyCode.Space) && !isGrounded))
        {
            readyToJump = false;
            currentJumps++;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Dash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time >= lastDashTime + dashCooldown && playerData.PlayerCanDash())
        {
            isDashing = true;
            Vector3 dashDirection = Vector3.zero;

            if (Input.GetKey(KeyCode.W))
                dashDirection += orientation.forward;
            if (Input.GetKey(KeyCode.S))
                dashDirection -= orientation.transform.forward;
            if (Input.GetKey(KeyCode.A))
                dashDirection -= orientation.transform.right;
            if (Input.GetKey(KeyCode.D))
                dashDirection += orientation.transform.right;

            if (dashDirection == Vector3.zero)
            {
                dashDirection = orientation.forward;
            }

            rb.AddForce(dashDirection.normalized * dashForce, ForceMode.Impulse);
            lastDashTime = Time.time;

            playerData.RemoveStamina();
        }
        float timeToWaitForMove = dashCooldown - 0.5f;
        if (timeToWaitForMove > 0.5f) timeToWaitForMove = 0.5f;
        if (Time.time >= lastDashTime + timeToWaitForMove)
            isDashing = false;
    }
}
