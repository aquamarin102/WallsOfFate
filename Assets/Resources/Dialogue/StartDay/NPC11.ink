INCLUDE ../GlobalSettings/global.ink

#speaker:Гонец #portrait:ms_yellow_neutral #layout:right
Люди устали и просят небольшой праздник перед битвой, господин.
#speaker:Гонец #portrait:ms_yellow_neutral #layout:left
*   Устроим праздник, пусть поднимут боевой дух! 
    Ваше решение мудро, это укрепит людей перед боем. #speaker:Гонец #portrait:ms_yellow_neutral #layout:right
    ~ Food -= 10
    ~ Gold -= 10
    ~ PeopleSatisfaction += 2
    -> END
*    Какой ещё праздник перед осадой? Совсем с ума сошли?!
     Понимаю, господин, передам ваши слова людям. #speaker:Гонец #portrait:ms_yellow_neutral #layout:right
      ~ PeopleSatisfaction -= 2
    -> END