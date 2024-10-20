using System.Collections;
using System.Collections.Generic;
using UnityEngine; // for Random.Range
using System.Linq;
using System.IO;

public class GameController : MonoBehaviour
{
    public List<List<string>> dialogueGroup; // list of [question, aiAnswer, humanAnswer, humanAnswer]
    public List<bool> refugees = new List<bool>(); // human (true) or AI (false)
    public int numOfRobots, numOfHumans;

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
        if (textAsset == null) {
            Debug.LogError("File not Found!");
            return; // exit
        }
        dialogueGroup = ParseFileContent(textAsset.text);

        //DisplayParsedContent();

        /* Initialize humans list */
        for (int i = 0; i < numOfHumans; i++)
            refugees.Add(true);
        for (int i = 0; i < numOfRobots; i++)
            refugees.Add(false);
        refugees = ShuffleListWithOrderBy(refugees);

        //AskQuestions();
    }

    private void AskQuestions()
    {
        int playerQuestionsToAsk = 3;
        while (playerQuestionsToAsk > 0)
        {
            bool isHuman = refugees[0]; // select whether first player is AI or human

            // TODO, increase this option to two (just make one work for now)
            /* select one random dialogues from the dialogueGroup list:
             * dialogueGroup: [question, aiAnswer, humanAnswer, humanAnswer]
             */
            int questionIdx = 0;
            int dialogueSize = dialogueGroup.Count;
            int dialogueIdx = Random.Range(0, dialogueGroup.Count);

            string playerQuestion = dialogueGroup[dialogueIdx][questionIdx];

            // TODO, implement gameplay loop
            /* Display the questions to the text box somehow.
             * Wait for player to select one using keyboard (1, 2, 3)
             * NPC reacts to playerQuestion (AI or human answer)
             */

            dialogueGroup.RemoveAt(dialogueIdx); // don't use this dialogue anymore
            playerQuestionsToAsk--;
        }
    }

    /* @brief this method takes the content of Dialogue.txt
     *        and splits it into groups of four options:
     *        [question, aiAnswer, humanAnswer, humanAnswer]
     *        and adds it to a list
     */
    private List<List<string>> ParseFileContent(string fileContent)
    {
        List<List<string>> parsedData = new List<List<string>>();
        string[] lines = fileContent.Split('\n'); // split the file content into lines

        foreach (string line in lines)
        {
            List<string> currentGroup = new List<string>();

            // Trim whitespace and check if the line is non-empty
            if (!string.IsNullOrEmpty(line))
                currentGroup.Add(line);

            // Once we have 4 elements in the current group, add it to the list and start a new group
            if (currentGroup.Count == 4)
            {
                parsedData.Add(currentGroup); // add dialogueGroup
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
            {
                Debug.Log(element);
            }
        }
    }

    void OnGUI()
    {
        // Optionally display the parsed content in the Unity UI for debugging
        if (dialogueGroup != null)
        {
            foreach (List<string> group in dialogueGroup)
            {
                GUILayout.Label("New Group:");
                foreach (string element in group)
                {
                    GUILayout.Label(element);
                }
            }
        }
    }

    public List<bool> ShuffleListWithOrderBy(List<bool> list)
    {
        System.Random random = new System.Random();
        return list.OrderBy(x => random.Next()).ToList();
    }
}