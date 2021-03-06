﻿using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreDisplay : MonoBehaviour {
    private string PrefKeyPrefix = "german_valentine_score_";
    private string PrefKeySize = "german_valentine_score_size";
    private TextMeshProUGUI display;

    private List<ScoreData> scores = new List<ScoreData>();
    void Start() {
        display = GetComponent<TextMeshProUGUI>();

        loadScores();
        sortScores();
        fillDisplay();
    }

    private void sortScores() {
        scores.Sort((x, y) => y.score.CompareTo(x.score));
    }

    private void loadScores() {
        if (PlayerPrefs.HasKey(PrefKeySize)) {
            int size = PlayerPrefs.GetInt(PrefKeySize);

            for (int i = 0; i < size; i++) {
                string scoreString = PlayerPrefs.GetString(PrefKeyPrefix + i.ToString());
                scores.Add(new ScoreData(scoreString));
            }
        }
    }
    private void fillDisplay() {
        string displayText = "";
        for (int i = 0; i < scores.Count; i++) {
            displayText += scores[i].getFormatedScore() + " - " + scores[i].name + "\n";
        }
        display.text = displayText;
    }
}
