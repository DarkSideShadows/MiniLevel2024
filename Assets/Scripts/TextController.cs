using UnityEngine;
using TMPro; // for TextMeshPro
using System.Collections.Generic;
using UnityEngine.UI;

/* class to change textContainer on screen */
public class TextController : MonoBehaviour
{
    /* textContainer can be any TMP object
     * will use this to display to multiple variable text containers:
     * button, main text, speech bubbles
     */
    public TextMeshProUGUI questionContainer, mainContainer, speechBubble;

    public void DisplayQuestion(string playerQuestion)
    {
        questionContainer.text = playerQuestion;
    }

    // called when button is clicked
    public void AskQuestion(Button button)
    {
        /* Access the Button's child text
         * Display that question to main text box
         */
        TextMeshProUGUI playerQuestionText = button.GetComponentInChildren<TextMeshProUGUI>();
        // TMPUGUI stores text on screen in TextMeshProUGUI.text, transfer to string vartype
        string playerQuestion = playerQuestionText.text;
        mainContainer.text = playerQuestion;
    }

    public void DisplayAIAnswer(string npcAnswer)
    {
        speechBubble.text = npcAnswer;
    }

    public void DisplayHumanAnswer(string humanAnswer)
    {
        speechBubble.text = humanAnswer;
    }
}