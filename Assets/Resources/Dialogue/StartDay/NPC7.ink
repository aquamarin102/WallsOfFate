INCLUDE ../GlobalSettings/global.ink

#speaker:Кочевник #portrait:black_man #layout:right
Пан, дорога длинна, и живот сводит голодом. Продай немного еды, золото есть.

*   Деньги вперёд, еда после.  #speaker:Кочевник #portrait:black_man #layout:left
    Держи, пан. Пусть дорога твоя будет безопасной. #speaker:Кочевник #portrait:black_man #layout:right
    ~ Food -= 20
    ~ Gold += 20
    -> END
*   Самим не хватает, не до торговли. #speaker:Кочевник #portrait:black_man #layout:left
    Не забывай, пан, голод толкает людей на отчаянные поступки... #speaker:Кочевник #portrait:black_man #layout:right
    -> END