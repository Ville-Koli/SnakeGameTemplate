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

    public Connection(int input, int output, int innovation){
        this.input = input;
        this.output = output;
        this.innovation = innovation;
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
}
public struct GenomeSettings{
    public float InheritP {get; set;}
    public float CrossoverP {get; set;}
    public float MutateNewNodeP {get; set;}
    public float MutateNewConnP {get; set;}
    public float CompabilityThreshold {get; set;}
}
public class Genome{
    int Genome_id {get; set;}
    int Input_nodes {get; set;}
    int Output_nodes {get; set;}
    float Fitness {get; set;}
    GenomeSettings genomeSettings;
    Func<float, float> SIGMOID = a => 1/(1 - math.exp(-a));
    List<Connection> connection_list = new List<Connection>();
    List<Node> node_list = new List<Node>();

    public void UpdateAllNodesActivationValues(){
        Dictionary<int, List<(int node, Connection c)>> connections = new Dictionary<int, List<(int node, Connection c)>>();
        foreach(var conn in connection_list){
            if(!connections.ContainsKey(conn.input)){
                for(int i = 0; i < node_list.Count; ++i){
                    if(conn.input == node_list[i].node_id){
                        connections.Add(i, new List<(int node, Connection c)>());
                        break;
                    }
                }
            }
            for(int i = 0; i < node_list.Count; ++i){
                if(conn.output == node_list[i].node_id){
                    connections[conn.input].Add((i, conn));
                    break;
                }
            }
        }
        foreach(var pair in connections){
            foreach(var N in pair.Value){
                if(N.c.enabled)
                    node_list[pair.Key].value += node_list[N.node].value * N.c.weight;
            }
            node_list[pair.Key].value = SIGMOID(node_list[pair.Key].value + node_list[pair.Key].bias);
        }
    }

    public Genome CrossoverConnections(Genome parent){
        Genome child = new Genome();
        connection_list.Sort((a, b) => {return a.innovation.CompareTo(b.innovation);});
        parent.connection_list.Sort((a, b) => {return a.innovation.CompareTo(b.innovation);});
        // add each connection randomly or add some of the excess connections
        for(int i = 0; i < connection_list.Count; ++i){
            if(parent.connection_list.Count > i && parent.connection_list[i].innovation == connection_list[i].innovation){
                float p = UnityEngine.Random.Range(0, 1f);
                if(p < genomeSettings.InheritP){
                    child.connection_list.Add(connection_list[i]);
                }else{
                    child.connection_list.Add(parent.connection_list[i]);
                }
            }else{
                if(parent.Fitness <= Fitness){
                    child.connection_list.Add(connection_list[i]);
                }else{
                    child.connection_list.Add(parent.connection_list[i]);
                }
            }
        }
        // add rest of the excess connections
        if(Fitness < parent.Fitness && connection_list.Count < parent.connection_list.Count){
            for(int i = connection_list.Count; i < parent.connection_list.Count; ++i){
                child.connection_list.Add(parent.connection_list[i]);
            }
        }
        return child;
    }

    public Genome CrossoverNodes(Genome parent){
        Genome child = new Genome();
        node_list.Sort((a, b) => {return a.node_id.CompareTo(b.node_id);});
        parent.node_list.Sort((a, b) => {return a.node_id.CompareTo(b.node_id);});
        for(int i = 0; i < node_list.Count; ++i){
            if(node_list[i].node_id == parent.node_list[i].node_id){
                float p = UnityEngine.Random.Range(0, 1f);
                if(p < genomeSettings.InheritP){
                    child.node_list.Add(node_list[i]);
                }else{
                    child.node_list.Add(parent.node_list[i]);
                }
            }else{
                if(Fitness < parent.Fitness){
                    child.node_list.Add(parent.node_list[i]);
                }else{
                    child.node_list.Add(node_list[i]);
                }
            }
        }
        if(parent.Fitness > Fitness){
            for(int i = node_list.Count; i < parent.node_list.Count; ++i){
                child.node_list.Add(parent.node_list[i]);
            }
        }
        return child;        
    }

    public Genome Crossover(Genome parent){
        Genome child = new Genome();
        child.CrossoverConnections(parent);
        child.CrossoverNodes(parent);
        return child;
    }
    
    public void MutateConnection(){
        Dictionary<int, List<int>> connections = new Dictionary<int, List<int>>();
        foreach(var conn in connection_list){
            if(!connections.ContainsKey(conn.input))
                connections.Add(conn.input, new List<int>());
            connections[conn.input].Add(conn.output);
        }
        int randomIndex = UnityEngine.Random.Range(0, connection_list.Count);
        int randomSecondIndex = UnityEngine.Random.Range(0, connection_list.Count);
        while(randomIndex != randomSecondIndex || connections[randomIndex].Contains(randomSecondIndex)){
            randomSecondIndex = UnityEngine.Random.Range(0, connection_list.Count);
        }
        connection_list.Add(new Connection(randomIndex, randomSecondIndex, connection_list.Count));
    }

    public void MutateNode(){
        int randomIndex = UnityEngine.Random.Range(0, connection_list.Count);
        connection_list[randomIndex].enabled = false;
        Node newNode = new Node(node_list.Count, TYPE.hidden);
        Connection connection1 = new Connection(connection_list[randomIndex].input, newNode.node_id, connection_list.Count);
        Connection connection2 = new Connection(newNode.node_id, connection_list[randomIndex].output, connection_list.Count + 1);
        node_list.Add(newNode);
        connection_list.Add(connection1);
        connection_list.Add(connection2);
    }
}

public class Network
{
    public List<Node> nodes = new List<Node>();
    public int input_size, output_size;
    public int[] layer_sizes;


}
