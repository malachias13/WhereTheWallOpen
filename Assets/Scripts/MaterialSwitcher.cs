using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ManaFlux
{
    public class MaterialSwitcher : MonoBehaviour
    {
        [SerializeField] private Material mat1;
        [SerializeField] private Material mat2;
        [SerializeField] private Material mat3;
        [SerializeField] private bool overide;

        // Start is called before the first frame update
        void Start()
        {
            if (overide) return;
            if (SceneManager.GetActiveScene().buildIndex == 1)
                this.GetComponent<MeshRenderer>().material = mat1;
            if (SceneManager.GetActiveScene().buildIndex == 2)
                this.GetComponent<MeshRenderer>().material = mat2;
            if (SceneManager.GetActiveScene().buildIndex == 3)
                this.GetComponent<MeshRenderer>().material = mat3;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

