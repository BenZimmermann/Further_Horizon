using UnityEngine;

public class GlobeAnimationManager : MonoBehaviour
{
    private Animator animator;
    [SerializeField, Tooltip("the globe")] GameObject globe;

    public void Awake()
    {
        animator = globe.GetComponent<Animator>();
        animator.SetBool("Idle", true);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            animator.SetBool("Open", true);
            animator.SetBool("Idle", false);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            animator.SetBool("Open", false);
            animator.SetBool("Idle", true);
        }
    }
}
