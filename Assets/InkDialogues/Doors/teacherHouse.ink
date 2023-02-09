INCLUDE ../globals.ink

CONST NPC_NAME = "Rogério"

{
    - last_finished_cutscene == 1:
        {PLAYER_ACTOR}: Esta não é a casa do {format_name(a_name)}.
    
    - last_finished_cutscene == 3:
        {PLAYER_ACTOR}: Esta não é a escola.
        
    -last_finished_cutscene == 4 && is_searching_for_key:
        {NPC_NAME}: {format_name(player_name)}, me avise quando terminar de procurar no parque, por favor. Tenho quase certeza que a chave deve estar por lá.
        
    -last_finished_cutscene == 4 && has_got_food:
        {PLAYER_ACTOR}: Eu preciso voltar para a escola para falar com a professora {format_name(professor_name)}.
        {PLAYER_ACTOR}: Acho que eu não deveria mais incomodar o professor {format_name(NPC_NAME)}.
    
    -last_finished_cutscene >= 4:
        {PLAYER_ACTOR}: Acho que eu não deveria incomodar o professor {format_name(NPC_NAME)} agora.

    - else:
        {PLAYER_ACTOR}: Eu não deveria entrar aqui.
}
