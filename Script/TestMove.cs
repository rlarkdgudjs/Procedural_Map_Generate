using UnityEngine;

public class TestMove : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Transform cameraTransform;
    public float mouseSensitivity = 2f;
    private float xRotation = 0f;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    void Update()
    {
        HandleMouseLook();
        HandleMovement();
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal"); 
        float moveZ = Input.GetAxis("Vertical"); 

        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;

        Vector3 move = camRight * moveX + camForward * moveZ;
        transform.position += move * moveSpeed * Time.deltaTime;
    }
}
