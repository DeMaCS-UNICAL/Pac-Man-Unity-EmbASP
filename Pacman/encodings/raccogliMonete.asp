%MODELLO INPUT
%casella(X,Y). 			 --> una generica casella percorribile eventualmente
%moneta(X,Y,Tipo).  	 --> una moneta con il tipo associato ("normale"/"speciale")
%pacmanDLV(Tempo,X,Y).	 --> la posizione di pacman in un certo istante di tempo
%monetaPiuVicina(X,Y). 	 --> la moneta più vicina a pacman

#maxint = 200.
int(1..200).

%Guess sulle mosse possibili
%up(T)|down(T)|right(T)|left(T):-pacmanDLV(T,X,Y),T<7.
next(up) :- up(0).
next(down) :- down(0).
next(left) :- left(0).
next(right) :- right(0).

%vincolo mossa consentita
:- up(T),pacmanDLV(T,X,Y),not casella(Z,Y), -(X,1,Z).
:- down(T),pacmanDLV(T,X,Y),not casella(Z,Y), +(X,1,Z).
:- left(T),pacmanDLV(T,X,Y),not casella(X,Z), -(Y,1,Z).
:- right(T),pacmanDLV(T,X,Y),not casella(X,Z), +(Y,1,Z).

%aggiorna posizione pacman
pacmanDLV(A,Z,Y):-up(T),pacmanDLV(T,X,Y), -(X,1,Z), +(T,1,A).
pacmanDLV(A,Z,Y):-down(T),pacmanDLV(T,X,Y), +(X,1,Z), +(T,1,A).
pacmanDLV(A,X,Z):-left(T),pacmanDLV(T,X,Y), -(Y,1,Z), +(T,1,A).
pacmanDLV(A,X,Z):-right(T),pacmanDLV(T,X,Y), +(Y,1,Z), +(T,1,A).

%aggiornamento delle monete raccolte
raccoltaMonetaNormale(T,X,Y):- pacmanDLV(T,X,Y),moneta(X,Y,"normale").
raccoltaMonetaNormale(A,X,Y):-raccoltaMonetaNormale(T,X,Y),T<7, +(T,1,A).

%calcolo moneta isolata
monetaIsolata(X1,Y1):- moneta(X1,Y1,"normale"),#count{X,Y : moneta(X,Y,"normale"),
X>=W,-(X1,2,W),int(W),
Y>=W1,-(Y1,2,W1),int(W1),
X<=W2,+(X1,2,W2),int(W2),
Y<=W3,+(Y1,2,W3),int(W3)}=1.

%controllo per preferire le monete isolate.
:~pacmanDLV(T,X,Y),not monetaIsolata(X,Y). [1:6]

%paga se non raccoglie monete normali.
:~ pacmanDLV(T,X,Y), not moneta(X,Y,"normale"). [1:5]
:~ pacmanDLV(T,X,Y), raccoltaMonetaNormale(A,X,Y),-(T,1,A). [1:5]

%salvo il primo istante in cui raccoglie una specifica moneta
raccoltaIstante(T1,X,Y):- #min{T:pacmanDLV(T,X,Y)}=T1,moneta(X,Y,"normale"),pacmanDLV(_,X,Y), int(T1).

%pago l'istante in cui ho raccolto una moneta in modo da minimizzarlo
:~raccoltaIstante(T,X,Y). [T:4]

% doveva raccogliere una monete speciale se intorno ci sono 0 monete normali
doveviRaccogliere(T,X1,Y1):- pacmanDLV(T,X1,Y1),#count{X,Y : moneta(X,Y,"normale"),not raccoltaMonetaNormale(A,X,Y),-(T,1,A),
X>=B,-(X1,1,B),
Y>=W,-(Y1,1,W),
X<=C,+(X1,1,C),
Y<=D,+(Y1,1,D)
}=0, not moneta(X1,Y1,"speciale").
:~doveviRaccogliere(T,X1,Y1). [1:3]

%non doveva raccogliere una moneta speciale se intorno c'è almeno una moneta normale
nonDoveviRaccogliere(T,X1,Y1):- pacmanDLV(T,X1,Y1),#count{X,Y : moneta(X,Y,"normale"),not raccoltaMonetaNormale(A,X,Y),-(T,1,A),
X>=B,-(X1,1,B),
Y>=W,-(Y1,1,W),
X<=C,+(X1,1,C),
Y<=D,+(Y1,1,D)}>0, moneta(X1,Y1,"speciale").
:~nonDoveviRaccogliere(T,X1,Y1).[1:3]

%calcolo la distanza finale dalla moneta più vicina
distanza(D) :- monetaPiuVicina(X1,Y1),pacmanDLV(7,X2,Y2),X2>=X1,Y2>=Y1,-(X2,X1,D1),-(Y2,Y1,D2),+(D1,D2,D).
distanza(D) :- monetaPiuVicina(X1,Y1),pacmanDLV(7,X2,Y2),X2>=X1,Y2<=Y1,-(X2,X1,D1),-(Y1,Y2,D2),+(D1,D2,D).
distanza(D) :- monetaPiuVicina(X1,Y1),pacmanDLV(7,X2,Y2),X2<=X1,Y2>=Y1,-(X1,X2,D1),-(Y2,Y1,D2),+(D1,D2,D).
distanza(D) :- monetaPiuVicina(X1,Y1),pacmanDLV(7,X2,Y2),X2<=X1,Y2<=Y1,-(X1,X2,D1),-(Y1,Y2,D2),+(D1,D2,D).

%distanza(D) :- monetaPiuVicina(X1,Y1),pacmanDLV(7,X2,Y2),&distanza(X1,Y1,X2,Y2;D).

% se non raccoglie alcuna moneta, minimizza la distanza dalla più vicina
:~ #count{R,C : raccoltaMonetaNormale(7,R,C)}=0,distanza(D). [D:2]

% se non raccoglie alcuna moneta, minimizza la distanza dalla più vicina
%:~ #count{R,C : raccoltaMonetaNormale(7,R,C)}=0, monetaPiuVicina(X1,Y1),pacmanDLV(7,X2,Y2),&distanza(X2,Y2,X1,Y1;D1). [D1:2]

%controllo per non andare molte volte sulla stessa casella
a(T):-pacmanDLV(T,X,Y),pacmanDLV(T1,X,Y),T!=T1.
:~a(T). [1:1]
