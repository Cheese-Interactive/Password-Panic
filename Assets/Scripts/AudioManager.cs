using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    [Header("References")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource soundSource;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private AudioClip correctSound;
    [SerializeField] private AudioClip partialIncorrectSound;
    [SerializeField] private AudioClip incorrectSound;

    private void Start() {

        musicSource.loop = true;

        PlayBackgroundMusic(backgroundMusic); // play background music

    }

    private void PlayBackgroundMusic(AudioClip backgroundMusic) {

        musicSource.clip = backgroundMusic;
        musicSource.Play();

    }

    public void PlaySound(SoundEffectType soundType) {

        switch (soundType) {

            case SoundEffectType.Correct:

                soundSource.PlayOneShot(correctSound);
                break;

            case SoundEffectType.PartialIncorrect:

                soundSource.PlayOneShot(partialIncorrectSound);
                break;

            case SoundEffectType.Incorrect:

                soundSource.PlayOneShot(incorrectSound);
                break;

        }
    }
}
