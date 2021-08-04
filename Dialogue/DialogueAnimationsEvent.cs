using System;
using UnityEngine;

public class DialogueAnimationsEvent : MonoBehaviour
{
    [SerializeField] private AnimationEvent[] animationEvent;
    private DialogueManager dialogueManager;

    private void Awake()
    {
        dialogueManager = FindObjectOfType<DialogueManager>();

        dialogueManager.OnAnimationEvent += DialogueManager_OnAnimationEvent;
    }

    private void DialogueManager_OnAnimationEvent(int eventId)
    {
        bool find = false;
        foreach(AnimationEvent a in animationEvent)
        {
            if(a.id == eventId)
            {
                find = true;
                StartAnimation(a.animators, a.animationName);
                break;
            }
        }

        if (find == false)
        {
            Debug.LogError("Id do evento não encontrado (" + eventId + ")" + "(Evento Animação)");
        }
    }

    private void StartAnimation(Animator[] animators, string[] name)
    {
        int index = 0;
        foreach(Animator a in animators)
        {
            a.Play(name[index], 0, 0);
            index++;
        }
    }
}

[Serializable]
public class AnimationEvent
{
    public int id;
    public Animator[] animators;
    public string[] animationName;
}
