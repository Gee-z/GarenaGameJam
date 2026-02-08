using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI text;
    public string normalText = "START";
    public string hoverText = "QUIT";

    public void OnPointerEnter(PointerEventData eventData)
    {
        text.text = hoverText;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        text.text = normalText;
    }
    public void StartGame()
    {
        AudioManager.Instance.PlaySFX(0);
        // SceneManager.LoadScene("GameScene"); 
    }

    public void QuitGame()
    {
        AudioManager.Instance.PlaySFX(0);
        Application.Quit();
    }
}
