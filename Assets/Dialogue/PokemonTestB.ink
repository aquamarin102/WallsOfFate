INCLUDE global.ink

Hello big Bob
Hi little Bob
Which pokemon do you choose?
+[Charmander]
   -> chosen("Charmander")
+[Bulbasaur]
    -> chosen("Bulbasaur")
+[Squirtle]
-> chosen("Squirtle")

===chosen(pokemon)===
~ pokemon_name = pokemon
You chose {pokemon}!
->END