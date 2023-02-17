using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OrderElimination
{
    public class TestGeneration : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            var creator = new SimpleMapGenerator();
            creator.GenerateMap();
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }   
}
