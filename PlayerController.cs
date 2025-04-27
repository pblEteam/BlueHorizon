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
    private Collider currentTrash; // 현재 충돌한 쓰레기를 저장하는 변수

    private float rotationX = 0f; // 상하 회전 변수

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        mainCamera = Camera.main;

        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError("Rigidbody가 이 오브젝트에 없습니다! FPSController에는 반드시 Rigidbody가 필요합니다.");
        }
    }


    void Update()
    {
        // 마우스로 시점 회전
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f); // 상하 각도 제한

        transform.Rotate(Vector3.up * mouseX); // 좌우 회전
        mainCamera.transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f); // 상하 회전

        // E 키가 눌렸을 때 충돌한 쓰레기 비활성화
        if (Input.GetKeyDown(KeyCode.E) && currentTrash != null)
        {
            currentTrash.gameObject.SetActive(false); // 쓰레기 객체 비활성화
            currentTrash = null; // 변수 초기화
            Debug.Log("쓰레기를 수집했습니다!");
        }
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 move = transform.right * h + transform.forward * v;

        float currentSpeed = Input.GetKey(KeyCode.T) ? runSpeed : walkSpeed;
        Vector3 velocity = move * currentSpeed;
        velocity.y = rb.velocity.y;  // y축은 점프 영향 받게 유지
        rb.velocity = velocity;

        //if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        //{
        //    rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        //    isGrounded = false;
        //}

    }
 

    void FixedUpdate()
    {
        // 키보드로 이동 처리
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = transform.forward * moveVertical + transform.right * moveHorizontal;
        GetComponent<Rigidbody>().MovePosition(transform.position + movement * speed * Time.fixedDeltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("trash"))
        {
            currentTrash = other; // 충돌한 쓰레기를 저장
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("trash"))
        {
            currentTrash = null; // 트리거에서 나가면 변수 초기화
        }
    }
}