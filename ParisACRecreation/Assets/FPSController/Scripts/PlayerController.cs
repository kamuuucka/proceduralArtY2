using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
   
    private CharacterController character;

    [Header("Movement Speed")]
    [SerializeField] private float _MovementSpeed = 3f;
    [Header("Gravity")]
    [SerializeField] private float _stickForce = -9.8f;
    [Header("Jump Force")]
    [SerializeField] private float _JumpHeight = 30f;
    Vector3 velocity;
    bool isGrounded;

    [SerializeField]
    private float _gravityMultiplier = 12.5f;

    private Vector3 _moveDir = Vector3.zero;
    private float walkingMultiplier = 1f;

    [SerializeField] private AudioClip steps;
    private AudioSource pAudioSource;

    private void OnEnable()
    {
        character = GetComponent<CharacterController>();
        pAudioSource = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }
 

    // Update is called once per frame
    void Update()
    {
        // Create local variable and initialize it
        // Multiple x and z with the movementspeed
        Vector3 desiredMove = transform.forward * Input.GetAxis("Vertical") + transform.right * Input.GetAxis("Horizontal");
        if (Input.GetKey("left shift"))
        {
            walkingMultiplier = 0.5f;
        }
        else
        {
            walkingMultiplier = 1f;
        }
        _moveDir.x = (desiredMove.x * _MovementSpeed) * walkingMultiplier;
        _moveDir.z = (desiredMove.z * _MovementSpeed) * walkingMultiplier;

        // If the spacebar is pressed AND we are on the ground
        // set the y pos with the jump amount
        if (Input.GetKeyDown("space") && character.isGrounded)
        {
            _moveDir.y = _JumpHeight;
        }

        // if the character is not grounded
        // Apply gravity
        if (!character.isGrounded)
        {
            _moveDir.y += _stickForce * _gravityMultiplier * Time.deltaTime;
        }

        if (_moveDir.x != 0f || _moveDir.z != 0f)
        {
            if (character.isGrounded)
            {
                if (pAudioSource.clip == null)
                {
                    pAudioSource.clip = steps;
                    if (!pAudioSource.isPlaying)
                    {
                        pAudioSource.Play();
                    }
                }
                else
                {
                    if (!pAudioSource.isPlaying)
                    {
                        pAudioSource.Play();
                    }
                }
            }
            else
            {
                pAudioSource.Stop();
            }
        }
        else
        {
            pAudioSource.Stop();
        }

        character.Move(_moveDir * Time.deltaTime);
    }
}
