using UnityEngine;
using System.IO;
public class test_results : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void export_stroop_results()
    {

        string filePath = Path.Combine(Application.persistentDataPath, "StroopResults.csv");
        // Create the file or overwrite it if it exists
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            // Write the header
            writer.WriteLine("Index,ReactionTime,Correctness,ColorWord,Color");
            
            for(int i = 0; i < generator.results_array.Count; i++)
            {   
                string index = i.ToString() ;
                string reactionTime = generator.results_array[i].reaction_time.ToString();
                string correctness = decide_correctness(generator.results_array[i].correctness);
                string color_word = generator.results_array[i].color_word.ToLower();
                string color = decide_color(generator.results_array[i].color_index);

                writer.WriteLine($"{index},{reactionTime},{correctness},{color_word},{color}");
            }
        }
        
        Debug.Log($"CSV file saved to: {filePath}");

    }

    string decide_color(int index)
    {   
        
        if(index == 0)
        {
            return "red";
        }

        else if(index == 1)
        {
            return "blue";
        }

        else if(index == 2)
        {
            return "green";
        }

        else if(index == 3)
        {
            return "yellow";
        }

        else if(index == 4)
        {
            return "purple";
        }

        else if(index == 5)
        {
            return "orange";
        }
        else
        {
            return "error";
        }
    }

    string decide_correctness(bool correctness)
    {
        if (correctness == true)
        {
            return "Correct";
        }

        else
        {
            return "Incorrect";
        }
    }
    
}
