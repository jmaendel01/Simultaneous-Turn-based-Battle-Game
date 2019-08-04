using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleManagerScript : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject AIPrefab;
    [SerializeField] public int PLAYER_COUNT;

    public static BattleManagerScript instance;
    public readonly float TURN_TIME = 5.0f;

    public static Text instructionText;

    public int getPlayersLeft()
    {
        return players.Count;
    }

    private List<GameObject> players;
    public GameObject getPlayerByNumber(int playerNumber)
    {
        return players[playerNumber];
    }
    public GameObject getPlayerByName(string playerName)
    {
        GameObject player = players.Find(x => x.GetComponent<Player>().getPlayerName() == playerName);
        return player;
    }

    private TimerScript timer;

    private List<IActionCommand> submittedActions;

    public delegate void turnManagement();
    public static event turnManagement resolvingTurn;
    public static event turnManagement newTurn;


    // This function is called to set a player's action for a given turn
    public void setAction(IActionCommand action)
    {
        // If there is already a command from that sender, remove it
        if(submittedActions.Exists(x => x.sender == action.sender))
        {
            submittedActions.Remove(submittedActions.Find(x => x.sender == action.sender));
        }

        submittedActions.Add(action);
    }

    // This function is called at the end of each turn and executes all of the actions in the list submittedActions
    private void resolveTurn()
    {
        // Notify input scripts that the turn is resolving and commands will not be accepted until next turn
        if (resolvingTurn != null)
            resolvingTurn();

        // Clear actions from last turn on the screen and give any players who didn't submit an action this turn a DEFEND action
        foreach (GameObject player in players)
        {
            // TODO: Could make this a Player.getField(string fieldName)
            foreach(Text child in player.GetComponentsInChildren<Text>()) {
                if (child.name == "Action")
                {
                    child.text = "";
                    break;
                }
            }

            if (!submittedActions.Exists(x => x.sender == player))
            {
                IActionCommand genericDefendCommand = new IActionCommand(IActionCommand.ActionType.DEFEND, player.GetComponent<Player>());
                submittedActions.Add(genericDefendCommand);
            }
        }

        // Sort by order of ActionTypes
        submittedActions.Sort((item1, item2) => item1.getActionType().CompareTo(item2.getActionType()));

        // Execute all the moves for the turn
        foreach (IActionCommand action in submittedActions)
        {
            Text actionText = action.sender.GetComponentsInChildren<Text>()[2];

            if (action.getActionType() == IActionCommand.ActionType.DEFEND)
            {
                actionText.text = "Defend";
            }
            else if (action.getActionType() == IActionCommand.ActionType.QUICKATTACK)
            {
                quickAttack(action.sender);
                actionText.text = "Quick Attack";
            }

            else if (action.getActionType() == IActionCommand.ActionType.HEAVYATTACK)
            {
                heavyAttack(action.sender, action.target);
                actionText.text = "Heavy Attack at " + action.target.getPlayerName();
            }
        }

        submittedActions.Clear();

        // Removes dead players
        foreach (GameObject player in players.ToArray())
        {
            if (player.GetComponent<Player>().getHealth() < 1)
            {
                if (player.GetComponent<Player>().isPlayer)
                {
                    newTurn -= player.GetComponent<InputManager>().newTurn;
                    newTurn -= player.GetComponent<InputManager>().resolvingTurn;
                    instructionText.text = "You have died.";
                }
                else
                {
                    newTurn -= player.GetComponent<AITurnManager>().newTurn;
                }
                player.SetActive(false);
                players.Remove(player);
            }
        }

        // Starts next turn
        if (newTurn != null && getPlayersLeft() > 1)
            newTurn();
        else
            instructionText.text = "Game over. ";
    }

    private void quickAttack(Player sender)
    {
        // Create a list of all other non-DEFEND commands
        List<IActionCommand> notDefending = new List<IActionCommand>();
        notDefending = submittedActions.FindAll(x => x.getActionType() != IActionCommand.ActionType.DEFEND && x.sender != sender);

        // quickAttack deals damage to each other player who is not DEFENDing
        foreach(IActionCommand item in notDefending)
        {
            item.sender.dealDamage(1);
        }
    }

    private void heavyAttack(Player sender, Player target)
    {
        // If the target is not QUICKATTACKing this round, heavyAttack succeeds
        if (!submittedActions.Exists(x => x.sender == target && x.getActionType() == IActionCommand.ActionType.QUICKATTACK))
        {
            target.GetComponent<Player>().dealDamage(2);
        }
    }

    // Setup function
    private void Awake()
    {
        instance = this;
        instructionText = GameObject.Find("InstructionText").GetComponent<Text>();

        timer = GameObject.Find("Timer").GetComponent<TimerScript>(); // or could create one here; timer is unnecessary is a single-player game
        resolvingTurn += timer.stopTimer;
        newTurn += timer.restartTimer;

        players = new List<GameObject>();

        spawnPlayer(0, true);

        // This could loop through a list of connected devices/sockets to add multiplayer capabilities
        for (int i = 1; i < PLAYER_COUNT; i++)
        {
            spawnPlayer(i, false);
        }

        submittedActions = new List<IActionCommand>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //playersLeft = PLAYER_COUNT;
        timer.Setup(TURN_TIME);
        timer.restartTimer();
        instructionText.text = "Press \"Q\" for Quick Attack, \"D\" for Defend, or any number to Heavy Attack the corresponding player \n Quick Attack > Heavy Attack > Defend > Quick Attack";
    }

    // Creates the GameObjects from prefabs to represent players based on whether the player is a player or AI
    private void spawnPlayer(int playerNumber, bool isPlayer)
    {
        Vector3 position = new Vector3(-8 + ((18/PLAYER_COUNT)*playerNumber + (18/(PLAYER_COUNT*2))), 0);
        GameObject player;

        if (isPlayer)
        {
            player = Instantiate(playerPrefab, position, Quaternion.identity, GameObject.Find("MainPanel").transform);
            player.GetComponent<Player>().Setup("Player " + playerNumber.ToString(), isPlayer);

            newTurn += player.GetComponent<InputManager>().newTurn;
            resolvingTurn += player.GetComponent<InputManager>().resolvingTurn;
        }
        else
        {
            player = Instantiate(AIPrefab, position, Quaternion.identity, GameObject.Find("MainPanel").transform);
            player.GetComponent<Player>().Setup("Player " + playerNumber.ToString() + " (AI)", isPlayer);

            newTurn += player.GetComponent<AITurnManager>().newTurn;
        }

        players.Add(player);
    }

    // LateUpdate is called after Update each frame
    private void LateUpdate()
    {
        // When the turn time has run out, resolve the turn
        if(timer.getTimeLeft() == 0.0f)
        {
            resolveTurn();
        }
    }
}