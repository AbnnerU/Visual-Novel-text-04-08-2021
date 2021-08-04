using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueTextAnimations : MonoBehaviour
{
    [SerializeField]private TMP_Text textComponent;

    [SerializeField] private AnimationCurve animCurve;

    [SerializeField] private Gradient gradient;

    private DialogueVisual dialogueVisual;

    private string[] animationType;

    //private AnimationType animationType;

    Dictionary<int, WordAnimationAtributes> wordsToChange = new Dictionary<int, WordAnimationAtributes>();

    private Vector3[] defaltVerticesPosition;

    private TMP_TextInfo _textInfo;

    private Mesh mesh;

    private int key = 0;

    private void Awake()
    {
        animationType = AllTags.animationType;

        _textInfo = textComponent.textInfo;

        mesh = textComponent.mesh;

        dialogueVisual = FindObjectOfType<DialogueVisual>();

        dialogueVisual.OnNewText += DialogueVisual_OnNewText;

    }

    public string CheckLinkID(string type)
    {
        for(int i = 0; i < animationType.Length; i++)
        {

            if (animationType[i].Contains(type))
            {
                return type;
            }
        }

        Debug.LogError(type + " não é um nome de animação valido. Verificar os <links>");
        return null;
    }


    //private void DialogueManager_OnSkipWriting()
    //{

    //}

    private void DialogueVisual_OnNewText()
    {
        key++;
        StopAllCoroutines();    

        wordsToChange.Clear();

        int linkCount = _textInfo.linkCount;

        print(linkCount);
        print(textComponent.text);

        if (linkCount > 0)
        {
            mesh = textComponent.mesh;

            defaltVerticesPosition = mesh.vertices;


            TMP_LinkInfo[] _linkInfo = _textInfo.linkInfo;

            //List<string> wordsToChange = new List<string>();

            for (int i = 0; i < linkCount; i++)
            {
                string animation = CheckLinkID(_linkInfo[i].GetLinkID());

                WordAnimationAtributes wordAtributes = new WordAnimationAtributes();

                wordAtributes.firtCharIndex = _linkInfo[i].linkTextfirstCharacterIndex;

                wordAtributes.animationType = animation;

                wordAtributes.lastCharIndex = _linkInfo[i].linkTextfirstCharacterIndex + _linkInfo[i].GetLinkText().Length-1;

                wordsToChange.Add(i, wordAtributes);
            }

            ExucuteWordsAnimations();
        }
    }

    private void ExucuteWordsAnimations()
    {
        //exeuteCoroutines = true;
        for(int i =0; i < wordsToChange.Count; i++)
        {
            //StopAllCoroutines();
            string animation = wordsToChange[i].animationType;

            if (animation.Contains(animationType[0]))
            {
               /* Coroutine newCoroutine =*/ StartCoroutine(Wave(wordsToChange[i].firtCharIndex, wordsToChange[i].lastCharIndex));

                //animCoroutines.Add(newCoroutine);
            }
            else if (animation.Contains(animationType[1]))
            {
                /*Coroutine newCoroutine =*/ StartCoroutine(Rainbow(wordsToChange[i].firtCharIndex, wordsToChange[i].lastCharIndex));

                //animCoroutines.Add(newCoroutine);

            }
        }

        //print(animCoroutines.Count);
        //print(animCoroutines[0]);
        //print(animCoroutines[1]);
    }

    IEnumerator Wave(int firtIndex,int lastIndex)
    {
        int currentKey = key;

        textComponent.ForceMeshUpdate();

        Vector3[] newVerticesPosition;

        newVerticesPosition = defaltVerticesPosition;

        while (currentKey == key)
        {
           
            for (int i = firtIndex; i < lastIndex + 1; i++)
                {
                   
                    int vertexIndex = _textInfo.characterInfo[i].vertexIndex;

                    float offsetY = animCurve.Evaluate(Mathf.Repeat(Time.time + (defaltVerticesPosition[vertexIndex].x * 0.001f), 1)) * 10;

                    newVerticesPosition[vertexIndex].y = defaltVerticesPosition[vertexIndex].y + offsetY;
                    newVerticesPosition[vertexIndex + 1].y = defaltVerticesPosition[vertexIndex + 1].y + offsetY;
                    newVerticesPosition[vertexIndex + 2].y = defaltVerticesPosition[vertexIndex + 2].y + offsetY;
                    newVerticesPosition[vertexIndex + 3].y = defaltVerticesPosition[vertexIndex + 3].y + offsetY;
                  
                }

            if (_textInfo.characterInfo[firtIndex].isVisible)
            {
                mesh.vertices = newVerticesPosition;

                textComponent.canvasRenderer.SetMesh(mesh);
            }
            yield return new WaitForSeconds(0.05f);
        }

        print(currentKey + " // " + key);
        yield return null;
    }


    IEnumerator Rainbow(int firtIndex, int lastIndex)
    {
        int currentKey = key;

        textComponent.ForceMeshUpdate();
        Color[] colors;

        Vector3[] vertices;

        mesh = textComponent.mesh;

        colors = mesh.colors;

        vertices = mesh.vertices;

        int wordCount = _textInfo.wordCount;

     
        while (currentKey == key)
        {           
                for (int i = firtIndex; i < lastIndex + 1; i++)
                {                
                       int vertexIndex = _textInfo.characterInfo[i].vertexIndex;

                       colors[vertexIndex] = gradient.Evaluate(Mathf.Repeat(Time.time + (vertices[vertexIndex].x * 0.001f), 1));
                       colors[vertexIndex + 1] = gradient.Evaluate(Mathf.Repeat(Time.time + (vertices[vertexIndex + 1].x * 0.001f), 1));
                       colors[vertexIndex + 2] = gradient.Evaluate(Mathf.Repeat(Time.time + (vertices[vertexIndex + 2].x * 0.001f), 1));
                       colors[vertexIndex + 3] = gradient.Evaluate(Mathf.Repeat(Time.time + (vertices[vertexIndex + 3].x * 0.001f), 1));
                }

                mesh.colors = colors;

                textComponent.canvasRenderer.SetMesh(mesh);

                yield return new WaitForSeconds(0.05f);
        }
       
        yield return null;
    }

}

public class WordAnimationAtributes
{
    public int firtCharIndex;

    public int lastCharIndex;

    public string animationType;
}


