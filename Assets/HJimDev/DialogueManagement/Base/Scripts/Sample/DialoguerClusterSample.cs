using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DialogueManagement
{
    namespace Core
    {
        namespace Sample
        {
            public class DialoguerClusterSample : DialoguerCluster
            {
                [SerializeField] Transform cameraTransform;
                [SerializeField] float cameraMovement = 3;
                Transform target;

                public override void SetTalker(string code)
                {
                    base.SetTalker(code);
                    Dialoguer dialoguer = GetCurrentDialoguer();
                    target = dialoguer ? dialoguer.transform : MainCharacter.transform;
                }

                private void Update()
                {
                    if (target)
                    {
                        Vector3 targetDir = target.position - cameraTransform.position;

                        Quaternion targetRotation = Quaternion.LookRotation(targetDir);

                        cameraTransform.rotation = Quaternion.Slerp(cameraTransform.rotation, targetRotation, cameraMovement * Time.deltaTime);
                    }
                }
            }
        }
    }
}