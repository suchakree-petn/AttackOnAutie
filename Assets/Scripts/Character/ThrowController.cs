using DG.Tweening;
using UnityEngine;
using Sirenix.OdinInspector;
using System;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

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
    [SerializeField] float speedMultiplier = 2f, normalObjectRadius = 2f, powerObjectRadius = 4f,throwAngle = 60f;

    [FoldoutGroup("Config")]
    public int ThrowAmount = 1;

    [FoldoutGroup("Config")]
    [SerializeField] private int maxBounces = 1;

    [FoldoutGroup("Config")]
    [SerializeField] float minThrowRange = -27f, maxThrowRange = 11, chargeSpeed = 1;

    [FoldoutGroup("Reference"), Required]
    [SerializeField] SpriteRenderer throwObject;

    [FoldoutGroup("Reference"), Required]
    [SerializeField] Image chargeGaugeUI;

    [FoldoutGroup("Reference"), Required]
    [SerializeField] GameObject chargeGaugeGroup;

    public event Action OnCollided, OnStartChage;

    private int currentBounceCount = 0;
    private Vector2 velocity, launchPos;
    private float moveTime, collideRadius;
    [SerializeField] float currentCharge;
    private bool isCollided, isThrowObjectMoving;
    public bool IsCharging { get; private set; }
    public bool IsThrew { get; private set; }

    void Start()
    {
        ResetThow();
        OnCollided += () =>
        {
            if (!isCollided)
            {
                throwObject.DOFade(0, 1);
                throwObject.transform.DOKill();

            }
        };
    }

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
        if (hit.collider.CompareTag("Character") || currentBounceCount >= maxBounces)
            return;

        Debug.Log(hit.collider.name, hit.collider);

        bool isWall = hit.collider.CompareTag("Wall");
        bool isOtherCharacter = hit.collider.CompareTag("Body");
        bool isCrit = hit.collider.CompareTag("Critical");
        CharacterController characterController = null;
        if (isOtherCharacter || isCrit)
        {
            characterController = hit.collider.GetComponentInParent<CharacterController>();
        }

        if (isWall || isOtherCharacter || isCrit)
        {
            OnCollided?.Invoke();
            Bounce(hit, 0.25f);
            if (characterController)
            {
                characterController.TakeDamage(isCrit, ThrowType);
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
        if (!IsCharging)
        {
            IsCharging = true;
            OnStartChage?.Invoke();
        }

        int dir = isFacingRight ? 1 : -1;
        currentCharge += dir * chargeSpeed * Time.deltaTime;
        // currentCharge = (maxThrowRange +minThrowRange)/2;

        UpdateChargeGaugeUI(currentCharge);

        if ((isFacingRight && currentCharge >= maxThrowRange) ||
            (!isFacingRight && currentCharge <= maxThrowRange))
        {
            currentCharge = maxThrowRange;
            Throw();
        }
    }

    public void ChargeAndThrow(float chargeValue)
    {
        if (!IsCharging)
        {
            IsCharging = true;
            OnStartChage?.Invoke();
        }

        int dir = isFacingRight ? 1 : -1;
        currentCharge = dir * chargeValue;

        UpdateChargeGaugeUI(currentCharge);
        ShowChargeGauge();

        if ((isFacingRight && currentCharge >= maxThrowRange) ||
            (!isFacingRight && currentCharge <= maxThrowRange))
        {
            currentCharge = maxThrowRange;
        }

        Throw(true);
    }

    private void UpdateChargeGaugeUI(float currentCharge)
    {
        float throwRange = Mathf.Abs(minThrowRange - maxThrowRange);
        float chargePercent = Mathf.Abs((currentCharge - minThrowRange) / throwRange);
        chargeGaugeUI.fillAmount = chargePercent * 0.35f;
    }

    [Button]
    public async void Throw(bool ignoreWind = false)
    {
        Color throwObjectColor = throwObject.color;
        throwObjectColor.a = 1;
        throwObject.color = throwObjectColor;
        int rotateDir = isFacingRight ? -1 : 1;
        throwObject.transform
            .DORotate(new(0, 0, rotateDir * 360), 1f, RotateMode.FastBeyond360)
            .SetLoops(-1)
            .SetEase(Ease.Linear);

        IsCharging = false;
        IsThrew = true;
        ThrowAmount--;

        float landingPos = ignoreWind ? currentCharge : currentCharge + WindManager.Instance.GetWindMultiplier(); ;
        landingPosition.x = landingPos;

        SetColliderAndScale();


        velocity = CalculateVelocity(launchPos, landingPosition);

        await UniTask.WaitForSeconds(2);

        if (ThrowAmount > 0)
        {
            float currentCharge = this.currentCharge;
            ResetThow();
            this.currentCharge = currentCharge;

            Throw();
        }
    }

    private void SetColliderAndScale()
    {
        collideRadius = ThrowType == ThrowType.Power ? powerObjectRadius : normalObjectRadius;
        float scale = ThrowType == ThrowType.Power ? 7 : 3.6f;
        throwObject.transform.localScale = new(scale, scale, scale);
    }

    private Vector2 CalculateVelocity(Vector2 start, Vector2 end)
    {
        float dx = end.x - start.x;
        float dy = end.y - start.y;

        float angleDeg = isFacingRight ? throwAngle : 180 - throwAngle;
        float angleRad = angleDeg * Mathf.Deg2Rad;

        float cos = Mathf.Cos(angleRad);
        float sin = Mathf.Sin(angleRad);

        float g = -Physics2D.gravity.y;
        float denominator = 2 * cos * cos * (dx * Mathf.Tan(angleRad) - dy);

        if (Mathf.Abs(denominator) < Mathf.Epsilon)
        {
            isThrowObjectMoving = false;
            return Vector2.zero;
        }

        isThrowObjectMoving = true;
        float speed = Mathf.Sqrt(Mathf.Max(0, g * dx * dx / denominator));
        return new Vector2(cos * speed, sin * speed);
    }
    [Button]
    public void ResetThow()
    {
        throwObject.transform.DOKill();
        throwObject.DOKill();
        throwObject.transform.rotation = new();
        Color throwObjectColor = throwObject.color;
        throwObjectColor.a = 0;
        throwObject.color = throwObjectColor;


        isThrowObjectMoving = false;
        moveTime = 0;
        currentBounceCount = 0;
        launchPos = transform.position + new Vector3(0, 4, 0);
        throwObject.transform.position = launchPos;
        isCollided = false;
        currentCharge = minThrowRange;
        chargeGaugeUI.fillAmount = 0;
        IsThrew = false;
        ThrowAmount = 1;
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
