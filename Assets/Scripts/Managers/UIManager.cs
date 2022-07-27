using System;
using States;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private CanvasGroup mainMenu;
        [SerializeField] private CanvasGroup pauseMenu;
        [SerializeField] private CanvasGroup finishScreen;

        [SerializeField] private Button mainMenuPrimaryButton;
        [SerializeField] private Button pauseMenuPrimaryButton;
        [SerializeField] private Button finishPrimaryButton;

        [SerializeField] private TextMeshProUGUI prepareText;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI livesText;
        [SerializeField] private TextMeshProUGUI finalScoreText;

        public static event Action OnPlay;
        public static event Action OnGameExit;
        public static event Action OnResume;
    
    
        private void OnEnable()
        {
            PlayerController.OnLivesChanged += UpdateLives;
            PlayerController.OnScoreChanged += UpdateScore;

            IntroState.OnIntro += HandleIntro;
            PrepareInvasionState.OnPrepare += SetPrepareTextActive;
            PauseState.OnPause += SetPauseMenuActive;
            FinishState.OnFinish += HandleFinish;
        }

        private void OnDisable()
        {
            PlayerController.OnLivesChanged -= UpdateLives;
            PlayerController.OnScoreChanged -= UpdateScore;

            IntroState.OnIntro -= HandleIntro;
            PrepareInvasionState.OnPrepare -= SetPrepareTextActive;
            PauseState.OnPause -= SetPauseMenuActive;
            FinishState.OnFinish -= HandleFinish;
        }

        private void UpdateScore(int score)
        {
            scoreText.text = "Score: " + score;
        }

        private void UpdateLives(int lives)
        {
            livesText.text = "Lives: " + lives;
        }

        private void HandleIntro(bool isActive)
        {
            if (isActive)
            {
                finishScreen.gameObject.SetActive(false);
                pauseMenu.gameObject.SetActive(false);
                mainMenu.gameObject.SetActive(true);
                mainMenuPrimaryButton.Select();
            }
            else
            {
                mainMenu.gameObject.SetActive(false);
            }
        }

        private void HandleFinish(int score)
        {
            finalScoreText.text = "Your score: " + score;
            finishScreen.gameObject.SetActive(true);
            finishPrimaryButton.Select();
        }

        private void SetPrepareTextActive(bool isActive)
        {
            prepareText.gameObject.SetActive(isActive);
        }

        private void SetPauseMenuActive(bool isActive)
        {
            pauseMenu.gameObject.SetActive(isActive);
            pauseMenuPrimaryButton.Select();
        }

        public void OnDifficultySet(int value)
        {
            var difficulty = value switch
            {
                0 => EnemyManager.EnemyDifficulty.Easy,
                1 => EnemyManager.EnemyDifficulty.Normal,
                2 => EnemyManager.EnemyDifficulty.Hard,
                _ => EnemyManager.EnemyDifficulty.Easy
            };
            EnemyManager.Instance.SetDifficulty(difficulty);
        }

        public void Play()
        {
            OnPlay?.Invoke();
        }

        public void Resume()
        {
            OnResume?.Invoke();
        }

        public void GameExit()
        {
            OnGameExit?.Invoke();
        }
    
        public void MainExit()
        {
            Application.Quit();
        }
    }
}