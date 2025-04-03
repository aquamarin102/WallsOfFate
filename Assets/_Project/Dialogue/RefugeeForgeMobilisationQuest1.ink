INCLUDE global.ink

Пожалуйста, не трогайте меня... Я просто хочу спрятаться здесь, пока война не закончится. #speaker:Бродяга #portrait:OldmenPortrait #layout:right

*   Тебе придется #speaker:Пан Яков #portrait:OldmenPortrait  #layout:left
    -> gate1RefugeeMainRoom

*   Похоже, мне придется убедить тебя силой... #speaker:Пан Яков #portrait:OldmenPortrait  #layout:left
    -> gate2RefugeeMainRoom

=== gate1RefugeeMainRoom ===
    Спасибо! Да благословит бог вашу семью!. #speaker:Бродяга #portrait:OldmenPortrait #layout:right
  ~ PowerCheckStart = true
    -> END

=== gate2RefugeeMainRoom ===
    Не бейте меня... #speaker:Бродяга #portrait:OldmenPortrait #layout:right
  ~ PowerCheckStart = true
    -> END