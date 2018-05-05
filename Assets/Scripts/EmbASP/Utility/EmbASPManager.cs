using Assets.Scripts.EmbASP.inputObjects;
using EmbASP3._5.it.unical.mat.embasp.languages.asp;
using it.unical.mat.embasp.@base;
using it.unical.mat.embasp.languages.asp;
using it.unical.mat.embasp.platforms.desktop;
using it.unical.mat.embasp.specializations.dlv.desktop;
using it.unical.mat.embasp.specializations.web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.EmbASP.Utility {
  class EmbASPManager {

    private static EmbASPManager instance;
    private static List<Distance>[,] distances_10;
    private static List<Distance>[,] distances_5;
    #region Parameters, Getters and Setters
    internal GameObject Pacman { get; set; }
    private GameObject blinky;
    private GameObject pinky;
    private GameObject inky;
    private GameObject clyde;
    internal SymbolicConstant PreviousMove { get; set; }
    internal Vector3 PreviousPos { get; set; }
    internal List<TileManager.Tile> Tiles { get; set; }
    internal TileManager manager;
    #endregion

    #region Constructors
    private EmbASPManager() {
      distances_5 = new List<Distance>[28, 32];
      distances_10 = new List<Distance>[28, 32];

      for (int i = 0; i < 28; i++)
        for (int j = 0; j < 32; j++) {
          distances_5[i, j] = new List<Distance>();
          distances_10[i, j] = new List<Distance>();
        }

      GenerateCharacters();

      ASPMapper.Instance.RegisterClass(typeof(Next));
      ASPMapper.Instance.RegisterClass(typeof(Distance));


      GenerateFacts(10);
      GenerateFacts(5);
      //string encodingResource = @"encodings\min_distances_10.asp";
      ////Debug.Log("DLV Started: " + numberOfSteps++);
      //Handler handler = new DesktopHandler(new DLVDesktopService(@"lib\dlv.exe"));
      //InputProgram encoding = new ASPInputProgram();
      //encoding.AddFilesPath(encodingResource);
      //handler.AddProgram(encoding);

      //Output o = handler.StartSync();
      //AnswerSets answers = (AnswerSets)o;

      ////Debug.Log("Answers: " + o.OutputString);
      //AnswerSet a = answers.Answersets[0];

      //foreach (object obj in a.Atoms) {
      //  //Debug.Log(obj.ToString());
      //  if (obj is Distance) {
      //    Distance d = (Distance)obj;
      //    distances_5[d.getX1(), d.getY1()].Add(d);
      //    //move = nextAction.getAction();
      //    //Debug.Log("Next Action: " + move);
      //  }
      //}

    }
    #endregion

    #region Singleton Instance
    internal static EmbASPManager Instance {
      get {
        if (instance == null)
          instance = new EmbASPManager();
        return instance;
      }
    }
    #endregion

    public void GenerateCharacters() {
      Tiles = new List<TileManager.Tile>();
      manager = GameObject.Find("Game Manager").GetComponent<TileManager>();
      Tiles = manager.tiles;

      Pacman = GameObject.Find("pacman");
      clyde = GameObject.Find("clyde");
      pinky = GameObject.Find("pinky");
      inky = GameObject.Find("inky");
      blinky = GameObject.Find("blinky");
      PreviousPos = new Vector3(0, 0);

    }

    public SymbolicConstant GetNextMove(InputProgram facts) {
      SymbolicConstant move = new SymbolicConstant();
      string encodingResource = @".\encodings\pacman.asp";
      //string encodingResource2 = @"encodings\min_distances_5.asp";
      //Debug.Log("DLV Started: " + numberOfSteps++);
      Handler handler = new DesktopHandler(new DLVDesktopService(@".\lib\dlv.exe"));
      InputProgram encoding = new ASPInputProgram();
      encoding.AddFilesPath(encodingResource);
      //InputProgram encoding2 = new ASPInputProgram();
      //encoding.AddFilesPath(encodingResource2);
      handler.AddProgram(encoding);
      //handler.AddProgram(encoding2);
      handler.AddProgram(facts);
      handler.AddOption(new OptionDescriptor("-filter=next"));
      Output o = handler.StartSync();
      AnswerSets answers = (AnswerSets)o;

      System.Random r = new System.Random();
      int answer = r.Next(answers.Answersets.Count);
      AnswerSet a = answers.Answersets[answer];
      foreach (object obj in a.Atoms) {
        //Debug.Log(obj.ToString());
        if (obj is Next) {
          Next nextAction = (Next)obj;
          
          move = nextAction.getAction();
          return move;
          //Debug.Log("Next Action: " + move);
        }
      }
      return move;
    }

    public SymbolicConstant ASPMove() {
      //PRENDE LA POSIZIONE DEL PACMAN
      Vector3 currentPos = new Vector3(Pacman.transform.position.x + 0.499f, Pacman.transform.position.y + 0.499f);
      var currentTile = Tiles[manager.Index((int)currentPos.x, (int)currentPos.y)];
      //Debug.Log("PACMAN POS --> X:" + currentPos.x + " Y: " + currentPos.y + "\n\nTile --> X: " + currentTile.x + " Y: " + currentTile.y);
      
      //FIND ADJACENT TILES TO THE CURRENT ONE
      TileManager.Tile down = currentTile.down;
      TileManager.Tile up = currentTile.up;
      TileManager.Tile left = currentTile.left;
      TileManager.Tile right = currentTile.right;

      InputProgram facts = new ASPInputProgram();
      StringBuilder disjunction = new StringBuilder();

      Pacman myPacman = new Pacman((int)currentPos.x, (int)currentPos.y);
      if (down != null) {
        if (disjunction.Length > 0)
          disjunction.Append("|");
        disjunction.Append("next(down)");
      }

      if (left != null) {
        if (disjunction.Length > 0)
          disjunction.Append("|");
        disjunction.Append("next(left)");
      }

      if (up != null) {
        if (disjunction.Length > 0)
          disjunction.Append("|");
        disjunction.Append("next(up)");
      }

      if (right != null) {
        if (disjunction.Length > 0)
          disjunction.Append("|");
        disjunction.Append("next(right)");
      }
      disjunction.Append(".");
      facts.AddProgram(disjunction.ToString());
      if (PreviousMove != null)
        facts.AddProgram("previous_action(" + PreviousMove.Value + ").");
      facts.AddObjectInput(myPacman);


      GameObject[] pacdots = GameObject.FindGameObjectsWithTag("pacdot");
      //Debug.Log("SIZE DOT: " + pacdots.Length);

      int count = 0;
      foreach (TileManager.Tile t in Tiles)
        if (!t.occupied)
          count++;
      //Debug.Log("SIZE TILE: " + count);
      //GameObject[] energizer; // sono pacdot anche loro

      //energizer = GameObject.FindGameObjectsWithTag("energizer");

      //CHECK THE CONTENT OF A TILE
      //Debug.Log("PacDot[0].pos = (" + pacdots[0].transform.position.x + "," + pacdots[0].transform.position.y + ")");

      //for (int i = 0; i < 28; i++)
      //  for (int j = 0; j < 32; j++)
      //    foreach (Distance d in distances[i,j])
      //      facts.AddObjectInput(d);


      for (int i = -1; i <= 1; i++) {
        if (myPacman.getX() + i > 0 && myPacman.getX() + i < 28)
          if (GameManager.scared)
            foreach (Distance d in distances_10[myPacman.getX() + i, myPacman.getY()])
              facts.AddObjectInput(d);
          else
            foreach (Distance d in distances_5[myPacman.getX() + i, myPacman.getY()])
              facts.AddObjectInput(d);
        if (myPacman.getY() + i > 0 && myPacman.getY() + i < 32) 
          if (GameManager.scared)
            foreach (Distance d in distances_10[myPacman.getX(), myPacman.getY() + i])
              facts.AddObjectInput(d);
          else
            foreach (Distance d in distances_5[myPacman.getX(), myPacman.getY() + i])
              facts.AddObjectInput(d);
      }

      if (GameManager.scared)
        facts.AddProgram("powerup.");


      foreach (GameObject p in pacdots)
        facts.AddProgram("pellet(" + (int)p.transform.position.x + "," + (int)p.transform.position.y + ").");

      facts.AddProgram("ghost(" + (int)blinky.transform.position.x + "," + (int)blinky.transform.position.y + ",blinky).");
      facts.AddProgram("ghost(" + (int)inky.transform.position.x + "," + (int)inky.transform.position.y + ",inky).");
      facts.AddProgram("ghost(" + (int)clyde.transform.position.x + "," + (int)clyde.transform.position.y + ",clyde).");
      facts.AddProgram("ghost(" + (int)pinky.transform.position.x + "," + (int)pinky.transform.position.y + ",pinky).");


      TileManager.Tile pacmanTile = new TileManager.Tile((int)Pacman.transform.position.x, (int)Pacman.transform.position.y);

      TileManager.Tile first_min = new TileManager.Tile((int)pacdots[0].transform.position.x, (int)pacdots[0].transform.position.y);
      var minDistance = 10E6;// manager.distance(pacmanTile, first_min);



      foreach (GameObject p in pacdots) {
        TileManager.Tile pacdotsTile = new TileManager.Tile((int)p.transform.position.x, (int)p.transform.position.y);
        var myDistance = manager.distance(pacmanTile, pacdotsTile);
        if (myDistance < minDistance) {
          minDistance = myDistance;
          first_min = pacdotsTile;
        }
      }

      facts.AddProgram("closestPellet(" + first_min.x + "," + first_min.y + ").");
      facts.AddProgram("distanceClosestPellet(" + (int)minDistance + ").");


      foreach (TileManager.Tile p in Tiles) {
        if (!p.occupied)
          facts.AddProgram("tile(" + p.x + "," + p.y + ").");
      }

      SymbolicConstant move = GetNextMove(facts);
      PreviousMove = move;
      //Debug.Log("CurrentMove: " + move);
      return move;
    }

    private void GenerateFacts(int dimension) {
      string encodingResource = @".\encodings\min_distances_" + dimension + ".asp";
      //Debug.Log("DLV Started: " + numberOfSteps++);
      Handler handler = new DesktopHandler(new DLVDesktopService(@".\lib\dlv.exe"));
      InputProgram encoding = new ASPInputProgram();
      encoding.AddFilesPath(encodingResource);
      handler.AddProgram(encoding);

      Output o = handler.StartSync();
      AnswerSets answers = (AnswerSets)o;

      //Debug.Log("Answers: " + o.OutputString);
      AnswerSet a = answers.Answersets[0];

      foreach (object obj in a.Atoms) {
        //Debug.Log(obj.ToString());
        if (obj is Distance) {
          Distance d = (Distance)obj;
          if (dimension == 10)
            distances_10[d.getX1(), d.getY1()].Add(d);
          else if (dimension == 5)
            distances_5[d.getX1(), d.getY1()].Add(d);

          //move = nextAction.getAction();
          //Debug.Log("Next Action: " + move);
        }
      }
    }

  }
}
