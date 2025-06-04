using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public static class VectorExtension
{
  public static Vector3 RandomInRange(this Vector3 pos, float range)
  {
    Vector3 randomPos = Random.insideUnitCircle * range;
    return pos + randomPos;
  }
  public static Vector2 RandomInRange(this Vector2 pos, float range)
  {
    Vector2 randomPos = Random.insideUnitCircle * range;
    return pos + randomPos;
  }
  public static Vector3 RandomOnNavMesh(this Vector3 pos, float range)
  {
    Vector3 randomInRange = Random.insideUnitCircle * range;
    if (NavMesh.SamplePosition(randomInRange, out NavMeshHit hit, range, NavMesh.AllAreas))
    {
      Debug.Log("Hit: " + randomInRange);
      return pos + randomInRange;
    }

    return pos;
  }


  public static Vector2 GetRandomPositionInBound(Vector2 minBounds, Vector2 maxBounds)
  {
    float randomX = Random.Range(minBounds.x, maxBounds.x);
    float randomY = Random.Range(minBounds.y, maxBounds.y);
    return new Vector2(randomX, randomY);
  }

  public static Vector3 GetRandomPositionInBound(Vector3Int minBounds, Vector3Int maxBounds)
  {
    float randomX = Random.Range(minBounds.x, maxBounds.x);
    float randomY = Random.Range(minBounds.y, maxBounds.y);
    return new Vector2(randomX, randomY);
  }

  public static bool IsReachTarget(this Vector3 pos, Vector3 target, float stopDistance)
  {
    return Vector2.Distance(pos, target) <= stopDistance;
  }


  public static Vector2 SetValue(this Vector2 vector, float? x = null, float? y = null)
  {
    return new Vector2(x ?? vector.x, y ?? vector.y);
  }

  public static Vector3 SetValue(this Vector3 vector, float? x = null, float? y = null, float? z = null)
  {
    return new Vector3(x ?? vector.x, y ?? vector.y, z ?? vector.z);
  }

  public static Vector3 WorldToScreenPoint(this Vector3 worldPoint, Camera camera = null)
  {
    if (camera == null)
    {
      camera = Camera.main;
    }
    return camera.WorldToScreenPoint(worldPoint);
  }

  public static Vector2 CanvasCenterPoint(CanvasScaler canvasScaler)
  {
    float screenWidth = Screen.width;
    float screenHeight = Screen.height;
    Vector2 center = new Vector2(screenWidth / 2, screenHeight / 2);
    return center;
  }
  public static Vector3 CalculateMove(Transform agentFollower, Transform leader, float avoidanceRadius, float followRadius, float spreadFactor)
  {

    Vector3 directionToLeader = leader.transform.position - agentFollower.transform.position;
    float distance = directionToLeader.magnitude;

    if (distance < avoidanceRadius)
    {
      // ถ้าอยู่ใกล้เกินไป ให้ถอยออกจาก Leader
      return -directionToLeader.normalized * 1.5f;
    }
    else if (distance > followRadius)
    {
      // ถ้าอยู่ไกลเกินไป ให้เข้าใกล้ Leader
      return directionToLeader.normalized * 0.75f;
    }
    else
    {
      Vector3 randomOffset = GetRandomPositionNearLeader(agentFollower, leader.position, spreadFactor);
      return randomOffset;

    }
  }

  public static Vector3 GetRandomPositionNearLeader(Transform agent, Vector3 leaderPosition, float spreadFactor)
  {
    float randomAngle = UnityEngine.Random.Range(0f, 360f) * Mathf.Deg2Rad;

    Vector3 randomOffset = new Vector3(Mathf.Cos(randomAngle) * spreadFactor,
    Mathf.Sin(randomAngle) * spreadFactor, 0);
    Vector3 newPos = leaderPosition + randomOffset;

    NavMeshHit hit;
    if (NavMesh.SamplePosition(newPos, out hit, 1.0f, NavMesh.AllAreas))
    {
      return (hit.position - agent.position).normalized * 0.5f;
    }
    return Vector3.zero;
  }
  

}
