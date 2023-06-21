

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    private Vector2 _mouseInput = Vector2.zero;
    private Vector2 _rotateAmount = Vector2.zero;
    private PlayerController _player;
    private float axisClamp = 0f;

    [SerializeReference]
    private float mouseSensitivity = 2f;

    // Start is called before the first frame update
    void Start()
    {
        _player = GetComponentInParent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {

        // Get the inputs from the mouse and multiply them with the mouseSensitivity
        _mouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        _rotateAmount.x = _mouseInput.x * mouseSensitivity;
        _rotateAmount.y = _mouseInput.y * mouseSensitivity;

        axisClamp -= _rotateAmount.y;


        // Get the current angles
        // We also want to rotate the PLAYER
        //     W means forward motion (vector3.forward) so we want to player to rotate towards where the camera is pointing to maintain that forward motion
        //     Else the character would only move forward in WORLD SPACE
        //     We dont rotate the camera on the Y axis since the BODY is being rotated on it -> else we would get double rotational values

        Vector3 targetCam = transform.rotation.eulerAngles;
        Vector3 targetPlayer = _player.transform.rotation.eulerAngles;

        targetCam.x -= _rotateAmount.y;
        targetPlayer.y += _rotateAmount.x;


        // CLAMP the rotation on the X axis (looking up/down)
        if (axisClamp > 90)
        {
            axisClamp = 90;
            targetCam.x = 90;
        }
        else if (axisClamp < -90)
        {
            axisClamp = -90;
            targetCam.x = 270;
        }

        transform.rotation = Quaternion.Euler(targetCam);
        _player.transform.rotation = Quaternion.Euler(targetPlayer);

    }
}
