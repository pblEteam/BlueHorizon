using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSController : MonoBehaviour
{
    public float speed = 5f;
    public float mouseSensitivity = 100f;
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    //public float jumpForce = 5f;

    private Rigidbody rb;
    private bool isGrounded = true;
    private Camera mainCamera;
    private Collider currentTrash; // ���� �浹�� �����⸦ �����ϴ� ����

    private float rotationX = 0f; // ���� ȸ�� ����

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        mainCamera = Camera.main;

        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError("Rigidbody�� �� ������Ʈ�� �����ϴ�! FPSController���� �ݵ�� Rigidbody�� �ʿ��մϴ�.");
        }
    }


    void Update()
    {
        // ���콺�� ���� ȸ��
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f); // ���� ���� ����

        transform.Rotate(Vector3.up * mouseX); // �¿� ȸ��
        mainCamera.transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f); // ���� ȸ��

        // E Ű�� ������ �� �浹�� ������ ��Ȱ��ȭ
        if (Input.GetKeyDown(KeyCode.E) && currentTrash != null)
        {
            currentTrash.gameObject.SetActive(false); // ������ ��ü ��Ȱ��ȭ
            currentTrash = null; // ���� �ʱ�ȭ
            Debug.Log("�����⸦ �����߽��ϴ�!");
        }
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 move = transform.right * h + transform.forward * v;

        float currentSpeed = Input.GetKey(KeyCode.T) ? runSpeed : walkSpeed;
        Vector3 velocity = move * currentSpeed;
        velocity.y = rb.velocity.y;  // y���� ���� ���� �ް� ����
        rb.velocity = velocity;

        //if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        //{
        //    rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        //    isGrounded = false;
        //}

    }
 

    void FixedUpdate()
    {
        // Ű����� �̵� ó��
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = transform.forward * moveVertical + transform.right * moveHorizontal;
        GetComponent<Rigidbody>().MovePosition(transform.position + movement * speed * Time.fixedDeltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("trash"))
        {
            currentTrash = other; // �浹�� �����⸦ ����
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("trash"))
        {
            currentTrash = null; // Ʈ���ſ��� ������ ���� �ʱ�ȭ
        }
    }
}