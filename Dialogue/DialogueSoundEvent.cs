using System;
using UnityEngine;

public class DialogueSoundEvent : MonoBehaviour
{
    [SerializeField] private SoundEvent[] soundEvent;
    private DialogueManager dialogueManager;

    private void Awake()
    {
        dialogueManager = FindObjectOfType<DialogueManager>();

        dialogueManager.OnSoundEvent += DialogueManager_OnSoundEvent;
    }

    private void DialogueManager_OnSoundEvent(int eventId)
    {
        bool find = false;
        foreach(SoundEvent s in soundEvent)
        {
            if (s.id == eventId)
            {
                find = true;
                PlaySounds(s.audioSources, s.audioClips);
                break;
            }
        }

        if (find == false)
        {
            Debug.LogError("Id do evento não encontrado (" + eventId + ")"+"(Evento Som)");
        }

    }

    public void PlaySounds(AudioSource[] audioSources, AudioClip[] audioClips)
    {
        int index = 0;

        foreach(AudioSource a in audioSources)
        {
            a.clip = audioClips[index];
            a.Play();

            index++;
        }
    }

}

[Serializable]
public class SoundEvent
{
    public int id;
    public AudioSource[] audioSources;
    public AudioClip[] audioClips;
}
