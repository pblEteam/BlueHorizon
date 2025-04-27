using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ObjectRemover : MonoBehaviour
{
    public float detectionDistance = 6f;
    public LayerMask trashLayer;
    public Slider bagSlider;
    public TMP_Text trashCountText;
    public int trashGoal = 10;

    private int currentTrashCount = 0;
    private int trashDisposalLayer;   // ✅ Trash Disposal 레이어 번호 저장

    void Start()
    {
        trashDisposalLayer = LayerMask.NameToLayer("Trash Disposal");  // 시작할 때 레이어 찾기
    }

    void Update()
    {
        Vector3 rayOrigin = Camera.main.transform.position;
        Vector3 rayDirection = Camera.main.transform.forward * detectionDistance;
        Debug.DrawRay(rayOrigin, rayDirection, Color.red);

        if (Input.GetKeyDown(KeyCode.F))
        {
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            RaycastHit hit;

            int trashLayer = LayerMask.NameToLayer("Trash");

            if (Physics.Raycast(ray, out hit, detectionDistance))
            {
                GameObject obj = hit.collider.gameObject;
                Debug.Log($"🎯 감지된 오브젝트: {obj.name}, 레이어: {LayerMask.LayerToName(obj.layer)}");

                if (obj.layer == trashLayer)
                {
                    Debug.Log("✅ Trash 레이어 감지됨");
                    Destroy(obj);

                    if (bagSlider != null)
                        bagSlider.value += 0.2f;

                    currentTrashCount++;
                    if (trashCountText != null)
                        trashCountText.text = $"{currentTrashCount} / {trashGoal}";

                    if (currentTrashCount >= trashGoal)
                        Debug.Log("🎉 퀘스트 완료!");
                }
                else
                {
                    Debug.Log("❌ Trash 레이어 감지 안됨");
                }
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == trashDisposalLayer)
        {
            Debug.Log("🚮 Trash Disposal 충돌 감지! 가방 비움");
            if (bagSlider != null)
                bagSlider.value = 0.4f;  // 가방 게이지 리셋
        }
    }
}
