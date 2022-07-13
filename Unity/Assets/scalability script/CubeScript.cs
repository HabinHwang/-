using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ubiq.Messaging;
using Ubiq.Samples;

public class CubeScript : MonoBehaviour, INetworkObject, INetworkComponent, ISpawnable
{

    public NetworkId Id { get; set; } = new NetworkId();

    NetworkContext ctx;
    public bool owner = true;
    private Rigidbody body;

    // Var for Dead Reckoning, simple implementation
    Vector3 beforePosition;
    Quaternion beforeRotation;
    float speed;
    Vector3 direction;
    bool firstFrame;
    int updateFrame = 1;
    const float epilson = 0.01F;

    public struct Message
    {
        public TransformMessage transform;
        public float speedForMessage;
        public Vector3 directionForMessage;
        public Message(Transform transform, float speedForMessage, Vector3 directionForMessage)
        {
            this.transform = new TransformMessage(transform);
            this.speedForMessage = speedForMessage;
            this.directionForMessage = directionForMessage;
        }
    }

    public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
    {
        var msg = message.FromJson<Message>();
        transform.localPosition = msg.transform.position;
        transform.localRotation = msg.transform.rotation;
        speed = msg.speedForMessage;
        direction = msg.directionForMessage;
        printUnit("processing message", msg.transform.position.ToString());
    }

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody>();
        ctx = NetworkScene.Register(this);
        //before = this.transform;
        firstFrame = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (owner)
        {
            //for local ownership


            if (firstFrame == true)
            {
                //speed = Vector3.Distance(this.transform.position, before.position);
                //direction = this.transform.position - before.position;
                speed = 0;
                direction = new Vector3(0,0,0);
                firstFrame = false;
                beforePosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                beforeRotation = new Quaternion(transform.rotation.w, transform.rotation.x, transform.rotation.y, transform.rotation.z);
                ctx.SendJson(new Message(transform, speed, direction));
                printUnit("sending message in first frame", transform.position.ToString());
            }
            else
            {
                float tempSpeed = Vector3.Distance(this.transform.position, beforePosition);
                Vector3 tempDirection = this.transform.position - beforePosition;
                Debug.Log("---------info in local---------");
                Debug.Log("tempSpeed is " + tempSpeed.ToString());
                Debug.Log("tempDirection is " + tempDirection.ToString());
                Debug.Log("position of block is " + transform.position.ToString());
                Debug.Log(transform.position.x.ToString() + " " + transform.position.y.ToString() + " " + transform.position.z.ToString());
                Debug.Log("before position of block is " + beforePosition.ToString());
                Debug.Log(beforePosition.x.ToString() + " " + beforePosition.y.ToString() + " " + beforePosition.z.ToString());
                if (tempSpeed != speed && tempDirection != direction && this.transform.rotation != beforeRotation)
                {
                    speed = tempSpeed;
                    direction = tempDirection;
                    ctx.SendJson(new Message(transform, speed, direction));
                    printUnit("sending message", transform.position.ToString());
                    updateFrame = 1;
                }
            }

        }
        else
        {
            //for remote ownership

            body.isKinematic = true;
            this.transform.position = this.transform.position + direction * speed;
            printUnit("update in remote", transform.position.ToString());
        }

        beforePosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        beforeRotation = new Quaternion(transform.rotation.w, transform.rotation.x, transform.rotation.y, transform.rotation.z);
    }

    public void OnSpawned(bool local)
    {
        owner = local;
    }

    private void printUnit(string s, string printPosition)
    {
        Debug.Log("---------" + s + "---------");
        Debug.Log("position is " + printPosition);
        Debug.Log("isKinematic is " + body.isKinematic.ToString());
        Debug.Log("speed is " + speed.ToString());
        Debug.Log("direction is " + direction.ToString());
    }
}
