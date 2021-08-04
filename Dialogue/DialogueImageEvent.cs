using System;
using UnityEngine;
using UnityEngine.UI;

public class DialogueImageEvent : MonoBehaviour
{
    [SerializeField] private ImageEvent[] imageEvents;
    private DialogueManager dialogueManager;

    private void Awake()
    {
        dialogueManager = FindObjectOfType<DialogueManager>();

        dialogueManager.OnImageEvent += DialogueManager_OnImageEvent;
    }

    private void DialogueManager_OnImageEvent(int eventId)
    {
        bool find = false;
        foreach(ImageEvent i in imageEvents)
        {
            if(i.id == eventId)
            {
                find = true;
                SetImages(i.imageObject, i.newSprite);
                break;
            }
        }

        if (find == false)
        {
            Debug.LogError("Id do evento não encontrado (" + eventId + ")"+"(Evento Imagem)");
        }
    }

    private void SetImages(Image[] imagesObj, Sprite[] sprites)
    {
        int index = 0;
        foreach(Image i in imagesObj)
        {
            i.sprite = sprites[index];
            i.SetNativeSize();
            index++;
        }
    }
    
}

[Serializable]
public class ImageEvent
{
    public int id;
    public Image[] imageObject;
    public Sprite[] newSprite;
}
