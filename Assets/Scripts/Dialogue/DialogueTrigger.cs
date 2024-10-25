using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour {

    private event EventHandler OnPlayerInRange;

    [Header("Visual Cue")]
    [SerializeField] private GameObject visualCue;

    [Header("Ink JSON")]
    [SerializeField] private TextAsset inkJSON;

    private bool playerInRange;

    private void Awake() {
        playerInRange = false;
        visualCue.SetActive(false);
    }

    private void Start() {
        OnPlayerInRange += DialogueTrigger_OnPlayerInRange;

        GameInput.Instance.OnInteractAction += GameInput_OnInteractAction;
        DialogueManager.Instance.OnDialogueEnd += DialogueManager_OnDialogueEnd;
    }

    private void DialogueManager_OnDialogueEnd(object sender, EventArgs e) {
        if (playerInRange) {
            visualCue.SetActive(true);
        } else {
            visualCue.SetActive(false);
        }
    }

    private void DialogueTrigger_OnPlayerInRange(object sender, EventArgs e) {
        if (playerInRange && !DialogueManager.Instance.IsDialoguePlaying()) {
            visualCue.SetActive(true);
        } else {
            visualCue.SetActive(false);
        }
    }

    private void GameInput_OnInteractAction(object sender, EventArgs e) {
        if (playerInRange && !DialogueManager.Instance.IsDialoguePlaying()) {
            DialogueManager.GetInstance().EnterDialogueMode(inkJSON);
            visualCue.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.GetComponent<PlayerController>()) {
            playerInRange = true;
            OnPlayerInRange?.Invoke(this, EventArgs.Empty);
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.GetComponent<PlayerController>()) {
            playerInRange = false;
            OnPlayerInRange?.Invoke(this, EventArgs.Empty);
        }
    }

    private void OnDestroy() {
        OnPlayerInRange -= DialogueTrigger_OnPlayerInRange;
        GameInput.Instance.OnInteractAction -= GameInput_OnInteractAction;
    }
}
