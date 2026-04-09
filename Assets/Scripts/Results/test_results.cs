using UnityEngine;
using System.IO;
using System;
public class test_results : MonoBehaviour
{   
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void export_all_results()
    {
        export_stroop_results();
        export_emotion_results();
        //arithmetic
        //n-back
    }

    public void export_stroop_results()
    {
        if(generator.results_array.Count != 0)
        {
            string timeStamp = DateTime.Now.ToString("dd-MM-yyyy_HH_mm");
            string filePath = Path.Combine(Application.persistentDataPath, $"StroopResults_{timeStamp}.csv");
            
            try
            {
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
                
                Debug.Log($"Stroop Results CSV file saved to: {filePath}");
            }
            
            catch (IOException e)
            {
                Debug.LogError($"FILE LOCKED: Could not write to {filePath}. Is it open in Excel? {e.Message}");
            }
            catch (Exception e)
            {
                Debug.LogError($"General Error writing Stroop CSV: {e.Message}");
            }
        }

        else if(generator.results_array.Count == 0)
        {
            Debug.Log("Stroop Results are empty. Stroop Results CSV file not saved!");
        }
    }

    public void export_emotion_results()
    {
        if(generator_emotion.results_array.Count != 0)
        {
            string timeStamp = DateTime.Now.ToString("dd-MM-yyyy_HH_mm");
            string filePath = Path.Combine(Application.persistentDataPath, $"EmotionResults_{timeStamp}.csv");
            
            try
            {
                // Create the file or overwrite it if it exists
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    // Write the header
                    writer.WriteLine("Index,ReactionTime,Correctness,Word,IntendedEmotion");
                    
                    for(int i = 0; i < generator_emotion.results_array.Count; i++)
                    {   
                        string index = i.ToString() ;
                        string reactionTime = generator_emotion.results_array[i].reaction_time.ToString();
                        string correctness = decide_correctness(generator_emotion.results_array[i].correctness);
                        string word = generator_emotion.results_array[i].word.ToLower();
                        string emotion = decide_emotion(generator_emotion.results_array[i].emotion);
                        

                        writer.WriteLine($"{index},{reactionTime},{correctness},{word},{emotion}");
                    }
                }
                
                Debug.Log($"Emotion Results CSV file saved to: {filePath}");
            }
            catch (IOException e)
            {
                Debug.LogError($"FILE LOCKED: Could not write to {filePath}. Is it open in Excel? {e.Message}");
            }
            catch (Exception e)
            {
                Debug.LogError($"General Error writing Emotion CSV: {e.Message}");
            }
        }

        else if(generator_emotion.results_array.Count == 0)
        {
            Debug.Log("Emotion Results are empty. Emotion Results CSV file not saved!");
        }

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

    string decide_emotion(int emotion_num)
    {
        if(emotion_num == 0)
        {
            return "bad";
        }

        else if(emotion_num == 1)
        {
            return "good";
        }

        else
        {
            return "Error!!!!";
        }
    }
    
}