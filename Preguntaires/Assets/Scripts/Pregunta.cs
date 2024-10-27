using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Answer
{
    [SerializeField] private string _info;
    public string Info { get { return _info; } }

    [SerializeField] private bool _isCorrect;
    public bool IsCorrect { get { return _isCorrect; } }

}

[CreateAssetMenu(fileName = "Nova Pregunta", menuName = "Quiz/Nova Pregunta")]
public class Pregunta : ScriptableObject
{
    public enum AnswerType {Three, Four}

    [SerializeField] private string _info = string.Empty;
    public string Info { get { return _info; } }

    [SerializeField] Answer[] _answers = null;
    public Answer[] Answers { get { return _answers; } }

    //Parametres

    [SerializeField] private AnswerType _type = AnswerType.Three;
    public AnswerType GetAnswerType { get { return _type; } }

    [SerializeField] private int score = 10;
    public int Score { get { return score; } }

    public List<int> GetCorrectAnswers()
    {
        List<int> CorrectAnswers = new List<int>();
        for(int i = 0; i < Answers.Length; i++)
        {
            if (Answers[i].IsCorrect)
            {
                CorrectAnswers.Add(i);
            }
        }
        return CorrectAnswers;
    }

}
