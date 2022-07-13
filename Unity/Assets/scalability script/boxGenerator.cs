using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Ubiq.XR;
using UnityEngine;

namespace Ubiq.Samples
{
    public interface FallingCube
    {
        void Attach(Hand hand);
    }
    public class boxGenerator : MonoBehaviour, IUseable
    {
        // public GameObject FireworkPrefab;

        private Hand follow;
        public GameObject FallingPrefab;

        public void UnUse(Hand controller)
        {
        }

        public void Use(Hand controller)
        {
            var cube = NetworkSpawner.SpawnPersistent(this, FallingPrefab);
            //cube.transform.position = ;
        }

        // private Rigidbody body;

        /*
        public void Use(Hand controller)
        {
            var firework = NetworkSpawner.SpawnPersistent(this, FireworkPrefab).GetComponents<MonoBehaviour>().Where(mb => mb is IFirework).FirstOrDefault() as IFirework;
            if (firework != null)
            {
                firework.Attach(controller);
            }
        }
        */

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
