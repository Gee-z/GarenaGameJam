using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
[System.Serializable]
public class HPDrainPhase
{
    public float drainSpeed;   
    public float duration;     
}
public class BattleUI : MonoBehaviour
{
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private Button[] actionButtons; 
    [SerializeField] private Slider enemyHPSlider;
    [SerializeField] private TMP_Text playerDamageText;
    [SerializeField] private TMP_Text enemyDamageText;
    [SerializeField] private List<HPDrainPhase> drainPhases;

    public void ShowMessage(string message)
    {
        messageText.text = message;
    }

    public void SetButtonsInteractable(bool value)
    {
        foreach (Button btn in actionButtons)
            btn.interactable = value;
    }

    public void ShowEnemyDamage(string text, Color color)
    {
        enemyDamageText.text = text;
        enemyDamageText.color = color;
        StopAllCoroutines();
        StartCoroutine(ClearTextAfterDelay(enemyDamageText));
    }

    public void ShowPlayerDamage(string text, Color color)
    {
        playerDamageText.text = text;
        playerDamageText.color = color;
        StopAllCoroutines();
        StartCoroutine(ClearTextAfterDelay(playerDamageText));
    }

    private IEnumerator ClearTextAfterDelay(TMP_Text txt)
    {
        yield return new WaitForSeconds(1.2f);
        txt.text = "";
    }
    public IEnumerator DrainEnemyHP()
    {
        foreach (var phase in drainPhases)
        {
            float timer = 0f;

            while (timer < phase.duration && enemyHPSlider.value > 0)
            {
                enemyHPSlider.value -= Time.deltaTime * phase.drainSpeed;
                timer += Time.deltaTime;
                yield return null;
            }
        }

        enemyHPSlider.value = 0;
    }
}
