using it.unical.mat.embasp.@base;
using it.unical.mat.embasp.languages.pddl;
using it.unical.mat.embasp.platforms.desktop;
using it.unical.mat.embasp.specializations.solver_planning_domains.desktop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.EmbASP.Planner {
  class PlanManager {


    private static string tileObjects;

    private static List<TileManager.Tile> upperLeft;
    private static List<TileManager.Tile> upperRight;
    private static List<TileManager.Tile> downLeft;
    private static List<TileManager.Tile> downRight;

    private static List<string> adjacentTiles;

    #region Singleton Instance
    private static PlanManager instance;

    internal static PlanManager Instance {
      get {
        if (instance == null)
          instance = new PlanManager();
        return instance;
      }
    }
    #endregion

    #region Constructors and Initialization
    private PlanManager() {
      Initialize();
    }

    public static void Initialize() {

      StringBuilder tileObjects_sb = new StringBuilder();

      upperLeft = new List<TileManager.Tile>();
      upperRight = new List<TileManager.Tile>();
      downLeft = new List<TileManager.Tile>();
      downRight = new List<TileManager.Tile>();

      TileManager manager = GameObject.Find("Game Manager").GetComponent<TileManager>();
      List<TileManager.Tile> tilesList = manager.tiles;

      var maxX = tilesList.Max(t => t.x)/2;
      var maxY = tilesList.Max(t => t.y)/2;
      //Debug.Log("X: " + maxX + " Y: " + maxY);


      adjacentTiles = new List<string>();

      foreach (TileManager.Tile t in tilesList)
        if (!t.occupied) {
          tileObjects_sb.Append(" ").Append(TileToString(t));
          if (t.x <= maxX && t.y <= maxY)
            downLeft.Add(t);
          else if (t.x <= maxX && t.y > maxY)
            upperLeft.Add(t);
          else if (t.x > maxX && t.y <= maxY)
            downRight.Add(t);
          else if (t.x > maxX && t.y > maxY)
            upperRight.Add(t);
        }

      tileObjects_sb.Append(" ").Append(TileToString(new TileManager.Tile((int)GameObject.Find("clyde").transform.position.x, (int)(GameObject.Find("clyde").transform.position.y))));
      tileObjects_sb.Append(" ").Append(TileToString(new TileManager.Tile((int)GameObject.Find("pinky").transform.position.x, (int)(GameObject.Find("pinky").transform.position.y))));
      tileObjects_sb.Append(" ").Append(TileToString(new TileManager.Tile((int)GameObject.Find("inky").transform.position.x, (int)(GameObject.Find("inky").transform.position.y))));
      tileObjects_sb.Append(" ").Append(TileToString(new TileManager.Tile((int)GameObject.Find("blinky").transform.position.x, (int)(GameObject.Find("blinky").transform.position.y))));


      tileObjects = tileObjects_sb.ToString();

      foreach (TileManager.Tile t in tilesList) {
        TileManager.Tile up = t.up;
        TileManager.Tile down = t.down;
        TileManager.Tile left = t.left;
        TileManager.Tile right = t.right;

        if (up != null)
          adjacentTiles.Add(new StringBuilder().Append(TileToString(t)).Append(" ").Append(TileToString(up)).ToString());
        if (down != null)
          adjacentTiles.Add(new StringBuilder().Append(TileToString(t)).Append(" ").Append(TileToString(down)).ToString());
        if (left != null)
          adjacentTiles.Add(new StringBuilder().Append(TileToString(t)).Append(" ").Append(TileToString(left)).ToString());
        if (right != null)
          adjacentTiles.Add(new StringBuilder().Append(TileToString(t)).Append(" ").Append(TileToString(right)).ToString());
      }
      Plan();
      //foreach (string s in adjacentTiles)
      //  Debug.Log("Adjacent: " + s);
      //Debug.Log("Tiles: " + tileObjects);
    }
    #endregion

    public static void Plan() {

    StringBuilder planBuilder = new StringBuilder();
      planBuilder.Append("(define (problem pacman-problem)\n")
          .Append("(:domain pacman)\n")
          .Append("(:objects ").Append(tileObjects)
          .Append(" - pos)\n")
          .Append("(:init\n")
          .Append("(At ").Append(TileToString(new TileManager.Tile((int)GameObject.Find("pacman").transform.position.x, (int)(GameObject.Find("pacman").transform.position.y)))).Append(")\n");


      foreach (GameObject s in GameObject.FindGameObjectsWithTag("pacdot"))
        planBuilder.Append("(FoodAt ").Append(TileToString(new TileManager.Tile((int)s.transform.position.x, (int)s.transform.position.y))).Append(")");
      planBuilder.Append("\n");

     
      //TODO FIND ENERGIZER
      //foreach (string s in capsule)
      //  planBuilder.Append("(CapsuleAt ").Append(s).Append(")\n");

      planBuilder.Append("(GhostAt ").Append(TileToString(new TileManager.Tile((int)GameObject.Find("clyde").transform.position.x, (int)(GameObject.Find("clyde").transform.position.y)))).Append(")\n");
      planBuilder.Append("(GhostAt ").Append(TileToString(new TileManager.Tile((int)GameObject.Find("pinky").transform.position.x, (int)(GameObject.Find("pinky").transform.position.y)))).Append(")\n");
      planBuilder.Append("(GhostAt ").Append(TileToString(new TileManager.Tile((int)GameObject.Find("inky").transform.position.x, (int)(GameObject.Find("inky").transform.position.y)))).Append(")\n");
      planBuilder.Append("(GhostAt ").Append(TileToString(new TileManager.Tile((int)GameObject.Find("blinky").transform.position.x, (int)(GameObject.Find("blinky").transform.position.y)))).Append(")\n");

      foreach (string s in adjacentTiles)
        planBuilder.Append("(Adjacent ").Append(s).Append(")");
      planBuilder.Append("\n");

      //TODO CHECK POWERED
      //if (pacman.powered)
      //planBuilder.Append("(Powered)\n");

      planBuilder.Append(")\n")
        .Append("(:goal\n")
        .Append("(and\n");


      Debug.Log("SIZE: " + downLeft.Count);
      foreach (GameObject s in GameObject.FindGameObjectsWithTag("pacdot")) {
        //if (upperLeft.Any(pos => pos.x == (int)s.transform.position.x && pos.y == (int)s.transform.position.y))
          planBuilder.Append("(not (FoodAt ").Append(TileToString(new TileManager.Tile((int)s.transform.position.x, (int)s.transform.position.y))).Append("))");
      
      }
        planBuilder.Append("\n");

      planBuilder.Append(")\n)\n)\n");

      Handler handler = new DesktopHandler(new SPDDesktopService());
      InputProgram inputProgramDomain = new PDDLInputProgram(PDDLProgramType.DOMAIN);
      inputProgramDomain.AddFilesPath(@"encodings\pacman-domain.pddl");
      InputProgram inputProgramProblem = new PDDLInputProgram(PDDLProgramType.PROBLEM);
      inputProgramProblem.AddProgram(planBuilder.ToString());
      handler.AddProgram(inputProgramDomain);
      handler.AddProgram(inputProgramProblem);

      Output o = handler.StartSync();
      Debug.Log(o.ErrorsString);
      Debug.Log(o.OutputString);

      if (!(o is Plan))
        return;

      Plan plan = (Plan)o;

      //foreach (it.unical.mat.embasp.languages.pddl.Action action in plan.Actions) {
      //  Debug.Log(action.Name + ",");
      //}

      //System.IO.File.WriteAllText(@"encodings\testpddl.pddl", planBuilder.ToString());

      //Debug.Log(planBuilder.ToString());
    }

    private static string TileToString(TileManager.Tile t) {
      return "p_" + t.x + "_" + t.y;
    }

    private static TileManager.Tile StringToTile(string t) {
      string[] array = t.Split('_');
      return new TileManager.Tile(Int32.Parse(array[1]), Int32.Parse(array[2]));
    }



  }
}
