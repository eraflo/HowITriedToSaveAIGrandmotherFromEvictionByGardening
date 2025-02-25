using UnityEngine;

namespace Plant
{
    public class GrowSpot : MonoBehaviour
    {
        [SerializeField] private GlobalPlantOptions plantOptions;
        [SerializeField] private GameObject dirtHolder;
        
        public PlantObject plant { get; private set; }
        
        private bool _isDigged;
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
            _currentPlant = Instantiate(plant.sproutStages[0], transform);
            
            SetDirtPile(plantOptions.dirtPileFilled);
            return true;
        }

        private void Grow()
        {
            Destroy(_currentPlant);
            _currentStage++;

            if (_currentStage < plant.sproutStages.Length)
            {
                _currentPlant = Instantiate(plant.sproutStages[_currentStage], transform);
            }
            else
            {
                _currentPlant = Instantiate(plant.grownPlant, transform);
                _fullyGrown = true;
            }
        }

        private void Harvest()
        {
            Destroy(_currentPlant);
            plant = null;
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
    }
}
