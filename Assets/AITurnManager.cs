using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AITurnManager : MonoBehaviour
{

    private InputState inputState;
    private Player playerScript;
    private IActionCommand currentMove;

    public void newTurn()
    {
        inputState = InputState.WAITINGFORINPUT;
    }

    // Start is called before the first frame update
    void Start()
    {
        playerScript = GetComponent<Player>();
        inputState = InputState.WAITINGFORINPUT;
    }

    // Update is called once per frame
    void Update()
    {
        // This determines the AI player's move each turn
        if (inputState == InputState.WAITINGFORINPUT)
        {
            int randomAction = Random.Range(0, 3);

            if(randomAction == 0)
            {
                currentMove = new IActionCommand(IActionCommand.ActionType.QUICKATTACK, playerScript);
                inputState = InputState.BUSY;
                BattleManagerScript.instance.setAction(currentMove);
            }

            if(randomAction == 1)
            {
                currentMove = new IActionCommand(IActionCommand.ActionType.DEFEND, playerScript);
                inputState = InputState.BUSY;
                BattleManagerScript.instance.setAction(currentMove);    
            }

            if (randomAction == 2)
            {
                Player randomTarget;

                do
                {
                    randomTarget = BattleManagerScript.instance.getPlayerByNumber(Random.Range(0, BattleManagerScript.instance.getPlayersLeft())).GetComponent<Player>();
                } while (randomTarget == gameObject.GetComponent<Player>());

                currentMove = new IActionCommand(IActionCommand.ActionType.HEAVYATTACK, playerScript, randomTarget);
                inputState = InputState.BUSY;
                BattleManagerScript.instance.setAction(currentMove);
            }
        }

        //Debug.Log(playerScript.getPlayerName() + " submitted " + (int)currentMove.getActionType());
    }
}
