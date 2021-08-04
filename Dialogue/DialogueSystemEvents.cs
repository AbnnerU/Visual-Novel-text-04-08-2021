using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class DialogueSystemEvents : MonoBehaviour
{
    [HideInInspector]
    public AllVSN allVSN;

    [HideInInspector]
    public VSNTextInfo textInfo;

    private DialogueManager dialogueManager;

    private string sceneName;

    private int index = -1;

    private bool alreadyLoad;

    private void Awake()
    {
        print(Data.vsnCurrentLineFilePath);
        sceneName = SceneManager.GetActiveScene().name;

        dialogueManager = FindObjectOfType<DialogueManager>();

        dialogueManager.OnUnlockEvent += DialogueManager_OnUnlockEvent;

        dialogueManager.OnSaveEvent += DialogueManager_OnSave;
    }

    private void Start()
    {
        if (SaveData.LoadVSNCurrentLine(Data.vsnCurrentLineFilePath) != null)
        {
            textInfo = SaveData.LoadVSNCurrentLine(Data.vsnCurrentLineFilePath);

            if (textInfo.textLine == null)
            {
                textInfo.textLine = new List<VSNCurrentLine>();
            }

            int index = textInfo.textLine.FindIndex(x => x.vsnName.Contains(sceneName));

            if (index >= 0)
            {
                dialogueManager.SetCurrentLine(textInfo.textLine[index].vsnLine);
            }
            else
            {
                VSNCurrentLine vsnCurrentLine = new VSNCurrentLine();
                vsnCurrentLine.vsnName = sceneName;
                vsnCurrentLine.vsnLine = 0;

                textInfo.textLine.Add(vsnCurrentLine);

                SaveData.SaveVSNLine(textInfo, Data.vsnCurrentLineFilePath);
            }
        }
        else
        {
            print(textInfo);
            if (textInfo.textLine == null)
            {
                textInfo.textLine = new List<VSNCurrentLine>();
            }

            VSNCurrentLine vsnCurrentLine = new VSNCurrentLine();
            vsnCurrentLine.vsnName = sceneName;
            vsnCurrentLine.vsnLine = 0;

            textInfo.textLine.Add(vsnCurrentLine);

            SaveData.SaveVSNLine(textInfo, Data.vsnCurrentLineFilePath);
        }
    }

    private void DialogueManager_OnUnlockEvent(string levelToUnlock)
    {
        if(allVSN.allVSNAtributes == null)
        {
            allVSN.allVSNAtributes = new List<VSNAtributes>();
        }

        allVSN = SaveData.Load(Data.vsnFilePath);

        //print("Tamanho " + levelToUnlock.Length);
        levelToUnlock = levelToUnlock.Substring(0, levelToUnlock.Length - 1);

        print(allVSN.allVSNAtributes[1].name + "/" + levelToUnlock);

        int index = allVSN.allVSNAtributes.FindIndex(x => x.name.Contains(levelToUnlock));
        print(index);

        if (index >= 0)
        {
            allVSN.allVSNAtributes[index].state = VSNAtributes.State.UNLOCKED;

            SaveData.Save(allVSN, Data.vsnFilePath);

            print("Level liberado " + levelToUnlock);
        }
        else
        {
            Debug.LogError(levelToUnlock + " não encontrado");
        }
    }

    private void DialogueManager_OnSave(int line)
    {
        if (textInfo.textLine == null)
        {
            textInfo.textLine = new List<VSNCurrentLine>();
        }

        //textInfo = SaveData.LoadVSNCurrentLine(Data.vsnCurrentLineFilePath);

        int index = textInfo.textLine.FindIndex(x => x.vsnName.Contains(sceneName));

        if (index >= 0)
        {
            textInfo.textLine[index].vsnLine = line;

            SaveData.SaveVSNLine(textInfo, Data.vsnCurrentLineFilePath);

            print("Salvo" + line);
        }
        else
        {
            Debug.LogError("Não encontrado :" + sceneName);
        }
    }
}
