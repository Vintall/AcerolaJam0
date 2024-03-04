using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GrassTest : MonoBehaviour
{
    [SerializeField] private Mesh mesh;
    [SerializeField] private Transform firstPoint;
    [SerializeField] private Transform secondPoint;

    private List<Vector3> _grassPositions;
    private int _attemptsCount = 5000;

    private void Start()
    {
        _grassPositions = new List<Vector3>(100000);
        var firstPointPosition = firstPoint.transform.position;
        var secondPointPosition = secondPoint.transform.position;

        for (var i = 0; i < _attemptsCount; ++i)
        {
            var randomPosition = new Vector3()
            {
                x = Random.Range(firstPointPosition.x, secondPointPosition.x),
                y = Random.Range(firstPointPosition.y, secondPointPosition.y),
                z = Random.Range(firstPointPosition.z, secondPointPosition.z)
            };
            var ray = new Ray()
            {
                direction = Vector3.down,
                origin = randomPosition
            };
            Debug.Log($"Pos: {randomPosition}");
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, 5) &&
                hitInfo.transform.tag.Equals("Land"))
            {
                _grassPositions.Add(randomPosition);
            }

            
        }

        Debug.Log($"Positions count: {_grassPositions.Count}");
    }

    private void OnDrawGizmos()
    {
        
    }
}
