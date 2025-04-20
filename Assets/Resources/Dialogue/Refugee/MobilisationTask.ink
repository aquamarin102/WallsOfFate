INCLUDE ../GlobalSettings/global.ink

#speaker:Бродяга #portrait:OldmenPortrait #layout:right
Пожалуйста... Мне нечего есть уже три дня. Не найдется ли у вас куска хлеба? 

#speaker:Бродяга #portrait:OldmenPortrait #layout:left
Сколько вы можете дать?

#speaker:Бродяга #portrait:OldmenPortrait #layout:right
У меня есть семейная реликвия — старый медальон. Я отдам его в обмен на еду.

*   Ладно, дам вам провизию. #speaker:Игрок #portrait:Default #layout:left
    Спасибо... Да благословит вас небо. #speaker:Бродяга #portrait:OldmenPortrait_Relieved #layout:right
    -> DONE

*   Убирайтесь отсюда! #speaker:Игрок #portrait:Default_Angry #layout:left
    *Бродяга съеживается* Я... я понимаю. Простите за беспокойство. #speaker:Бродяга #portrait:OldmenPortrait_Sad #layout:right
    ~ PeopleSatisfaction -= 2 
    -> DONE