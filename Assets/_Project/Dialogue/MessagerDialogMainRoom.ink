INCLUDE ../GlobalSettings/global.ink

=== MessagerDialogMainRoom ===
Ваша светлость! Срочные новости! Войска противника замечены в долине! Они движутся к крепости! #speaker:Гонец #portrait:ms_yellow_neutral #layout:right
Мы должны подготовиться к осаде. Кузнец уже работает над оружием во внутреннем дворе, но ему нужна помощь. Пожалуйста, поговорите с ним! #speaker:Гонец #portrait:ms_yellow_neutral #layout:right

+ [ыделить 20 золотых на укрепления] #speaker:Гонец #portrait:ms_yellow_neutral #layout:left
    ~ Gold -= 100
    ~ CastleStrength += 30
    -> DONE
    
+ [Мобилизовать 50 ед. продовольствия] #speaker:Гонец #portrait:ms_yellow_neutral #layout:left
    ~ Food -= 50
    ~ CastleStrength += 20
    
    -> DONE

+ [Успокоить народ] #speaker:Гонец #portrait:ms_yellow_neutral #layout:left
    ~ PeopleSatisfaction += 25
    ~ CastleStrength -= 10
    -> DONE

-> DONE