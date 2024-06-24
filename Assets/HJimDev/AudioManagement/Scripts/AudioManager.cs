using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AudioManagement
{
    public class AudioManager : MonoBehaviour
    {
        private static AudioManager manager;
        
        Music mainMusic;
        Music currentMusic;
        Transform soundContainer;
        bool infoFixed;
        AudioSettings settings;

        public static AudioManager DefaultManager
        {
            get
            {
                if (!manager)
                {
                    GameObject instance = new("MusicManager");
                    manager = instance.AddComponent<AudioManager>();
                    DontDestroyOnLoad(instance);
                }
                return manager;
            }
        }

        public AudioSettings Settings {
            get { return settings; }
            set {
                settings = value;
                if (!infoFixed)
                {
                    infoFixed = true;
                    GameObject container = new("SoundContainer");
                    container.transform.SetParent(transform);
                    container.transform.localPosition = Vector3.zero;
                    soundContainer = container.transform;
                }
            }
        }

        public void PlayMainMusic(string music)
        {
            AudioClip clip = Resources.Load<AudioClip>(JoinPaths(Settings.MusicsFolder, music));
            PlayMainMusic(clip);
        }

        public void PlayMainMusic()
        {
            if (mainMusic) PlayMainMusic(mainMusic.Clip);
        }

        public void PlayMainMusic(AudioClip clip)
        {
            if (infoFixed)
            {
                if (!mainMusic)
                {
                    mainMusic = Instantiate(Settings.MusicPrefab, transform).GetComponent<Music>();
                    currentMusic = mainMusic;
                    currentMusic.Play(clip);
                }
                else if (mainMusic.Clip.name == clip.name)
                {
                    if (currentMusic != mainMusic)
                    {
                        if (currentMusic) currentMusic.Destroy();
                        currentMusic = mainMusic;
                        currentMusic.Play();
                    }
                }
                else
                {
                    if (currentMusic != mainMusic)
                    {
                        Destroy(mainMusic.gameObject);
                    }
                    currentMusic.Destroy();
                    mainMusic = Instantiate(Settings.MusicPrefab, transform).GetComponent<Music>();
                    currentMusic = mainMusic;
                    currentMusic.Play(clip);
                }
            }
        }

        public void PlaySecondaryMusic(string music)
        {
            AudioClip clip = Resources.Load<AudioClip>(JoinPaths(Settings.MusicsFolder, music));
            PlaySecondaryMusic(clip);
        }

        public void PlaySecondaryMusic(AudioClip clip)
        {
            if (infoFixed)
            {
                if (!currentMusic || currentMusic.Clip.name != clip.name)
                {
                    if (mainMusic && currentMusic == mainMusic) currentMusic.Pause();
                    else if (currentMusic) currentMusic.Destroy();
                    currentMusic = Instantiate(Settings.MusicPrefab, transform).GetComponent<Music>();
                    currentMusic.Play(clip);
                }
            }
        }

        public void PlaySound(string sound)
        {
            if (soundContainer.childCount < Settings.MaxSoundInstances)
            {
                AudioClip clip = Resources.Load<AudioClip>(JoinPaths(Settings.SoundsFolder, sound));
                PlaySound(clip);
            }   
        }

        public void PlaySound(AudioClip clip)
        {
            if (infoFixed)
            {
                if (soundContainer.childCount < Settings.MaxSoundInstances)
                {
                    Instantiate(Settings.SoundPrefab, soundContainer).GetComponent<Sound>().Play(clip);
                }
            }
        }

        public static string JoinPaths(string path1, string path2)
        {
            if (path1.Length == 0) return path2;
            if (path2.Length == 0) return path1;
            path1 = path1[^1] == '/' && path2[0] == '/' ? path1[..^1] : path1;
            return path1 + (path1[^1] != '/' && path2[0] != '/' ? "/" : "") + path2;
        }
    }
}