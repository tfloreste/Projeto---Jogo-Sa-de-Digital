using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Quiz Question", menuName = "Quiz Question")]
public class QuizQuestion : ScriptableObject
{
    public string question;
    public string[] alternatives;
    public int correctAnswerIndex;


}
