using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AudioManagement
{
    namespace Sample
    {
        public class AudioManagerSample : MonoBehaviour
        {
            public void PlaySound()
            {
                AudioManager.DefaultManager.PlaySound("explosion");
            }

            public void PlayMainMusic()
            {
                AudioManager.DefaultManager.PlayMainMusic();
            }

            public void PlaySecondaryMusic()
            {
                string[] musics = { "Music01", "Music02", "Music03", "Music04", "Music05" };
                int index = Random.Range(0, musics.Length);
                Debug.Log("Playing " + musics[index]);
                AudioManager.DefaultManager.PlaySecondaryMusic(musics[index]);
            }
        }
    }
}