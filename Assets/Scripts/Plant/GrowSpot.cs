using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Plant
{
    public class GrowSpot : MonoBehaviour
    {
        [SerializeField] private GlobalPlantOptions plantOptions;
        [SerializeField] private GameObject dirtHolder;
        
        public PlantObject plant { get; private set; }
        
        private bool _isDigged;
        private bool _isWatered;
        private GameObject _currentPlant;
        private int _currentStage;
        private bool _fullyGrown;
        private float _time;

        private void Update()
        {
            if (plant is null)
            {
                return;
            }

            if (_fullyGrown)
            {
                return;
            }

            if (!_isWatered)
            {
                // TODO: dead plant
                return;
            }

            _time += Time.deltaTime;
            
            if (_time >= plant.growthTime)
            {
                Grow();
                _time = 0;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.name == "Trowel")
            {
                Dig();
                return;
            }

            if (other.gameObject.name.StartsWith("Seed"))
            {
                if (Plant(other.gameObject.GetComponent<Seed>().plant))
                {
                    Destroy(other.gameObject);
                }
                
                return;
            }

            if (other.gameObject.name == "Watering Can")
            {
                Water();
                return;
            }
        }

        private void Dig()
        {
            if (_isDigged)
            {
                return;
            }

            _isDigged = true;
            SetDirtPile(plantOptions.dirtPileDigged);
        }

        private bool Plant(PlantObject plantObject)
        {
            if (!_isDigged)
            {
                return false;
            }
            
            if (_currentPlant is not null)
            {
                return false;
            }
            
            plant = plantObject;
            SetPlant(plant.sproutStages[0]);
            
            SetDirtPile(plantOptions.dirtPileFilled);
            return true;
        }

        private void Water()
        {
            if (_isWatered)
            {
                return;
            }

            _isWatered = true;
        }

        private void Grow()
        {
            _currentStage++;

            if (_currentStage < plant.sproutStages.Length)
            {
                SetPlant(plant.sproutStages[_currentStage]);
            }
            else
            {
                SetPlant(plant.grownPlant.gameObject);
                _currentPlant.GetComponent<XRGrabInteractable>().selectExited.AddListener(Harvest);
                _fullyGrown = true;
            }
        }

        private void Harvest(SelectExitEventArgs args)
        {
            Destroy(_currentPlant);
            plant = null;
            _isDigged = false;
            _isWatered = false;
            _currentStage = 0;
            _fullyGrown = false;
            _time = 0;
            // TODO: add to inventory
        }

        private void SetDirtPile(GameObject newDirt)
        {
            Destroy(dirtHolder);
            dirtHolder = Instantiate(newDirt, transform);
            dirtHolder.transform.localScale = new Vector3(40f, 40f, 40f);
        }

        private void SetPlant(GameObject newPlant)
        {
            if (_currentPlant is not null)
            {
                Destroy(_currentPlant);
            }
            
            _currentPlant = Instantiate(newPlant, transform);
            _currentPlant.transform.localPosition -= new Vector3(0, 0.2f, 0);
        }
    }
}
