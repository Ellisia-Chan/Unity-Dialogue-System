using Ink.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour {

    public event EventHandler OnDialogueEnd;

    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogText;

    private Story currentStory;

    private bool dialogueIsPlaying;

    public static DialogueManager Instance { get; private set; }

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
        dialoguePanel.SetActive(false);

        GameInput.Instance.OnContinueAction += GameInput_OnContinueAction;
    }

    private void GameInput_OnContinueAction(object sender, EventArgs e) {
        if (dialogueIsPlaying) {
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
        } else {
            StartCoroutine(ExitDialogueMode());
        }
    }

    public bool IsDialoguePlaying() {
        return dialogueIsPlaying;
    }

    private void OnDestroy() {
        GameInput.Instance.OnContinueAction -= GameInput_OnContinueAction;
    }
}
