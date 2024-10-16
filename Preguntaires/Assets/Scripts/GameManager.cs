using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    Pregunta[] _preguntas = null;
    public Pregunta[] Preguntas { get { return _preguntas; } }

    [SerializeField] private GameEvents _events = null;

    private List<Dades_Resposta> _pickedAnswers = new List<Dades_Resposta>();
    private List<int> PreguntesAcabades = new List<int>();
    private int preguntaActual = 0;

    void Start()
    {
        LoadPreguntes();

        foreach (var pregunta in Preguntas)
        {
            Debug.Log(pregunta.Info);
        }

        //Display();
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

    Pregunta GetPreguntaRandom()
    {
        var randomIndex = GetRandomPreguntaIndex();
        preguntaActual += randomIndex;

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
}
