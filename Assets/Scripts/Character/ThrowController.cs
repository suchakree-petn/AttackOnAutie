using DG.Tweening;
using UnityEngine;
using Sirenix.OdinInspector;
using System;
using UnityEngine.UI;



#if UNITY_EDITOR
using UnityEditor;
#endif

public class ThrowController : MonoBehaviour
{
    [FoldoutGroup("Config")]
    public ThrowType ThrowType;

    [FoldoutGroup("Config")]
    [SerializeField] bool isFacingRight = true;

    [FoldoutGroup("Config")]
    [SerializeField] Vector2 landingPosition;

    [FoldoutGroup("Config")]
    [SerializeField] float speedMultiplier = 2f, normalObjectRadius = 2f, powerObjectRadius = 4f;

    [FoldoutGroup("Config")]
    [SerializeField] private int maxBounces = 1;

    [FoldoutGroup("Config")]
    [SerializeField] float minThrowRange = -27f, maxThrowRange = 11, chargeSpeed = 1;

    [FoldoutGroup("Config")]
    [SerializeField] float minGuaranteeHit = 12.5f, maxGuaranteeHit = 15.5f;

    [FoldoutGroup("Reference"), Required]
    [SerializeField] SpriteRenderer throwObject;

    [FoldoutGroup("Reference"), Required]
    [SerializeField] Image chargeGaugeUI;

    [FoldoutGroup("Reference"), Required]
    [SerializeField] GameObject chargeGaugeGroup;

    public event Action OnCollided;

    private int currentBounceCount = 0;
    private Vector2 velocity, launchPos;
    private float moveTime, collideRadius;
    [SerializeField] float currentCharge;
    private bool isCollided, isThrowObjectMoving;
    public bool IsCharging { get; private set; }
    public bool IsThrew { get; private set; }

    void Update()
    {
        if (!isThrowObjectMoving)
            return;

        MoveThrowingObject();

        RaycastHit2D hit = CheckCollide();
        if (hit)
        {
            HandleCollision(hit);
        }

        if (isCollided && throwObject.transform.position.y <= landingPosition.y)
        {
            isThrowObjectMoving = false;
        }
    }

    private void HandleCollision(RaycastHit2D hit)
    {
        if (hit.collider.gameObject == gameObject || currentBounceCount >= maxBounces)
            return;

        bool isWall = hit.collider.CompareTag("Wall");

        bool isOtherCharacter = false;
        bool isCrit = false;
        CharacterController characterController = null;
        if (!isWall)
        {
            isOtherCharacter = hit.collider.CompareTag("Body");
            isCrit = hit.collider.CompareTag("Critical");
            characterController = hit.collider.GetComponentInParent<CharacterController>();
        }

        if (isWall || isOtherCharacter || isCrit)
        {
            Bounce(hit, 0.25f);
            OnCollided?.Invoke();
            if (characterController)
            {
                characterController.TakeDamage(isCrit);
            }
        }
    }

    private void Bounce(RaycastHit2D hit, float velocityMultiplier)
    {
        velocity = Vector2.Reflect(velocity, hit.normal) * velocityMultiplier;
        velocity.y = Mathf.Abs(velocity.y);
        launchPos = throwObject.transform.position;
        moveTime = 0;
        currentBounceCount++;
        isCollided = true;
    }

    private void MoveThrowingObject()
    {
        moveTime += Time.deltaTime * speedMultiplier;
        float dx = velocity.x * moveTime;
        float dy = velocity.y * moveTime - 0.5f * (-Physics2D.gravity.y) * moveTime * moveTime;

        throwObject.transform.position = launchPos + new Vector2(dx, dy);
    }

    private RaycastHit2D CheckCollide()
    {
        return Physics2D.CircleCast(throwObject.transform.position, collideRadius, Vector2.zero);
    }

    [Button]
    public void ChargeThrow()
    {
        IsCharging = true;

        int dir = isFacingRight ? 1 : -1;
        currentCharge += dir * chargeSpeed * Time.deltaTime;

        UpdateChargeGaugeUI(currentCharge);

        if ((isFacingRight && currentCharge >= maxThrowRange) ||
            (!isFacingRight && currentCharge <= maxThrowRange))
        {
            currentCharge = maxThrowRange;
            Throw();
        }
    }

    private void UpdateChargeGaugeUI(float currentCharge)
    {
        float throwRange = Mathf.Abs(minThrowRange - maxThrowRange);
        float chargePercent = Mathf.Abs((currentCharge - minThrowRange) / throwRange);
        chargeGaugeUI.fillAmount = chargePercent * 0.35f;
    }

    [Button]
    public void Throw()
    {
        IsCharging = false;
        IsThrew = true;

        float windMultiplier = WindManager.Instance.GetWindMultiplier();
        currentCharge += windMultiplier * 5f;

        landingPosition.x = currentCharge;
        collideRadius = ThrowType == ThrowType.Power ? powerObjectRadius : normalObjectRadius;

        Vector2 start = launchPos;
        Vector2 end = landingPosition;
        float dx = end.x - start.x;
        float dy = end.y - start.y;

        float angleDeg = isFacingRight ? 45f : 135f;
        float angleRad = angleDeg * Mathf.Deg2Rad;

        float cos = Mathf.Cos(angleRad);
        float sin = Mathf.Sin(angleRad);

        float g = -Physics2D.gravity.y;
        float denominator = 2 * cos * cos * (dx * Mathf.Tan(angleRad) - dy);

        if (Mathf.Abs(denominator) < Mathf.Epsilon)
        {
            velocity = Vector2.zero;
            isThrowObjectMoving = false;
            return;
        }

        float speed = Mathf.Sqrt(Mathf.Max(0, g * dx * dx / denominator));
        velocity = new Vector2(cos * speed, sin * speed);
        isThrowObjectMoving = true;
    }
    [Button]
    public void ResetThow()
    {
        isThrowObjectMoving = false;
        moveTime = 0;
        currentBounceCount = 0;
        launchPos = transform.position + new Vector3(0, 4, 0);
        throwObject.transform.position = launchPos;
        isCollided = false;
        currentCharge = minThrowRange;
        chargeGaugeUI.fillAmount = 0;
        IsThrew = false;
    }

    public void ShowChargeGauge()
    {
        chargeGaugeGroup.SetActive(true);
    }

    public void HideChargeGauge()
    {
        chargeGaugeGroup.SetActive(false);
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new(minThrowRange, landingPosition.y), new(minThrowRange, landingPosition.y * 5));
#if UNITY_EDITOR
        Handles.color = Color.yellow;
        Handles.Label(new(minThrowRange, landingPosition.y * 5), "Min"); Gizmos.color = Color.magenta;
#endif

        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(new(maxThrowRange, landingPosition.y), new(maxThrowRange, landingPosition.y * 5));
#if UNITY_EDITOR
        Handles.color = Color.magenta;
        Handles.Label(new(maxThrowRange, landingPosition.y * 5), "Max");
#endif

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(landingPosition, normalObjectRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(landingPosition, powerObjectRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(throwObject.transform.position, normalObjectRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(throwObject.transform.position, powerObjectRadius);

    }
}
