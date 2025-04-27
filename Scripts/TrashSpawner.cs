using UnityEngine;

public class TrashSpawner : MonoBehaviour
{
    public int spawnCount = 10;
    public float spawnRadius = 1200f;
    public float fixedY = 38.55236f;

    private GameObject[] allTrashPrefabs;
    private Vector3 centerPosition = new Vector3(261.9159f, 0f, 123.2014f);

    void Start()
    {
        allTrashPrefabs = Resources.LoadAll<GameObject>("TrashPrefabs");

        if (allTrashPrefabs.Length == 0)
        {
            Debug.LogError("❌ TrashPrefabs 폴더에 프리팹이 없습니다!");
            return;
        }

        for (int i = 0; i < spawnCount; i++)
        {
            GameObject randomTrash = allTrashPrefabs[Random.Range(0, allTrashPrefabs.Length)];

            // ✔️ 원형으로 무작위 위치 생성
            Vector2 randomCircle = Random.insideUnitCircle * spawnRadius * 2;
            Vector3 spawnPos = new Vector3(
                centerPosition.x + randomCircle.x,
                fixedY,
                centerPosition.z + randomCircle.y
            );

            GameObject instance = Instantiate(randomTrash, spawnPos, Quaternion.identity);
            instance.transform.localScale = Vector3.one * 4f;

            // ✅ Trash 레이어 자동 지정
            instance.layer = LayerMask.NameToLayer("Trash");

            // Collider 없으면 추가
            if (instance.GetComponent<Collider>() == null)
                instance.AddComponent<BoxCollider>();

            // Rigidbody 없으면 추가 + 물리 설정
            Rigidbody rb = instance.GetComponent<Rigidbody>();
            if (rb == null) rb = instance.AddComponent<Rigidbody>();

            rb.useGravity = true;
            rb.isKinematic = false;
            rb.ResetCenterOfMass();
            rb.ResetInertiaTensor();
        }

        Debug.Log($"🎉 총 {spawnCount}개의 쓰레기 생성 완료.");
    }
}
