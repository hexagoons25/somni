using TMPro;
using System;
using System.Linq;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Unity.VisualScripting;

[Serializable]
public struct Dialogue
{
    public string by;
    [Multiline]
    public string content;
    public Texture2D background;
    public AudioClip audio;
}


public class Scene : MonoBehaviour
{
    public Dialogue[] dialogues;
    public float dialogueSpeed = 0.05F;

    private int index = 0;
    private TextMeshProUGUI contentLabel;
    private TextMeshProUGUI speakerLabel;
    private RawImage backgroundRenderer;
    private AudioSource effectPlayer;
    private CanvasGroup group;

    private readonly char[] semiStopChars = { ',' };
    private readonly char[] fullStopChars = { '.', '!', '?' };

    public void Run()
    {
        if (Global.viewingScene)
        {
            return;
        }
        Global.viewingScene = true;
        Cursor.visible = true;
        GameObject groupObj = transform.Find("Group").gameObject;
        group = groupObj.GetComponent<CanvasGroup>();
        backgroundRenderer = transform.Find("Background").gameObject.GetComponent<RawImage>();
        speakerLabel = groupObj.transform.Find("DialogueBox").gameObject.transform.Find("Speaker").gameObject.GetComponent<TextMeshProUGUI>();
        contentLabel = groupObj.transform.Find("DialogueBox").gameObject.transform.Find("Content").gameObject.GetComponent<TextMeshProUGUI>();
        effectPlayer = transform.Find("EffectPlayer").gameObject.GetComponent<AudioSource>();

        index = 0;
        SetupDialogue();
        StartCoroutine(TypeDialogue());
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (contentLabel.text == dialogues[index].content)
            {
                NextDialogue();
            }
            else
            {
                StopAllCoroutines();
                effectPlayer.Stop();
                contentLabel.text = dialogues[index].content;
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Destroy();
        }
    }

    private IEnumerator TypeDialogue()
    {
        foreach (char c in dialogues[index].content.ToCharArray())
        {
            contentLabel.text += c;
            float multiplier = fullStopChars.Contains(c) ? 6.0F : semiStopChars.Contains(c) ? 3.0F : 1.0F;
            if (contentLabel.text.Length == dialogues[index].content.Length && effectPlayer.isPlaying)
            {
                effectPlayer.Stop();
            }
            yield return new WaitForSeconds(dialogueSpeed * multiplier);
        }
    }

    private void SetupDialogue()
    {
        if (effectPlayer.isPlaying)
        {
            effectPlayer.Stop();
        }

        Dialogue dialogue = dialogues[index];
        if (dialogue.by == string.Empty)
        {
            group.alpha = 0;
            group.interactable = false;
            contentLabel.text = string.Empty;
        }
        else
        {
            group.alpha = 1;
            group.interactable = true;
            speakerLabel.text = dialogue.by + ":";
            contentLabel.text = string.Empty;
        }

        backgroundRenderer.texture = dialogue.background;
        if (dialogue.audio != null)
        {
            effectPlayer.clip = dialogue.audio;
            effectPlayer.Play();
        }
    }

    private void NextDialogue()
    {
        if (index < dialogues.Length - 1)
        {
            index++;
            SetupDialogue();
            StartCoroutine(TypeDialogue());
        }
        else
        {
            Destroy();
        }
    }

    public void Destroy()
    {
        Global.viewingScene = false;
        Cursor.visible = false;
        speakerLabel.text = string.Empty;
        contentLabel.text = string.Empty;
        gameObject.SetActive(false);
    }
}
