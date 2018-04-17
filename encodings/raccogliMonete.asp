%MODELLO INPUT
% tile(X,Y). 			 --> una generica tile percorribile eventualmente
% pellet(X,Y,Tipo).  	 --> una pellet con il tipo associato ("normal"/"special")
% pacman(Tempo,X,Y).	 --> la posizione di pacman in un certo istante di tempo
% closestPellet(X,Y). 	 --> la pellet più vicina a pacman
% next(Direzione, Tempo) -->

#maxint = 380.
int(1..380).

%next(C,T):- pacman(T,X,Y), next2(C), T<7.

%Guess sulle mosse possibili
%next(up,T) | next(down,T) | next(right,T) | next(left,T) :- pacman(T,X,Y),T<7.

%vincolo mossa consentita
:- next(up,T),pacman(T,X,Y),not tile(Z,Y), -(X,1,Z).
:- next(down,T),pacman(T,X,Y),not tile(Z,Y), +(X,1,Z).
:- next(left,T),pacman(T,X,Y),not tile(X,Z), -(Y,1,Z).
:- next(right,T),pacman(T,X,Y),not tile(X,Z), +(Y,1,Z).

%aggiorna posizione pacman
pacman(A,Z,Y):-next(up,T),pacman(T,X,Y), -(X,1,Z), +(T,1,A).
pacman(A,Z,Y):-next(down,T),pacman(T,X,Y), +(X,1,Z), +(T,1,A).
pacman(A,X,Z):-next(left,T),pacman(T,X,Y), -(Y,1,Z), +(T,1,A).
pacman(A,X,Z):-next(right,T),pacman(T,X,Y), +(Y,1,Z), +(T,1,A).

%aggiornamento delle monete raccolte
raccoltapelletnormal(T,X,Y):- pacman(T,X,Y),pellet(X,Y,"normal").
raccoltapelletnormal(A,X,Y):-raccoltapelletnormal(T,X,Y),T<7, +(T,1,A).

%calcolo pellet isolata
pelletIsolata(X1,Y1):- pellet(X1,Y1,"normal"),#count{X,Y : pellet(X,Y,"normal"),
X>=W,-(X1,2,W),int(W),
Y>=W1,-(Y1,2,W1),int(W1),
X<=W2,+(X1,2,W2),int(W2),
Y<=W3,+(Y1,2,W3),int(W3)}=1.

%controllo per preferire le monete isolate.
:~pacman(T,X,Y),not pelletIsolata(X,Y). [1:6]

%paga se non raccoglie monete normali.
:~ pacman(T,X,Y), not pellet(X,Y,"normal"). [1:5]
:~ pacman(T,X,Y), raccoltapelletnormal(A,X,Y),-(T,1,A). [1:5]

%salvo il primo istante in cui raccoglie una specifica pellet
raccoltaIstante(T1,X,Y):- #min{T:pacman(T,X,Y)}=T1,pellet(X,Y,"normal"),pacman(_,X,Y), int(T1).

%pago l'istante in cui ho raccolto una pellet in modo da minimizzarlo
:~raccoltaIstante(T,X,Y). [T:4]

% doveva raccogliere una monete special se intorno ci sono 0 monete normali
doveviRaccogliere(T,X1,Y1):- pacman(T,X1,Y1),#count{X,Y : pellet(X,Y,"normal"),not raccoltapelletnormal(A,X,Y),-(T,1,A),
X>=B,-(X1,1,B),
Y>=W,-(Y1,1,W),
X<=C,+(X1,1,C),
Y<=D,+(Y1,1,D)
}=0, not pellet(X1,Y1,"special").
:~doveviRaccogliere(T,X1,Y1). [1:3]

%non doveva raccogliere una pellet special se intorno c'è almeno una pellet normal
nonDoveviRaccogliere(T,X1,Y1):- pacman(T,X1,Y1),#count{X,Y : pellet(X,Y,"normal"),not raccoltapelletnormal(A,X,Y),-(T,1,A),
X>=B,-(X1,1,B),
Y>=W,-(Y1,1,W),
X<=C,+(X1,1,C),
Y<=D,+(Y1,1,D)}>0, pellet(X1,Y1,"special").
:~nonDoveviRaccogliere(T,X1,Y1).[1:3]

%calcolo la distanza finale dalla pellet più vicina
distanza(D) :- closestPellet(X1,Y1),pacman(7,X2,Y2),X2>=X1,Y2>=Y1,-(X2,X1,D1),-(Y2,Y1,D2),+(D1,D2,D).
distanza(D) :- closestPellet(X1,Y1),pacman(7,X2,Y2),X2>=X1,Y2<=Y1,-(X2,X1,D1),-(Y1,Y2,D2),+(D1,D2,D).
distanza(D) :- closestPellet(X1,Y1),pacman(7,X2,Y2),X2<=X1,Y2>=Y1,-(X1,X2,D1),-(Y2,Y1,D2),+(D1,D2,D).
distanza(D) :- closestPellet(X1,Y1),pacman(7,X2,Y2),X2<=X1,Y2<=Y1,-(X1,X2,D1),-(Y1,Y2,D2),+(D1,D2,D).

%distanza(D) :- closestPellet(X1,Y1),pacman(7,X2,Y2),&distanza(X1,Y1,X2,Y2;D).

% se non raccoglie alcuna pellet, minimizza la distanza dalla più vicina
:~ #count{R,C : raccoltapelletnormal(7,R,C)}=0,distanza(D). [D:2]

% se non raccoglie alcuna pellet, minimizza la distanza dalla più vicina
%:~ #count{R,C : raccoltapelletnormal(7,R,C)}=0, closestPellet(X1,Y1),pacman(7,X2,Y2),&distanza(X2,Y2,X1,Y1;D1). [D1:2]

%controllo per non andare molte volte sulla stessa tile
a(T):-pacman(T,X,Y),pacman(T1,X,Y),T!=T1.
:~a(T). [1:1]
