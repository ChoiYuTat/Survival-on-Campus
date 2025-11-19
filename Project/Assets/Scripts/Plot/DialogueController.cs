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

    private bool typing = false;
    private bool waitingForInput = false;

    private void Start()
    {
        director.Pause();
    }

    public void ShowDialogue(string message)
    {
        Debug.Log("Showing dialogue: " + message);
        dialogueBox.enabled = true;
        dialogueText.text = "";
        NameText.text= "";
        key.keyID = message;
        source.RefreshTextElementsAndKeys();
        source.LoadLanguage(setter.getLanguageIndex());

        string[] textIndex = textBuffer.text.Split('/'); //Plot Design(Name:/Dialogue)
        NameText.text = textIndex[0];
        StartCoroutine(TypeSentence(textIndex[1]));

        PauseGame(); // Pause the game
        director.playableGraph.GetRootPlayable(0).SetSpeed(1); // Ensure timeline is playing
        waitingForInput = true;
        director.Pause(); 
    }

    public void PauseGame() 
    {
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
    }

    IEnumerator TypeSentence(string sentence)
    {
        typing = true;
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSecondsRealtime(0.05f);

            if (!typing)
            {
                dialogueText.text = sentence;
                yield break;
            }
        }
        typing = false;
    }

    void Update()
    {
        if (waitingForInput && (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0)))
        {
            if (typing)
            {
                typing = false;
                return;
            }
            waitingForInput = false;
            dialogueBox.enabled = false;
            //Time.timeScale = 1f; // Resume the game
            director.Resume(); 
        }
    }
}
