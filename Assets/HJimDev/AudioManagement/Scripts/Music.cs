using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ScheduleManagement.Schedulers;
using ScheduleManagement.Schedulables;


namespace AudioManagement
{
    [RequireComponent(typeof(AudioSource))]
    public class Music : MonoBehaviour
    {
        AudioManager manager;
        AudioSource audioSource;
        Scheduler scheduler;
        float volume;
        bool infoFixed;

        public AudioClip Clip { get { return audioSource.clip; } }

        void SetInformation()
        {
            if (!infoFixed)
            {
                infoFixed = true;
                audioSource = GetComponent<AudioSource>();
                audioSource.loop = true;
                audioSource.playOnAwake = false;
                audioSource.volume = 0;
                manager = AudioManager.DefaultManager;
                scheduler = new();
                scheduler.Add("UpVolume", new Schedulable(manager.Settings.TimeToUpDownMusicVolume / manager.Settings.IntervalsToUpDownMusicVolume, () =>
                {
                    volume += manager.Settings.MusicVolume / manager.Settings.IntervalsToUpDownMusicVolume;
                    if(volume < manager.Settings.MusicVolume) scheduler.Start("UpVolume"); else volume = manager.Settings.MusicVolume;
                    audioSource.volume = volume;
                }));
                scheduler.Add("DownVolume", new Schedulable(manager.Settings.TimeToUpDownMusicVolume / manager.Settings.IntervalsToUpDownMusicVolume, () =>
                {
                    volume -= manager.Settings.MusicVolume / manager.Settings.IntervalsToUpDownMusicVolume;
                    if (volume > 0) scheduler.Start("DownVolume"); else volume = 0;
                    audioSource.volume = volume;
                }));
                scheduler.Add("PauseLater", new Schedulable(manager.Settings.TimeToUpDownMusicVolume, () => audioSource.Pause()));
                scheduler.Add("StopLater", new Schedulable(manager.Settings.TimeToUpDownMusicVolume, () => audioSource.Stop()));
                scheduler.Add("DestroyLater", new Schedulable(manager.Settings.TimeToUpDownMusicVolume, () => Destroy(gameObject)));
            }
        }

        private void Update()
        {
            if (infoFixed) scheduler.Process();
        }

        public void Play(AudioClip clip)
        {
            SetInformation();
            audioSource.clip = clip;
            Play();
        }

        public void Play()
        {
            if(infoFixed)
            {
                scheduler.Cancel("DownVolume");
                scheduler.Start("UpVolume");
                audioSource.Play();
            }
        }

        public void Pause()
        {
            if (infoFixed)
            {
                scheduler.Cancel("UpVolume");
                scheduler.Start("DownVolume");
                scheduler.Start("PauseLater");
            }
        }

        public void Stop()
        {
            if (infoFixed)
            {
                scheduler.Cancel("UpVolume");
                scheduler.Start("DownVolume");
                scheduler.Start("StopLater");
            }
        }

        public void Destroy()
        {
            if (infoFixed)
            {
                Stop();
                scheduler.Start("DestroyLater");
            }
        }
    }
}