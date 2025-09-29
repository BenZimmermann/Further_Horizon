using UnityEngine;

public class GlobeAnimationManager : MonoBehaviour
{
    private Animator animator;
    [SerializeField, Tooltip("the globe")] GameObject globe;

    public void Awake()
    {
        //setting the animation to idle when the game starts
        animator = globe.GetComponent<Animator>();
        animator.SetBool("Idle", true);
    }
    private void OnTriggerEnter(Collider other)
    {
        //if the player is near the globe, play the open animation
        if (other.CompareTag("Player"))
        {
            animator.SetBool("Open", true);
            animator.SetBool("Idle", false);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        //reset to its original position when the player exits the trigger area
        if (other.CompareTag("Player"))
        {
            animator.SetBool("Open", false);
            animator.SetBool("Idle", true);
        }
    }
}
