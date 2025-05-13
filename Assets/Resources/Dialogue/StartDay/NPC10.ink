INCLUDE ../GlobalSettings/global.ink

#speaker:Слуга #portrait:black_man #layout:right
Господин, крыша склада прохудилась, зерно может испортиться!
#speaker:Слуга #portrait:black_man #layout:left
*   Почините быстро, или вы сами станете пищей!
    Сделаем сразу, пан! #speaker:Слуга #portrait:black_man #layout:right
    ~ Gold -= 20
    -> END
*   Нет денег на это. Пусть едят сырое зерно, если придётся. 
    Да, пан. Но помните – голод бьёт хуже стрелы. #speaker:Слуга #portrait:black_man #layout:right
    ~ Food -= 20
    -> END