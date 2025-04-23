INCLUDE ../GlobalSettings/global.ink

#speaker:Беженец #portrait:black_man #layout:right
Пан, дай приют мне и моим людям, иначе смерть наша будет на твоей совести.
*   Ладно, но будьте готовы умереть за эту крепость. #speaker:Беженец #portrait:black_man #layout:left
    Лучше смерть в бою, чем от голода. #speaker:Беженец #portrait:black_man #layout:right
    ~ Food -= 10
    ~ CastleStrength += 20
    -> END
*   Моей совести нечего бояться. Уходите.  #speaker:Беженец #portrait:black_man #layout:left
    Помяните наше горе добрым словом, если ещё можете. #speaker:Беженец #portrait:black_man #layout:right
    -> END