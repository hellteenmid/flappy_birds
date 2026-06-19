using UnityEngine;
using TMPro; // Обязательно добавляем работу с красивым текстом!

public class FloatingText : MonoBehaviour
{
    [Header("Настройки")]
    public float moveSpeed = 1.5f;
    public float destroyTime = 1.5f;
    public Vector3 offset = new Vector3(0, 0.5f, 0);
    public float baseSize = 0.05f;

    void Start()
    {
        transform.position += offset;
        UpdateSizeAndRotation();
        Destroy(gameObject, destroyTime);
    }

    void Update()
    {
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;
        UpdateSizeAndRotation();
    }

    void UpdateSizeAndRotation()
    {
        if (Camera.main != null)
        {
            transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
            float distance = Vector3.Distance(Camera.main.transform.position, transform.position);
            transform.localScale = Vector3.one * (distance * baseSize);
        }
    }

    // НОВАЯ ФУНКЦИЯ: Пушка будет передавать сюда очки убитой птицы
    public void SetPoints(int points)
    {
        TextMeshPro tmp = GetComponent<TextMeshPro>();
        if (tmp != null)
        {
            tmp.text = "+" + points.ToString();
        }
    }
}