using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueVisual : MonoBehaviour
{
    [SerializeField] private TMP_Text namesText;
    [SerializeField] private TMP_Text setenceText;

    //[SerializeField] private DialogueTextAnimations dialogueTextAnimations;

    [SerializeField] private GameObject answersOptionsPanel;
    [SerializeField] private GameObject[] optButtons;
    [SerializeField] private TMP_Text[] optText;

    [SerializeField] private float textWriteSpeed = 0.05f;

    [HideInInspector]
    public List<Button> buttonsComponent;

    public Action OnNewText;

    private DialogueManager dialogueManager;

    private TMP_TextInfo _textInfo;

    private int level = 0;

    private int currentOptIndex;

    private string currentButtonText;

    

    private void Awake()
    {
        _textInfo = setenceText.textInfo;

        answersOptionsPanel.SetActive(false);

        dialogueManager = FindObjectOfType<DialogueManager>();

        dialogueManager.OnNewDialogueSetence += DialogueManager_OnNewDialogue;

        dialogueManager.OnActiveAnswers += DialogueManager_OnActiveAnswers;

        dialogueManager.OnSkipWriting += DialogueManager_OnSkipWriting;

        foreach (GameObject g in optButtons)
        {
            Button button = g.GetComponent<Button>();

            if (button)
            {
                buttonsComponent.Add(button);
            }
            else
            {
                print("objeto " + g + " não tem o componente button");
            }
        }
    }

    private void DialogueManager_OnSkipWriting()
    {
       
        StopAllCoroutines();
        setenceText.ForceMeshUpdate();

        int totalCharacters = _textInfo.characterCount;

        setenceText.maxVisibleCharacters = totalCharacters;

        dialogueManager.FinishedWriting();
    }

    private void DialogueManager_OnNewDialogue(string name, string setence)
    {
        namesText.text = name;
        
        setenceText.text = setence;

        setenceText.ForceMeshUpdate();

        OnNewText?.Invoke();

        //dialogueManager.FinishedWriting();
        setenceText.maxVisibleCharacters = 0;
       
        StartCoroutine(WriteSetence());
    }

    private void DialogueManager_OnActiveAnswers(string answersOptions)
    {
        answersOptionsPanel.SetActive(true);

        string[] textToButtons = answersOptions.Split(';');

        for (int i = 0; i < textToButtons.Length; i++)
        {
            currentOptIndex = i;
            if (textToButtons[i].Contains("null"))
            {
                optButtons[i].SetActive(false);
            }
            else
            {
                currentButtonText = textToButtons[i];

                optButtons[i].SetActive(true);
                optText[i].text = currentButtonText;

                if (SeachLink(optText[i],currentButtonText) == false)
                {
                    buttonsComponent[i].interactable = true;
                    //optText[i].text = currentButtonText;
                }

            }
        }
    }

    private bool SeachLink(TMP_Text textComponent,string text)
    {
        textComponent.ForceMeshUpdate();
        TMP_TextInfo info = textComponent.textInfo;
        

        int linkCount = info.linkCount;
        if (linkCount > 0)
        {
            ChooseAction(info.linkInfo[0]);

            return true;
        }
        else
        {
            return false;
        }
    }

    private void ChooseAction(TMP_LinkInfo _linkInfo)
    {
        string linkID = _linkInfo.GetLinkID();
        print(linkID);
        if (linkID.Contains("LOCK"))
        {
            int value = StringToInt(_linkInfo.GetLinkText());

            if (value >= 0)
            {
                if (level >= value)
                {
                    print("Comparação lock");
                    int substringVAlue = _linkInfo.linkTextfirstCharacterIndex + _linkInfo.linkTextLength;

                    buttonsComponent[currentOptIndex].interactable = true;
                    optText[currentOptIndex].text = currentButtonText.Substring(substringVAlue);
                }
                else
                {
                    buttonsComponent[currentOptIndex].interactable = false;
                    optText[currentOptIndex].text = "";
                }
            }
        }

    }

    private int StringToInt(string text)
    {
        int textToInt;

        if (int.TryParse(text, out textToInt))
        {
            return textToInt;
        }
        else
        {
            Debug.LogError("O valor passado no texto '" + text + "' não pode ser convertido para int");
            return -1;
        }
    }

    public void DisableAnswersPanel()
    {
        answersOptionsPanel.SetActive(false);
    }

    IEnumerator WriteSetence()
    {

        setenceText.ForceMeshUpdate();
        //T/*MP_TextInfo textInfo = setenceText.textInfo;*/
        int totalCharacters = _textInfo.characterCount;

        int count = 0;

        while (count < totalCharacters)
        {
            count++;
            setenceText.maxVisibleCharacters = count;

            yield return new WaitForSeconds(textWriteSpeed);
        }

        dialogueManager.FinishedWriting();
    }

}
