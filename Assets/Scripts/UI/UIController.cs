using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    [Header("References")]
    private GameManager gameManager;

    [Header("Pre Game")]
    [SerializeField] private CanvasGroup preGameScreen;
    [SerializeField] private Button playButton;

    [Header("Epilepsy")]
    [SerializeField] private bool epilepsyEnabled;
    [SerializeField] private Toggle epilepsyToggle;
    [SerializeField] private Color[] preGameBackgroundColors;
    [SerializeField] private float preGameColorFadeDuration;
    private Image preGameScreenImage;
    private Coroutine epilepsyCoroutine;

    [Header("Information UI")]
    [SerializeField] private Transform informationUIParent;
    [SerializeField] private PasswordElementText informationTextPrefab;
    private List<PasswordElementText> passwordElementTexts;

    [Header("Attempts")]
    [SerializeField] private TMP_Text attemptsText;

    [Header("Password")]
    [SerializeField] private Transform passwordScreen;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private Button submitButton;
    private Image passwordInputImage;
    private Color passwordInputStartColor;

    [Header("Correct")]
    [SerializeField] private Color correctColor;
    [SerializeField] private float correctFadeDuration;

    [Header("Partial Correct")]
    [SerializeField] private Color partialCorrectColor;
    [SerializeField] private float partialCorrectFadeDuration;

    [Header("Incorrect")]
    [SerializeField] private Color incorrectColor;
    [SerializeField] private float incorrectFadeDuration;

    [Header("Guesses")]
    [SerializeField] private Transform guessesParent;
    [SerializeField] private TMP_Text guessPrefab;

    [Header("Game Complete")]
    [SerializeField] private CanvasGroup gameCompleteScreen;
    [SerializeField] private float gameCompleteFadeDuration;
    [SerializeField] private TMP_Text gameCompletePasswordText;
    [SerializeField] private Button gameCompleteReplayButton;

    [Header("Game Over")]
    [SerializeField] private CanvasGroup gameOverScreen;
    [SerializeField] private float gameOverFadeDuration;
    [SerializeField] private TMP_Text gameOverPasswordText;
    [SerializeField] private Button gameOverReplayButton;

    [Header("Loading Screen")]
    [SerializeField] private CanvasGroup loadingScreen;
    [SerializeField] private float loadingFadeDuration;

    private void Awake() {

        passwordElementTexts = new List<PasswordElementText>(); // create list in awake so it doesn't reset after game manager changes it

        epilepsyToggle.isOn = epilepsyEnabled;
        epilepsyToggle.onValueChanged.AddListener((value) => OnEpilepsyUpdate(value));

    }

    private void Start() {

        gameManager = FindObjectOfType<GameManager>();
        passwordInputImage = passwordInput.GetComponent<Image>();
        preGameScreenImage = preGameScreen.GetComponent<Image>();
        passwordInputStartColor = passwordInputImage.color;

        playButton.onClick.AddListener(PlayGame);

        RefreshLayout(passwordScreen); // refresh layout

        passwordInput.onEndEdit.AddListener(OnEndEdit);
        submitButton.onClick.AddListener(OnSubmit);

        gameCompleteScreen.alpha = 0f; // hide game complete screen for fade later
        gameCompleteScreen.gameObject.SetActive(false); // disable game complete screen

        gameCompleteReplayButton.onClick.AddListener(OnPlayAgain);

        gameOverScreen.alpha = 0f; // hide game over screen for fade later
        gameOverScreen.gameObject.SetActive(false); // disable game over screen

        gameOverReplayButton.onClick.AddListener(OnPlayAgain);

        loadingScreen.alpha = 1f; // hide loading screen for fade later
        loadingScreen.gameObject.SetActive(true); // enable loading screen
        loadingScreen.DOFade(0f, loadingFadeDuration).OnComplete(() => loadingScreen.gameObject.SetActive(false)); // fade out loading screen and disable it

        if (epilepsyEnabled)
            epilepsyCoroutine = StartCoroutine(FadePreGameColors()); // start fading pre game colors

    }

    private void Update() {

        // make sure password input is always focused
        if (passwordInput.isFocused == false) {

            EventSystem.current.SetSelectedGameObject(passwordInput.gameObject, null);
            passwordInput.OnPointerClick(new PointerEventData(EventSystem.current));

        }
    }

    private void OnEpilepsyUpdate(bool value) {

        epilepsyEnabled = value;

        if (epilepsyEnabled) {

            epilepsyCoroutine = StartCoroutine(FadePreGameColors()); // start fading pre game colors

        } else if (epilepsyCoroutine != null) {

            StopCoroutine(epilepsyCoroutine); // stop fading pre game colors
            preGameScreenImage.DOColor(preGameBackgroundColors[0], preGameColorFadeDuration).SetEase(Ease.InOutSine); // reset color

        }
    }

    private IEnumerator FadePreGameColors() {

        Tweener tween = null;

        while (true) {

            for (int i = 0; i < preGameBackgroundColors.Length; i++) {

                if (tween != null) tween.Kill(); // kill previous tween

                tween = preGameScreenImage.DOColor(preGameBackgroundColors[i], preGameColorFadeDuration).SetEase(Ease.InOutSine);

                while (tween.IsActive()) yield return null; // wait for color to change

            }
        }
    }

    private void PlayGame() {

        preGameScreen.DOFade(0f, preGameColorFadeDuration).OnComplete(() => preGameScreen.gameObject.SetActive(false)).SetEase(Ease.InOutSine); // fade out pre game screen and disable it

    }

    public PasswordElementText AddInformation(string category, string information) {

        PasswordElementText text = Instantiate(informationTextPrefab, informationUIParent.transform);
        text.Initialize("Favorite " + category + ": " + information);
        passwordElementTexts.Add(text);
        RefreshLayout(informationUIParent);
        return text;

    }

    public void ResetAllInformationColors() {

        foreach (PasswordElementText text in passwordElementTexts)
            text.ResetColor();

    }

    public void UpdateAttemptsText(int attemptsRemaining) {

        attemptsText.text = "Attempts Remaining: " + attemptsRemaining;

    }

    private void OnEndEdit(string input) {

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) OnSubmit();

    }

    private void OnSubmit() {

        if (passwordInput.text.Length == 0) return; // no input (empty string)

        AddGuess(passwordInput.text); // add guess to UI
        gameManager.ValidateSubmission(passwordInput.text);

        passwordInput.text = ""; // clear input field

    }

    public void ShowGameOverScreen() {

        gameOverPasswordText.text = "Password: " + gameManager.GetPassword(); // set password text

        gameOverScreen.gameObject.SetActive(true); // enable game over screen
        RefreshLayout(gameOverScreen.transform); // refresh layout
        gameOverScreen.DOFade(1f, gameOverFadeDuration); // fade in game over screen

    }

    public void ShowGameCompleteScreen() {

        gameCompletePasswordText.text = "Password: " + gameManager.GetPassword(); // set password text

        gameCompleteScreen.gameObject.SetActive(true); // enable game complete screen
        RefreshLayout(gameCompleteScreen.transform); // refresh layout
        gameCompleteScreen.DOFade(1f, gameCompleteFadeDuration); // fade in game complete screen

    }

    public void FlashCorrectColor() {

        passwordInputImage.DOColor(correctColor, correctFadeDuration / 2f).OnComplete(() => passwordInputImage.DOColor(passwordInputStartColor, correctFadeDuration / 2f));
        passwordInput.interactable = false;

    }

    public void FlashPartialCorrectColor() {

        passwordInputImage.DOColor(partialCorrectColor, partialCorrectFadeDuration / 2f).OnComplete(() => passwordInputImage.DOColor(passwordInputStartColor, partialCorrectFadeDuration / 2f));

    }

    public void FlashIncorrectColor() {

        passwordInputImage.DOColor(incorrectColor, incorrectFadeDuration / 2f).OnComplete(() => passwordInputImage.DOColor(passwordInputStartColor, incorrectFadeDuration / 2f));

    }

    private void AddGuess(string guess) {

        Instantiate(guessPrefab, guessesParent).text = guess;
        RefreshLayout(guessesParent); // refresh layout

    }

    private void OnPlayAgain() {

        loadingScreen.gameObject.SetActive(true); // enable loading screen
        gameManager.StartReloadScene();
        loadingScreen.DOFade(1f, loadingFadeDuration).OnComplete(() => gameManager.FinishReloadScene()); // fade in loading screen

    }

    private void RefreshLayout(Transform layout) {

        LayoutRebuilder.ForceRebuildLayoutImmediate(layout.GetComponent<RectTransform>());

    }
}
