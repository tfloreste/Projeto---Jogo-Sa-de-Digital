INCLUDE ../globals.ink

CONST NPC_NAME = "Matheus"

{NPC_NAME}: Olá {format_name(player_name)}! A prova da sua turma já acabou?
{NPC_NAME}: A sua professora {format_name(professor_name)}, minha irmã, vai ficar até tarde na escola hoje. Espero que ela não tenha esquecido de levar algo para comer.

{is_searching_for_key:
    {NPC_NAME}: Então o {format_name("Rogério")} acabou se trancando dentro de casa e não conseuguiu levar o almoço da minha irmã?
    {NPC_NAME}: Isso é bem a cara dele mesmo.
    {NPC_NAME}: Infelizmente não encontrei a chave por aqui, {format_name(player_name)}
}