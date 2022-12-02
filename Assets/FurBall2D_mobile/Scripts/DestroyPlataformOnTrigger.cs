using UnityEngine;

public class DestroyPlataformOnTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlataformEndPosition"))
        {
            Destroy(collision.gameObject.transform.parent.gameObject);
            EndlessRunnerManager.Instance.PlataformDestroyed();
        }
    }
}
