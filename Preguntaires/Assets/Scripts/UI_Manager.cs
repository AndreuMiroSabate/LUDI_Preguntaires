using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

[Serializable()]
public struct UIManagerParameters
{
    [Header("Opcions Respostes")]
    [SerializeField] float margins;
    public float Margins { get { return margins; } }

    [Header("ResolutionScreenColor")]
    [SerializeField] Color correctcolor;
    public Color CorrectColor { get { return correctcolor; } }
    [SerializeField] Color incorrectColor;
    public Color IncorrectColor { get { return incorrectColor; } }
    [SerializeField] Color finalBGColor;
    public Color FinalBGColor { get { return finalBGColor; } }

}
[Serializable()]
public struct UIElements
{
    [SerializeField] RectTransform contenidorRespostes;
    public RectTransform ContenidorRespostes { get { return contenidorRespostes; } }

    [SerializeField] TextMeshProUGUI preguntesInfoTextObject;
    public TextMeshProUGUI PreguntesInfoTextObject { get { return preguntesInfoTextObject; } }

    [SerializeField] TextMeshProUGUI textPuntuacio;
    public TextMeshProUGUI TextPuntuacio { get { return textPuntuacio; } }

    [Space]

    [SerializeField] Animator resolutionSAnimator;
    public Animator ResolutionSAnimator { get { return resolutionSAnimator; } }

    [SerializeField] Image resulutionBG;
    public Image ResulutionBG { get { return resulutionBG; } }

    [SerializeField] TextMeshProUGUI resolutionStateInfoText;
    public TextMeshProUGUI ResolutionStateInfoText { get { return resolutionStateInfoText; } }

    [SerializeField] TextMeshProUGUI resolutionScoreText;
    public TextMeshProUGUI ResolutionScoreText { get { return resolutionScoreText; } }

    [Space]

    [SerializeField] TextMeshProUGUI textMaximaPuntuacio;
    public TextMeshProUGUI TextMaximaPuntuacio { get { return textMaximaPuntuacio; } }

    [SerializeField] CanvasGroup mainCanvas;
    public CanvasGroup MainCanvas { get { return mainCanvas; } }

    [SerializeField] RectTransform finishUIElements;
    public RectTransform FinishUIElements { get { return finishUIElements; } }
}
public class UI_Manager : MonoBehaviour
{
    // Start is called before the first frame update
    public enum ResolutionScreenType {Correct, Incorrect, Finish}

    [Header("Reference")]
    [SerializeField] GameEvents events;

    [Header("EleMENTS UI (Prefabs)")]
    [SerializeField] Dades_Resposta respostaPrefab;

    [SerializeField] UIElements uIElements;

    [Space]
    [SerializeField] UIManagerParameters mParameters;

    List<Dades_Resposta> respostaActual = new List<Dades_Resposta>();

    private int resolutionState = 0;
    private IEnumerator IE_DisplayTimedRes;

    private void OnEnable()
    {
        events.UpdateQuestionUI += UpdatePreguntaUI;
        events.DisplayResolutionScreen += DisplayResolution;
    }
    private void OnDisable()
    {
        events.UpdateQuestionUI -= UpdatePreguntaUI;
        events.DisplayResolutionScreen -= DisplayResolution;
    }

    void Start()
    {
        resolutionState = Animator.StringToHash("ScreenState");
    }

    void UpdatePreguntaUI(Pregunta pregunta)
    {
        uIElements.PreguntesInfoTextObject.text = pregunta.Info;
        CreaRespostes(pregunta);

    }
    void DisplayResolution(ResolutionScreenType type, int score)
    {
        UpdateResolutionUI(type, score);
        uIElements.ResolutionSAnimator.SetInteger(resolutionState, 2);
        uIElements.MainCanvas.blocksRaycasts = false;

        if (type!= ResolutionScreenType.Finish)
        {
            if(IE_DisplayTimedRes != null)
            {
                StopCoroutine(IE_DisplayTimedRes);
            }
            IE_DisplayTimedRes = DisplayTimeRes();
            StartCoroutine(IE_DisplayTimedRes);
        }
    }

    IEnumerator DisplayTimeRes()
    {
        uIElements.ResolutionSAnimator.SetInteger(resolutionState, 1);
        yield return new WaitForSeconds(GameUtility.ResolutionDelayTime);
        uIElements.ResolutionSAnimator.SetInteger(resolutionState, 0);
        uIElements.MainCanvas.blocksRaycasts = true;
    }

    void UpdateResolutionUI(ResolutionScreenType type, int score)
    {
        var highscore = PlayerPrefs.GetInt(GameUtility.SavePrefKey);

        switch (type)
        {
            case ResolutionScreenType.Correct:
                uIElements.ResulutionBG.color = mParameters.CorrectColor;
                uIElements.ResolutionStateInfoText.text = "CORRECT";
                uIElements.ResolutionScoreText.text = "+" + score;
                break;
            case ResolutionScreenType.Incorrect:
                uIElements.ResulutionBG.color = mParameters.IncorrectColor;
                uIElements.ResolutionStateInfoText.text = "NHA A";
                uIElements.ResolutionScoreText.text = "+" + score;
                break;
            case ResolutionScreenType.Finish:
                uIElements.ResulutionBG.color = mParameters.FinalBGColor;
                uIElements.ResolutionStateInfoText.text = "FINALE SCORE";

                StartCoroutine(CalculateScore());
                uIElements.FinishUIElements.gameObject.SetActive(true);
                uIElements.TextMaximaPuntuacio.gameObject.SetActive(true);
                uIElements.TextMaximaPuntuacio.text = ((highscore > events.SturtupHighscore) ? "<color=yellow </color>" : string.Empty) + "Highscore: " + highscore;
                break;
        }
    }

    IEnumerator CalculateScore()
    {
        var scoreValue = 0;
        while (scoreValue < events.CurrentFinalScore)
        {
            scoreValue++;
            uIElements.ResolutionScoreText.text += scoreValue.ToString();
            yield return null;
        }
    }

    void CreaRespostes(Pregunta pregunta)
    {
        EraseRespostes();

        float offset = 0 - mParameters.Margins;
        for (int i = 0; i < pregunta.Answers.Length; i++)
        {
            Dades_Resposta novaResposta = (Dades_Resposta)Instantiate(respostaPrefab, uIElements.ContenidorRespostes);
            novaResposta.UpdateDades(pregunta.Answers[i].Info,i);

            novaResposta.Rect.anchoredPosition = new Vector2(0, offset);

            offset -= (novaResposta.Rect.sizeDelta.y + mParameters.Margins);
            uIElements.ContenidorRespostes.sizeDelta = new Vector2(uIElements.ContenidorRespostes.sizeDelta.x, offset * -1);

            respostaActual.Add(novaResposta);
        }
    }
    void EraseRespostes()
    {
        foreach (var resposta in respostaActual)
        {
            Destroy(resposta.gameObject);
        }
        respostaActual.Clear();
    }
}
