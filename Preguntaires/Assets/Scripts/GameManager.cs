using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using System.Linq;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    Pregunta[] _preguntas = null;
    public Pregunta[] Preguntas { get { return _preguntas; } }

    [SerializeField] private GameEvents _events = null;

    private List<Dades_Resposta> _pickedAnswers = new List<Dades_Resposta>();
    private List<int> PreguntesAcabades = new List<int>();
    private int preguntaActual = 0;

    private IEnumerator IE_WaitTillNextRound = null;

    private bool ISFinished
    {
        get
        {
            return(PreguntesAcabades.Count < Preguntas.Length) ? false : true;
        }
    }

    private void OnEnable()
    {
        _events.UpdateQuestionAnswer += UpdateRespostes;
    }
    private void OnDisable() 
    {
        _events.UpdateQuestionAnswer -= UpdateRespostes;
    }

    void Start()
    {
        LoadPreguntes();
        var seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        UnityEngine.Random.InitState(seed);

        Display();
    }

    public void UpdateRespostes(Dades_Resposta novaResposta)
    {
        if (Preguntas[preguntaActual].GetAnswerType == Pregunta.AnswerType.Four)
        {
            foreach(var resposta in _pickedAnswers)
            {
                if(resposta != novaResposta)
                {
                    resposta.Reset();
                }
                _pickedAnswers.Clear();
                _pickedAnswers.Add(novaResposta);
            }
        }
    }

    public void EraseAnswers()
    {
        _pickedAnswers = new List<Dades_Resposta>();
    }

    void Display()
    {
        EraseAnswers();
        var pregunta = GetPreguntaRandom();

        if (_events.UpdateQuestionUI != null)
        {
            _events.UpdateQuestionUI(pregunta);
        }
        else
        {
            Debug.LogWarning("Vaja... Alguna cosa no ha anat bé");
        }
    }

    public void Accept()
    {
        bool isCorrect = CheckRespostes();
        PreguntesAcabades.Add(preguntaActual);

        UpdateScore((isCorrect) ? 100 : -25);

        var type = (ISFinished) ? UI_Manager.ResolutionScreenType.Finish : (isCorrect) ? UI_Manager.ResolutionScreenType.Correct : UI_Manager.ResolutionScreenType.Incorrect;

        if(_events.DisplayResolutionScreen != null)
        {
            _events.DisplayResolutionScreen(type, Preguntas[preguntaActual].Score);
        }

        if( IE_WaitTillNextRound != null )
        {
            StopCoroutine(IE_WaitTillNextRound );
        }
        IE_WaitTillNextRound = WaitTillNextRound();
        StartCoroutine(IE_WaitTillNextRound);
    }

    IEnumerator WaitTillNextRound ()
    {
        yield return new WaitForSeconds(GameUtility.ResolutionDelayTime);
        Display();
    }

    Pregunta GetPreguntaRandom()
    {
        var randomIndex = GetRandomPreguntaIndex();
        preguntaActual = randomIndex;

        return Preguntas[preguntaActual];
    }
    int GetRandomPreguntaIndex()
    {
        var random = 0;
        if (PreguntesAcabades.Count < _preguntas.Length)
        {
            do
            {
                random = UnityEngine.Random.Range(0, _preguntas.Length);
            }while (PreguntesAcabades.Contains(random) || random == preguntaActual);
        }
        return random;
    }

    void LoadPreguntes()
    {
        Object[] objs = Resources.LoadAll("Preguntes", typeof(Pregunta));
        _preguntas = new Pregunta[objs.Length];
        for(int i = 0; i < objs.Length; i++)
        {
            _preguntas[i] = (Pregunta)objs[i];
        }
    }

    bool CheckRespostes()
    {
        if(!CompareRespostes())
        {  
            return false; 
        }
        return true;
    }
    bool CompareRespostes()
    {
        if (_pickedAnswers.Count > 0)
        {
            List<int> correctRespostes = Preguntas[preguntaActual].GetCorrectAnswers();
            List<int> pickedRespostes = _pickedAnswers.Select(x => x.answerIndex).ToList();

            var compareCR = correctRespostes.Except(pickedRespostes).ToList();
            var comparePR = pickedRespostes.Except(correctRespostes).ToList();

            return !compareCR.Any() && !comparePR.Any();
        }
        return false;

    }

    private void UpdateScore(int add)
    {
        _events.CurrentFinalScore += add;

        if(_events.ScoreUpdated != null)
        {
            _events.ScoreUpdated();
        }
    }
}
