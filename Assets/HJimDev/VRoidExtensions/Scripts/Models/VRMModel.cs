using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniVRM10;

namespace VRoidExtensions
{
    namespace Models
    {
        public class VRMModel : HumanoidModel
        {
            [SerializeField] Controllers.LookAtController lookAt;

            public override void SetInformation()
            {
                if (!fixedInfo)
                {
                    if (!head)
                    {
                        head = model.transform.Find("Root/J_Bip_C_Hips/J_Bip_C_Spine/J_Bip_C_Chest/J_Bip_C_UpperChest/J_Bip_C_Neck/J_Bip_C_Head");
                    }
                    if (!leftHand)
                    {
                        leftHand = model.transform.Find("Root/J_Bip_C_Hips/J_Bip_C_Spine/J_Bip_C_Chest/J_Bip_C_UpperChest/J_Bip_L_Shoulder/J_Bip_L_UpperArm/J_Bip_L_LowerArm/J_Bip_L_Hand");
                    }
                    if (!rightHand)
                    {
                        rightHand = model.transform.Find("Root/J_Bip_C_Hips/J_Bip_C_Spine/J_Bip_C_Chest/J_Bip_C_UpperChest/J_Bip_R_Shoulder/J_Bip_R_UpperArm/J_Bip_R_LowerArm/J_Bip_R_Hand");
                    }
                    base.SetInformation();
                    model.GetComponent<Vrm10Instance>().LookAtTarget = lookAt.transform;
                    //model.GetComponent<UniVRM10.look.VRMLookAtHead>().Target = lookAt.transform;
                }
            }

            public void Blink()
            {
                Animator.SetTrigger("Blink");
            }

            /* LookAt */
            public void LookAt(Transform target)
            {
                lookAt.Follow(target);
            }

            public void LookAt(Vector3 target)
            {
                lookAt.Follow(target);
            }

            public void StopLookingAt()
            {
                lookAt.Follow(null);
            }
        }
    }
}