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

    public SymbolicConstant GetNextMove(InputProgram facts) {
      SymbolicConstant move = new SymbolicConstant();
      string encodingResource = @"encodings\pacman.asp";
      //Debug.Log("DLV Started: " + numberOfSteps++);
      Handler handler = new DesktopHandler(new DLVDesktopService(@"lib\dlv.exe"));
      InputProgram encoding = new ASPInputProgram();
      encoding.AddFilesPath(encodingResource);
      handler.AddProgram(encoding);
      handler.AddProgram(facts);

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

      foreach (GameObject p in pacdots)
        facts.AddProgram("pellet(" + (int)p.transform.position.x + "," + (int)p.transform.position.y + ").");

      facts.AddProgram("ghost(" + (int)blinky.transform.position.x + "," + (int)blinky.transform.position.y + ",blinky).");
      facts.AddProgram("ghost(" + (int)inky.transform.position.x + "," + (int)inky.transform.position.y + ",inky).");
      facts.AddProgram("ghost(" + (int)clyde.transform.position.x + "," + (int)clyde.transform.position.y + ",clyde).");
      facts.AddProgram("ghost(" + (int)pinky.transform.position.x + "," + (int)pinky.transform.position.y + ",pinky).");

      foreach (TileManager.Tile p in Tiles) {
        if (!p.occupied)
          facts.AddProgram("tile(" + p.x + "," + p.y + ").");
      }

      SymbolicConstant move = GetNextMove(facts);
      PreviousMove = move;
      //Debug.Log("CurrentMove: " + move);
      return move;
    }
  }
}
