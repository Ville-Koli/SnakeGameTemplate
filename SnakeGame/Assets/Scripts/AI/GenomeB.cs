using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Timeline;



public class GeneticAlgorithm {
    public const double FITNESS_NOT_COMPUTED = -100;
    public static NeatConfig neatConfig = new NeatConfig();
    public static Func<float, float> sigmoid = x => 1.0f/(1 + math.exp(-x));
    public class NeatConfig{
        public double init_mean = 0.0f;
        public double init_stdev = 1f;
        public double min = -20;
        public double max = 20;
        public double mutation_rate = 0.2f;
        public double mutation_power = 1.2f;
        public double replace_rate = 0.05f;
        public double survival_treshold = 0.5f;
        public double population_size = 200;
        public int num_inputs = 7;
        public int num_outputs = 4;
        public System.Random rand = new System.Random();

        public double new_value(){
            double u1 = 1.0-rand.NextDouble(); //uniform(0,1] random doubles
            double u2 = 1.0-rand.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                        Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
            double randNormal =
                        init_mean + init_stdev * randStdNormal;
            return clamp(randNormal);
        }
        public double mutate_delta(double value){
            double u1 = 1.0-rand.NextDouble(); //uniform(0,1] random doubles
            double u2 = 1.0-rand.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                        Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
            double randNormal =
                        mutation_power * randStdNormal;
            return clamp(value + randNormal);     
        }

        double clamp(double x){
            return math.min(max, math.max(min, x));
        }
    }
    public class NeuronGene {
        public static int global_neuron_id = 0;
        public int neuron_id;
        public double bias;
        public Func<float, float> activation;
        public NeuronGene(int neuron_id, double bias, Func<float, float> activation){
            this.neuron_id = neuron_id;
            this.bias = bias;
            this.activation = activation;
        }
        public static NeuronGene neuron_mutator(){
            ++global_neuron_id;
            return new NeuronGene(global_neuron_id, neatConfig.new_value(), sigmoid);
        }
        public static NeuronGene new_neuron(int id){
            return new NeuronGene(id, neatConfig.new_value(), sigmoid);
        }
    }

    public class LinkId {
        public int input_id;
        public int output_id;
        public LinkId(int input_id, int output_id){
            this.input_id = input_id;
            this.output_id = output_id;
        }
        public bool Equals(LinkId b){
            return input_id == b.input_id && output_id == b.output_id;
        }
    }

    public class LinkGene {
        public LinkId link_id;
        public double weight;
        public bool is_enabled;
        public LinkGene(LinkId link_id, double weight, bool is_enabled){
            this.link_id = link_id;
            this.weight = weight;
            this.is_enabled = is_enabled;
        }
        public static LinkGene link_mutator(LinkId link_id){
            return new LinkGene(link_id, neatConfig.new_value(), UnityEngine.Random.Range(0f, 1f) > 0.5f ? true : false);
        }
        public static LinkGene new_link(LinkId link_id){
            return new LinkGene(link_id, neatConfig.new_value(), UnityEngine.Random.Range(0f, 1f) > 0.5f ? true : false);
        }
    }
    
    public class Individual {
        public Genome genome;
        public double fitness;
        public Individual(){}
        public Individual(Genome genome, double fitness){
            this.genome = genome;
            this.fitness = fitness;
        }
    }

    public class Genome{
        public static int g_genome_id = 0;
        public int genome_id;
        public int num_inputs;
        public int num_outputs;
        public List<NeuronGene> neurons;
        public List<LinkGene> links;
        public Genome(int genome_id, int num_inputs, int num_outputs){
            this.genome_id = genome_id;
            this.num_inputs = num_inputs;
            this.num_outputs = num_outputs;
            neurons = new List<NeuronGene>();
            links = new List<LinkGene>();
        }
        public static int Next(){
            ++g_genome_id;
            return g_genome_id;
        }

        public List<int> make_input_ids(){
            List<int> inputs = new List<int>();
            // collect negative ids to a list
            for(int i = 0; i < num_inputs; ++i){
                inputs.Add(-i - 1);
            }
            return inputs;
        }
        public List<int> make_output_ids(){
            List<int> outputs = new List<int>();
            // collect negative ids to a list
            for(int i = 0; i < num_outputs; ++i){
                outputs.Add(i);
            }
            return outputs;
        }

        NeuronGene choose_hidden_neuron(){
            int attempt = 0;
            int max_attempt = num_inputs + num_outputs;
            while(attempt < max_attempt){
                var random_neuron = neurons[UnityEngine.Random.Range(0, neurons.Count)];
                int countOfInputs = 0;
                int countOfOutputs = 0;
                foreach(var link in links){
                    if(link.link_id.input_id == random_neuron.neuron_id){
                        ++countOfInputs;
                    }else if(link.link_id.output_id == random_neuron.neuron_id){
                        ++countOfOutputs;
                    }
                    if(countOfInputs > 0 && countOfOutputs > 0){
                        return random_neuron;
                    }
                }
                ++attempt;
            }
            return null;
        }

        public int choose_a_random_input_hidden_neuron(){
            var probability = UnityEngine.Random.Range(0, 1f);
            if(probability < 0.5f){
                NeuronGene hidden = choose_hidden_neuron();
                if(hidden != null){
                    return ((NeuronGene) hidden).neuron_id;
                }
            }
            return UnityEngine.Random.Range(-1, -num_inputs - 1);
        }

        public int choose_a_random_output_hidden_neuron(){
            var probability = UnityEngine.Random.Range(0, 1f);
            if(probability < 0.5f){
                NeuronGene hidden = choose_hidden_neuron();
                if(hidden != null){
                    return ((NeuronGene) hidden).neuron_id;
                }
            }
            return UnityEngine.Random.Range(0, num_outputs);
        }

        public static bool would_create_a_cycle(List<LinkGene> links, int input, int output){
            bool is_cycle;
            foreach(var link in links){
                if(link.link_id.input_id == output){
                    if(link.link_id.output_id == input){
                        return true;
                    }
                    is_cycle = would_create_a_cycle(links, input, link.link_id.output_id);
                    if(is_cycle) return true;
                }
            }
            return false;
        }

        public void struct_mutate_add_link(){
            int input_id = choose_a_random_input_hidden_neuron();
            int output_id = choose_a_random_output_hidden_neuron();
            if(input_id == output_id) return;
            LinkId link_id = new LinkId(input_id, output_id);
            // don't duplicate links
            LinkGene existing_link = links.Find(link => {return link_id.Equals(link.link_id);});
            if(existing_link != null){
                existing_link.is_enabled = true;
                return;
            }
            if(would_create_a_cycle(links, input_id, output_id)){
                return;
            }
            Debug.Log("trying to form a link between ids: " + input_id + " " + output_id + " is links null: " + (links == null));
            LinkGene new_link = LinkGene.link_mutator(link_id);
            links.Add(new_link);
        }

        public void struct_mutate_remove_link(){
            if(links.Count == 0){
                return;
            }
            var random_link = links[UnityEngine.Random.Range(0, links.Count)];
            links.Remove(random_link);
        }
        public void struct_mutate_change_link(){
            if(links.Count == 0){
                return;
            }
            var random_link = links[UnityEngine.Random.Range(0, links.Count)];
            random_link.weight = neatConfig.mutate_delta(random_link.weight);
        }
        public void struct_mutate_add_neuron(){
            if(links.Count == 0){
                return;
            }
            var id = UnityEngine.Random.Range(0, links.Count);
            var random_link = links[id];
            links[id].is_enabled = false;
            //Debug.Log("did we actually set it false: " + links[id].is_enabled);

            NeuronGene new_neuron = NeuronGene.neuron_mutator();
            neurons.Add(new_neuron);

            LinkId link_id = new LinkId(random_link.link_id.input_id, new_neuron.neuron_id);
            LinkId link_id_2 = new LinkId(new_neuron.neuron_id, random_link.link_id.output_id);
            LinkGene splitted_link = new LinkGene(link_id, 1.0f, true);
            LinkGene splitted_link_2 = new LinkGene(link_id_2, random_link.weight, true);
            // Debug.Log("trying to form a link between ids: " + random_link.link_id.input_id + " " + new_neuron.neuron_id + " " + random_link.link_id.output_id + " random link weight: " + random_link.weight);
            // >> BUG HERE <<
            links.Add(splitted_link); 
            links.Add(splitted_link_2);
        }
        public void struct_mutate_remove_neuron(){
            if(neurons.Count - num_inputs - num_outputs <= 0){
                return;
            }
            var random_hidden_neuron = choose_hidden_neuron();
            if(random_hidden_neuron == null) return;
            NeuronGene rhn = random_hidden_neuron;
            links.RemoveAll(link => {return link.link_id.input_id == rhn.neuron_id || link.link_id.output_id == rhn.neuron_id;});
            neurons.Remove(rhn);
        }

        public Genome copy_genome(){
            Genome genome = new Genome(Genome.Next(), num_inputs, num_outputs);
            for(int i = 0; i < neurons.Count; ++i){
                genome.neurons.Add(new NeuronGene(neurons[i].neuron_id, neurons[i].bias, neurons[i].activation));
            }
            for(int i = 0; i < links.Count; ++i){
                genome.links.Add(new LinkGene(new LinkId(links[i].link_id.input_id, links[i].link_id.output_id), links[i].weight, links[i].is_enabled));
            }
            return genome;
        }
    }

    public static NeuronGene crossover_neuron(NeuronGene a, NeuronGene b){
        if(a.neuron_id != b.neuron_id){
            return null;
        }
        double bias = UnityEngine.Random.Range(0, 1f) > 0.5 ? a.bias : b.bias;
        Func<float, float> activation = UnityEngine.Random.Range(0, 1f) > 0.5 ? a.activation : b.activation;
        return new NeuronGene(a.neuron_id, bias, activation);
    }

    public static LinkGene crossover_link(LinkGene a, LinkGene b){
        if(!a.Equals(b)){
            return null;
        }
        LinkId link_id = a.link_id;
        double weight = UnityEngine.Random.Range(0, 1f) > 0.5 ? a.weight : b.weight;
        bool is_enabled = UnityEngine.Random.Range(0, 1f) > 0.5 ? a.is_enabled : b.is_enabled;
        return new LinkGene(link_id, weight, is_enabled);
    }

    public static Genome crossover(Individual dominant, Individual recessive){
        //Genome.Next(), a.genome.num_inputs, a.genome.num_outputs
        Genome offspring = new Genome(Genome.Next(), dominant.genome.num_inputs, dominant.genome.num_outputs);
        // inherit neuron genes
        foreach(var dominant_neuron in dominant.genome.neurons){
            NeuronGene recessive_neuron = recessive.genome.neurons.Find(neuron => {return dominant_neuron.neuron_id == neuron.neuron_id;});
            if(recessive_neuron == null || recessive_neuron.activation == null){
                offspring.neurons.Add(dominant_neuron);
            }else{
                offspring.neurons.Add(crossover_neuron(dominant_neuron, recessive_neuron));
            }
        }
        // inherit links
        foreach(var dominant_link in dominant.genome.links){
            LinkGene recessive_link = recessive.genome.links.Find(link => {return link.Equals(dominant_link.link_id);});
            if(recessive_link == null || (recessive_link.link_id.input_id == 0 && recessive_link.link_id.output_id == 0)){
                offspring.links.Add(dominant_link);
            }else{
                LinkGene crossover_link_gene = (LinkGene)crossover_link(dominant_link, recessive_link);
                if(crossover_link_gene.link_id.input_id == 0 && crossover_link_gene.link_id.output_id == 0){
                    offspring.links.Add(dominant_link);
                }else{
                    offspring.links.Add(crossover_link_gene);
                }
                //Debug.Log(recessive_link_conv.link_id.input_id + " " + recessive_link_conv.link_id.output_id + " " + dominant_link.link_id.input_id + " " + dominant_link.link_id.output_id);
                //offspring.links.Add(crossover_link_gene);
            }
        }
        return offspring;        
    }
    public static void mutate(Genome parent){
        if(UnityEngine.Random.Range(0, 1f) >= neatConfig.mutation_rate) return;
        int mutation_type = UnityEngine.Random.Range(0, 4);
        switch(mutation_type){
            case 0:
                parent.struct_mutate_add_link();
                break;
            case 1:
                parent.struct_mutate_add_neuron();
                break;
            case 2:
                parent.struct_mutate_remove_link();
                break;
            case 3:
                parent.struct_mutate_remove_neuron();
                break;
        }   
        if(UnityEngine.Random.Range(0, 1f) < 0.01f){
            parent.struct_mutate_change_link();
        }
    }
    public static void mutate_prominent_link_change(Genome parent){
        if(UnityEngine.Random.Range(0, 1f) >= neatConfig.mutation_rate) return;
        int mutation_type = UnityEngine.Random.Range(0, 30);
        switch(mutation_type){
            case 0:
                parent.struct_mutate_add_link();
                break;
            case 1:
                parent.struct_mutate_add_neuron();
                break;
            case 2:
                parent.struct_mutate_remove_link();
                break;
            case 3:
                parent.struct_mutate_remove_neuron();
                break;
        }
        parent.struct_mutate_change_link();
    }
    public class Population{
        public List<Individual> population = new List<Individual>();
        public NeatConfig config;
        public Population(NeatConfig config){
            this.config = config;
            for(int i = 0; i < config.population_size; ++i){
                Individual new_individual = new Individual();
                new_individual.genome = new_genome();
                new_individual.fitness = FITNESS_NOT_COMPUTED;
                population.Add(new_individual);
            }
        }
        private Genome new_genome(){
            Genome genome = new Genome(Genome.Next(), config.num_inputs, config.num_outputs);
            for(int neuron_id = 0; neuron_id < config.num_outputs; ++neuron_id){
                genome.neurons.Add(NeuronGene.new_neuron(neuron_id));
            }
            for(int neuron_id = 0; neuron_id < config.num_inputs; ++neuron_id){
                int input_id = -neuron_id - 1;
                genome.neurons.Add(NeuronGene.new_neuron(input_id));
                for(int output_id = 0; output_id < config.num_outputs; ++output_id){
                    genome.links.Add(LinkGene.new_link(new LinkId(input_id, output_id)));
                }
            }
            return genome;
        }
        public List<Individual> reproduce(){
            population.Sort((i1, i2) => {return i2.fitness.CompareTo(i1.fitness);});
            int reproduction_cutoff = (int)Mathf.Ceil((float)config.survival_treshold * population.Count);
            List<Individual> new_population = new List<Individual>();
            for(int i = 0; i < population.Count * 0.1f; ++i){
                new_population.Add(population[i]);
            }
            int spawn_size = (int) (population.Count * 0.9f);
            while(spawn_size-- > 0){
                int p1 = UnityEngine.Random.Range(0, reproduction_cutoff);
                int p2 = UnityEngine.Random.Range(0, reproduction_cutoff);
                Genome offspring = crossover(population[p1], population[p2]);
                mutate(offspring); // BUG HERE
                new_population.Add(new Individual(offspring, FITNESS_NOT_COMPUTED));
            }
            return new_population;
        }
        public List<Individual> reproduce_hill_climb(){
            population.Sort((i1, i2) => {return i2.fitness.CompareTo(i1.fitness);});
            int reproduction_cutoff = (int)Mathf.Ceil((float)config.survival_treshold * population.Count);
            List<Individual> new_population = new List<Individual>() {new Individual(population[0].genome.copy_genome(), FITNESS_NOT_COMPUTED)};
            for(int i = 0; i < population.Count - 1; ++i){
                Genome offspring = population[0].genome.copy_genome();
                for(int j = 0; j < i; ++j){
                    mutate_prominent_link_change(offspring);
                }
                new_population.Add(new Individual(offspring, FITNESS_NOT_COMPUTED));
            }
            return new_population;
        }
    }

    public struct NeuronInput{
        public int input_id;
        public double weight;
    }
    public struct Neuron{
        public int neuron_id;
        public Func<float, float> activation;
        public double bias;
        public List<NeuronInput> inputs;
    }

    public class FeedForwardNeuralNetwork {
        private List<int> input_ids;
        private List<int> output_ids;
        private List<Neuron> neurons;
        public FeedForwardNeuralNetwork(List<int> inputs, List<int> outputs, List<Neuron> neurons){
            this.input_ids = inputs;
            this.output_ids = outputs;
            this.neurons = neurons;
        }
        public List<int> get_inputs(){
            return input_ids;
        }
        public List<int> get_outputs(){
            return output_ids;
        }
        public List<Neuron> get_neurons(){
            return this.neurons;
        }
        public List<double> activate(List<double> inputs){
            if(input_ids.Count != inputs.Count){
                return null;
            }
            Dictionary<int, double> values = new Dictionary<int, double>();
            for(int i = 0; i < inputs.Count; ++i){
                int input_id = input_ids[i];
                values[input_id] = inputs[i];
                //Debug.Log($"INPUT VALUES: id: {input_id} in inputs: {inputs[i]} result: {values[input_id]}");
            }
            foreach(var output_id in output_ids){
                values[output_id] = 0;
            }
            foreach(var neuron in neurons){
                double value = 0.0f;
                if(values.ContainsKey(neuron.neuron_id)){
                    value = values[neuron.neuron_id];
                }
                foreach(NeuronInput input in neuron.inputs){
                    if(values.ContainsKey(input.input_id)){
                        value += values[input.input_id] * input.weight;
                        //Debug.Log($"\t\t Neuron {neuron.neuron_id} -> Output Value {input.input_id}, value: {values[input.input_id]},  weight: {input.weight}, outcoming: {value}");
                    }
                }
                //Debug.Log($"Bef Neuron {neuron.neuron_id} -> Output Value {value}, bias: {neuron.bias}");
                value += neuron.bias;
                value = neuron.activation((float)value);
                values[neuron.neuron_id] = value;
                //Debug.Log($"Aft Neuron {neuron.neuron_id} -> Output Value {value}, bias: {neuron.bias}");
            }
            List<double> outputs = new List<double>();
            foreach(var output_id in output_ids){
                if(values.ContainsKey(output_id)){
                    outputs.Add(values[output_id]);
                }
            }
            return outputs;
        }
    }

    public static List<List<int>> feed_forward_layers(List<int> inputs, List<int> outputs, List<LinkGene> links){
        List<List<int>> layers = new List<List<int>>(){inputs};
        int layer_counter = 0;
        int counter = 0;
        int maximum_layers = 100;
        while(counter < layers[layer_counter].Count && layers.Count < maximum_layers){
            if(layers[layer_counter].Count > 0 && layers.Count == layer_counter + 1){
                layers.Add(new List<int>());
            }
            foreach(var link in links){
                if(link.link_id.input_id == layers[layer_counter][counter]){
                    if(!layers[layer_counter + 1].Any(elem => link.link_id.output_id == elem) && link.link_id.input_id != link.link_id.output_id){
                        layers[layer_counter + 1].Add(link.link_id.output_id);
                    }
                }
            }
            counter++;
            if(counter == layers[layer_counter].Count){
                counter = 0;
                layer_counter++;
            }
        }
        if(layers.Count >= maximum_layers){
            Debug.Log("MAXIMUM LAYERS EXCEEDED!");
        }
        layers.RemoveAll(layer => layer.Count == 0);
        return layers;
    }
    public static FeedForwardNeuralNetwork create_from_genome(Genome genome){
        List<int> inputs = genome.make_input_ids();
        List<int> outputs = genome.make_output_ids();
        List<List<int>> layers = feed_forward_layers(inputs, outputs, genome.links);
        List<Neuron> neurons = new List<Neuron>();
        foreach(var layer in layers){
            foreach(int neuron_id in layer){
                List<NeuronInput> neuron_inputs = new List<NeuronInput>();
                foreach(var link in genome.links){
                    if(neuron_id == link.link_id.output_id){
                        NeuronInput n_input = new NeuronInput();
                        n_input.input_id = link.link_id.input_id;
                        n_input.weight = link.weight;
                        neuron_inputs.Add(n_input);
                    }
                }
                NeuronGene neuron_gene_opt = genome.neurons.Find(x => {return x.neuron_id == neuron_id;});
                if(neuron_gene_opt != null){
                    NeuronGene neuron_gene = neuron_gene_opt;
                    Neuron neuron = new Neuron();
                    neuron.bias = neuron_gene.bias;
                    neuron.inputs = neuron_inputs;
                    neuron.activation = neuron_gene.activation;
                    neuron.neuron_id = neuron_id;
                    neurons.Add(neuron);
                }
            }
        }
        return new FeedForwardNeuralNetwork(inputs, outputs, neurons);
    }

    public class NetworkSaver{
        public static bool write_ids(List<int> ids, string identifier, StreamWriter sw){
                sw.Write(identifier);
                for(int i = 0; i < ids.Count; ++i){
                    sw.Write($"{ids[i]} ");
                }
                sw.Write("\n");
                return true;
        }

        public static bool write_neuron(Neuron neuron, StreamWriter sw){
            StringBuilder neuron_string = new StringBuilder($"N {neuron.neuron_id} {neuron.bias} [");
            foreach(var input in neuron.inputs){
                neuron_string.Append($"{input.input_id} {input.weight} | ");
            }
            neuron_string.Append("]\n");
            sw.Write(neuron_string.ToString());
            return true;
        }
        public static bool write_networks(string path, FeedForwardNeuralNetwork network){
            using(StreamWriter sw = new StreamWriter(path)){
                write_ids(network.get_inputs(), "INPUT ", sw);
                write_ids(network.get_outputs(), "OUTPUT ", sw);
                List<Neuron> neurons = network.get_neurons();
                foreach(var neuron in neurons){
                    write_neuron(neuron, sw);
                }
            }
            return true;
        } 
    }
}