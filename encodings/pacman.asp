%action(left).action(right).action(up).action(down).

% 1 -> destra, 2 -> sinistra, 3 -> sopra, 4 -> sotto
%next(X) | not_next(X) :- action(X).

% next(left) | left(right). % supponendo che left e right siano le 2 possibili azioni
:- not #count{X: next(X)} = 1.

nextCell(X,Y) :- pacman(Px, Y), next(right), X=Px+1, tile(X,Y).
nextCell(X,Y) :- pacman(Px, Y), next(left), X=Px-1, tile(X,Y).
nextCell(X,Y) :- pacman(X, Py), next(up), Y=Py+1, tile(X,Y).
nextCell(X,Y) :- pacman(X, Py), next(down), Y=Py-1, tile(X,Y).

empty(X,Y) :- tile(X,Y), not pellet(X,Y).

:~ nextCell(X,Y), empty(X,Y). [1:3]
:~ previous_action(X), next(Y), X!=Y. [1:2]

distance(D) :- ghost(Gx,Gy,_), pacman(Px,Py), Dx=Gx-Px, Dy=Gy-Py, Gx>=Px, Gy>=Py, D=Dx+Dy.
distance(D) :- ghost(Gx,Gy,_), pacman(Px,Py), Dx=Px-Gx, Dy=Py-Gy, Px>Gx, Py>Gy, D=Dx+Dy.
distance(D) :- ghost(Gx,Gy,_), pacman(Px,Py), Dx=Gx-Px, Dy=Py-Gy, Px>=Gx, Gy>=Py, D=Dx+Dy.
distance(D) :- ghost(Gx,Gy,_), pacman(Px,Py), Dx=Px-Gx, Dy=Gy-Py, Px>=Gx, Gy>=Py, D=Dx+Dy.
