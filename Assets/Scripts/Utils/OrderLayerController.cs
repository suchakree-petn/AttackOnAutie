using Spine.Unity;
using UnityEngine;

public class OrderLayerController : MonoBehaviour
{
  SpriteRenderer sprite;
  MeshRenderer mesh;

  private void Awake()
  {
    TryGetComponent<SpriteRenderer>(out sprite);
    TryGetComponent<MeshRenderer>(out mesh);
  }

  private void Update()
  {
    if (sprite != null)
      sprite.sortingOrder = Mathf.RoundToInt(transform.position.y * -100);
    if (mesh != null)
      mesh.sortingOrder = Mathf.RoundToInt(transform.position.y * -100);
  }
}
