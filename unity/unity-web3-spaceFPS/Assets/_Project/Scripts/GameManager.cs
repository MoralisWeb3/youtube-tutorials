using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

//Moralis
using Moralis;
using Moralis.Platform.Queries;
using MoralisWeb3ApiSdk;
using Assets.Scripts;

namespace Main
{
    public class GameManager : MonoBehaviour
    {
        public static Action AllEnemiesDead;

        [Header("Control Variables")]
        [SerializeField] private int desiredEnemyCount;
        private int _currentAliveEnemiesCount;
        
        [Header("Enemy Prefabs")]
        [SerializeField] private GameObject bossPrefab;
        [SerializeField] private GameObject enemyPrefab;
        
        [Header("Spawn Corners")]
        [SerializeField] private Transform leftCorner;
        [SerializeField] private Transform rightCorner;
        [SerializeField] private Transform topCorner;

        [Header("UI Panels")]
        [SerializeField] private GameObject hudPanel;
        [SerializeField] private GameObject gameOverPanel;

        [Header("Moralis Mug")]
        [SerializeField] private GameObject moralisMug;

        //Moralis Database
        private MoralisQuery<EnemyData> _allEnemiesQuery;
        private MoralisLiveQueryCallbacks<EnemyData> _enemyCallbacks;
        
        #region UNITY_LIFECYCLE

        private void OnEnable()
        {
            //LiveQuery Callbacks that we subscribe to when we have logged in to Moralis.
            _enemyCallbacks = new MoralisLiveQueryCallbacks<EnemyData>();

            _enemyCallbacks.OnCreateEvent += ((item, requestId) => {
                
            });
            _enemyCallbacks.OnUpdateEvent += ((item, requestId) => {
                
            });
            _enemyCallbacks.OnDeleteEvent += ((item, requestId) =>
            {
                if (_currentAliveEnemiesCount == 0) return;

                _currentAliveEnemiesCount--;
                Debug.Log("Current alive enemies = " + _currentAliveEnemiesCount);
            });

            MoralisWeb3Manager.LoggedInSuccessfully += StartGameLoop;
            PlayerController.Dead += GameOver;
            Enemy.Dead += OnEnemyDead;
            Boss.Dead += OnBossDead;
            
            //We make sure MoralisMug is not enabled.
            moralisMug.SetActive(false);
        }

        private void OnDisable()
        {
            MoralisWeb3Manager.LoggedInSuccessfully -= StartGameLoop;
            PlayerController.Dead -= GameOver;
            Enemy.Dead -= OnEnemyDead;
            Boss.Dead -= OnBossDead;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                StartCoroutine(QuitGame());
            }
        }

        private void OnApplicationQuit()
        {
            //This only executes when we stop playing on the Editor.
#if UNITY_EDITOR
            if (_currentAliveEnemiesCount == 0) return;

            DeleteAllEnemiesData();
#endif
        }

        #endregion

        #region GAME_LOOP

        private IEnumerator GameLoop()
        {
            yield return StartCoroutine(GeneratingEnemies());
            
            yield return StartCoroutine(EnemiesRound());

            yield return StartCoroutine(BossRound());
            
            yield return StartCoroutine(AwardTime());
        }
        
        private IEnumerator GeneratingEnemies()
        {
            GenerateEnemies();
            
            //We stay here while all the enemies are being generated.
            while (_currentAliveEnemiesCount != desiredEnemyCount)
            {
                yield return null;
            }
        }
        
        private IEnumerator EnemiesRound()
        {
            if (desiredEnemyCount <= 0)
            {
                yield break;
            }

            //While there are some enemies left we stay here.
            while (_currentAliveEnemiesCount > 0)
            {
                yield return null;
            }
            
            AllEnemiesDead?.Invoke();
        }
        
        private IEnumerator BossRound()
        {
            Debug.Log("NO ENEMIES LEFT!");
            
            //Instantiate Boss
            var boss = Instantiate(bossPrefab, Vector3.zero, Quaternion.identity);
            
            // While boss is alive...
            while (boss.gameObject != null)
            {
                yield return null;
            }
            
            Debug.Log("BOSS DESTROYED!");
        }
        
        private IEnumerator AwardTime()
        {
            yield return null;
            
            Debug.Log("Time to claim the NFT!");
        }

        #endregion

        #region EVENT_HANDLERS
        
        private void StartGameLoop()
        {
            //IMPORTANT: We create a query to get ALL the EnemyData objects. We will use this query in multiple methods.
            _allEnemiesQuery = MoralisInterface.GetClient().Query<EnemyData>();
            
            //We add a SUBSCRIPTION to that query so we get callback when something happens (object created, deleted, etc.).
            MoralisLiveQueryController.AddSubscription("EnemyData", _allEnemiesQuery, _enemyCallbacks);
            
            hudPanel.SetActive(true);
            StartCoroutine(GameLoop());
        }

        private void GameOver()
        {
            hudPanel.SetActive(false);
            gameOverPanel.SetActive(true);
        }
        
        private void OnEnemyDead(string deadEnemyId)
        {
            DeleteEnemyDataByObjectId(deadEnemyId);
        }

        private void OnBossDead(Vector3 bossLastPosition)
        {
            //Now MoralisMugNFT script takes care of everything.
            moralisMug.transform.position = bossLastPosition;
            moralisMug.SetActive(true);

            hudPanel.SetActive(false);
        }

        #endregion
        
        #region PRIVATE_METHODS

        private async void GenerateEnemies()
        {
            //For the enemy count that we choose...
            for (int i = 0; i < desiredEnemyCount; i++)
            {
                //We first calculate a random instantiation position between our spawn points.
                float xPos = Random.Range(leftCorner.position.x, rightCorner.position.x);
                float yPos = Random.Range(leftCorner.position.y, topCorner.position.y);
                float zPos = Random.Range(leftCorner.position.z, rightCorner.position.z);
                
                var instantiationPos = new Vector3(xPos, yPos, zPos);
                
                //Then we create the EnemyData object.
                EnemyData enemyDb = MoralisInterface.GetClient().Create<EnemyData>();
                
                //And we set the calculated position.
                enemyDb.initPosition = new List<float>
                {
                    instantiationPos.x,
                    instantiationPos.y,
                    instantiationPos.z
                };
                
                //We set a random multiplier for each enemy to have different speed and size.
                var randomMultiplier = Random.Range(1f, 3f);
                enemyDb.size *= randomMultiplier;
                enemyDb.speed *= randomMultiplier;

                //Finally we save the object to the DB.
                var success = await enemyDb.SaveAsync();

                //If it's successfully saved, we will Instantiate our local enemy in Unity.
                if (success)
                {
                    var newLocalEnemy = Instantiate(enemyPrefab, instantiationPos, Quaternion.identity);
                    newLocalEnemy.GetComponent<Enemy>().Initialize(enemyDb.objectId, enemyDb.size, enemyDb.speed);
                        
                    _currentAliveEnemiesCount++;
                }
            }
        }

        private async void DeleteAllEnemiesData()
        {
            //We get a "List" of the required objects using the correspondent query.
            IEnumerable<EnemyData> enemies = await _allEnemiesQuery.FindAsync();
            var enemiesList = enemies.ToList();
            
            if (enemiesList.Any())
            {
                //If there are some, we delete them.
                foreach (var enemy in enemiesList)
                {
                    await enemy.DeleteAsync();
                }
            }
            else
            {
                Debug.Log("There ara no EnemyData objects in the DB to delete.");
            }
        }
        
        private async void DeleteEnemyDataByObjectId(string objectId)
        {
            //We want to query just one specific object.
            MoralisQuery<EnemyData> query = MoralisInterface.GetClient().Query<EnemyData>().WhereEqualTo("objectId", objectId);
            IEnumerable<EnemyData> enemiesToDelete = await query.FindAsync(); //We will just find one because "objectId" is unique.
            
            var enemiesToDeleteList = enemiesToDelete.ToList();
            if (enemiesToDeleteList.Any())
            {
                //If there are some, we delete them.
                foreach (var enemy in enemiesToDeleteList)
                {
                    await enemy.DeleteAsync();
                    
                    //The LiveQuery subscription will take care of that. Go check the events on "OnEnable()".
                }
            }
            else
            {
                Debug.Log("There ara no EnemyData objects in the DB to delete.");
            }
        }

        private IEnumerator QuitGame()
        {
            if (_currentAliveEnemiesCount > 0)
            {
                DeleteAllEnemiesData();
            }

            while (_currentAliveEnemiesCount > 0)
            {
                yield return null;
            }
            
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }

        #endregion
    }
}
