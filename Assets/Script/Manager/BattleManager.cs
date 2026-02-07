using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState
{
    PlayerTurn,
    EnemyTurn,
    Busy,
    Win
}
public class BattleManager : MonoBehaviour
{
    public BattleState battleState;
    [SerializeField] BattleUI battleUI;
    [SerializeField] private int ineffectiveHitsBeforeCrit = 3;
    [SerializeField] private List<string> uselessActionMessages = new List<string>();
    private int playerAttackCount;

    private void Start()
    {
        playerAttackCount = 0;
        StartPlayerTurn();
    }

    private void StartPlayerTurn()
    {
        battleState = BattleState.PlayerTurn;
        battleUI.ShowMessage("Choose an action.");
        battleUI.SetButtonsInteractable(true);
    }

    public void OnActionButtonPressed(int actionIndex)
    {
        Debug.Log("Button pressed: " + actionIndex);
        if (battleState != BattleState.PlayerTurn)
            return;

        if (actionIndex == 0)
        {
            StartCoroutine(PlayerAttack());
        }
        else
        {
            StartCoroutine(UselessAction());
        }
    }

    private IEnumerator PlayerAttack()
    {
        battleState = BattleState.Busy;
        battleUI.SetButtonsInteractable(false);

        playerAttackCount++;

        battleUI.ShowMessage("You attacked!");
        yield return new WaitForSeconds(0.8f);

        if (playerAttackCount <= ineffectiveHitsBeforeCrit)
        {
            battleUI.ShowEnemyDamage("0", Color.gray);
            battleUI.ShowMessage("It's ineffective...");
            yield return new WaitForSeconds(1.0f);

            battleState = BattleState.EnemyTurn;
            StartCoroutine(EnemyAttack());
            yield break;
        }
        battleUI.ShowMessage("CRITICAL HIT!");
        yield return new WaitForSeconds(0.6f);

        battleUI.ShowEnemyDamage("100000", Color.yellow);
        yield return new WaitForSeconds(1.2f);
        yield return StartCoroutine(battleUI.DrainEnemyHP());
        WinBattle();
    }

    private IEnumerator EnemyAttack()
    {
        battleUI.ShowMessage("Enemy attacks!");
        yield return new WaitForSeconds(0.6f);

        battleUI.ShowPlayerDamage("9999", Color.red);
        battleUI.ShowMessage("Enemy dealt 9999 damage!");
        yield return new WaitForSeconds(1.0f);

        StartPlayerTurn();
    }
    private IEnumerator UselessAction()
    {
        battleState = BattleState.Busy;

        battleUI.ShowMessage(GetRandomUselessMessage());
        yield return new WaitForSeconds(1.2f);

        battleState = BattleState.EnemyTurn;
        StartCoroutine(EnemyAttack());
    }

    private void WinBattle()
    {
        battleState = BattleState.Win;
        battleUI.SetButtonsInteractable(false);
        //change scene
        battleUI.ShowMessage("Enemy fainted! You win!");
    }
    private string GetRandomUselessMessage()
    {
        if (uselessActionMessages == null || uselessActionMessages.Count == 0)
            return "Nothing happened...";

        return uselessActionMessages[Random.Range(0, uselessActionMessages.Count)];
    }
}
