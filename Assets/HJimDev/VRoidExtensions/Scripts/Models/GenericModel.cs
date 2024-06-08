using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRoidExtensions
{
    namespace Models
    {
        public class GenericModel : MonoBehaviour
        {
            [SerializeField] protected GameObject model;
            [SerializeField] AudioSource audioSource;

            public Animator Animator { get; private set; }
            public AudioSource Audio { get; private set; }
            public Transform Transform { get; private set; }

            protected bool fixedInfo;

            public virtual void SetInformation()
            {
                if (!fixedInfo)
                {
                    fixedInfo = true;

                    Animator = model.GetComponent<Animator>();
                    Transform = model.transform;
                    Audio = audioSource;
                }
            }

            public void Anim(string animation)
            {
                Animator.SetTrigger(animation);
            }

            public void ReplaceModel(GameObject model)
            {
                if (this.model) DestroyImmediate(this.model);
                this.model = model;
                this.model.transform.SetParent(transform);
                this.model.transform.localPosition = Vector3.zero;
                this.model.transform.localRotation = Quaternion.identity;
                this.model.transform.localScale = Vector3.one;
            }
        }
    }
}