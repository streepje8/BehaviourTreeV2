using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SmokeBomb : MonoBehaviour
{
    public Transform target;
    public Collider smokeCollider;
    public float height = 25;
    public float gravity = -18;
    private Rigidbody rb;
    private List<Guard> guards = new List<Guard>();
    private float lifetime = 0f;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = CalculateLaunchData().initialVelocity;
        rb.angularVelocity = new Vector3(2f, 0,0);
        guards = FindObjectsOfType<Guard>().ToList();
        lifetime = 10f;
    }

    private void Update()
    {
        lifetime -= Time.deltaTime;
        foreach (Guard g in guards)
        {
            if (smokeCollider.bounds.Contains(g.transform.position))
            {
                g.smokedTime = 2f;
            }
        }
        if (lifetime <= 0)
            Destroy(gameObject);
    }

    //From https://github.com/SebLague/Kinematic-Equation-Problems
    LaunchData CalculateLaunchData() {
        var position = target.position;
        var position1 = rb.position;
        float displacementY = position.y - position1.y;
        Vector3 displacementXZ = new Vector3 (position.x - position1.x, 0, position.z - position1.z);
        float time = Mathf.Sqrt(-2*height/gravity) + Mathf.Sqrt(2*(displacementY - height)/gravity);
        Vector3 velocityY = Vector3.up * Mathf.Sqrt (-2 * gravity * height);
        Vector3 velocityXZ = displacementXZ / time;

        return new LaunchData(velocityXZ + velocityY * -Mathf.Sign(gravity), time);
    }
    
    struct LaunchData {
        public readonly Vector3 initialVelocity;
        public readonly float timeToTarget;

        public LaunchData (Vector3 initialVelocity, float timeToTarget)
        {
            this.initialVelocity = initialVelocity;
            this.timeToTarget = timeToTarget;
        }
		
    }
}
