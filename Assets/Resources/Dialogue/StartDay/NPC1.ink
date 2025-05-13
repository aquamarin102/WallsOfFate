INCLUDE ../GlobalSettings/global.ink

#speaker:Крестьянин   #portrait:black_man   #layout:right
Пан, люди голодают. Дозволь открыть амбары и раздать хоть немного зерна.

#speaker:Крестьянин          #portrait:black_man   #layout:left
*   Ладно, накормите их, но чтобы без лишних разговоров.
    #speaker:Крестьянин   #portrait:black_man   #layout:right
    Благодарим, пан! Люди не забудут твоей доброты.
    ~ Food -= 10
    ~ PeopleSatisfaction += 2
    -> END

*   Не смей тратить мои запасы! Пусть выкручиваются сами.  
    #speaker:Крестьянин   #portrait:black_man   #layout:right
    Понимаю, пан, но и людям память короткой не будет.
    ~ PeopleSatisfaction -= 3
    -> END
