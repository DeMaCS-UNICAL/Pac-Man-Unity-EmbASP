%% INPUT %%
% next(left) | next(right) | next(up) | next(down).
% pellet(X,Y).
% pacman(X,Y).
% ghost(X,Y, name).
% tile(X,Y).
% closestPellet(X,Y).
% distanceClosestPellet(X,Y).
% previous_action(X). %% left, right, up, down


number(1..10).

:- not #count{X: next(X)} = 1.

nextCell(X,Y) :- pacman(Px, Y), next(right), X=Px+1, tile(X,Y).
nextCell(X,Y) :- pacman(Px, Y), next(left), X=Px-1, tile(X,Y).
nextCell(X,Y) :- pacman(X, Py), next(up), Y=Py+1, tile(X,Y).
nextCell(X,Y) :- pacman(X, Py), next(down), Y=Py-1, tile(X,Y).

empty(X,Y) :- tile(X,Y), not pellet(X,Y).

nextPacmanPosition(X,Y) :- next(up), pacman(X,Py), +(Py,1,Y), tile(X,Y).
nextPacmanPosition(X,Y) :- next(down), pacman(X,Py), -(Py,1,Y), tile(X,Y).
nextPacmanPosition(X,Y) :- next(right), pacman(Px,Y), +(Px,1,X), tile(X,Y).
nextPacmanPosition(X,Y) :- next(left), pacman(Px,Y), -(Px,1,X), tile(X,Y).

distancePacmanNextGhost(D, G) :- nextPacmanPosition(Xp, Yp), ghost(Xg, Yg, G),
%                                 #absdiff(Xp, Xg, Xd), number(Xd),
%                                 #absdiff(Yp, Yg, Yd), number(Yd), D = Xd + Yd.
                                    min_distance(Xp,Yp,Xg,Yg,D).
minDistancePacmanNextGhost(MD) :- #min{D : distancePacmanNextGhost(D, _)} = MD, number(MD).

% distanceFromGhost(X, Y, 0) :- ghost(X, Y, _).
% distanceFromGhost(Xa, Ya, Dp1) :- distanceFromGhost(X, Y, D), adjacent(X, Y, Xa, Ya), number(Da), D = Dp1 - 1, number(Dp1).%, not distanceFromGhost(Xa, Ya, Da), Da < D.

% distancePacmanNextGhost(D, a) :- nextPacmanPosition(X, Y), distanceFromGhost(X, Y, D).

% adjacent(R1,C1,R2,C2) :- tile(R1,C1), tile(R2,C2), step(DR,DC), R2 = R1 + DR, C2 = C1 + DC.
% adjacent(R1,C1,R2,C2) :- tile(R1,C1), tile(R2,C2), stepN(DR,DC), R2 = R1 - DR, C2 = C1 - DC.
% step(0,1).
% stepN(0,1).
% step(1,0).
% stepN(1,0).

:~ minDistancePacmanNextGhost(MD), Min=10-MD, not powerup. [Min:4]
:~ minDistancePacmanNextGhost(MD), powerup. [MD:4]
%:~ minDistancePacmanNextGhost(MD), number(N), Min=N-MD. [Min:4]

:~ nextCell(X,Y), empty(X,Y). [1:3]
:~ nextPacmanPosition(X,Y), closestPellet(X,Y), distanceClosestPellet(D). [D:2]
:~ previous_action(X), next(Y), X!=Y. [1:1]
