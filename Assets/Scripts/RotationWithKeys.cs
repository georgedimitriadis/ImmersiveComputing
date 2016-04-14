using UnityEngine;
using System.Collections;

/// MouseLook rotates the transform based on the mouse delta.
/// Minimum and Maximum values can be used to constrain the possible rotation

/// To make an FPS style character:
/// - Create a capsule.
/// - Add the MouseLook script to the capsule.
///   -> Set the mouse look to use LookX. (You want to only turn character but not tilt it)
/// - Add FPSInputController script to the capsule
///   -> A CharacterMotor and a CharacterController component will be automatically added.

/// - Create a camera. Make the camera a child of the capsule. Reset it's transform.
/// - Add a MouseLook script to the camera.
///   -> Set the mouse look to use LookY. (You want the camera to tilt up and down like a head. The character already turns.)
[AddComponentMenu("Camera-Control/Keys Look")]
public class RotationWithKeys : MonoBehaviour
{

    
    public float sensitivityX = 50F;
    public float sensitivityY = 50F;

    public float minimumX = -90F;
    public float maximumX = 90F;

    public float minimumY = -90F;
    public float maximumY = 90F;

    float rotationY = 0F;

    void Update()
    {

        float rotationX = Input.GetAxis("RotateLeftRight") * (maximumX - minimumX);

        rotationY = Input.GetAxis("RotateUpDown") * (maximumY - minimumY);

        transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
    }

    void Start()
    {
        // Make the rigid body not change rotation
        if (GetComponent<Rigidbody>())
            GetComponent<Rigidbody>().freezeRotation = true;
    }
}