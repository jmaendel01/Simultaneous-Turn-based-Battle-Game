using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Instances of this class are created to represent each individual action of each player each turn
public class IActionCommand
{

    public enum ActionType
    {
        // DEFEND: Blocks QUICKATTACK
        DEFEND,

        // QUICKATTACK: Deal one damamge to each other player who isn't DEFENDing
        QUICKATTACK,

        // HEAVYATTACK: Deal two damage to a target if they are not QUICKATTACKing
        HEAVYATTACK
    }

    private ActionType actionType;
    public ActionType getActionType()
    {
        return actionType;
    }

    public string getActionTypeString()
    {
        return "enum";
    }

    public readonly Player target;
    public readonly Player sender;

    public IActionCommand(ActionType type, Player sender)
    {
        actionType = type;
        this.sender = sender;
    }

    public IActionCommand(ActionType type, Player sender, Player target)
    {
        actionType = type;
        this.sender = sender;
        this.target = target;
    }

}
