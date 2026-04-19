using UnityEngine;

public class SkiSticker : MonoBehaviour
{
    private float velocityThreshold;
    private Rigidbody2D rb;
    private bool stuck;

    public void Setup(float threshold)
    {
        velocityThreshold = threshold;
        rb = GetComponent<Rigidbody2D>();
    }

    // Comprueba si el objeto choca contra el suelo con suficiente fuerza
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (stuck) return;

        if (collision.gameObject.CompareTag("Ground"))
        {
            if (collision.relativeVelocity.magnitude > velocityThreshold)
            {
                StuckToGround(collision);
            }
        }
    }

    // Fija el objeto al suelo creando un Joint
    private void StuckToGround(Collision2D collision)
    {
        stuck = true;
        
        FixedJoint2D joint = gameObject.AddComponent<FixedJoint2D>();
        
        if (collision.rigidbody != null)
        {
            joint.connectedBody = collision.rigidbody;
        }
        else
        {
            joint.connectedAnchor = collision.contacts[0].point;
        }

        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
}