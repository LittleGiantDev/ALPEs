using UnityEngine;

public class SkiSticker : MonoBehaviour
{
    private float velocityThreshold;
    private Rigidbody2D myRb;
    private bool stuck;

    public void Setup(float threshold)
    {
        velocityThreshold = threshold;
        myRb = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (stuck || myRb == null) return;

        if (collision.gameObject.CompareTag("Ground"))
        {
            if (collision.relativeVelocity.magnitude > velocityThreshold)
            {
                StuckToGround(collision);
            }
        }
    }

    private void StuckToGround(Collision2D collision)
    {
        stuck = true;
        
        if (collision.rigidbody != null)
        {
            FixedJoint2D joint = gameObject.AddComponent<FixedJoint2D>();
            joint.connectedBody = collision.rigidbody;
        }
        else
        {
            myRb.linearVelocity = Vector2.zero;
            myRb.angularVelocity = 0f;
            myRb.bodyType = RigidbodyType2D.Kinematic;
        }
    }
}