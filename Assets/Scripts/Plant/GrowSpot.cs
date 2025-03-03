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
        private bool _isDead;
        private bool _weedsTrimmed = true;
        private GameObject _currentPlant;
        private GameObject _weed;
        private int _currentStage;
        private bool _fullyGrown;
        private float _time;
        private int _previousDay;

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

            if (_isDead)
            {
                return;
            }

            if (!_isWatered)
            {
                // TODO: dead plant
                return;
            }

            if (plant.growType == GrowType.Time)
            {
                _time += Time.deltaTime;
                
                if (_time >= plant.growthTimeSeconds)
                {
                    Grow();
                    _time = 0;
                }
            }
            else
            {
                if (_previousDay == TimeOfDayManager.Instance.Day)
                {
                    return;
                }
                
                _previousDay = TimeOfDayManager.Instance.Day;
                _isWatered = false;
                Debug.Log("Progressing plant growth");

                if (_weedsTrimmed)
                {
                    Grow();
                    TryGrowWeed();
                }
                else
                {
                    Die();
                }
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
            
            if (other.gameObject.name == "Scissors")
            {
                TrimWeeds();
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
            _previousDay = TimeOfDayManager.Instance.Day;
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
                Debug.Log("Plant grew");
            }
            else
            {
                SetPlant(plant.grownPlant.gameObject);
                _currentPlant.GetComponent<XRGrabInteractable>().selectExited.AddListener(Harvest);
                _currentPlant.GetComponent<FullyGrown>().plant = plant;
                _fullyGrown = true;
                Debug.Log("Plant fully grown");
            }
        }

        private void TryGrowWeed()
        {
            if (!_weedsTrimmed)
            {
                return;
            }
            
            _weedsTrimmed = Random.Range(0, 100) > 25;
            if (_weedsTrimmed)
            {
                return;
            }
            
            _weed = Instantiate(plant.weed.gameObject, transform);
            _weed.transform.localPosition += new Vector3(0, 0.2f, 0);
            
            Debug.Log("Weed grown");
        }

        private void Die()
        {
            if (_isDead)
            {
                return;
            }
            
            _isDead = true;
            // TODO: model
            
            Debug.Log("Plant died");
        }

        private void Harvest(SelectExitEventArgs args)
        {
            Destroy(_weed);
            plant = null;
            _isDigged = false;
            _isWatered = false;
            _isDead = false;
            _weedsTrimmed = true;
            _currentStage = 0;
            _fullyGrown = false;
            _time = 0;
        }

        private void TrimWeeds()
        {
            if (_weedsTrimmed)
            {
                return;
            }

            Destroy(_weed);
            _weedsTrimmed = true;
        }

        private void SetDirtPile(GameObject newDirt)
        {
            Destroy(dirtHolder);
            dirtHolder = Instantiate(newDirt, transform);
            //dirtHolder.transform.localScale = new Vector3(40f, 40f, 40f);
        }

        private void SetPlant(GameObject newPlant)
        {
            if (_currentPlant is not null)
            {
                Destroy(_currentPlant);
            }
            
            _currentPlant = Instantiate(newPlant, transform);
            _currentPlant.transform.localPosition += new Vector3(0, 0.1f, 0);
        }
    }
}
