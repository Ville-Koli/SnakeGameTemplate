using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Node
{
    public List<int> Inputs {get; set;}
    public List<float> Weights {get; set;}
    public float value = 0f;
    public float bias = 0f;
    public void ReCalculateValue(List<Node> nodes){
        value = 0f;
        int weightIndex = 0;
        foreach(int i in Inputs){
            value += nodes[i].value * Weights[weightIndex];
            ++weightIndex;
        }
        value += bias;
    }
}

public class Network
{
    public List<Node> nodes;
    public int input_size, output_size;
    public int[] layer_sizes;


    public Network(int input_size, int output_size, params int[] layer_size){
        this.input_size = input_size;
        this.output_size = output_size;
        this.layer_sizes = layer_size;
        int nodes_in_layer = layer_size.Sum() + output_size;
        for(int i = 0; i < input_size; ++i){
            nodes.Add(new Node());
        }
        for(int i = 0; i < nodes_in_layer; ++i){
            nodes.Add(new Node());
            for(int j = 0; j < nodes.Count; ++j)
                nodes[i].Inputs.Add(j);
                nodes[i].Weights.Add(UnityEngine.Random.Range(0f, 1f));
        }
    }

    public static Network Crossover(Network a, Network b){
        Network network_child = new Network(a.input_size, a.output_size, a.layer_sizes);
        for(int i = 0; i < a.nodes.Count; ++i){
            Node cNode = a.nodes[i];
            for(int j = 0; j < b.nodes[i].Weights.Count; ++i){
                cNode.Weights[j] = (cNode.Weights[j] + b.nodes[i].Weights[j]) / 2;
            }
            cNode.bias = (cNode.bias + b.nodes[i].bias) / 2;
            network_child.nodes.Add(cNode);
        }
        return network_child;
    }

    public static Network Mutation(Network a){
        Network network_child = new Network(a.input_size, a.output_size, a.layer_sizes);
        int index = UnityEngine.Random.Range(0, a.nodes.Count - 1);
        network_child.nodes[index].Weights[UnityEngine.Random.Range(0, a.nodes[index].Weights.Count - 1)] = UnityEngine.Random.Range(0f, 1f);
        network_child.nodes[index].bias = UnityEngine.Random.Range(0f, 1f);
        return network_child;
    }
}
