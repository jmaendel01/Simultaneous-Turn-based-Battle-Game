using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    private InputState inputState;
    public InputState GetInputState()
    {
        return inputState;
    }

    private Player playerScript;
    private IActionCommand currentMove;

    public void resolvingTurn()
    {
        inputState = InputState.BUSY;
    }

    public void newTurn()
    {
        inputState = InputState.WAITINGFORINPUT;
        BattleManagerScript.instructionText.text = "New Turn. Press \"Q\" for Quick Attack, \"D\" for Defend, or any number to Heavy Attack the corresponding player \n Quick Attack > Heavy Attack > Defend > Quick Attack";
    }

    // Start is called before the first frame update
    void Start()
    {
        playerScript = GetComponent<Player>();
        inputState = InputState.WAITINGFORINPUT;
    }

    void Update()
    {
        // Checking for command instructions
        if (inputState == InputState.WAITINGFORINPUT)
        {
            // Quick Attack
            if (Input.GetKeyDown("q"))
            {
                currentMove = new IActionCommand(IActionCommand.ActionType.QUICKATTACK, playerScript);

                BattleManagerScript.instructionText.text = "The Quick Attack command has been selected. This will deal one damage to each other player who isn't defending. \nSubmit with space or enter or cancel with escape or backspace";

                inputState = InputState.WAITINGFORCONFIRMATION;
            }

            // Defend
            else if (Input.GetKeyDown("d"))
            {
                currentMove = new IActionCommand(IActionCommand.ActionType.DEFEND, playerScript);

                BattleManagerScript.instructionText.text = "The Defend command has been selected. This will protect you from from Quick Attacks. \nSubmit with space or enter or cancel with escape or backspace";

                inputState = InputState.WAITINGFORCONFIRMATION;
            }

            // Heavy Attack
            else
            {
                // Checks each number to
                for (int i = 0; i < BattleManagerScript.instance.PLAYER_COUNT; i++)
                {
                    if (Input.GetKeyDown(i.ToString()))
                    {
                        Player targetPlayer = BattleManagerScript.instance.getPlayerByName("Player " + i).GetComponent<Player>();

                        if (targetPlayer != null && !targetPlayer.Equals(playerScript))
                        {
                            currentMove = new IActionCommand(IActionCommand.ActionType.HEAVYATTACK, playerScript, targetPlayer);

                            BattleManagerScript.instructionText.text = "The Heavy Attack command has been selected with a target of " + currentMove.target.getPlayerName() + ". \nSubmit with space or enter or cancel with escape or backspace";

                            inputState = InputState.WAITINGFORCONFIRMATION;
                        }
                        
                    }
                }
            }

            //BattleManagerScript.instructionText.text = "No valid key was pressed. Press \"Q\" for Quick Attack, \"D\" for Defend, or any number to Heavy Attack the corresponding player \n Quick Attack > Heavy Attack > Defend > Quick Attack";

        }

        // Checking for confirmation or cancellation of chosen command
        if (inputState == InputState.WAITINGFORCONFIRMATION)
        {
            if (Input.GetKeyDown("space") || Input.GetKeyDown("return") || Input.GetKeyDown("enter"))
            {
                BattleManagerScript.instance.setAction(currentMove);

                BattleManagerScript.instructionText.text = "The command was submitted. To change your command, press \"Q\" for Quick Attack, \"D\" for Defend, or any number to Heavy Attack the corresponding player \n Quick Attack > Heavy Attack > Defend > Quick Attack";

                inputState = InputState.WAITINGFORINPUT;
            }

            else if (Input.GetKeyDown("escape") || Input.GetKeyDown("backspace"))
            {
                currentMove = null;

                BattleManagerScript.instructionText.text = "The command was cancelled. Press \"Q\" for Quick Attack, \"D\" for Defend, or any number to Heavy Attack the corresponding player \n Quick Attack > Heavy Attack > Defend > Quick Attack";

                inputState = InputState.WAITINGFORINPUT;
            }
        }
    }

}
