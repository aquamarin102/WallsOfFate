INCLUDE ../GlobalSettings/global.ink

#speaker:Слуга #portrait:black_woman #layout:right
Специи кончились, пищу едва можно есть.

*   Купите эти чёртовы специи, иначе они мне все мозги съедят! #speaker:Слуга #portrait:black_woman #layout:left
    Исполним, пан, будет вкусно, словно на королевском пиру. #speaker:Слуга #portrait:black_woman #layout:right
    ~ Gold -= 10
    ~ PeopleSatisfaction += 1
    -> END
*   Они на войне или на свадьбе? Пусть едят, что дают. #speaker:Слуга #portrait:black_woman #layout:left
    Понял, пан. #speaker:Слуга #portrait:black_woman #layout:right
    ~ PeopleSatisfaction -= 1
    -> END