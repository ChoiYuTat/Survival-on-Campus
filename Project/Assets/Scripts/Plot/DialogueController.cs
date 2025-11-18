using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using System.Collections;
using LanguageLocalization;
using UnityEngine.InputSystem;
using static Unity.VisualScripting.Member;

public class DialogueController : MonoBehaviour
{
    public Text dialogueText, textBuffer;
    public Canvas dialogueBox;
    public PlayableDirector director;
    public Localization_KEY key;
    public Localization_SOURCE source;
    public OptionSetter setter;

    private bool waitingForInput = false;

    private void Start()
    {
        director.Pause();
    }

    public void ShowDialogue(string message)
    {

        dialogueBox.enabled = true;
        dialogueText.text = "";
        key.keyID = message;
        source.RefreshTextElementsAndKeys();
        source.LoadLanguage(setter.getLanguageIndex());
        Debug.Log(setter.getLanguageIndex());
        StartCoroutine(TypeSentence(textBuffer.text));
        waitingForInput = true;
        director.Pause(); 
    }

    IEnumerator TypeSentence(string sentence)
    {
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.05f);
        }
    }

    void Update()
    {
        if (waitingForInput && (Input.GetKeyDown(KeyCode.Z) || Input.GetMouseButtonDown(0)))
        {

            waitingForInput = false;
            dialogueBox.enabled = false;
            director.Resume(); 
        }
    }
}
