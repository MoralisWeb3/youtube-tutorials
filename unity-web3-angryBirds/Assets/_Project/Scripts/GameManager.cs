using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace AngryBirdsWeb3
{
    public class GameManager : MonoBehaviour
    {
        // Game Constants
        private const int LifeCount = 3;
        private const int WinningScore = 1500;
        
        [Header("World Elements")]
        [SerializeField] private Slingshot slingshot;
        
        [Header("Prefabs")]
        [SerializeField] private GameObject birdPrefab;
        [SerializeField] private GameObject lifeIcon;
        [SerializeField] private ScoreObject scoreObjectPrefab;

        [Header("Game UI")]
        [SerializeField] private Transform lifeContainer;
        [SerializeField] private TextMeshProUGUI scoreLabel;

        [Header("UI Panels")]
        [SerializeField] private GameObject gameUiPanel;
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private GameObject gameWonPanel;
        
        [Header("Components")] 
        [SerializeField] private AudioSource audioSource;

        // Control variables
        private bool _isGameFinished;
        private int _currentScore;
        private int _currentLifeCount;
        private Texture2D _nftTexture;


        #region UNITY_LIFECYCLE

        private void OnEnable()
        {
            CharacterSelector.OnCharacterSelected += StartGame;
            Bird.IsDead += DecreaseLifeCount;
            WorldObject.IsDestroyed += CalculateNewScore;
        }

        private void OnDisable()
        {
            CharacterSelector.OnCharacterSelected -= StartGame;
            Bird.IsDead -= DecreaseLifeCount;
            WorldObject.IsDestroyed -= CalculateNewScore;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                Application.Quit();
            }
        }

        #endregion


        #region PRIVATE_METHODS

        private void StartGame(Texture2D nftTexture)
        {
            _nftTexture = nftTexture;
            
            gameUiPanel.SetActive(true);
            SetLifeCount();
            slingshot.AddProjectile(birdPrefab, _nftTexture);
        }
        
        private void SetLifeCount()
        {
            for (int i = 0; i < LifeCount; i++)
            {
                var newLifeIcon = Instantiate(lifeIcon, lifeContainer);

                // If we have selected the NFT image
                if (_nftTexture != null)
                {
                    var img = newLifeIcon.GetComponent<Image>();
                    if (img != null)
                    {
                        img.sprite = Sprite.Create(_nftTexture, new Rect(0.0f, 0.0f, _nftTexture.width, _nftTexture.height), new Vector2(0.5f, 0.5f));;
                    }
                    else
                    {
                        Debug.LogError("We haven't found an Image Component.");
                    }
                }
            }

            _currentLifeCount = LifeCount;
        }

        private void DecreaseLifeCount()
        {
            if (_isGameFinished) return;
            
            foreach (Transform life in lifeContainer)
            {
                //We just destroy one.
                Destroy(life.gameObject);
                _currentLifeCount--;
                break;
            }

            if (_currentLifeCount > 0)
            {
                //And we add a new projectile to the slingshot.
                slingshot.AddProjectile(birdPrefab, _nftTexture);
            }
            else
            {
                GameOver();
            }
        }

        private void GameWon()
        {
            _isGameFinished = true;
            
            gameWonPanel.SetActive(true);
            audioSource.Play();
        }

        private void GameOver()
        {
            _isGameFinished = true;
            
            gameOverPanel.SetActive(true);
            audioSource.Play();
        }

        #endregion


        #region PUBLIC_METHODS

        public void RestartGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        #endregion
        

        #region EVENT_HANDLERS

        private void CalculateNewScore(Vector3 destroyedObjectPos, int destroyedObjectScore)
        {
            if (_isGameFinished) return;
            
            _currentScore += destroyedObjectScore;
            scoreLabel.text = "SCORE: " + _currentScore;
            
            var scoreObject = Instantiate(scoreObjectPrefab, destroyedObjectPos, Quaternion.identity);
            scoreObject.Init(destroyedObjectScore);

            if (_currentScore >= WinningScore)
            {
                GameWon();
            }
        }

        #endregion
    }
}
