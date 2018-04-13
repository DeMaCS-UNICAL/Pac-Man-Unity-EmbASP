%MODELLO INPUT
%casella(X,Y).		   		--> una generica casella percorribile eventualmente
%moneta(X,Y,Tipo).     		--> una moneta con il tipo associato ("normale"/"speciale")
%pacmanDLV(Tempo,X,Y). 		--> la posizione di pacman in un certo istante di tempo
%monetaPiuVicina(X,Y). 		--> la moneta più vicina a pacman 
%nemico(ID,Peso,X,Y).		--> un nemico con il suo Id, il peso che dipende dalla distanza da pacman e la sua posizione

#maxint = 200.
int(1..200).

%Guess sulle mosse possibili
up(T)|down(T)|right(T)|left(T):-pacmanDLV(T,X,Y),T<7.

%vincolo mossa consentita
:- up(T),pacmanDLV(T,X,Y),not casella(A,Y),-(X,1,A),int(A).
:- down(T),pacmanDLV(T,X,Y),not casella(A,Y),+(X,1,A),int(A).
:- left(T),pacmanDLV(T,X,Y),not casella(X,A),-(Y,1,A),int(A).
:- right(T),pacmanDLV(T,X,Y),not casella(X,A),+(Y,1,A),int(A).

%aggiorna posizione pacman
pacmanDLV(A,Z,Y):-up(T),pacmanDLV(T,X,Y), -(X,1,Z), +(T,1,A).
pacmanDLV(A,Z,Y):-down(T),pacmanDLV(T,X,Y), +(X,1,Z), +(T,1,A).
pacmanDLV(A,X,Z):-left(T),pacmanDLV(T,X,Y), -(Y,1,Z), +(T,1,A).
pacmanDLV(A,X,Z):-right(T),pacmanDLV(T,X,Y), +(Y,1,Z), +(T,1,A).

distanzaFinale(ID,D) :- nemico(ID,Peso,X1,Y1),pacmanDLV(7,X2,Y2),X2>=X1,Y2>=Y1,
-(X2,X1,D1),int(D1),-(Y2,Y1,D2),int(D2),+(D1,D2,D),int(D).
distanzaFinale(ID,D) :- nemico(ID,Peso,X1,Y1),pacmanDLV(7,X2,Y2),X2>=X1,Y2<=Y1,
-(X2,X1,D1),int(D1),-(Y1,Y2,D2),int(D2),+(D1,D2,D),int(D).
distanzaFinale(ID,D) :- nemico(ID,Peso,X1,Y1),pacmanDLV(7,X2,Y2),X2<=X1,Y2>=Y1,
-(X1,X2,D1),int(D1),-(Y2,Y1,D2),int(D2),+(D1,D2,D),int(D).
distanzaFinale(ID,D) :- nemico(ID,Peso,X1,Y1),pacmanDLV(7,X2,Y2),X2<=X1,Y2<=Y1,
-(X1,X2,D1),int(D1),-(Y1,Y2,D2),int(D2),+(D1,D2,D),int(D).

distanzaCorrente(T,ID,D) :- nemico(ID,Peso,X1,Y1),pacmanDLV(T,X2,Y2),X2>=X1,Y2>=Y1,
-(X2,X1,D1),int(D1),-(Y2,Y1,D2),int(D2),+(D1,D2,D),int(D).
distanzaCorrente(T,ID,D) :- nemico(ID,Peso,X1,Y1),pacmanDLV(T,X2,Y2),X2>=X1,Y2<=Y1,
-(X2,X1,D1),int(D1),-(Y1,Y2,D2),int(D2),+(D1,D2,D),int(D).
distanzaCorrente(T,ID,D) :- nemico(ID,Peso,X1,Y1),pacmanDLV(T,X2,Y2),X2<=X1,Y2>=Y1,
-(X1,X2,D1),int(D1),-(Y2,Y1,D2),int(D2),+(D1,D2,D),int(D).
distanzaCorrente(T,ID,D) :- nemico(ID,Peso,X1,Y1),pacmanDLV(T,X2,Y2),X2<=X1,Y2<=Y1,
-(X1,X2,D1),int(D1),-(Y1,Y2,D2),int(D2),+(D1,D2,D),int(D).


%vincolo per evitare di tornare due volte sulla stessa posizione
a(T):-pacmanDLV(T,X,Y),pacmanDLV(T1,X,Y),T<T1.
:~ a(T). [1:20]

%paga se va su una posizione dove c'è un nemico
b(ID):-pacmanDLV(T,X,Y),nemico(ID,_,X,Y).
:~ b(ID). [1:19]

%per ogni istante paga la distanza corrente da un dato nemico ad un livello che dipende dal peso di quel nemico
c(D,ID,T):- distanzaCorrente(T,ID,D),D<=7.
:~ c(D,ID,T),-(19,D,A),int(A). [1:A]

%si ha un'urgenza se ci sono più di 2 nemici intorno a pacman all'istante T
urgenza(T):-pacmanDLV(T,X,Y),not #count{ID: nemico(ID,_,X1,Y1),
X1>=A,-(X,3,A),int(A),
Y1>=B,-(Y,3,B),int(B),
X1<=C,+(X,3,C),int(C),
Y1<=D,+(Y,3,D),int(D)}=0.

%cerca di prendere la moneta speciale se ha urgenza
:~urgenza(T),pacmanDLV(A,R,C),not moneta(R,C,speciale),+(T,1,A), int(A). [1:11]

%paga la distanza finale da un nemico ad un livello che dipende dal peso di quel nemico
d(D,ID,Peso):-distanzaFinale(ID,D),nemico(ID,Peso,_,_).
:~ d(D,ID,Peso),-(200,D,A),int(A). [A:Peso]

% aggiornamento delle monete raccolte
raccoltaMonetaNormale(T,X,Y):- pacmanDLV(T,X,Y),moneta(X,Y,"normale").
raccoltaMonetaNormale(A,X,Y):-raccoltaMonetaNormale(T,X,Y),T<7,+(T,1,A), int(A).

%paga se non raccoglie monete normali.
:~ pacmanDLV(T,X,Y), not moneta(X,Y,"normale"). [1:2]
:~ pacmanDLV(T,X,Y), raccoltaMonetaNormale(A,X,Y),-(T,1,A),int(A). [1:2]

distanzaPacmanMonetaVicina(D) :- monetaPiuVicina(X1,Y1),pacmanDLV(7,X2,Y2),X2>=X1,Y2>=Y1,
-(X2,X1,D1),int(D1),-(Y2,Y1,D2),int(D2),+(D1,D2,D),int(D).
distanzaPacmanMonetaVicina(D) :- monetaPiuVicina(X1,Y1),pacmanDLV(7,X2,Y2),X2>=X1,Y2<=Y1,
-(X2,X1,D1),int(D1),-(Y1,Y2,D2),int(D2),+(D1,D2,D),int(D).
distanzaPacmanMonetaVicina(D) :- monetaPiuVicina(X1,Y1),pacmanDLV(7,X2,Y2),X2<=X1,Y2>=Y1,
-(X1,X2,D1),int(D1),-(Y2,Y1,D2),int(D2),+(D1,D2,D),int(D).
distanzaPacmanMonetaVicina(D) :- monetaPiuVicina(X1,Y1),pacmanDLV(7,X2,Y2),X2<=X1,Y2<=Y1,
-(X1,X2,D1),int(D1),-(Y1,Y2,D2),int(D2),+(D1,D2,D),int(D).

%se non raccoglie la moneta piu vicina, minimizza la distanza
monetaPiuVicinaRaccolta :- raccoltaMonetaNormale(_,X,Y),monetaPiuVicina(X,Y).
:~ distanzaPacmanMonetaVicina(D),not monetaPiuVicinaRaccolta. [D:1]
