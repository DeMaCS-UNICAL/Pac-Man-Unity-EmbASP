using System;
using UnityEngine;
using System.Collections;
using EmbASP3._5.it.unical.mat.embasp.languages.asp;
using Assets.Scripts.EmbASP.Utility;
using Assets.Scripts.EmbASP.Planner;
using System.Collections.Generic;
using Assets.Scripts.EmbASP.inputObjects;

public class PlayerController : MonoBehaviour {

  public static int numberOfSteps = 0;
  public float speed = 0.4f;
  Vector2 _dest = Vector2.zero;
  Vector2 _dir = Vector2.zero;
  Vector2 _nextDir = Vector2.zero;


  [Serializable]
  public class PointSprites {
    public GameObject[] pointSprites;
  }

  public PointSprites points;

  public static int killstreak = 0;

  // script handles
  private GameGUINavigation GUINav;
  private GameManager GM;
  private ScoreManager SM;


  private bool _deadPlaying = false;

  // Use this for initialization
  void Start() {
    GM = GameObject.Find("Game Manager").GetComponent<GameManager>();
    SM = GameObject.Find("Game Manager").GetComponent<ScoreManager>();
    GUINav = GameObject.Find("UI Manager").GetComponent<GameGUINavigation>();
    _dest = transform.position;
  }

  bool unaVolta = true;

List<Next> nextMoves = new List<Next>();
  // Update is called once per frame
  void FixedUpdate() {
    switch (GameManager.gameState) {
      case GameManager.GameState.Game:

        if (nextMoves.Count <= 0 || nextMoves == null) {
          nextMoves = EmbASPManager.Instance.ASPMove();
          }
        //PlanManager plan = PlanManager.Instance;

        SymbolicConstant newMove = EmbASPManager.Instance.PreviousMove;
        Vector3 currentPos = new Vector3((int)(EmbASPManager.Instance.Pacman.transform.position.x + 0.499f), (int) (EmbASPManager.Instance.Pacman.transform.position.y + 0.499f));
        
        //Debug.Log(currentPos + " " + previousPos);
        nextMoves.Sort((m1,m2) => m1.getTime().CompareTo(m2.getTime()));

        Next n = nextMoves[0];
        newMove = n.getAction();

        //Debug.Log(currentPos + " " + EmbASPManager.Instance.PreviousPos);
        if (Math.Abs(currentPos.x - EmbASPManager.Instance.PreviousPos.x) + Math.Abs(currentPos.y - EmbASPManager.Instance.PreviousPos.y) >= 1) {
          EmbASPManager.Instance.PreviousPos = currentPos;
          //nextMoves = EmbASPManager.Instance.ASPMove();
        }
   
        ReadInputAndMove(newMove);
        Animate();
        break;

      case GameManager.GameState.Dead:
        if (!_deadPlaying)
          StartCoroutine("PlayDeadAnimation");
        break;
    }


  }

  IEnumerator PlayDeadAnimation() {
    _deadPlaying = true;
    GetComponent<Animator>().SetBool("Die", true);
    yield return new WaitForSeconds(1);
    GetComponent<Animator>().SetBool("Die", false);
    _deadPlaying = false;

    if (GameManager.lives <= 0) {
      UnityEngine.Debug.Log("Treshold for High Score: " + SM.LowestHigh());
      if (GameManager.score >= SM.LowestHigh())
        GUINav.getScoresMenu();
      else
        GUINav.H_ShowGameOverScreen();
    }

    else
      GM.ResetScene();
  }

  void Animate() {
    Vector2 dir = _dest - (Vector2)transform.position;
    GetComponent<Animator>().SetFloat("DirX", dir.x);
    GetComponent<Animator>().SetFloat("DirY", dir.y);
  }

  bool Valid(Vector2 direction) {
    // cast line from 'next to pacman' to pacman
    // not from directly the center of next tile but just a little further from center of next tile
    Vector2 pos = transform.position;
    direction += new Vector2(direction.x * 0.45f, direction.y * 0.45f);
    RaycastHit2D hit = Physics2D.Linecast(pos + direction, pos);
    return hit.collider.name == "pacdot" || (hit.collider == GetComponent<Collider2D>());
  }

  bool ValidPacDot(Vector2 direction) {
    // cast line from 'next to pacman' to pacman
    // not from directly the center of next tile but just a little further from center of next tile
    Vector2 pos = transform.position;
    direction += new Vector2(direction.x * 0.45f, direction.y * 0.45f);
    RaycastHit2D hit = Physics2D.Linecast(pos + direction, pos);
    return hit.collider.name == "pacdot";
  }

  public void ResetDestination() {
    _dest = new Vector2(15f, 11f);
    GetComponent<Animator>().SetFloat("DirX", 1);
    GetComponent<Animator>().SetFloat("DirY", 0);
  }

  void ReadInputAndMove(SymbolicConstant nextStep) {

    // move closer to destination
    Vector2 p = Vector2.MoveTowards(transform.position, _dest, speed);
    GetComponent<Rigidbody2D>().MovePosition(p);

    // get the next direction from keyboard
    //Debug.Log("NEXT: <" + nextStep + ">");
    if (nextStep.Value.Equals("right")) _nextDir = Vector2.right;
    if (nextStep.Value.Equals("left")) _nextDir = -Vector2.right;
    if (nextStep.Value.Equals("up")) _nextDir = Vector2.up;
    if (nextStep.Value.Equals("down")) _nextDir = -Vector2.up;

    // if pacman is in the center of a tile
    if (Vector2.Distance(_dest, transform.position) < 0.00001f) {
      if (Valid(_nextDir)) {
        _dest = (Vector2)transform.position + _nextDir;
        _dir = _nextDir;
      }
      else   // if next direction is not valid
      {
        if (Valid(_dir))  // and the prev. direction is valid
          _dest = (Vector2)transform.position + _dir;   // continue on that direction

        // otherwise, do nothing
      }
    nextMoves.RemoveAt(0);
    }

  }

  public Vector2 getDir() {
    return _dir;
  }

  public void UpdateScore() {
    killstreak++;

    // limit killstreak at 4
    if (killstreak > 4) killstreak = 4;

    Instantiate(points.pointSprites[killstreak - 1], transform.position, Quaternion.identity);
    GameManager.score += (int)Mathf.Pow(2, killstreak) * 100;

  }
}
