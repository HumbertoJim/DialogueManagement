using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DialogueManagement;
using DialogueManagement.Core;
using DialogueManagement.Tools;

public class CharacterDialogueModule : Module
{
    [Header("Dialoguers")]
    [SerializeField] Dialoguer[] dialoguers;
    const string INITIAL_COMMAND = "initial";
    const string LOAD_COMMAND = "";
    const string MOOD_COMMAND = "";
    const string ANIM_COMMAND = "";
    const string MOVE_COMMAND = "";
    const string ROTATE_COMMAND = "";
    const string IMPROVE_RELATION_COMMAND = "";
    const string DECREASE_RELATION_COMMAND = "";
    const string REGISTER_INTERACTION_COMMAND = "";
    const string ENABLE_COMMAND = "";
    const string DISABLE_COMMAND = "";

    Dictionary<string, Dialoguer> Dialoguers { get; set; }

    private void Start()
    {
        Dialoguers = new Dictionary<string, Dialoguer>();
        foreach(Dialoguer dialoguer in dialoguers)
        {
            Dialoguers[dialoguer.Code] = dialoguer;
        }
    }

    public override void HandleCommand(string command)
    {
        if (StringExtension.TextStartsWith(command, LOAD_COMMAND))
        {
            command = command[LOAD_COMMAND.Length..].Trim();
            if(Dialoguers.TryGetValue(command, out Dialoguer dialoguer))
            {
                dialoguer.LookAt(playerController.GetHead());
                dialoguer.Turn(playerController.GetHead());
            }
        }
        else if (StringExtension.TextStartsWith(command, MOOD_COMMAND))
        {
            command = command[MOOD_COMMAND.Length..].Trim();
            string[] sentence = command.Split(' ');
            if (sentence.Length > 1 && Dialoguers.TryGetValue(sentence[0], out Dialoguer dialoguer))
            {
                dialoguer.FacialExpression(command[sentence[0].Length..].Trim());
            }
        }
        else if (StringExtension.TextStartsWith(command, ANIM_COMMAND))
        {
            command = command[ANIM_COMMAND.Length..].Trim();
            string[] sentence = command.Split(' ');
            if (sentence.Length > 1 && Dialoguers.TryGetValue(sentence[0], out Dialoguer dialoguer))
            {
                dialoguer.Anim(command[sentence[0].Length..].Trim());
            }
        }
        else if (StringExtension.TextStartsWith(command, MOVE_COMMAND))
        {
            command = command[MOVE_COMMAND.Length..].Trim();
            string[] sentence = command.Split(' ');
            if (sentence.Length > 1 && Dialoguers.TryGetValue(sentence[0], out Dialoguer dialoguer))
            {
                string[] position = command[sentence[0].Length..].Trim().Split(',');
                dialoguer.Move(new Vector3(float.Parse(position[0]), float.Parse(position[1]), float.Parse(position[2])));
            }
        }
        else if (StringExtension.TextStartsWith(command, ROTATE_COMMAND))
        {
            command = command[ROTATE_COMMAND.Length..].Trim();
            string[] sentence = command.Split(' ');
            if (sentence.Length > 1 && Dialoguers.TryGetValue(sentence[0], out Dialoguer dialoguer))
            {
                string[] rotation = command[sentence[0].Length..].Trim().Split(',');
                dialoguer.Rotate(new Vector3(float.Parse(rotation[0]), float.Parse(rotation[1]), float.Parse(rotation[2])));
            }
        }
        else if (StringExtension.TextStartsWith(command, IMPROVE_RELATION_COMMAND))
        {
            command = command[IMPROVE_RELATION_COMMAND.Length..].Trim();
            string[] sentence = command.Split(' ');
            if (sentence.Length > 1 && Dialoguers.TryGetValue(sentence[0], out Dialoguer dialoguer))
            {
                dialoguer.ImproveRelationship(int.Parse(command[sentence[0].Length..].Trim()));
            }
        }
        else if (StringExtension.TextStartsWith(command, DECREASE_RELATION_COMMAND))
        {
            command = command[DECREASE_RELATION_COMMAND.Length..].Trim();
            string[] sentence = command.Split(' ');
            if (sentence.Length > 1 && Dialoguers.TryGetValue(sentence[0], out Dialoguer dialoguer))
            {
                dialoguer.DecreaseRelationship(int.Parse(command[sentence[0].Length..].Trim()));
            }
        }
        else if (StringExtension.TextStartsWith(command, REGISTER_INTERACTION_COMMAND))
        {
            command = command[REGISTER_INTERACTION_COMMAND.Length..].Trim();
            if (Dialoguers.TryGetValue(command, out Dialoguer dialoguer))
            {
                dialoguer.RegisterInteraction();
            }
        }
        else if (StringExtension.TextStartsWith(command, ENABLE_COMMAND))
        {
            command = command[ENABLE_COMMAND.Length..].Trim();
            if (Dialoguers.TryGetValue(command, out Dialoguer dialoguer))
            {
                dialoguer.SetEnable(true);
            }
        }
        else if (StringExtension.TextStartsWith(command, DISABLE_COMMAND))
        {
            command = command[DISABLE_COMMAND.Length..].Trim();
            if (Dialoguers.TryGetValue(command, out Dialoguer dialoguer))
            {
                dialoguer.SetEnable(false);
            }
        }
        else
        {
            dialogueManager.ErrorMessages = "KEY CHARACTER COMMAND ERROR: invalid KEY COMMAND in \"" + command + "\"";
        }
    }
}
