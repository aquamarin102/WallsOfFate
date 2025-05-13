INCLUDE ../GlobalSettings/global.ink
# speaker: Глава стражи
# portrait: GuardPortrait
# layout: right
Пан, чем прикажете заняться?

*   В кузне пропадают гвозди.
    # speaker: Глава стражи
    # portrait: GuardPortrait
    # layout: left
    -> interrogateBlacksmith

*   Кузнец обвиняет бродягу в краже. Арестуй бродягу у ворот. Он вор
    # speaker: Глава стражи
    # portrait: GuardPortrait
    # layout: left
    -> arrestBeggar

=== interrogateBlacksmith ===
    # speaker: Глава стражи
    # portrait: GuardPortrait
    # layout: left
    Нужно разобраться. Допроси кузнеца как следует. Он что-то скрывает.
    # speaker: Глава стражи
    # portrait: GuardPortrait
    # layout: right
    Будет сделано! Мы найдём правду!
   
    ~ PeopleSatisfaction -= 1
    ~ CastleStrength -= 1
    -> END

=== arrestBeggar ===
    # speaker: Глава стражи
    # portrait: GuardPortraitn
    # layout: left
    Кузнец обвиняет бродягу в краже. Арестуй бродягу у ворот. Он вор.
    # speaker: Глава стражи
    # portrait: GuardPortraitn
    # layout: right
    Я думаю справедливым наказанием будет забрать его в армию.
    
    ~ PeopleSatisfaction -= 1
    -> END