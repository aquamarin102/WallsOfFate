INCLUDE ../GlobalSettings/global.ink

#speaker:Бродяга #portrait:OldmenPortrait #layout:right
Пощадите, Ваша Светлость! Я не воин! Мне бы только дожить до утра, не выходя из тени этих стен.

* Ты трус и дезертир. Мне придется казнить тебя. Здесь нет места тем, кто готов лишь брать, но не отдавать. К тому же трусы заражают страхом весь полк.
    #speaker:Кузнец
    #portrait:OldmenPortrait
    #layout:left
    -> gate1RefugeeMainRoom

* Возьмешь меч — честь тебе. Откажешься — встанешь в строй под кнутом. Но в бой ты пойдёшь, хочешь ты того или нет.
    #speaker:Кузнец
    #portrait:OldmenPortrait
    #layout:left
    -> gate2RefugeeMainRoom

=== gate1RefugeeMainRoom ===
Ладно… Ладно. Я пойду. Только не бейте больше.
#speaker:Кузнец
#portrait:OldmenPortrait
#layout:left
-> END

=== gate2RefugeeMainRoom ===
Нет! Я не пойду! Лучше умру здесь, чем на поле!
#speaker:Кузнец
#portrait:OldmenPortrait
#layout:left
~ PowerCheckStart = true
-> END