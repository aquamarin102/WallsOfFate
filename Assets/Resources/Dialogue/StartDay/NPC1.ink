INCLUDE ../GlobalSettings/global.ink

#speaker:Крестьянин #portrait:black_man #layout:right
Пан, люди голодают. Дозволь открыть амбары и раздать хоть немного зерна.
*   Ладно, накормите их, но чтобы без лишних разговоров. #speaker:Крестьянин #portrait:black_man #layout:left
    Благодарим, пан! Люди не забудут твоей доброты. #speaker:Крестьянин #portrait:black_man #layout:right
    ~ Food -= 10
    ~ PeopleSatisfaction += 2
    -> END
*   Не смей тратить мои запасы! Пусть выкручиваются сами.  #speaker:Крестьянин #portrait:black_man #layout:left
    Понимаю, пан, но и людям память короткой не будет. #speaker:Крестьянин #portrait:black_man #layout:right
    ~ PeopleSatisfaction -= 3
    -> END