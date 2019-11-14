﻿/*
 * Copyright (c) 2017 Razeware LLC
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * Notwithstanding the foregoing, you may not use, copy, modify, merge, publish, 
 * distribute, sublicense, create a derivative work, and/or sell copies of the 
 * Software in any work that is designed, intended, or marketed for pedagogical or 
 * instructional purposes related to programming, coding, application development, 
 * or information technology.  Permission for such use, copying, modification,
 * merger, publication, distribution, sublicensing, creation of derivative works, 
 * or sale is expressly withheld.
 *    
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    public List<GameObject> bullets = new List<GameObject>();
    public int shots = 0;
    public int hits = 0;

    [SerializeField]
    private Text hitsText;
    [SerializeField]
    private Text shotsText;
    [SerializeField]
    private GameObject menu;
    [SerializeField]
    private GameObject[] targets;
    private bool isPaused = false;

    private void Awake()
    {
        Pause();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void Pause()
    {
        menu.SetActive(true);
        Cursor.visible = true;
        Time.timeScale = 0;
        isPaused = true;
    }

    public void Unpause()
    {
        menu.SetActive(false);
        Cursor.visible = false;
        Time.timeScale = 1;
        isPaused = false;
    }

    public bool IsGamePaused()
    {
        return isPaused;
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (isPaused)
            {
                Unpause();
            }
            else
            {
                Pause();
            }
        }
    }

    public void SaveGame()
    {
    //1
    //create a Save instance with all data for current session saved into it
    Save save = CreateSaveGameObject();

    //2
    //there will be a file named gamesave.save on computer
    BinaryFormatter bf = new BinaryFormatter();
    FileStream file = File.Create(Application.persistentDataPath +
      "/gamesave.save");
    bf.Serialize(file, save);
    file.Close();

    //3
    //resets game so after player saves, everything is in default state
    hits = 0;
    shots = 0;
    shotsText.text = "Shots: " + shots;
    hitsText.text = "Hits: " + hits;

    ClearRobots();
    ClearBullets();
    Debug.Log("Game Saved");
    }

    public void NewGame()
    {
        hits = 0;
        shots = 0;
        shotsText.text = "Shots: " + shots;
        hitsText.text = "Hits: " + hits;

        ClearRobots();
        ClearBullets();
        RefreshRobots();

        Unpause();
    }

    private void ClearRobots()
    {
        foreach (GameObject target in targets)
        {
            target.GetComponent<Target>().DisableRobot();
        }
    }

    private void ClearBullets()
    {
        foreach (GameObject bullet in bullets)
        {
            Destroy(bullet);
        }
    }

    private void RefreshRobots()
    {
        foreach (GameObject target in targets)
        {
            target.GetComponent<Target>().RefreshTimers();
        }
    }

    public void AddShot()
    {
        shots++;
        shotsText.text = "Shots: " + shots;
    }

    public void AddHit()
    {
        hits++;
        hitsText.text = "Hits: " + hits;
    }

    public void LoadGame()
    {
      //1
      //checks to see that save file exists
      if(File.Exists(Application.persistentDataPath + "/gamesave.save"))
    {
      ClearBullets();
      ClearRobots();
      RefreshRobots();

      //2
      //create a BinaryFormatter
      //creates Save object and closes FileStream
      BinaryFormatter bf = new BinaryFormatter();
      FileStream file = File.Open(Application.persistentDataPath +
        "/gamesave.save", FileMode.Open);
      Save save = (Save)bf.Deserialize(file);
      file.Close();

      //3
      //need to convert save info into game state
      //loops through saved robot position and adds a robot at that position
      for(int i = 0; i<save.livingTargetPositions.Count; i++)
      {
        int position = save.livingTargetPositions[i];
        Target target = targets[position].GetComponent<Target>();
        target.ActivateRobot((RobotTypes)save.livingTargetsTypes[i]);
        target.GetComponent<Target>().ResetDeathTimer();
      }

      //4
      //updates UI to have right hits and shots set
      shotsText.text = "Shots: " + save.shots;
      hitsText.text = "Hits: " + save.hits;
      shots = save.shots;
      hits = save.hits;

      Debug.Log("Game Loaded");

      Unpause();
    }

    else
    {
      Debug.Log("No game saved!");
    }
    }

    public void SaveAsJSON()
    {
    Save save = CreateSaveGameObject();
    string json = JsonUtility.ToJson(save);

    //if you wanted to download a save file from web and load it into game
    //Save save = JsonUtility.FromJson<Save>(json); 

    Debug.Log("Saving as JSON: " + json);
    }

  private Save CreateSaveGameObject()
  {
    Save save = new Save();
    int i = 0;
    foreach(GameObject targetGameObject in targets)
    {
      Target target = targetGameObject.GetComponent<Target>();
      if(target.activeRobot != null)
      {
        save.livingTargetPositions.Add(target.position);

        save.livingTargetsTypes.Add((int)target.activeRobot.GetComponent<Robot>().type);
        i++;
      }
    }

    save.hits = hits;
    save.shots = shots;

    return save;
  }
}
