 using System.Collections.Generic;
 using System.Collections;
 using System;
 using UnityEngine;

 public class QuestionInterpreter : MonoBehaviour
 {
     // Questionnaire
     /*  | - Data string split
         Pertanyaan | Opsi 1 | Opsi 2 | Opsi 3 | Opsi 4 | Jawaban (0, 4)

         ; - Split question
         Pertanyaan | Opsi 1 | Opsi 2 | Opsi 3 | Opsi 4 | Jawaban (0, 4);
         Pertanyaan | Opsi 1 | Opsi 2 | Opsi 3 | Opsi 4 | Jawaban (0, 4)
      */

     public TextAsset [] w1_questionnaire; // World 1
     TextAsset questionnaire;
     private void LoadQuestion (int World, int Level)
     {
         switch (World)
         {
             case 0: // World 0
                 questionnaire = w1_questionnaire [Level];
                 break;
         }

         InterpetQuestion (questionnaire);
     }

     [TextArea]
     public string readQuestionnaire;
     public string [] questionDataPool;
     private void InterpetQuestion (TextAsset Questionnaire)
     {
         readQuestionnaire = Questionnaire.text;

         questionDataPool = readQuestionnaire.Split (new char [] { ';' }, StringSplitOptions.None);
     }

     protected internal String GetQuestionData (int World, int Level, int QuestionNumber)
     {
         LoadQuestion (World, Level);
         return questionDataPool [QuestionNumber];
     }
 }