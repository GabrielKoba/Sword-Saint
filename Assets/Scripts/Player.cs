using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using UnityEngine.Windows;
using Unity.VisualScripting;
using static Unity.Burst.Intrinsics.X86;
using static UnityEditor.PlayerSettings;
using UnityEngine.UIElements;

[RequireComponent(typeof(Rigidbody2D), typeof(EntityStats))]
public class Player : MonoBehaviour {

    private Camera _camera;
    private InputManager _inputManager;
    private Rigidbody2D _rigidbody2D;
    private EntityStats _entityStats;

    [Separator("Player Settings")]

    [Header("Attack Stats")]
    [SerializeField] float attackCooldown = 0.5f;
    private float _attackCooldownElapsed;

    [Header("Block Stats")]
    [SerializeField] float deflectDuration = 0.1f;
    private float _deflectDurationElapsed;

    [Header("Movement Stats")]
    [SerializeField] private float movementSpeed = 40f;
    [SerializeField] private float decaySpeed = 5f;
    [Space]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius;
    [SerializeField] private LayerMask groundLayers;
    [Space]
    [SerializeField] private float jumpForce = 10f;
    [SerializeField][Range(0.0f, 1.0f)] private float airControlPercentage;
    [Space]
    [SerializeField] private float dashForce = 16f;

    [Header("Statuses")]
    public bool canAttack;
    public bool isAttacking;
    [Space]
    public bool canBlock;
    public bool isBlocking;
    [Space]
    public bool canMove = true;
    public bool facingRight = true;
    [Space]
    public bool isGrounded;
    public bool isJumping;
    public bool isFalling;
    [Space]
    public bool canDash;
    public bool isDashing;

    [Header("Cosmetic")]
    [SerializeField] private Transform upperBody;
    [SerializeField] public float upClamp = 120f;
    [SerializeField] public float downClamp = 30f;

    private Vector3 _mousePos;
    private Quaternion _aimRotation;

    void Start() {
        _camera = Camera.main;
        _inputManager = InputManager.Instance;
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _entityStats = GetComponent<EntityStats>();

        facingRight = true;
    }

    void Update() {
    }

    void FixedUpdate() {
        Inputs();
        GroundCheck();
        FallHandler();
    }

    private void Inputs() {
        Aim(_inputManager.aimInput);
        Move(_inputManager.moveInput);
        Jump(_inputManager.moveInput, _inputManager.jumpInput);

        Attack(_inputManager.primaryInput, _inputManager.alternateInput);
        Block(_inputManager.secondaryInput);
    }

    #region Movement
    private void Aim(Vector2 pointer) {
        // Gets mouse position.
        Vector3 _mousePos = Camera.main.ScreenToWorldPoint(pointer);
        // Gets mouse direction and assigns a rotation to point to it to aimRotation.
        _aimRotation = Quaternion.LookRotation(Vector3.forward, _mousePos - transform.position);

        Vector3 upperBodyAngle = _aimRotation.eulerAngles;
        upperBodyAngle.z = upperBodyAngle.z > 180 ? upperBodyAngle.z - 360 : upperBodyAngle.z;
        upperBodyAngle.z = facingRight ? Mathf.Clamp(upperBodyAngle.z, -upClamp, -downClamp) : Mathf.Clamp(upperBodyAngle.z, upClamp, downClamp);

        upperBody.rotation = Quaternion.Euler(upperBodyAngle);

        // Flips player to face mouse side.
        if (pointer.x > Screen.width / 2f && !facingRight || pointer.x < Screen.width / 2f && facingRight) {
            Flip();
        }
    }

    private void Move(Vector2 direction) {
        if (canMove && direction != Vector2.zero) {
            // Moves player.
            _rigidbody2D.velocity = new Vector2(direction.x * (isGrounded ? movementSpeed : movementSpeed * airControlPercentage) * 10 * Time.deltaTime, _rigidbody2D.velocity.y);
        }
        else {
            _rigidbody2D.velocity = new Vector2(-1 * decaySpeed * Time.deltaTime, _rigidbody2D.velocity.y);
        }
    }
    private void Jump(Vector2 direction, bool jump) {
        // If player can jump...
        if (isGrounded && jump) {
            _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, jumpForce);
            isJumping = true;
        }
        else if (!jump) {
            isJumping = false;
        }
    }

    private void Dash() {
        // If player can dash...
        if (!isGrounded && canDash) {
            Debug.Log("Dash");
            // _rigidbody2D.AddForce((_mousePos - transform.position) * dashForce * 100, ForceMode2D.Impulse);
            canDash = false;
        }
        else if (isGrounded) {
            canDash = true;
        }
    }

    private void GroundCheck() {
        // Raycasts circle to check for ground.
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayers);
    }

    private void FallHandler() {
        // If not grounded turn on gravity...
        if (!isGrounded) {
            // If is falling tone up gravity scale.
            if (_rigidbody2D.velocity.y < 0.0f) {
                isFalling = true;
            }
            // If not falling tone down gravity scale.
            else if (_rigidbody2D.velocity.y > 0.0f) {
                isFalling = false;
            }
        }
        // Else if grounded turn off gravity...
        else if (isGrounded) {
            isFalling = false;
        }
    }
 
    private void Flip() {
        Vector3 currentScale = gameObject.transform.localScale;
        currentScale.x *= -1;

        gameObject.transform.localScale = currentScale;

        facingRight = !facingRight;

        var tempClamp = upClamp;
        upClamp = downClamp;
        downClamp = tempClamp;
    }
    #endregion

    #region Combat
    
    private void Attack(bool primary, bool alternate) {
        if (primary && isBlocking) {
            Debug.Log("Throw Bo Shuriken");
        }
        else if (primary && alternate) {
            Dash();
        }
        else if (primary) {
            Debug.Log("Basic Attack");
        }
    }

    private void Block(bool blocking) {
        if (blocking && canBlock) {
            isBlocking = true;
        }
        else {
            isBlocking = false;
        }

        _entityStats.isBlocking = isBlocking;
    }

    #endregion
}