INCLUDE globals.ink

{ last_finished_cutscene:
- 1: -> para_casa_de_a
- 3: -> para_escola
- 4: -> retorno_casa_de_a
    
}

=== para_casa_de_a ===
{PLAYER_ACTOR}: Se me lembro bem a casa do {a_name} fica no canto superior esquerdo da cidade.
{PLAYER_ACTOR}: É bem fácil de achar, já que é a mãe dele gosta MUITO de flores.
-> END

=== para_escola ===
{PLAYER_ACTOR}: Agora eu preciso voltar para a escola conversar com a professora {professor_name}.
{PLAYER_ACTOR}: A escola fica no centro da cidade, no canto direito.
-> END

=== retorno_casa_de_a ===
{PLAYER_ACTOR}: Agora que eu tirei algumas das dúvidas que tinha sobre ansiedade, preciso voltar para a casa do {a_name} falar com ele.
-> END
