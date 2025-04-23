INCLUDE ../GlobalSettings/global.ink

#speaker:Слуга #portrait:black_woman #layout:right
Несколько бойцов свалило болезнью. Лекарь просит удвоить пайки.

*   Пусть едят, нам нужны крепкие руки. #speaker:Слуга #portrait:black_woman #layout:left
    Скажу лекарю, пан. Солдаты будут благодарны. #speaker:Слуга #portrait:black_woman #layout:right
    ~ Food -= 15
    -> END
    
*   Они должны быть готовы умереть, а не ныть о лишней миске. #speaker:Слуга #portrait:black_woman #layout:left
    Передам твои слова, пан. #speaker:Слуга #portrait:black_woman #layout:right
     ~ CastleStrength -= 20
     ~ PeopleSatisfaction -= 1
    -> END