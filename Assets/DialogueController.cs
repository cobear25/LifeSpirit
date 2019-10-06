using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueController : MonoBehaviour
{
    [SerializeField]
    private Text dialogueText;
    public float textDelay;

    [HideInInspector]
    private int messageIndex = 0;
    private bool showingChoices = false;

    [HideInInspector]
    // Start is called before the first frame update
    void Start()
    {
        dialogueText.CrossFadeAlpha(0.0f, 0.0f, true);
    }

    // Update is called once per frame
    void Update()
    {
    }

    IEnumerator typeTextCoroutine;

    string[] messages;
    public void DisplayMessages(string[] messages) {
        this.messages = messages;
        messageIndex = 0;
        DisplayMessage();
    }

    public void DisplayMessage() {
        dialogueText.text = "";
        dialogueText.text = messages[messageIndex];
        dialogueText.CrossFadeAlpha(1.0f, 3.0f, true);
        Invoke("FadeOutMessage", 3.0f);
        messageIndex++;
        if (messageIndex < messages.Length) {
            Invoke("DisplayMessage", 5.5f);
        }
    }

    void FadeOutMessage() {
        dialogueText.CrossFadeAlpha(0.0f, 2.0f, true);
    }

    
    IEnumerator TypeText (string message) {
         foreach (char letter in message.ToCharArray()) {
             dialogueText.text += letter;
             yield return new WaitForSeconds (textDelay);
         }
     }

    public void Dismiss() {
        dialogueText.text = "";
        messageIndex = 0;
    }
}