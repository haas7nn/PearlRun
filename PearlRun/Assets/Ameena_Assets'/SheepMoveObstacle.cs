using UnityEngine;

namespace Ursaanimation.CubicFarmAnimals
{
    public class SheepMoveObstacle : MonoBehaviour
    {
        [Header("Movement Points")]
        public Transform startPoint;
        public Transform endPoint;

        [Header("Movement")]
        public float speed = 5f;
        public bool hideAtEnd = true;

        [Header("Animation")]
        public Animator animator;
        public string runAnimation = "run_forward";

        private bool isMoving = false;

        private void Start()
        {
            if (animator == null)
            {
                animator = GetComponent<Animator>();
            }

            if (startPoint != null)
            {
                transform.position = startPoint.position;
            }

            gameObject.SetActive(false);
        }

        private void Update()
        {
            if (!isMoving || endPoint == null) return;

            transform.position = Vector3.MoveTowards(
                transform.position,
                endPoint.position,
                speed * Time.deltaTime
            );

            Vector3 direction = endPoint.position - transform.position;
            direction.y = 0f;

            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }

            if (Vector3.Distance(transform.position, endPoint.position) < 0.2f)
            {
                isMoving = false;

                if (hideAtEnd)
                {
                    gameObject.SetActive(false);
                }
            }
        }

        public void StartSheepRun()
        {
            if (startPoint != null)
            {
                transform.position = startPoint.position;
            }

            gameObject.SetActive(true);
            isMoving = true;

            if (animator != null)
            {
                animator.Play(runAnimation);
            }
        }
    }
}