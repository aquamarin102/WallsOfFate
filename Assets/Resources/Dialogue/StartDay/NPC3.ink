INCLUDE ../GlobalSettings/global.ink

#speaker:Стражник #portrait:black_guard #layout:right
Вор сбежал, забрал припасы. Прикажешь гнаться?

*   Вернуть всё обратно! Хочу видеть его голову на пике! #speaker:Стражник #portrait:black_guard #layout:left
    Будет сделано, пан. Жаль только бойцов отвлекать. #speaker:Стражник #portrait:black_guard #layout:right
    ~ CastleStrength -= 20
    -> END
*    Не трать людей, пусть подыхает в лесу. #speaker:Стражник #portrait:black_guard #layout:left
     Как скажете. Пусть вороны его судят. #speaker:Стражник #portrait:black_guard #layout:right
    ~ Food -= 10
    -> END