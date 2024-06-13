using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DialogueManagement
{
    namespace Base
    {
        namespace Sample
        {
            public class DialoguerClusterSample : Core.DialoguerCluster
            {
                [SerializeField] Transform cameraTransform;
                [SerializeField] float cameraMovement = 3;
                Transform target;

                public override void SetTalker(string code)
                {
                    base.SetTalker(code);
                    if(CurrentTalker == MainCharacter.Code)
                    {
                        target = MainCharacter.transform;
                    }
                    else if(CurrentDialoguer != null)
                    {
                        target = CurrentDialoguer.transform;
                    }
                    else
                    {
                        target = null;
                    }
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