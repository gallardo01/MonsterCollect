/*
Simple Sound Manager (c) 2016 Digital Ruby, LLC
http://www.digitalruby.com

Source code may no longer be redistributed in source format. Using this in apps and games is fine.
*/

using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using VoxelBusters.CoreLibrary;

// Be sure to add this using statement to your scripts
// using DigitalRuby.SoundManagerNamespace

namespace DigitalRuby.SoundManagerNamespace
{
    public class SoundManagerDemo : SingletonBehaviour<SoundManagerDemo>
    {
        private bool isMute = true;

        void Awake()
        {
            //SoundManager.StopAll();
            DontDestroyOnLoad(transform.gameObject);
            if (PlayerPrefs.GetInt("Audio") == 1)
            {
                isMute = true;
            }
            else
            {
                isMute = false;
            }
            setVolumeAll(PlayerPrefs.GetInt("Audio"));
        }

        public AudioSource[] SoundAudioSources;

        private void PlaySound(int index)
        {
            SoundAudioSources[index].PlayOneShotSoundManaged(SoundAudioSources[index].clip);
        }

        private void PlayMusic(int index)
        {
            SoundAudioSources[index].PlayLoopingMusicManaged(1.0f, 1.0f, false);
        }
        public void setVolumeAll(int volume)
        {
            for (int i = 0; i < SoundAudioSources.Length; i++)
            {
                SoundAudioSources[i].volume = volume;
                SoundAudioSources[i].mute = volume == 0 ? true : false;
            }
        }
        public void playOneShot(int index)
        {
            PlaySound(index);
        }

        public void playMusic(int index)
        {
             PlayMusic(index);
        }

        private void Start()
        {
            //SoundManager.StopSoundsOnLevelLoad = true;
        }

        public void StopAudio(int num)
        {
            SoundAudioSources[num].Stop();
        }
    }
}
