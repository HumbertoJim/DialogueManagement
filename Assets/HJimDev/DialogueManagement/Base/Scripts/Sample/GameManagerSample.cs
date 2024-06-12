using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DialogueManagement
{
    namespace Core
    {
        namespace Sample
        {
            public class GameManagerSample : MonoBehaviour
            {
                [Header("Dependencies")]
                [SerializeField] DialogueManager dialogueManager;
    
                [Header("Test")]
                [SerializeField] string dialogueCode;
                [SerializeField] bool playOnAwake = true;

                bool play = false;

                private void Start()
                {
                    if(playOnAwake) play = true;
                }

                private void Update()
                {
                    if (play)
                    {
                        play = false;
                        PlayDialogue();
                    }
                }

                public void PlayDialogue()
                {
                    dialogueManager.PlayDialogue(dialogueCode);
                }
            }
        }
    }
}