INCLUDE ../globals.ink

CONST NPC_NAME = "Tereza"

{NPC_NAME}: O dia de hoje está muito bonito!

{
    - last_finished_cutscene == 4 && !is_searching_for_key && !has_found_key:
        {NPC_NAME}: Você está procurando a casa da {format_name(professor_name)}?
        {NPC_NAME}: É esta casa logo a minha esquerda.
        {NPC_NAME}: Acredito que o marido dela, o {format_name("Rogério")} esteja lá.
}