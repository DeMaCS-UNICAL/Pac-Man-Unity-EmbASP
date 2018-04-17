using Assets.Scripts.EmbASP.inputObjects;
using EmbASP3._5.it.unical.mat.embasp.languages.asp;
using it.unical.mat.embasp.@base;
using it.unical.mat.embasp.languages.asp;
using it.unical.mat.embasp.platforms.desktop;
using it.unical.mat.embasp.specializations.dlv.desktop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.EmbASP.Utility {
  class EmbASPManager {

    private static EmbASPManager instance;

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
      Tiles = new List<TileManager.Tile>();
      manager = GameObject.Find("Game Manager").GetComponent<TileManager>();
      Tiles = manager.tiles;

      Pacman = GameObject.Find("pacman");
      clyde = GameObject.Find("clyde");
      pinky = GameObject.Find("pinky");
      inky = GameObject.Find("inky");
      blinky = GameObject.Find("blinky");
      PreviousPos = new Vector3(0, 0);

      ASPMapper.Instance.RegisterClass(typeof(Next));
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


    public List<Next> GetNextMove(InputProgram facts) {
      SymbolicConstant move = new SymbolicConstant();
      string encodingResource = @"encodings\raccogliMonete.asp";
      //Debug.Log("DLV Started: " + numberOfSteps++);
      Handler handler = new DesktopHandler(new DLVDesktopService(@"lib\dlv.exe"));
      InputProgram encoding = new ASPInputProgram();
      encoding.AddFilesPath(encodingResource);
      handler.AddProgram(encoding);
      handler.AddProgram(facts);

      Output o = handler.StartSync();

      //Debug.Log("OUT: "+ o.OutputString);
      //Debug.Log("ERR:" + o.ErrorsString);

      AnswerSets answers = (AnswerSets)o;

      Debug.Log("OUT: " + o.OutputString);
      Debug.Log("ERR: " + o.ErrorsString);
      List<Next> nextMoves = new List<Next>();

      System.Random r = new System.Random();
      int answer = r.Next(answers.Answersets.Count);
      AnswerSet a = answers.Answersets[answer];
      foreach (object obj in a.Atoms) {
        if (obj is Next) {
          Debug.Log(obj.ToString());
          Next nextAction = (Next)obj;
          
          move = nextAction.getAction();
          nextMoves.Add(nextAction);
          //Debug.Log("Next Action: " + move);
        }
      }
      return nextMoves;
    }

    public List<Next> ASPMove() {
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


      var Px = (int)currentPos.y - 31;
      var Py = (int)currentPos.x - 28;

      if (Px < 0) Px *= -1;
      if (Py < 0) Py *= -1;

      Pacman myPacman = new Pacman(0, Px, Py);


      //next(up, T) | next(down, T) | next(right, T) | next(left, T) :-pacman(T, X, Y),T < 7.
      if (down != null) {
        if (disjunction.Length > 0)
          disjunction.Append("|");
        //disjunction.Append("next2(down)");
        disjunction.Append("next(down, T)");
      }

      if (left != null) {
        if (disjunction.Length > 0)
          disjunction.Append("|");
        //disjunction.Append("next2(left)");
        disjunction.Append("next(left, T)");
      }

      if (up != null) {
        if (disjunction.Length > 0)
          disjunction.Append("|");
        //disjunction.Append("next2(up)");
        disjunction.Append("next(up, T)");
      }

      if (right != null) {
        if (disjunction.Length > 0)
          disjunction.Append("|");
        //disjunction.Append("next2(right)");
        disjunction.Append("next(right, T)");
      }

      disjunction.Append(" :- pacman(T,X,Y), T<7");
      disjunction.Append(".");
      facts.AddProgram(disjunction.ToString());
      if (PreviousMove != null)
        facts.AddProgram("previous_action(" + PreviousMove.Value + ").");
      facts.AddObjectInput(myPacman);


      GameObject[] pacdots = GameObject.FindGameObjectsWithTag("pacdot");
      //Debug.Log("SIZE DOT: " + pacdots.Length);

      //int count = 0;
      //foreach (TileManager.Tile t in Tiles)
      //  if (!t.occupied)
      //    count++;
      //Debug.Log("SIZE TILE: " + count);
      //GameObject[] energizer; // sono pacdot anche loro

      //energizer = GameObject.FindGameObjectsWithTag("energizer");

      //CHECK THE CONTENT OF A TILE
      //Debug.Log("PacDot[0].pos = (" + pacdots[0].transform.position.x + "," + pacdots[0].transform.position.y + ")");


      TileManager.Tile pacmanTile = new TileManager.Tile((int)Pacman.transform.position.x, (int)Pacman.transform.position.y);
      
      TileManager.Tile first_min = new TileManager.Tile((int)pacdots[0].transform.position.x, (int)pacdots[0].transform.position.y);
      var minDistance = manager.distance(pacmanTile, first_min);



      foreach (GameObject p in pacdots) {
        TileManager.Tile pacdotsTile = new TileManager.Tile((int)p.transform.position.x, (int)p.transform.position.y);
        var myDistance = manager.distance(pacmanTile, pacdotsTile);
        if (myDistance < minDistance) {
          minDistance = myDistance;
          first_min = pacdotsTile;
        }
      }
      //foreach (TileManager.Tile t in Tiles) {
      //  bool found = false;
      //  foreach (GameObject p in pacdots) {
      //    if (t.x == (int)p.transform.position.x && t.y == (int)p.transform.position.y)
      //      found = true;
      //  }
      //  if (!found)
      //    Debug.Log("TILE: " + t.x + ", " + t.y);
      //}

      //pacdots.OrderBy(p => (int)p.transform.position.y);
      using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(@"encodings\check.txt", true)) {

        foreach (TileManager.Tile t in Tiles)
          if (!t.occupied)
            file.WriteLine("Tile(" + t.x + ", " + t.y + ")");

        foreach (GameObject t in pacdots.OrderBy(p => (int)p.transform.position.x))
          file.WriteLine("Pellet(" + (int)t.transform.position.x + ", " + (int)t.transform.position.y + ")");
      }


      var Cx = ((int)first_min.y - 31);
      var Cy = ((int)first_min.x - 28);

      if (Cx < 0) Cx = Cx * (-1);
      if (Cy < 0) Cy = Cy * (-1);
      //Debug.Log("PELLET: X: " + first_min.x + " Y: " + first_min.y);
      //Debug.Log("PELLET DOPO: X: " + Cx + " Y: " + Cy);
      facts.AddProgram("closestPellet(" + Cx + "," + Cy + ").");
      facts.AddProgram("distanceClosestPellet(" + (int)minDistance  + ").");

      int energizer = 0;
      foreach (GameObject p in pacdots) {
        var x = ((int)p.transform.position.y - 31);
        var y = ((int)p.transform.position.x - 28);

        if (x < 0) x = x * (-1);
        if (y < 0) y = y * (-1);

        if ((int)p.transform.position.x == 15) {
          Debug.Log("PELLET: X: " + (int)p.transform.position.x + " Y: " + (int)p.transform.position.y);
          Debug.Log("PELLET DOPO: X: " + x + " Y: " + y);
        }

        if (energizer++ < 4)
          facts.AddProgram("pellet(" + x + "," + y + ",\"special\").");
        else
          facts.AddProgram("pellet(" + x + "," + y + ",\"normal\").");

      }


      facts.AddProgram("ghost(" + (int)blinky.transform.position.x + "," + (int)blinky.transform.position.y + ",blinky).");
      facts.AddProgram("ghost(" + (int)inky.transform.position.x + "," + (int)inky.transform.position.y + ",inky).");
      facts.AddProgram("ghost(" + (int)clyde.transform.position.x + "," + (int)clyde.transform.position.y + ",clyde).");
      facts.AddProgram("ghost(" + (int)pinky.transform.position.x + "," + (int)pinky.transform.position.y + ",pinky).");

      foreach (TileManager.Tile p in Tiles) {
        if (!p.occupied) {
          var x = (p.y - 31);
          var y = (p.x - 28);

          if (x < 0) x = x * (-1);
          if (y < 0) y = y * (-1);

          facts.AddProgram("tile(" + x + "," + y + ").");
        }
      }

      List<Next> move = GetNextMove(facts);
      PreviousMove = move[0].getAction();
      //Debug.Log("CurrentMove: " + move);
      return move;
    }
  }
}
