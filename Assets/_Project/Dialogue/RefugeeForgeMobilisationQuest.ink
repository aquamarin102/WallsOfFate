INCLUDE global.ink

Пожалуйста, не трогайте меня... Я просто хочу спрятаться здесь, пока война не закончится. 
# speaker: Кузнец
# portrait: OldmenPortrait
# layout: right

*   Ладно
    # speaker: Кузнец
    # portrait: OldmenPortrait
    # layout: left
    -> gate1RefugeeMainRoom

*   Похоже, мне придется убедить вас силой...
    # speaker: Кузнец
    # portrait: OldmenPortrait
    # layout: left
    -> gate2RefugeeMainRoom

=== gate1RefugeeMainRoom ===
    "Спасибо! Да благословит бог вашу семью!"
    # speaker: Кузнец
    # portrait: OldmenPortrait
    # layout: left
    -> END

=== gate2RefugeeMainRoom ===
    "Не бейте меня..."
    # speaker: Кузнец
    # portrait: OldmenPortrait
    # layout: left
  ~ PowerCheckStart = true
    -> END