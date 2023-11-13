using UnityEngine;

public class RelativeMovement : MonoBehaviour
{
    public Vector3 damagedEnemyPos;
    public Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        transform.position = damagedEnemyPos;
        animator.SetTrigger("damaged"); 
    }
}
