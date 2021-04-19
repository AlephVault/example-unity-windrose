using AlephVault.Unity.Support.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NetworkedSamples
{
    namespace Behaviours
    {
        public class SampleAutoRedirectToLogin : MonoBehaviour
        {
            public float delay = 0f;

            // Start is called before the first frame update
            async void Start()
            {
                float current = 0f;
                while (current < delay) {
                    await Tasks.Blink();
                    current += Time.deltaTime;
                }
                SceneManager.LoadSceneAsync("Assets/Samples/Networked/Scenes/SingleMap/Login.unity");
            }
        }
    }
}