INCLUDE ../GlobalSettings/global.ink

#speaker:Слуга #portrait:black_man #layout:right
Господин, крыша склада прохудилась, зерно может испортиться!

*   Почините быстро, или вы сами станете пищей! #speaker:Слуга #portrait:black_man #layout:left
    Сделаем сразу, пан! #speaker:Слуга #portrait:black_man #layout:right
    ~ Gold -= 20
    -> END
*   Нет денег на это. Пусть едят сырое зерно, если придётся. #speaker:Слуга #portrait:black_man #layout:left
    Да, пан. Но помните – голод бьёт хуже стрелы. #speaker:Слуга #portrait:black_man #layout:right
    ~ Food -= 20
    -> END