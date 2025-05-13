INCLUDE ../GlobalSettings/global.ink
# speaker: Кузнец
# portrait: SmithyPortrait
# layout: right
Пан, мой отец перед смертью зарыл во дворе целый склад доспехов. Но мне сейчас некогда искать - вся кузница завалена заказами для армии.

*   Где искать?
    # speaker: Кузнец
    # portrait: SmithyPortrait
    # layout: left
    -> askLocation

*   Сам ищи.
    # speaker: Кузнец
    # portrait: SmithyPortrait
    # layout: left
    -> refuseHelp

=== askLocation ===
    # speaker: Кузнец
    # portrait: SmithyPortrait
    # layout: left
    Где именно искать эти сокровища?
    # speaker: Кузнец
    # portrait: SmithyPortrait
    # layout: right
    Говорил, что под камнем у северной стены.
    ~ TalkedWithMagnate = true
    -> END

=== refuseHelp ===
    # speaker: Кузнец
    # portrait: SmithyPortrait
    # layout: left
    Сам ищи свои сокровища. Не барское это дело под камнями лазать.
    # speaker: Кузнец
    # portrait: SmithyPortrait
    # layout: right
    Как знаете. Только потом не жалуйтесь, что армия недоснабжена.
    ~ QuestComplete = true
    ~ CastleStrength -= 30 
    -> END