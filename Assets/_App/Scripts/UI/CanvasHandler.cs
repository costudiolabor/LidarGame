using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasHandler : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private AudioSource audioSourceClick;
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject losePanel;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private Button buttonStart;
    [SerializeField] private Button buttonExit;
    [SerializeField] private Button buttonAgain;
    [SerializeField] private Button buttonNext;
    [SerializeField] private TMP_Text textCountEnemy;


    public event Action StartEvent, ExitEvent, AgainEvent, NextEvent;
    private OSCHandler _oscHandler;

    public void Initialize(OSCHandler oscHandler)
    {
        _oscHandler = oscHandler;
        _oscHandler.StartEvent += OnStart;
        _oscHandler.StopEvent += OnExit;
        buttonStart.onClick.AddListener(OnStart);
        buttonExit.onClick.AddListener(OnExit);
        buttonAgain.onClick.AddListener(OnAgain);
        buttonNext.onClick.AddListener(OnNext);
        gamePanel.SetActive(false);
    }

    private void OnStart()
    {
        StartEvent?.Invoke();
        audioSourceClick.Play();
        startPanel.SetActive(false);
        gamePanel.SetActive(true);
        Debug.Log("start");
    }
    
    private void OnExit()
    {
        ExitEvent?.Invoke();
        audioSourceClick.Play();
        startPanel.SetActive(true);
        Debug.Log("Exit");
    }

    private void OnAgain()
    {
        audioSourceClick.Play();
        losePanel.SetActive(false);
        AgainEvent?.Invoke();
        Debug.Log("again");
    }
    
    private void OnNext()
    {
        audioSourceClick.Play();
        winPanel.SetActive(false);
        NextEvent?.Invoke();
        Debug.Log("next");
    }
    
    public void ShowLose()
    {
        losePanel.SetActive(true);
    }
    
    public void ShowWin()
    {
        winPanel.SetActive(true);
    }


    public void UpdateEnemy(int count)
    {
        textCountEnemy.text = count.ToString();
    }
    
    private void OnDestroy()
    {
        _oscHandler.StartEvent -= OnStart;
        _oscHandler.StopEvent -= OnExit;
        
        buttonStart.onClick.RemoveAllListeners();
        buttonExit.onClick.RemoveAllListeners();
        buttonAgain.onClick.RemoveAllListeners();
        buttonNext.onClick.RemoveAllListeners();
        
        StartEvent = null;
        ExitEvent = null;
        AgainEvent = null;
        NextEvent = null;
    }
  
}
