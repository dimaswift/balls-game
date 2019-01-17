using System;
using BallGame.Controllers;
using BallGame.Model;
using BallGame.Views;
using UnityEngine;

namespace BallGame
{
    public class App : MonoBehaviour
    {
        public event Action OnSimulationFinished;
        public event Action OnSimulationStarted;
        
        public SimulationController SimulationController
        {
            get { return _simulationController; }
        }
        
        public SimulationView SimulationView
        {
            get { return _simulationView; }
        }

        [SerializeField] TextAsset _configData; 
        [SerializeField] float _minRadius = .2f;
        [SerializeField] SimulationView _simulationView;
        
        SimulationController _simulationController;
        [SerializeField]
        SimulationConfig _config;

        const string PREFS_SAVE_KEY = "simulation";

        void Awake()
        {
            _config = new ConfigLoader(_configData.text).LoadConfig();
            _simulationView.Init(_config);
            StartSimulation(new Simulation());
        }

        public void StartSimulation(Simulation simulation)
        {
            if (_simulationController != null)
                _simulationController.OnSimulationFinished -= SimulationDidFinish;
            
            _simulationView.Dispose();
            _simulationController = new SimulationController(_config, simulation, _minRadius);
            _simulationController.OnSimulationFinished += SimulationDidFinish;
            _simulationView.SetUp(_simulationController);
            if (OnSimulationStarted != null)
                OnSimulationStarted();
        }

        void Update()
        {
            if(_simulationController == null)
                return;
            _simulationController.Tick(Time.deltaTime);
            _simulationView.Render();
        }

        void SimulationDidFinish()
        {
            if (OnSimulationFinished != null)
                OnSimulationFinished();
        }
        
        public void SaveSimulation()
        {
            if(_simulationController == null)
                return;
            var sim = _simulationController.Simulation;
            PlayerPrefs.SetString(PREFS_SAVE_KEY, JsonUtility.ToJson(sim));
        }

        public void CreateNewSimulation()
        {
            if(_simulationController == null)
                return;
            StartSimulation(new Simulation());
            _simulationView.StartSpawningAnimation();
        }

        public void LoadSimulationFromJson()
        {
            var json = PlayerPrefs.GetString(PREFS_SAVE_KEY);
            if(string.IsNullOrEmpty(json))
                return;
            var sim = JsonUtility.FromJson<Simulation>(json);
            StartSimulation(sim);
        }
    }
}