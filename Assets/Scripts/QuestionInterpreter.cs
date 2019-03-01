using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionInterpreter : MonoBehaviour
{
    // Questionnaire
    /*  | - Data stribng split
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
            case 0: // World 1
                questionnaire = w1_questionnaire [Level];
                break;
        }

        InterpetQuestion (questionnaire);
    }

    private void InterpetQuestion (TextAsset Questionnaire)
    {

    }
}