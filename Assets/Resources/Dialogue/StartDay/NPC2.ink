INCLUDE ../GlobalSettings/global.ink

#speaker:Музыкантка #portrait:black_woman #layout:right
Милостивый пан, позволь нам переночевать и подкрепиться. Взамен усладим слух ваших людей перед битвой.
*   Пусть поют, может, хоть кто-то забудет о смерти на пороге. #speaker:Музыкантка #portrait:black_woman #layout:left
    Песнь будет о твоей щедрости. #speaker:Музыкантка #portrait:black_woman #layout:right
    ~ Food -= 5
    ~ PeopleSatisfaction += 2
    -> END
*   Здесь война, а не балаган! #speaker:Музыкантка #portrait:black_woman #layout:left
    Как знаешь, пан, но помни – не одними мечами битва выиграна. #speaker:Музыкантка #portrait:black_woman #layout:right
    ~ PeopleSatisfaction -= 1
    -> END