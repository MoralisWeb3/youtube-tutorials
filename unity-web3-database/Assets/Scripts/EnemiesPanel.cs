using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

//Moralis
using MoralisWeb3ApiSdk;
using Moralis.Platform.Objects;
using Moralis.Platform.Queries;

public class Enemy : MoralisObject
{
    public string Name { get; set; }
    public int Level { get; set; }

    public Enemy() : base("Enemy") {}
}

public class EnemiesPanel : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform contentT;
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private Button createButton;

    private List<Enemy> _localEnemies = new List<Enemy>();

    private void Start()
    {
        GetEnemies();
    }

    public void HandleOnCreateButtonClicked()
    {
        CreateEnemy();
    }

    private async void GetEnemies()
    {
        MoralisQuery<Enemy> query = MoralisInterface.GetClient().Query<Enemy>();
        IEnumerable<Enemy> enemies = await query.FindAsync();

        var enemiesList = enemies.ToList();
        if (enemiesList.Any())
        {
            foreach (var enemy in enemiesList)
            {
                var newEnemyPrefab = Instantiate(enemyPrefab, contentT);
                newEnemyPrefab.GetComponent<EnemyPrefab>().Initialize(enemy.Name, enemy.Level);
                
                _localEnemies.Add(enemy);
            }
        }
    }
    
    private async void CreateEnemy()
    {
        //Create Enemy Moralis Object and SAVE it to the database
        Enemy enemy = MoralisInterface.GetClient().Create<Enemy>();
        enemy.Name = nameInputField.text;
        enemy.Level = Random.Range(1, 15); //We set a random Level
        
        var result = await enemy.SaveAsync();

        if (result)
        {
            //Getting the last enemy saved in the DB = The Enemy we just created.
            MoralisQuery<Enemy> query = MoralisInterface.GetClient().Query<Enemy>().OrderByDescending("createdAt").Limit(1);
            IEnumerable<Enemy> enemies = await query.FindAsync();

            var enemiesList = enemies.ToList();
            if (enemiesList.Any())
            {
                var newEnemyPrefab = Instantiate(enemyPrefab, contentT);
                newEnemyPrefab.GetComponent<EnemyPrefab>().Initialize(enemiesList[0].Name, enemiesList[0].Level);
                
                _localEnemies.Add(enemiesList[0]);
            }
        }
    }
    
    public void HandleOnValueChanged()
    {
        createButton.interactable = nameInputField.text != String.Empty;
    }
}
