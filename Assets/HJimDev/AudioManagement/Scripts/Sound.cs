using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AudioManagement
{
    [RequireComponent(typeof(AudioSource))]
    public class Sound : MonoBehaviour
    {
        AudioSource audioSource;
        bool infoFixed;
        bool played;

        void SetInformation()
        {
            if (!infoFixed)
            {
                infoFixed = true;
                audioSource = GetComponent<AudioSource>();
                audioSource.loop = false;
                audioSource.playOnAwake = false;
            }
        }

        public void Play(AudioClip clip)
        {
            SetInformation();
            played = true;
            audioSource.volume = AudioManager.DefaultManager.Settings.SoundVolume;
            audioSource.clip = clip;
            audioSource.Play();
        }

        void Update()
        {
            if(played)
            {
                if(!audioSource.isPlaying)
                {
                    played = false;
                    Destroy(gameObject);
                }
            }
        }
    }
}