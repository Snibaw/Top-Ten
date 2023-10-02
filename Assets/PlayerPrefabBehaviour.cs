using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerPrefabBehaviour : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text numberText;
    private GameManager gameManager;
    private bool isRevealed = false;
    private void Start() 
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        numberText.gameObject.SetActive(false);
    }

    public void SetName(string name)
    {
        nameText.text = name;
    }
    public void ClickOnPlayer()
    {
        if(gameManager.isItChosePhase)
        {
            if(isRevealed) return;
            isRevealed = true;
            numberText.text = gameManager.GetNumber(nameText.text).ToString();
            numberText.gameObject.SetActive(true);
        }
        else
        {
            gameManager.removeName(nameText.text);
            Destroy(gameObject);
        }
    }
}
