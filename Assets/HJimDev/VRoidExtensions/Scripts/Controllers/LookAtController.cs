using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VRoidExtensions
{
    namespace Controllers
    {
        public class LookAtController : MonoBehaviour
        {
            [SerializeField] Transform observer;
            [SerializeField] float movementSpeed = 2f;
            Vector3 initialLocalPosition;

            private void Start()
            {
                initialLocalPosition = transform.localPosition;
            }

            Transform target;
            Vector3 targetPosition;

            bool followingTarget;

            public void Follow(Transform target)
            {
                this.target = target;
                targetPosition = Vector3.zero;
                if (target == null)
                {
                    followingTarget = false; ;
                }
                else
                {
                    followingTarget = true;
                }
            }
            public void Follow(Vector3 targetPosition)
            {
                this.targetPosition = targetPosition;
                target = null;
                followingTarget = true;
            }

            private void Update()
            {
                if (followingTarget)
                {
                    if (target != null)
                    {
                        transform.position = Vector3.Lerp(transform.position, target.position, movementSpeed * Time.deltaTime);
                    }
                    else
                    {
                        transform.position = Vector3.Lerp(transform.position, targetPosition, movementSpeed * Time.deltaTime);
                    }
                }
                else
                {
                    transform.localPosition = Vector3.Lerp(transform.localPosition, initialLocalPosition, movementSpeed * Time.deltaTime);
                }
            }
        }
    }
}