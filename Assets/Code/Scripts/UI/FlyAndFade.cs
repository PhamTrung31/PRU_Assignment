using UnityEngine;

public class FlyAndFade : MonoBehaviour
{
    public float flySpeed = 2f;
    public float fadeDuration = 1.2f;
    public float scaleMultiplier = 1.5f;

    private float timer = 0f;
    private SpriteRenderer sprite;

    private Vector3 initialScale;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        initialScale = transform.localScale;
    }

    void Update()
    {
        timer += Time.deltaTime;

        // Move Up
        transform.position += Vector3.up * flySpeed * Time.deltaTime;

        // Scale Up
        transform.localScale = Vector3.Lerp(initialScale, initialScale * scaleMultiplier, timer / fadeDuration);

        // Fade Out
        if (sprite != null)
        {
            Color c = sprite.color;
            c.a = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            sprite.color = c;
        }

        if (timer >= fadeDuration)
        {
            Destroy(gameObject);
        }
    }
}
