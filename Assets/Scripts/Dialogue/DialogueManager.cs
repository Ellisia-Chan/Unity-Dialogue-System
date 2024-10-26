using Ink.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DialogueManager : MonoBehaviour {

    public static DialogueManager Instance { get; private set; }

    public event EventHandler OnDialogueEnd;

    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogText;

    [Header("Choice UI")]
    [SerializeField] private GameObject[] choices;
    private TextMeshProUGUI[] choicesText;

    private Story currentStory;
    private bool dialogueIsPlaying;
    private bool isDisplayingChoices;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Debug.LogWarning("Found more than One Instance of Dialogue Manager in the scene");
        } else {
            Instance = this;
        }
    }

    public static DialogueManager GetInstance() {
        return Instance;
    }

    private void Start() {
        dialogueIsPlaying = false;
        isDisplayingChoices = false;
        dialoguePanel.SetActive(false);

        GetChoicesText();

        GameInput.Instance.OnContinueAction += GameInput_OnContinueAction;
    }

    private void GameInput_OnContinueAction(object sender, EventArgs e) {
        if (dialogueIsPlaying && !isDisplayingChoices) {
            ContinueStory();
        }
    }

    public void EnterDialogueMode(TextAsset inkJSON) {
        currentStory = new Story(inkJSON.text);
        dialogueIsPlaying = true;
        dialoguePanel.SetActive(true);

        ContinueStory();
    }

    private IEnumerator ExitDialogueMode() {
        yield return new WaitForSeconds(0.2f);

        OnDialogueEnd?.Invoke(this, EventArgs.Empty);

        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
        dialogText.text = "";
    }

    private void ContinueStory() {
        if (currentStory.canContinue) {
            dialogText.text = currentStory.Continue();
            DisplayChoiceses();
        } else {
            StartCoroutine(ExitDialogueMode());
        }
    }

    private void DisplayChoiceses() {
        List<Choice> currentChoices = currentStory.currentChoices;

        isDisplayingChoices = currentChoices.Count > 0;

        if (!isDisplayingChoices) {
            if (choices[0].gameObject.activeSelf) {
                foreach (GameObject choice in choices) {
                    choice.SetActive(false);
                }
            }
            return;
        }

        // Check if UI can support the number of choices coming in from ink file
        if (currentChoices.Count > choices.Length) {
            Debug.LogError("More Choices were given than the UI can support. Number of choices given: " + currentChoices.Count);
        }

        int index = 0;
        foreach (Choice choice in currentChoices) {
            choices[index].gameObject.SetActive(true);
            choicesText[index].text = choice.text;
            index++;
        }
    }

    private void GetChoicesText() {
        choicesText = new TextMeshProUGUI[choices.Length];

        int index = 0;
        foreach (GameObject choice in choices) {
            choicesText[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
            index++;
        }
    }

    public void MakeChoice(int choiceIndex) {
        isDisplayingChoices = false;
        EventSystem.current.SetSelectedGameObject(null);

        currentStory.ChooseChoiceIndex(choiceIndex);
        ContinueStory();
    }

    public bool IsDialoguePlaying() {
        return dialogueIsPlaying;
    }

    private void OnDestroy() {
        GameInput.Instance.OnContinueAction -= GameInput_OnContinueAction;
    }
}
