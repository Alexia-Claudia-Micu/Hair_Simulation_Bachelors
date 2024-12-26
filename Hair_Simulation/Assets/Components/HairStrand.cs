using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class HairStrand : MonoBehaviour
{
    public GameObject emitter;

    public List<StrandVertex> Vertices;
    public List<StrandSpring> Springs;

    void Start()
    {
        this.Vertices = new List<StrandVertex>();
        this.Springs = new List<StrandSpring>();

        StrandVertex vertex0 = new StrandVertex(new Vector3(0    , 6f  , 0), Constants.StraightHairMass, true); this.Vertices.Add(vertex0);
        StrandVertex vertex1 = new StrandVertex(new Vector3(0.4f , 5.6f, 0), Constants.StraightHairMass); this.Vertices.Add(vertex1);
        StrandVertex vertex2 = new StrandVertex(new Vector3(0    , 5.2f, 0), Constants.StraightHairMass); this.Vertices.Add(vertex2);
        StrandVertex vertex3 = new StrandVertex(new Vector3(-0.4f, 4.8f, 0), Constants.StraightHairMass); this.Vertices.Add(vertex3);
        StrandVertex vertex4 = new StrandVertex(new Vector3(0.0f , 4.4f, 0), Constants.StraightHairMass); this.Vertices.Add(vertex4);
        StrandVertex vertex5 = new StrandVertex(new Vector3(0.4f , 4.0f, 0), Constants.StraightHairMass); this.Vertices.Add(vertex5);
        StrandVertex vertex6 = new StrandVertex(new Vector3(0.0f , 3.6f, 0), Constants.StraightHairMass); this.Vertices.Add(vertex6);
        StrandVertex vertex7 = new StrandVertex(new Vector3(-0.4f, 3.2f, 0), Constants.StraightHairMass); this.Vertices.Add(vertex7);
        StrandVertex vertex8 = new StrandVertex(new Vector3(0.0f , 2.8f, 0), Constants.StraightHairMass); this.Vertices.Add(vertex8);
        StrandVertex vertex9 = new StrandVertex(new Vector3(0.4f , 2.4f, 0), Constants.StraightHairMass); this.Vertices.Add(vertex9);
        StrandVertex vertex10 = new StrandVertex(new Vector3(0.0f , 2.0f, 0), Constants.StraightHairMass); this.Vertices.Add(vertex10);

        AddSpring(0, 1);
        AddSpring(1, 2);
        AddSpring(2, 3);
        AddSpring(3, 4);
        AddSpring(4, 5);
        AddSpring(5, 6);
        AddSpring(6, 7);
        AddSpring(7, 8);
        AddSpring(8, 9);
        AddSpring(9, 10);
    }

    void AddSpring(int from, int to)
    {
        float restLength = (Vertices[to].Position - Vertices[from].Position).magnitude;
        Springs.Add(new StrandSpring(from, to, restLength, Constants.StraightHairStiffness, Constants.StraightHairDamping));
    }

    void Update()
    {
        SimulateStrand();
        UpdateVertices();
    }

    void UpdateVertices()
    {
        float deltaTime = Time.deltaTime;

        foreach (var vertex in this.Vertices)
        {
            if(vertex.isRoot)
            {
                Vector3 emitterVelocity = emitter.GetComponent<Rigidbody>().linearVelocity;
                vertex.Velocity = emitterVelocity;
                vertex.Position += vertex.Velocity * deltaTime;
            }
            else
            {
                Vector3 gravityForce = Constants.Gravity * vertex.Mass;
                vertex.Velocity += (gravityForce / vertex.Mass) * deltaTime;
                vertex.Position += vertex.Velocity * deltaTime;
            }
        }
    }

    void SimulateStrand()
    {
        float deltaTime = Time.deltaTime;

        foreach (var spring in this.Springs)
        {
            StrandVertex vertexFrom = this.Vertices[spring.VertexFrom];
            StrandVertex vertexTo = this.Vertices[spring.VertexTo];

            Vector3 direction = vertexTo.Position - vertexFrom.Position;
            float currentLength = direction.magnitude;

            direction = direction.normalized;

            float springForceMagnitude = spring.Stiffness * (currentLength - spring.RestLength); // displacement from the rest position
            Vector3 springForce = springForceMagnitude * direction;

            Vector3 relativeVelocity = vertexTo.Velocity - vertexFrom.Velocity;
            Vector3 dampingForce = spring.Damping * relativeVelocity;

            Vector3 vertexFrom_Force = springForce + dampingForce;
            Vector3 vertexTo_Force = -vertexFrom_Force;

            if(!vertexFrom.isRoot)
            {
                vertexFrom.Velocity += (vertexFrom_Force / vertexFrom.Mass) * deltaTime;
            }
            vertexTo.Velocity += (vertexTo_Force / vertexTo.Mass) * deltaTime;
        }
    }
}
