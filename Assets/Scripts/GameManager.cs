using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("BeforeStart")]
    private string currentName = "Unknown";
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject layoutGroup;
    [SerializeField] private TMP_Text numberOfPlayersText;
    private int numberOfPlayers = 0;
    [SerializeField] private GameObject InputNamePanel;
    [SerializeField] private TMP_InputField nameInputField;
    private bool isInputNamePanelActive = false;
    private List<string> names = new List<string>();
    private List<int> numbers = new List<int>(){1,2,3,4,5,6,7,8,9,10};
    private List<int> numberChosen = new List<int>();
    [SerializeField] private GameObject[] hideWhenStart;

    [Header("AfterStart")]
    [SerializeField] private GameObject[] showWhenStart;
    [SerializeField] private GameObject showNumberPanel;
    [SerializeField] private TMP_Text nameTxt;
    [SerializeField] private GameObject numberTxt;
    [SerializeField] private GameObject revealButton;
    [SerializeField] private GameObject nextButton;
    [SerializeField] private TMP_Text choosePhaseText;
    [SerializeField] private GameObject[] endGameButton;  
    [SerializeField] private Sprite gameMasterSprite;
    int indexInList = 0;
    public bool isItChosePhase = false;
    private int playerEliminated = 0;

    // Start is called before the first frame update

    private void Start() 
    {
        InputNamePanel.SetActive(false);
        showNumberPanel.SetActive(false);
        choosePhaseText.gameObject.SetActive(false);
        numberOfPlayersText.text = numberOfPlayers.ToString() + " Player(s)";

        foreach(GameObject obj in showWhenStart)
        {
            obj.SetActive(false);
        }
        foreach(GameObject obj in endGameButton)
        {
            obj.SetActive(false);
        }

        //Load the names of the players from PlayerPrefs
        if(PlayerPrefs.HasKey("NumberOfPlayers"))
        {
            numberOfPlayers = PlayerPrefs.GetInt("NumberOfPlayers");
            numberOfPlayersText.text = numberOfPlayers.ToString() + " Player(s)";
            for(int i = 1; i <= numberOfPlayers; i++)
            {
                string name = PlayerPrefs.GetString("PlayerName"+i.ToString());
                GameObject player = Instantiate(playerPrefab, layoutGroup.transform);
                player.GetComponent<PlayerPrefabBehaviour>().SetName(name);
                names.Add(name);
            }
        }
    }
    
    public void ShowInputNamePanel()
    {
        if(isInputNamePanelActive) return;

        isInputNamePanelActive = true;
        InputNamePanel.SetActive(true);
        currentName = "Unknown";
        nameInputField.text = currentName;
    }
    public void HideInputNamePanel()
    {
        if(!isInputNamePanelActive) return;

        isInputNamePanelActive = false;
        InputNamePanel.SetActive(false);
    }
    public void OnEndEditName()
    {
        currentName = nameInputField.text;
    }
    public void removeName(string name)
    {
        names.Remove(name);
        numberOfPlayers--;
        numberOfPlayersText.text = numberOfPlayers.ToString() + " Player(s)";

        //Find the number corresponding in the PlayerPrefs corresponding to this name
        for(int i = 1; i <= PlayerPrefs.GetInt("NumberOfPlayers"); i++)
        {
            if(PlayerPrefs.GetString("PlayerName"+i.ToString()) == name)
            {
                //foreach remaining player, decrease the number by 1
                for(int j = i+1; j <= PlayerPrefs.GetInt("NumberOfPlayers"); j++)
                {
                    PlayerPrefs.SetString("PlayerName"+(j-1).ToString(), PlayerPrefs.GetString("PlayerName"+j.ToString()));
                }
                PlayerPrefs.DeleteKey("PlayerName"+PlayerPrefs.GetInt("NumberOfPlayers").ToString());
                PlayerPrefs.SetInt("NumberOfPlayers", PlayerPrefs.GetInt("NumberOfPlayers")-1);
                PlayerPrefs.Save();
                break;
            }
        }
    }
    public void AddName()
    {
        if(numberOfPlayers >= 10) return;

        HideInputNamePanel();
        numberOfPlayers++;
        numberOfPlayersText.text = numberOfPlayers.ToString() + " Player(s)";
        GameObject player = Instantiate(playerPrefab, layoutGroup.transform);
        player.GetComponent<PlayerPrefabBehaviour>().SetName(currentName);
        names.Add(currentName);

        //Save the name of this player in PlayerPrefs
        PlayerPrefs.SetString("PlayerName"+numberOfPlayers.ToString(), currentName);
        PlayerPrefs.SetInt("NumberOfPlayers", numberOfPlayers);
        PlayerPrefs.Save();


    }
    public void StartGame()
    {
        if(numberOfPlayers < 4 || numberOfPlayers > 10) return;

        foreach(GameObject obj in hideWhenStart)
        {
            obj.SetActive(false);
        }  
        foreach(GameObject obj in showWhenStart)
        {
            obj.SetActive(true);
        }
        ShowNumberPannel();
    }
    public void ShowNumberPannel()
    {
        string nameTemp = names[indexInList];
        if(nameTemp == "Fedi" || nameTemp == "fedi")
            nameTemp += ", le mec nul au baby :)";
        nameTxt.text = nameTemp;
        numberTxt.SetActive(false);
        showNumberPanel.SetActive(true);
        revealButton.SetActive(true);
        nextButton.SetActive(false);
    }
    public void RevealNumber()
    {
        int numberIndex = Random.Range(0, numbers.Count);
        //Remove number from list
        numberChosen.Add(numbers[numberIndex]);
        numbers.RemoveAt(numberIndex);
        //Show number
        numberTxt.SetActive(true);
        numberTxt.GetComponent<TMP_Text>().text = numberChosen[indexInList].ToString();   

        revealButton.SetActive(false);
        nextButton.SetActive(true);   

    }
    public void NextNumber()
    {
        indexInList++;
        if(indexInList >= numberOfPlayers)
        {
            StartChoosePhase();
            return;
        }
        ShowNumberPannel();
    }
    private void StartChoosePhase()
    {
        HideNumberPanel();
        isItChosePhase = true;
        choosePhaseText.gameObject.SetActive(true);
        FindAGameMaster();
    }
    private void FindAGameMaster()
    {
        GameObject[] playerPortraits = GameObject.FindGameObjectsWithTag("PlayerPortrait");
        int gameMasterIndex = Random.Range(0, playerPortraits.Length);
        playerPortraits[gameMasterIndex].GetComponent<Image>().sprite = gameMasterSprite;
    }
    public void HideNumberPanel()
    {
        showNumberPanel.SetActive(false);
    }
    public int GetNumber(string name)
    {
        int index = names.IndexOf(name);
        playerEliminated++;
        if(playerEliminated >= numberOfPlayers)
        {
            choosePhaseText.gameObject.SetActive(false);
            foreach(GameObject obj in endGameButton)
            {
                obj.SetActive(true);
            }
            Debug.Log("EndOfGame");
        }
        return numberChosen[index];
    }
}
