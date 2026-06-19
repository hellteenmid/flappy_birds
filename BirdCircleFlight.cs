using UnityEngine;
using System.Collections;

public class BirdCircleFlight : MonoBehaviour
{
    [Header("Настройки полета")]
    public float speed = 5f;
    public float travelDistance = 10f;
    public Vector3 beakFix = new Vector3(0, 90, 0);
    public bool startFlyingLeft = false;

    [Header("Настройки игры")]
    public int points = 1;
    public float respawnTime = 7f;

    [Header("Настройки падения")]
    public float fallSpeed = 4f;
    public float forwardFallSpeed = 2f;
    public float deathDuration = 2f;

    [Range(10f, 90f)]
    public float fallAngle = 65f;

    // === ИСПРАВЛЕНИЕ 1: Разделяем переменные ===
    private Vector3 originalSpawnPosition; // Настоящий "дом" птицы
    private Vector3 patrolAnchor;          // Временный якорь для отсчета дистанции
    private Vector3 flyDirection;

    [HideInInspector]
    public bool isDead = false;

    private Animator animator;
    private Collider col;

    void Start()
    {
        // Запоминаем родное место один раз при старте игры
        originalSpawnPosition = transform.position;
        patrolAnchor = transform.position;

        SetDirection();

        animator = GetComponent<Animator>();
        if (animator == null) animator = GetComponentInChildren<Animator>();
        col = GetComponent<Collider>();
    }

    void SetDirection()
    {
        // Птицы летают строго в заданном направлении (влево или вправо)
        if (startFlyingLeft) flyDirection = Vector3.left;
        else flyDirection = Vector3.right;
    }

    void Update()
    {
        if (isDead)
        {
            transform.position += Vector3.down * fallSpeed * Time.deltaTime;
            transform.position += flyDirection * forwardFallSpeed * Time.deltaTime;

            Quaternion forwardRot = Quaternion.LookRotation(flyDirection);
            Quaternion diveRot = Quaternion.Euler(fallAngle, 0, 0);
            transform.rotation = forwardRot * diveRot * Quaternion.Euler(beakFix);
            return;
        }

        transform.position += flyDirection * speed * Time.deltaTime;

        // ИСПРАВЛЕНИЕ 2: Считаем дистанцию от временного якоря, а не от дома
        if (Vector3.Distance(patrolAnchor, transform.position) >= travelDistance)
        {
            flyDirection = -flyDirection;
            patrolAnchor = transform.position;
        }

        if (flyDirection != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(flyDirection);
            transform.rotation = targetRot * Quaternion.Euler(beakFix);
        }
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        if (animator != null) animator.SetTrigger("Die");
        if (col != null) col.enabled = false;

        StartCoroutine(RespawnRoutine());
    }

    IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(deathDuration);

        transform.position = new Vector3(0, -1000, 0);

        yield return new WaitForSeconds(respawnTime - deathDuration);

        // === ИСПРАВЛЕНИЕ 3: Полный сброс параметров воскрешения ===
        transform.position = originalSpawnPosition; // Возвращаем строго на родное место
        patrolAnchor = originalSpawnPosition;       // Сбрасываем якорь разворота
        SetDirection();                             // Возвращаем правильный взгляд

        if (animator != null)
        {
            // === ИСПРАВЛЕНИЕ 4: Магия сброса аниматора ===
            // Rebind() полностью перезагружает Аниматор к заводским настройкам (в состояние по умолчанию).
            // Теперь нам вообще не важно, как называется анимация полета в файле.
            animator.Rebind();
            animator.Update(0f);
        }

        if (col != null) col.enabled = true;
        isDead = false;
    }
}