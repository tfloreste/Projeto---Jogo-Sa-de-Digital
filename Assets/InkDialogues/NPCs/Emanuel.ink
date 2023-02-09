INCLUDE ../globals.ink

CONST NPC_NAME = "Emanuel"

{NPC_NAME}: Eu ouvi uma história de que se você jogar uma moeda na fonte e fazer um pedido, o <style="C1">Gato Primordial</style> vai torná-lo realidade.
{NPC_NAME}: Resolvi fazer o teste e joguei uma moeda ontem a tarde. Hoje ela não está mais aqui.
{NPC_NAME}: Será que o <style="C1">Gato Primordial</style> pegou a minha moeda? 
{NPC_NAME}: Espero que ele realize o meu desejo!

{is_searching_for_key:
    {NPC_NAME}: Oh! Então você está procurando uma chave que alguém perdeu por aqui?
    {NPC_NAME}: Bom, eu encontrei uma chave aqui sim um pouco mais cedo.
    {NPC_NAME}: Achei que fosse a chave que seria usada para abrir o meu tesouro, assim que o <style="C1">Gato Primordial</style> realizasse meu desejo.
    {NPC_NAME}: Bom... parece que não é esse o caso então você pode ficar com ela.
    {NPC_NAME}: <style="C1">*Entrega a chava para {player_name}*</style>
    {NPC_NAME}: Se por acaso não for essa a chave que você está procurando, me avise tudo bem? 
    {NPC_NAME}: Quem sabe realmente não seja a chave necessária para abrir o tesouro que me aguarda.
    
    ~is_searching_for_key = false
    ~has_found_key = true
}