#speaker:Стражник #portrait:GuardPortrait #layout:right
Ваша Светлость! Среди беженцев, что укрылись в стенах замка, есть мужчины. С их помощью мы могли бы усилить гарнизон. Однако они отказываются брать в руки оружие. Не знаю, что делать с ними…

*   Я займусь этим. #speaker:Стражник #portrait:GuardPortrait #layout:left
    -> refugees_quest_started

*   Так заставьте их! #speaker:Стражник #portrait:GuardPortrait #layout:left
    -> skip_quest

=== refugees_quest_started ===
    Спасибо, ваша светлость! Мы на вас надеемся. #speaker:Стражник #portrait:GuardPortrait #layout:right
    -> DONE


=== skip_quest ===
Вы — наш лидер, Ваша Светлость. Народ смотрит на Вас, и Ваше слово решающее для них… speaker:Стражник #portrait:GuardPortrait #layout:right
-> DONE