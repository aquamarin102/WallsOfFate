INCLUDE ../GlobalSettings/global.ink

#speaker:Стражник #portrait:black_guard #layout:right
Поймали разбойников, что с ними делать?

*   На виселицу всех! Пусть служат примером! #speaker:Стражник #portrait:black_guard #layout:left
    Исполним немедля. #speaker:Стражник #portrait:black_guard #layout:right
    ~ PeopleSatisfaction += 1
    -> END
*   Если готовы к бою – пусть искупают кровью.  #speaker:Стражник #portrait:black_guard #layout:left
    Бой лучше петли, пан. #speaker:Стражник #portrait:black_guard #layout:right
     ~ CastleStrength += 20
    -> END