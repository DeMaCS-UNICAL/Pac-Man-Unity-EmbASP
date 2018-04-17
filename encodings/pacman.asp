%% INPUT %%
% next(left) | next(right) | next(up) | next(down).
% pellet(X,Y).
% pacman(X,Y).
% ghost(X,Y, name).
% tile(X,Y).
% closestPellet(X,Y).
% distanceClosestPellet(X,Y).
% previous_action(X). %% left, right, up, down

:- not #count{X: next(X)} = 1.
nextCell(X,Y) :- pacman(_,Px,Y), next(right), X=Px+1, tile(X,Y).
nextCell(X,Y) :- pacman(_,Px,Y), next(left), X=Px-1, tile(X,Y).
nextCell(X,Y) :- pacman(_,X,Py), next(up), Y=Py+1, tile(X,Y).
nextCell(X,Y) :- pacman(_,X,Py), next(down), Y=Py-1, tile(X,Y).

empty(X,Y) :- tile(X,Y), not pellet(X,Y,"normal").

:~ nextCell(X,Y), empty(X,Y). [1:2]
:~ previous_action(X), next(Y), X!=Y. [1:1]

nextPacmanPosition(X,Y) :- next(up), pacman(X,Py), +(Py,1,Y).
nextPacmanPosition(X,Y) :- next(down), pacman(X,Py), -(Py,1,Y).
nextPacmanPosition(X,Y) :- next(right), pacman(Px,Y), +(Px,1,X).
nextPacmanPosition(X,Y) :- next(left), pacman(Px,Y), -(Px,1,X).

:~ nextPacmanPosition(X,Y), closestPellet(X,Y), distanceClosestPellet(D). [D:3]
