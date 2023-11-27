using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    public Transform targetPos;
    private float maxDistance = .5f;
    private Vector3 direction;
    private Vector3 movement;
    private Quaternion rotation;
    [HideInInspector] public bool follow = true;

    [SerializeField] private ParticleSystem _promptParticles;
    public ParticleSystem.EmissionModule fairyDust;

    // Start is called before the first frame update
    void Start()
    {
        fairyDust = _promptParticles.emission;
        fairyDust.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(this.transform.position, targetPos.position) > maxDistance)
        {
            fairyDust.enabled = true;
            MoveTowardsTarget();
        }
        else
        {
            fairyDust.enabled = false;
        }

    }

    private void MoveTowardsTarget()
    {
        if (follow)
        {
            direction = (targetPos.position - this.transform.position).normalized;
            movement = direction * PlayerMovement.instance.moveSpeed * Time.deltaTime;

            direction.y = 0;

            rotation = Quaternion.LookRotation(direction);

            transform.rotation = rotation;

            transform.position += movement;
        }
    }


}
