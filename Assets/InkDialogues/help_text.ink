INCLUDE globals.ink

{ last_finished_cutscene:
- 1: -> para_casa_de_a
- 3: -> para_escola
- 4: -> check_scene4_conditions
- else: -> developer_message
}

=== para_casa_de_a ===
{PLAYER_ACTOR}: Se me lembro bem a casa do {format_name(a_name)} fica no canto superior esquerdo da cidade.
{PLAYER_ACTOR}: É bem fácil de achar, já que a mãe dele {format_important_text("gosta MUITO de flores")}.
-> END

=== para_escola ===
{PLAYER_ACTOR}: Agora eu preciso voltar para a escola conversar com a professora {format_name(professor_name)}.
{PLAYER_ACTOR}: A escola fica no centro da cidade, no canto direito.
{PLAYER_ACTOR}: A região da escola possui quatro salas de aula, uma fonte e uma cantina, cercadas por um muro verde.
{PLAYER_ACTOR}: Eu não consigo deixar de achar estranho que as salas de aula são iguais a todas as casas do vilarejo.
-> END

=== retorno_casa_de_a ===
{PLAYER_ACTOR}: Agora que eu tirei algumas das dúvidas que tinha sobre ansiedade, preciso voltar para a casa do {format_name(a_name)} falar com ele.
-> END

=== developer_message ===
Desenvolvedor: Oh! Parece que alguma coisa errada aconteceu.
Desenvolvedor: Você realmente não deveria poder clicar nesse botão neste momento. Sinto muito!
-> END


=== check_scene4_conditions ===
{   
    - !is_searching_for_key && !has_found_key:
        {PLAYER_ACTOR}: Eu preciso encontrar a casa da professora {format_name(professor_name)}.
        {PLAYER_ACTOR}: Pelo que ela me disse, a casa fica na parte {format_important_text("inferior")} do vilarejo,  {format_important_text("à esquerda da casa da dona Tereza")}.
        {PLAYER_ACTOR}: Se não me engano é a {format_important_text("sexta casa, contando da direita para a esquerda")}.
        
    - is_searching_for_key:
        {PLAYER_ACTOR}: Aparentemente o professor {format_name("Rogério")} perdeu a chave em algum lugar no parque, que fica no centro do vilarejo.
        {PLAYER_ACTOR}: Talvez alguém tenha encontrado, então acho que deveria começar perguntando para as pessoas lá.
        
    - has_found_key && !has_got_food:
        {PLAYER_ACTOR}: Agora que encontrei a chave, preciso devolver ela para o professor {format_name("Rogério")}.
        {PLAYER_ACTOR}: Se não me engano, a casa fica na região {format_important_text("sul, à esquerda da casa da dona Tereza")}.
        
    - has_got_food && !gave_food_to_teacher:
        {PLAYER_ACTOR}: Finalmente consegui algo para a professora {format_name(professor_name)} comer.
        {PLAYER_ACTOR}: Só preciso voltar para a escola e entregar para ela.
        
    - gave_food_to_teacher:
        -> retorno_casa_de_a
}

-> END