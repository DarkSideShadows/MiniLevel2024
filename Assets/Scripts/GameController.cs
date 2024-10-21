using System.Collections;
using System.Collections.Generic;
using UnityEngine; // for Random.Range
using System.Linq;
using System.IO;
using UnityEngine.UI;

using TMPro;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;
using UnityEngine.UIElements;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;


public class GameController : MonoBehaviour
{
    public List<List<string>> dialogueGroup; // list of [question, aiAnswer, humanAnswer, humanAnswer]
    public List<bool> refugees = new List<bool>(); // human (true) or AI (false)
    public int numOfRobots, numOfHumans;

    public List<GameObject> characters;
    public float characterSpeed = 5;
    public float frequency = 1;
    public float amplitude = 0.25f;

    public float fallSpeed = 1;

    public TMP_Text optionA, optionB, responseBubble, responseText, questionBubble, questionText;

    public UnityEngine.UI.Button buttonA, buttonB, admit, deny;

    public int numOfRefugees;
    public int minHumans, maxHumans;
    private int counter = 0;  // To track the number of questions asked
    private bool human;

    private List<string> question_1, question_2;  // To store the selected questions
    private bool optionSelected = false;
    private bool decisionMade = false;
    private int selectedOption = 0;
    private bool decision = false;
    private int robotsAdmitted = 0;
    private int humansDenied = 0;

    private bool gameOver = false;

    // Start is called before the first frame update
    void Start()
    {
        /* Read and load Questions.txt from resources folder.
         * The question is index 0, AI answer is index 1, and human answers are indices 2 and 3.
         * so for one group: [question, aiAnswer, humanAnswer, humanAnswer]
         */
        dialogueGroup = new List<List<string>>();
        /* load file at Assets/Resources/Dialogue.txt */
        TextAsset textAsset = Resources.Load<TextAsset>("Dialogue");
        if (textAsset == null)
        {
            Debug.LogError("File not Found!");
            return; // exit
        }
        dialogueGroup = ParseFileContent(textAsset.text);

        // For debugging, display contents in Unity console
        //DisplayParsedContent();

        /* Initialize humans list */
        for (int i = 0; i < numOfHumans; i++)
            refugees.Add(true);
        for (int i = 0; i < numOfRobots; i++)
            refugees.Add(false);
        refugees = ShuffleListWithOrderBy(refugees);

        // Add listeners to buttons
        buttonA.onClick.AddListener(() => OnOptionSelected(1));  // 1 for option A
        buttonB.onClick.AddListener(() => OnOptionSelected(2));  // 2 for option B

        StartCoroutine(GameplayLoop());

        //AskQuestion();
    }

    //private void AskQuestion()
    //{
    //    int playerQuestionsToAsk = 1;
    //    while (playerQuestionsToAsk > 0)
    //    {
    //        bool isHuman = refugees[0]; // select whether first player is AI or human

    //        // TODO, increase this option to two (just make one work for now)
    //        /* select one random dialogues from the dialogueGroup list:
    //         * dialogueGroup: [question, aiAnswer, humanAnswer, humanAnswer]
    //         */
    //        int questionIdx = 0;
    //        int dialogueSize = dialogueGroup.Count;
    //        int dialogueIdx = Random.Range(0, dialogueSize);

    //        string playerQuestion = dialogueGroup[dialogueIdx][questionIdx];

    //        // TODO, implement gameplay loop
    //        /* Display the questions to the text box somehow.
    //         * Wait for player to select one using keyboard (1, 2, 3)
    //         * NPC reacts to playerQuestion (AI or human answer)
    //         */
    //        Debug.Log(playerQuestion);

    //        /* FindObjectOfType method looks through all GameObjects in "MainLevel" scene */
    //        // find GameObject with TextController component script (it is TextManager)
    //        TextController textController = FindObjectOfType<TextController>();

    //        /* 1. display question options */
    //        textController.DisplayQuestion(playerQuestion);
    //        /* 2. wait for player to choose question */

    //        dialogueGroup.RemoveAt(dialogueIdx); // don't reuse dialogue
    //        playerQuestionsToAsk--;
    //    }
    //}

    private IEnumerator GameplayLoop()
    {

        // Continue looping through phases until the humans list is empty
        while (refugees.Count > 0)
        {    
            optionSelected = false;
            decisionMade = false;
            selectedOption = 0;
            decision = false;

            // Call the Phase coroutine and wait for it to finish
            yield return StartCoroutine(Phase());
            
        }

        // After the loop, perform any actions when the game is over
        gameOver = true;

        if(robotsAdmitted > 0)
        {
            // TODO: Handle Scene changes
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 3);
            Debug.Log("The AI infiltrated your city and took over!");
        } else
        {
            // TODO: Handle Scene changes

            if (humansDenied > 0)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
                Debug.Log("The AI failed to infiltrated your city, but at least one innocent human lost their life at your hands!");
            } else
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                Debug.Log("The AI failed to infiltrated your city, and no innocent humans lost their lives at your hands!");
            }
        }
    }
    private IEnumerator Phase()
    {
        counter = 0;

        int characterIndex = Random.Range(0, characters.Count);
        float sinAdder = -Time.time * frequency;
        while (characters[characterIndex].transform.position.x < 0)
        {
            if(Mathf.Sin(Time.time * frequency + sinAdder) < 0)
            {
                sinAdder += Mathf.PI;
            }
            characters[characterIndex].transform.position = new Vector3(characters[characterIndex].transform.position.x + Time.deltaTime * characterSpeed, Mathf.Sin(Time.time * frequency + sinAdder) * amplitude + 0.5f, 0);
            yield return null;
        }
        // TODO: Animation of the character walking up

        while (counter < 3)
        {
            human = refugees[0];

            // Select 2 hypothetical questions from the questions list
            int index = Random.Range(0, dialogueGroup.Count);
            question_1 = dialogueGroup[index];
            dialogueGroup.RemoveAt(index);

            index = Random.Range(0, dialogueGroup.Count);
            question_2 = dialogueGroup[index];
            dialogueGroup.RemoveAt(index);

            optionA.SetText(question_1[0]);
            optionB.SetText(question_2[0]);

            // Reset selection flags
            optionSelected = false;
            selectedOption = 0;

            // Wait until the player selects an option (either button A or B)
            yield return new WaitUntil(() => optionSelected);

            responseBubble.gameObject.SetActive(true);
            questionBubble.gameObject.SetActive(true);

            // Respond to the player's selection
            if (human)
            {
                int rand = (int)Random.Range(2,4);
                // If it's a human, respond with a human answer
                if (selectedOption == 1)
                {
                    responseBubble.text = question_1[rand];
                    responseText.text = question_1[rand];
                    questionBubble.text = question_1[0];
                    questionText.text = question_1[0];
                    Debug.Log("Human Answer: " + question_1[rand]);
                }
                else
                {
                    responseBubble.text = question_2[rand];
                    responseText.text = question_2[rand];
                    questionBubble.text = question_2[0];
                    questionText.text = question_2[0];
                    Debug.Log("Human Answer: " + question_2[rand]);
                }
            }
            else
            {
                // If it's AI, respond with the AI answer
                if (selectedOption == 1)
                {
                    responseBubble.text = question_1[1];
                    responseText.text = question_1[1];
                    questionBubble.text = question_1[0];
                    questionText.text = question_1[0];
                    Debug.Log("AI Answer: " + question_1[1]);
                }
                else
                {
                    responseBubble.text = question_2[1];
                    responseText.text = question_2[1];
                    questionBubble.text = question_2[0];
                    questionText.text = question_2[0];
                    Debug.Log("AI Answer: " + question_2[1]);
                }
            }

            counter++;  // Increment counter
        }
        // Remove Options Text

        optionA.SetText("");
        optionB.SetText("");

        // Allow a decision
        admit.onClick.AddListener(() => OnDecision(true)); // True for the Admit Button
        deny.onClick.AddListener(() => OnDecision(false)); // False for the Deny Button

        // Wait until the player selects an option (either admit or deny)
        yield return new WaitUntil(() => decisionMade);

        // Disable decisions
        admit.onClick.RemoveAllListeners(); // True for the Admit Button
        deny.onClick.RemoveAllListeners(); // False for the Deny Button

        // Adjust game state based on decision outcome
        if (refugees[0])
        {
            if(!decision)
            {
                humansDenied++;
                Debug.Log("You killed a human.");
            }
        } else
        {
            if(decision)
            {
                robotsAdmitted++;
                Debug.Log("You let in a robot.");
            }
        }

        // Clear Text Bubbles
        responseBubble.gameObject.SetActive(false);
        questionBubble.gameObject.SetActive(false);

        // Remove the last interview from the list
        refugees.RemoveAt(0);

        // TODO: Animation of the character's fate
            // Use decision == false for denied, decision == true for admitted
        if(decision == false)
        {
            float startTime = Time.time;
            while(characters[characterIndex].transform.position.y > -15)
            {
                characters[characterIndex].transform.position -= new Vector3(0, fallSpeed * (Time.time - startTime) * (Time.time - startTime), 0);
                yield return null;
            }
        }
        else
        {
            sinAdder = -Time.time * frequency;
            while (characters[characterIndex].transform.position.x < 15)
            {
                if (Mathf.Sin(Time.time * frequency + sinAdder) < 0)
                {
                    sinAdder += Mathf.PI;
                }
                characters[characterIndex].transform.position = new Vector3(characters[characterIndex].transform.position.x + Time.deltaTime * characterSpeed, Mathf.Sin(Time.time * frequency + sinAdder) * amplitude + 0.5f, 0);
                yield return null;
            }
        }

        characters.RemoveAt(characterIndex);

        Debug.Log("End of Phase.");
    }

    private void OnOptionSelected(int option)
    {
        // Player selected an option
        optionSelected = true;
        selectedOption = option;  // Store the selected option
    }

    private void OnDecision(bool admit)
    {
        decision = admit;
        decisionMade = true;
    }

    /* @brief this method takes the content of Dialogue.txt
     *        and splits it into groups of four options:
     *        [question, aiAnswer, humanAnswer, humanAnswer]
     *        and adds it to a list
     */
    private List<List<string>> ParseFileContent(string fileContent)
    {
        List<List<string>> parsedData = new List<List<string>>();
        List<string> currentGroup = new List<string>();
        string[] lines = fileContent.Split('\n'); // split file content into lines

        foreach (string line in lines)
        {
            // Trim whitespace and check if the line is non-empty
            if (!string.IsNullOrEmpty(line))
                currentGroup.Add(line);

            // Once we have 4 elements in the current group, add it to the list and start a new group
            if (currentGroup.Count == 4)
            {
                /* Create a new list and pass that reference to parsedData */
                parsedData.Add(new List<string>(currentGroup)); // add a copy of the current group
                currentGroup.Clear(); // remove all elements, reuse same list for next group
            }
        }

        return parsedData;
    }

    // Display the parsed content to console
    public void DisplayParsedContent()
    {
        foreach (List<string> group in dialogueGroup)
        {
            Debug.Log("New Group:");
            foreach (string element in group)
                Debug.Log(element);
        }
    }

    public List<bool> ShuffleListWithOrderBy(List<bool> list)
    {
        System.Random random = new System.Random();
        return list.OrderBy(x => random.Next()).ToList();
    }
}