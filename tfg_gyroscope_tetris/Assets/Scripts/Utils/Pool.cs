using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool : MonoBehaviour
{
    [SerializeField] private GameObject _poolingTarget;
    [SerializeField] private float _initialNumber;
    [SerializeField] private Transform _pooledItemsContainer;

    private List<GameObject> _pooledItems;

    private void Start()
    {
        _pooledItems = new List<GameObject>();
        PopulatePool();
    }

    private void PopulatePool()
    {
        for(int i = 0; i < _initialNumber; i++)
        {
            GameObject item = Instantiate(_poolingTarget, _pooledItemsContainer);
            item.gameObject.SetActive(false);

            _pooledItems.Add(item);
        }
    }

    public GameObject GetItem()
    {
        foreach(GameObject go in _pooledItems)
        {
            if (!go.activeInHierarchy)
                return go;
        }

        //If we are using all the items we need to get a few more
        PopulatePool();

        return GetItem();
    }
}
