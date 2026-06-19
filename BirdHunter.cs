using UnityEngine;
using UnityEngine.UI;

public class BirdHunter : MonoBehaviour
{
    [Header("Интерфейс")]
    public Text scoreText;
    private int score = 0;

    [Header("Эффекты")]
    public GameObject floatingTextPrefab;

    void Start()
    {
        // Больше никаких SpawnWave! Птицы уже стоят на сцене.
        if (scoreText != null) scoreText.text = score.ToString();
    }

    void Update()
    {
        if (Input.anyKeyDown || Input.GetMouseButtonDown(0) || Input.touchCount > 0)
        {
            Shoot();
        }
    }

    void Shoot()
    {
        if (Camera.main == null) return;

        Vector3 inputPos = Input.mousePosition;
        if (Input.touchCount > 0) inputPos = Input.GetTouch(0).position;

        Ray ray = Camera.main.ScreenPointToRay(inputPos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000f))
        {
            if (hit.collider.CompareTag("Bird"))
            {
                BirdCircleFlight bird = hit.collider.GetComponentInParent<BirdCircleFlight>();

                if (bird != null && !bird.isDead)
                {
                    bird.Die(); // Запускаем падение и таймер респавна

                    // Спавним всплывающий текст и передаем ему очки
                    if (floatingTextPrefab != null)
                    {
                        GameObject textObj = Instantiate(floatingTextPrefab, hit.point, Quaternion.identity);
                        FloatingText ft = textObj.GetComponent<FloatingText>();
                        if (ft != null) ft.SetPoints(bird.points); // Говорим тексту написать +2 или +5
                    }

                    // Прибавляем не 1, а стоимость птицы!
                    score += bird.points;
                    if (scoreText != null) scoreText.text = score.ToString(); // Просто цифра, без слова "Счет"
                }
            }
        }
    }
}