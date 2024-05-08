using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Quiz : MonoBehaviour
{
    [Header("Sorular")]
    [SerializeField] TextMeshProUGUI questionText;
    [SerializeField] List<QuestionSO> question=new List<QuestionSO>();

    QuestionSO currentQuestion;

    [Header("Cevaplar")]
    [SerializeField] GameObject[] answerButtons;
    int correctAnswerIndex;
    bool hasAnsweredEarly=true;

    [Header("Button Colors")]
    [SerializeField] Sprite defaultAnswerSprite;
    [SerializeField] Sprite correctAnswerSprite;

    [Header("Timer")]
    [SerializeField] Image timerImage;
    Timer timer;

    [Header("Scoring")]
    [SerializeField] TextMeshProUGUI scoreText;
    ScoreKeeper scoreKeeper;

    [Header("ProgressBar")]
    [SerializeField] Slider progressBar;

    public bool isComplete;
    void Awake()
    {
        timer=FindAnyObjectByType<Timer>();
        scoreKeeper=FindAnyObjectByType<ScoreKeeper>();
        progressBar.maxValue=question.Count;
        progressBar.value=0;
    }

    void Update()
    {
        timerImage.fillAmount=timer.fillFraction;        
        if(timer.loadNextQuestion)
        {
            if(progressBar.value==progressBar.maxValue)
           {
            isComplete=true;
            return;
           }

            hasAnsweredEarly=false;
            GetNextQuestion();
            timer.loadNextQuestion=false;
        }
        else if(!hasAnsweredEarly && !timer.isAnsweringQuestion)
        {
            DisplayAnswer(-1);
            SetButtonState(false);

        }
    }

    public void OnAnswerSelected(int index)
    {
        hasAnsweredEarly=true;
        DisplayAnswer(index);      
        SetButtonState(false);
        timer.CancelTimer();
        scoreText.text="Skor: "+scoreKeeper.CalculateScore()+"%";

    }
    void DisplayAnswer(int index)
    {
        Image buttonImage;

        if(index==currentQuestion.GetCorrectAnswerIndex())
        {
            questionText.text="Doğru bildiniz Tebrikler!";
            buttonImage=answerButtons[index].GetComponent<Image>();
            buttonImage.sprite=correctAnswerSprite;
            scoreKeeper.IncrementCorrectAnswer();
        }
        else
        {
            correctAnswerIndex=currentQuestion.GetCorrectAnswerIndex();
            string correctAnswer =currentQuestion.GetAnswer(correctAnswerIndex);
            questionText.text="Maalesef yanliş cevap, olmasi gereken;\n "+ correctAnswer;
            buttonImage=answerButtons[correctAnswerIndex].GetComponent<Image>();
            buttonImage.sprite=correctAnswerSprite;
        }
    }
    void GetNextQuestion()
    {
        if(question.Count>0)
        {
        SetButtonState(true);
        SetDefaultButtonSprites();
        GetRandomQuestion();
        DisplayQuestion();
        progressBar.value++;
        scoreKeeper.IncrementQuestionsSeen();
        }
       
    }
    void GetRandomQuestion()
    {
        int index=Random.Range(0,question.Count);
        currentQuestion=question[index];

        if(question.Contains(currentQuestion))
        {
            question.Remove(currentQuestion);
        }
        
    }

    void DisplayQuestion()
    {
        questionText.text = currentQuestion.GetQuestion();

        for(int i =0; i<answerButtons.Length;i++)
        {
        TextMeshProUGUI buttonText= answerButtons[i].GetComponentInChildren<TextMeshProUGUI>();
        buttonText.text=currentQuestion.GetAnswer(i);

        }
    }
    void SetButtonState(bool state)
    {
        for(int i=0;i<answerButtons.Length;i++)
        {
            Button button=answerButtons[i].GetComponent<Button>();
            button.interactable=state;

        }
    }

    void SetDefaultButtonSprites()
    {
        for(int i=0;i<answerButtons.Length;i++)
        {
            Image buttonImage =answerButtons[i].GetComponent<Image>();
            buttonImage.sprite=defaultAnswerSprite;
        }

    }
    


}
