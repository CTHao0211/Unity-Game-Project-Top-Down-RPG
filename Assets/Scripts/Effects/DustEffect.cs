using UnityEngine;

public class DustEffect : MonoBehaviour
{
    public GameObject dustPrefab;      // Gán DustPrefab
    public Transform footPosition;     // Vị trí dưới chân
    public float spawnRate = 0.1f;     // Khoảng cách giữa các hạt bụi
    private float nextSpawn;

    private Rigidbody2D rb;            // Nếu 2D
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        nextSpawn = 0f;
    }

    private void Update()
    {
        // Lấy vận tốc tổng (X và Y)
        float currentVelocity = rb.velocity.magnitude;

        bool isMoving = currentVelocity > 0.1f; 

        if (isMoving && Time.time >= nextSpawn)
        {
            SpawnDust();
            nextSpawn = Time.time + spawnRate;
        }
    }


    private void SpawnDust()
    {
        if (dustPrefab != null && footPosition != null)
        {
            GameObject dust = Instantiate(dustPrefab, footPosition.position, Quaternion.identity);
            Destroy(dust, 1f); // tự động hủy sau 1s
        }
    }
}
