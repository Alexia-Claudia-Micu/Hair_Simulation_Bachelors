using UnityEngine;

public class CharacterAnimationController : MonoBehaviour
{
    public Animator animator;


    void Update()
    {
        // Toggle IsRotating on and off when the "R" key is pressed
        if (Input.GetKeyDown(KeyCode.R))
        {
            bool isRotating = animator.GetBool("IsRotating");
            bool newIsRotating = !isRotating;
            animator.SetBool("IsRotating", newIsRotating);

            // Log the key press and the new animation state
            Debug.Log("R key pressed. IsRotating: " + newIsRotating);
        }

        // Toggle IsMovingSideToSide on and off when the "S" key is pressed
        if (Input.GetKeyDown(KeyCode.S))
        {
            bool isMovingSideToSide = animator.GetBool("IsMovingSideToSide");
            bool newIsMovingSideToSide = !isMovingSideToSide;
            animator.SetBool("IsMovingSideToSide", newIsMovingSideToSide);

            // Log the key press and the new animation state
            Debug.Log("S key pressed. IsMovingSideToSide: " + newIsMovingSideToSide);
        }

        // Toggle IsMovingFrontBack on and off when the "F" key is pressed
        if (Input.GetKeyDown(KeyCode.F))
        {
            bool isMovingFrontBack = animator.GetBool("IsMovingFrontBack");
            bool newIsMovingFrontBack = !isMovingFrontBack;
            animator.SetBool("IsMovingFrontBack", newIsMovingFrontBack);

            // Log the key press and the new animation state
            Debug.Log("F key pressed. IsMovingFrontBack: " + newIsMovingFrontBack);
        }
    }
}
