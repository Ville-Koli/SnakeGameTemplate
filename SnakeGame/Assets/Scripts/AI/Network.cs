using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public enum TYPE {input, output, hidden}
public class Connection{
    public int input;
    public int output;
    public bool enabled = true;
    public int innovation;
    public float weight = UnityEngine.Random.Range(-1f, 1f);

    public Connection(int input, int output){
        this.input = input;
        this.output = output;
    }

    public Connection(int input, int output, float weight){
        this.input = input;
        this.output = output;
        this.weight = weight;
    }

    public Connection(int input, int output, int innovation){
        this.input = input;
        this.output = output;
        this.innovation = innovation;
    }
    public Connection(int input, int output, float weight, int innovation){
        this.input = input;
        this.output = output;
        this.innovation = innovation;
        this.weight = weight;
    }
    public void SetInnovation(int innovation){
        this.innovation = innovation;
    }

    public void SetWeight(float newWeight){
        weight = newWeight;
    }
}

public class Node
{
    public int node_id;
    public float value = 0f;
    public float bias = UnityEngine.Random.Range(-1f, 1f);
    public TYPE type;
    public Node(int node_id, TYPE type){
        this.node_id = node_id;
        this.type = type;
    }
    public Node(TYPE type, float bias){
        this.type = type;
        this.bias = bias;
    }
    public Node(TYPE type, float value, float bias){
        this.type = type;
        this.bias = bias;
        this.value = value;
    }

    public Node(int node_id, TYPE type, float value, float bias){
        this.node_id = node_id;
        this.type = type;
        this.bias = bias;
        this.value = value;
    }
}

public class Network
{
    public List<Node> nodes = new List<Node>();
    public int input_size, output_size;
    public int[] layer_sizes;


}
