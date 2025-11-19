using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using System.Collections;
using LanguageLocalization;
using UnityEngine.InputSystem;
using static Unity.VisualScripting.Member;

public class DialogueController : MonoBehaviour
{
    public Text dialogueText, textBuffer, NameText;
    public Canvas dialogueBox;
    public PlayableDirector director;
    public Localization_KEY key;
    public Localization_SOURCE source;
    public OptionSetter setter;
    public luna player;
    public EnemyPathfindingSystem[] enemies;

    private bool waitingForInput = false;

    private void Start()
    {
        director.Pause();
    }

    public void ShowDialogue(string message)
    {

        dialogueBox.enabled = true;
        dialogueText.text = "";
        NameText.text= "";
        key.keyID = message;
        source.RefreshTextElementsAndKeys();
        source.LoadLanguage(setter.getLanguageIndex());

        string[] textIndex = textBuffer.text.Split('/'); //Plot Design(Name:/Dialogue)
        NameText.text = textIndex[0];
        StartCoroutine(TypeSentence(textIndex[1]));

        //StopMovement();
        Time.timeScale = 0f; // Pause the game
        director.playableGraph.GetRootPlayable(0).SetSpeed(1);
        waitingForInput = true;
        director.Pause(); 
    }

    IEnumerator TypeSentence(string sentence)
    {
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSecondsRealtime(0.05f);
        }
    }

    void Update()
    {
        if (waitingForInput && (Input.GetKeyDown(KeyCode.Z) || Input.GetMouseButtonDown(0)))
        {

            waitingForInput = false;
            dialogueBox.enabled = false;
            //continueMovement();
            Time.timeScale = 1f; // Resume the game
            director.Resume(); 
        }
    }

    void StopMovement() 
    {
        player.enabled = false;
        foreach (var enemy in enemies)
        {
            enemy.enabled = false;
        }
    }

    void continueMovement() 
    {
        player.enabled = true;
        foreach (var enemy in enemies)
        {
            enemy.enabled = true;
        }
    }
}
