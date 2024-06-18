using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AudioManagement
{
    public class AudioSettings : MonoBehaviour
    {
        [Header("Resources Folder")]
        [SerializeField] string musicsFolder = "Musics";
        [SerializeField] string soundsFolder = "Sounds";

        [Header("Prefabs")]
        [SerializeField] GameObject musicPrefab;
        [SerializeField] GameObject soundPrefab;
        [SerializeField] [Range(1, 30)] int maxSoundInstances = 10;

        [Header("Volume")]
        [SerializeField] [Range(0f, 1f)] float soundVolume = 1;
        [SerializeField] [Range(0f, 1f)] float musicVolume = 1;
        [SerializeField] [Range(0.1f, 10)] float timeToUpDownMusicVolume = 3;
        [SerializeField] [Range(1, 30)] int intervalsToUpDownMusicVolume = 15;

        [Header("Defaults")]
        [SerializeField] AudioClip mainMusic;

        public string MusicsFolder { get { return musicsFolder; } set { musicsFolder = value; } }
        public string SoundsFolder { get { return soundsFolder; } set { soundsFolder = value; } }
        public GameObject MusicPrefab
        {
            get
            {
                if (!musicPrefab)
                {
                    musicPrefab = new("MusicPrefab");
                    musicPrefab.transform.SetParent(transform);
                    musicPrefab.transform.localPosition = Vector3.zero;
                    musicPrefab.AddComponent<AudioSource>();
                    musicPrefab.AddComponent<Music>();
                }
                return musicPrefab;
            }
            set { musicPrefab = value; }
        }
        public GameObject SoundPrefab
        {
            get
            {
                if (!soundPrefab)
                {
                    soundPrefab = new("SoundPrefab");
                    soundPrefab.transform.SetParent(transform);
                    soundPrefab.transform.localPosition = Vector3.zero;
                    soundPrefab.AddComponent<AudioSource>();
                    soundPrefab.AddComponent<Sound>();
                }
                return soundPrefab;
            }
            set { soundPrefab = value; }
        }
        public int MaxSoundInstances { get { return maxSoundInstances; } set { maxSoundInstances = value; } }
        public float MusicVolume
        {
            get { return musicVolume; }
            set { musicVolume = value > 1f ? 1f : (value < 0f ? 0f : value); }
        }
        public float SoundVolume
        {
            get { return soundVolume; }
            set { soundVolume = value > 1f ? 1f : (value < 0f ? 0f : value); }
        }
        public float TimeToUpDownMusicVolume
        {
            get { return timeToUpDownMusicVolume; }
            set { timeToUpDownMusicVolume = value < 0.1f ? 0.1f : value; }
        }
        public int IntervalsToUpDownMusicVolume
        {
            get { return intervalsToUpDownMusicVolume; }
            set { intervalsToUpDownMusicVolume = value < 1 ? 1 : value; }
        }

        protected virtual void Start()
        {
            AudioManager.DefaultManager.Settings = this;
            AudioManager.DefaultManager.PlayMainMusic(mainMusic);
        }
    }
}