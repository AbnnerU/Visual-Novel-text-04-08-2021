using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DialogueManager : MonoBehaviour
{
    [SerializeField] private TextAsset textFile;

    public List<string> allValidyFileLines;

    public Dictionary<string, AnswersOptionsLine> answersGroup = new Dictionary<string, AnswersOptionsLine>();

    private string[] mainTags;

    private string[] eventsTag;

    private string[] optionsTag;

    private string[] systemTag;

    private string[] allFileLines;

    private int currentLine;

    private string currentAnswersId;

    public enum DialogueMode { NORMAL, WRITING, ANSWERSOPTIONS, ENDED };

    public enum EventType { IMAGE,SOUND,ANIMATION}

    private DialogueMode dialogueState = DialogueMode.NORMAL;

    //Events
    public Action<string, string> OnNewDialogueSetence;

    public Action<string> OnActiveAnswers;

    public Action<int> OnSaveEvent;

    public Action<string> OnUnlockEvent;

    public Action<int> OnImageEvent;

    public Action<int> OnSoundEvent;

    public Action<int> OnAnimationEvent;

    public Action OnSkipWriting;

    private void Awake()
    {
        mainTags = AllTags.mainTags;
        optionsTag = AllTags.optionsTag;
        systemTag = AllTags.systemTags;
        eventsTag = AllTags.eventsTag;

        SplitFile();

        AssociateAnswers();

        currentLine = allValidyFileLines.FindIndex(x => x.Contains(mainTags[0]));
    }

    public void NextLine()
    {
        //print("NextLine");
        if(dialogueState == DialogueMode.WRITING)
        {
            OnSkipWriting?.Invoke();
            return;
        }
        if (dialogueState == DialogueMode.NORMAL)
        {
            currentLine++;
            string lineText = allValidyFileLines[currentLine];

            if (lineText.Contains(mainTags[2]))//n
            {

                string name = lineText.Substring(mainTags[2].Length);

                currentLine++;

                string setence = allValidyFileLines[currentLine].Substring(mainTags[3].Length);

                OnNewDialogueSetence?.Invoke(name, setence);

                dialogueState = DialogueMode.WRITING;
            }
            else if (lineText.Contains(mainTags[4]))//answers
            {
                dialogueState = DialogueMode.ANSWERSOPTIONS;

                string answerId = lineText.Substring(mainTags[4].Length);

                currentAnswersId = answerId;

                print(allValidyFileLines[currentLine + 1].Substring(mainTags[5].Length));

                OnActiveAnswers?.Invoke(allValidyFileLines[currentLine + 1].Substring(mainTags[5].Length));
               
            }
            else if (lineText.Contains(systemTag[0]))//UNLOCK
            {
                string levelToUnlock = lineText.Substring(systemTag[0].Length);
                OnUnlockEvent?.Invoke(levelToUnlock);
                //print("Level liberado: " + levelToUnlock);

                NextLine();

            }
            else if (lineText.Contains(systemTag[1]))//SAVE
            {
                OnSaveEvent?.Invoke(currentLine);

                print("Salvo " + currentLine);

                NextLine();
                
            }
            else if (lineText.Contains(eventsTag[0]))//IMAGEEVENT
            {
                string valueText = lineText.Substring(eventsTag[0].Length);

                CallEvent(valueText, EventType.IMAGE);              
            }
            else if (lineText.Contains(eventsTag[1]))//SOUNDEVENT
            {
                string valueText = lineText.Substring(eventsTag[1].Length);

                CallEvent(valueText, EventType.SOUND);
            }
            else if (lineText.Contains(eventsTag[2]))//ANIMATIONEVENT
            {
                string valueText = lineText.Substring(eventsTag[2].Length);

                CallEvent(valueText, EventType.ANIMATION);
            }
            else if (lineText.Contains(mainTags[1]))//end
            {
                dialogueState = DialogueMode.ENDED;
                print("ENDED");
            }
        }

    }

    public void SelectedOption(int number)
    {
        AnswersOptionsLine temp = null;

        if(answersGroup.TryGetValue(currentAnswersId, out temp))
        {
            if (number == 1)
                currentLine = temp.opt1Line;
            else if (number == 2)
                currentLine = temp.opt2Line;
            else if (number == 3)
                currentLine = temp.opt3Line;
            else if (number == 4)
                currentLine = temp.opt4Line;

            dialogueState = DialogueMode.NORMAL;

            NextLine();
        }
        else
        {
            Debug.LogError("Esse id da resposta não existe: "+ currentAnswersId);
        }
    }

    private void SplitFile()
    {
        allFileLines = textFile.text.Split('\n');

        foreach (string s in allFileLines)
        {
            bool haveMainTag = false;
            foreach (string tags1 in mainTags)
            {
                if (s.Contains(tags1))
                {
                    allValidyFileLines.Add(s);
                    haveMainTag = true;
                    break;
                }        
            }

            if (haveMainTag == false)
            {
                bool haveSystemTag = false;
                foreach (string tags2 in systemTag)
                {
                    if (s.Contains(tags2))
                    {
                        allValidyFileLines.Add(s);
                        haveSystemTag = true;
                        break;
                    }
                }

                if(haveSystemTag == false)
                {
                    foreach(string tags3 in eventsTag)
                    {
                        if (s.Contains(tags3))
                        {
                            allValidyFileLines.Add(s);
                            break;
                        }
                    }
                }
            }
        }
    }

    private void AssociateAnswers()
    {
        for (int i = 0; i < allValidyFileLines.Count; i++)
        {
            if (allValidyFileLines[i].Contains(mainTags[4]))
            {
                AnswersOptionsLine answerAssocied = new AnswersOptionsLine();
                string id = allValidyFileLines[i].Substring(mainTags[4].Length);

                for (int y = i; y < allValidyFileLines.Count; y++)
                {
                   foreach(string s in optionsTag)
                   {
                        if (allValidyFileLines[y].Contains(s))
                        {
                            string optionId = allValidyFileLines[y].Substring(s.Length);

                            if (optionId == id)
                            {
                                if (s == AllTags.optionsTag[0])
                                {
                                    answerAssocied.opt1Line = y;
                                }
                                else if (s == AllTags.optionsTag[1])
                                {
                                    answerAssocied.opt2Line = y;
                                }
                                else if (s == AllTags.optionsTag[2])
                                {
                                    answerAssocied.opt3Line = y;
                                }
                                else if (s == AllTags.optionsTag[3])
                                {
                                    answerAssocied.opt4Line = y;
                                }
                            }
                        }
                   }
                }

                answerAssocied.idLegth = id.Length;

                answersGroup.Add(id, answerAssocied);

            }
        }
        
    }

    private void CallEvent(string value, EventType eventType)
    {
        int toIntValue;

        if (int.TryParse(value, out toIntValue))
        {
            if(eventType == EventType.IMAGE)
            {
                OnImageEvent?.Invoke(toIntValue);
            }
            else if(eventType == EventType.SOUND)
            {
                OnSoundEvent?.Invoke(toIntValue);
            }
            else if(eventType == EventType.ANIMATION)
            {
                OnAnimationEvent?.Invoke(toIntValue);
            }
           
            NextLine();
        }
        else
        {
            Debug.LogError("Não foi possivel converter o id do evento (" + value + ")"+"("+eventType+")");
        }
    }

    public void SetCurrentLine(int value)
    {
        currentLine = value;
    }

    public void FinishedWriting()
    {
        dialogueState = DialogueMode.NORMAL;
    }

}

public class AnswersOptionsLine
{
    public int idLegth;
    //public int buttonOptTextLine;
    public int opt1Line;
    public int opt2Line;
    public int opt3Line;
    public int opt4Line;
}

