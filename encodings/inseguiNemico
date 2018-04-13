%MODELLO INPUT
%casella(X,Y).		   --> una generica casella percorribile eventualmente
%moneta(X,Y,Tipo).     --> una moneta con il tipo associato ("normale"/"speciale")
%pacmanDLV(Tempo,X,Y). --> la posizione di pacman in un certo istante di tempo
%nemicoPiuVicino(X,Y). --> il nemico più vicino che pacman deve provare a raggiungere
%numeroMosseMassimo(X) --> il numero massimo di mosse che verranno generate

%Guess sulle mosse possibili
up(T)|down(T)|right(T)|left(T):-pacmanDLV(T,X,Y),not raggiunto(T),numeroMosseMassimo(K),T<K.

raggiuntoNemico :- pacmanDLV(T,X,Y),nemicoPiuVicino(X,Y).
distanza(D) :- nemicoPiuVicino(X1,Y1),numeroMosseMassimo(K),pacmanDLV(K,X2,Y2),X2>=X1,Y2>=Y1,-(X2,X1,D1),int(D1),-(Y2,Y1,D2),int(D2),+(D1,D2,D),int(D).
distanza(D) :- nemicoPiuVicino(X1,Y1),numeroMosseMassimo(K),pacmanDLV(K,X2,Y2),X2>=X1,Y2<=Y1,-(X2,X1,D1),int(D1),-(Y1,Y2,D2),int(D2),+(D1,D2,D),int(D).
distanza(D) :- nemicoPiuVicino(X1,Y1),numeroMosseMassimo(K),pacmanDLV(K,X2,Y2),X2<=X1,Y2>=Y1,-(X1,X2,D1),int(D1),-(Y2,Y1,D2),int(D2),+(D1,D2,D),int(D).
distanza(D) :- nemicoPiuVicino(X1,Y1),numeroMosseMassimo(K),pacmanDLV(K,X2,Y2),X2<=X1,Y2<=Y1,-(X1,X2,D1),int(D1),-(Y1,Y2,D2),int(D2),+(D1,D2,D),int(D).

%distanza con python
%distanza(D) :- nemicoPiuVicino(X1,Y1),numeroMosseMassimo(K),pacmanDLV(K,X2,Y2),&distanza(X1,Y1,X2,Y2;D).

%paga per ogni volta che ritorna su una posizione già visitata
a(T):- pacmanDLV(T,X,Y),pacmanDLV(T1,X,Y),T<T1.
:~ a(T). [1:5]

%se non raggiunge il nemico, paga la distanza in modo da minimizzarla
:~ not raggiuntoNemico, distanza(D). [D:4]

%se lo raggiunge, pago per quanto impiega a raggiungerlo
raggiunto(T) :- pacmanDLV(T,X,Y),nemicoPiuVicino(X,Y).
:~ raggiunto(T). [T:3]

%vincolo mossa consentita
:- up(T),pacmanDLV(T,X,Y),not casella(A,Y),-(X,1,A),int(A).
:- down(T),pacmanDLV(T,X,Y),not casella(A,Y),+(X,1,A),int(A).
:- left(T),pacmanDLV(T,X,Y),not casella(X,A),-(Y,1,A),int(A).
:- right(T),pacmanDLV(T,X,Y),not casella(X,A),+(Y,1,A),int(A).

%aggiorna posizione pacman
pacmanDLV(B,A,Y):-up(T),pacmanDLV(T,X,Y),-(X,1,A),int(A),+(T,1,B),int(B).
pacmanDLV(B,A,Y):-down(T),pacmanDLV(T,X,Y),+(X,1,A),int(A),+(T,1,B),int(B).
pacmanDLV(B,X,A):-left(T),pacmanDLV(T,X,Y),-(Y,1,A),int(A),+(T,1,B),int(B).
pacmanDLV(B,X,A):-right(T),pacmanDLV(T,X,Y),+(Y,1,A),int(A),+(T,1,B),int(B).

%paga a livello 2 se non raccoglie monete normali, a livello 1 se non raccoglie monete speciali
:~ pacmanDLV(T,X,Y), not moneta(X,Y,"normale"). [1:2]
:~ pacmanDLV(T,X,Y), not moneta(X,Y,"speciale"). [1:1]
