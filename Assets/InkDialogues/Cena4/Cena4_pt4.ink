INCLUDE ../globals.ink

VAR NPC_NAME = "Rogério"

{PLAYER_ACTOR}: Voltei professora!
{PLAYER_ACTOR}: Consegui pegar algo para você comer com o professor {format_name(NPC_NAME)}.
{PLAYER_ACTOR}: Ele me deu esse pote cheio de cookies.

{PROFESSOR_ACTOR}: Oi {format_name(player_name)}!
{PROFESSOR_ACTOR}: Que bom que deu tudo certo.
{PROFESSOR_ACTOR}: Mas você até que demorou um pouquinho, aconteceu alguma coisa lá?

{PLAYER_ACTOR}: Bom, até que sim. Vou te contar tudo.
{PROFESSOR_ACTOR}: Você pode me contar enquanto comemos, o que acha?
{PROFESSOR_ACTOR}: Tem cookies suficientes para nós dois, e você deve estar com fome depois de toda essa aventura.
{PLAYER_ACTOR}: Acho que um pouco...
{PLAYER_ACTOR}: Pra ser sincero, a última vez que eu comi foi antes de vir aqui fazer a prova.

{PROFESSOR_ACTOR}: Menino! Você está em fase de crescimento, precisa se alimentar direito!
{PROFESSOR_ACTOR}: Tudo bem que esses cookies não são a coisa mais saudável que existe, mas é melhor do que ficar de estomago vazio.


/* fade out e fade in */

{PROFESSOR_ACTOR}: Não acredito que o {format_name(NPC_NAME)} se trancou de novo com aquela tranca automática dele.
{PLAYER_ACTOR}: Então não foi a primeira vez?
{PROFESSOR_ACTOR}: Quem dera fosse.
{PROFESSOR_ACTOR}: Eu perdi a conta já de quantas vezes isso aconteceu.

{PLAYER_ACTOR}: Isso é bem a cara do professor {format_name(NPC_NAME)} mesmo.

{PROFESSOR_ACTOR}: Aliás {format_name(player_name)}, acho que eu estava te devendo terminar uma explicação.
{PROFESSOR_ACTOR}: Se eu não me engano você tinha me pergutado a diferença entre psicologos e psiquiatras, certo?

{PLAYER_ACTOR}: Isso mesmo, professora.

{PROFESSOR_ACTOR}: Bom, vamos lá...
{PROFESSOR_ACTOR}: Como eu estava dizendo, existe uma série de diferenças entre esses profissionais.  
{PROFESSOR_ACTOR}: Um psiquiatra é um profissional formado em medicina. Essencialmente, ele se preocupa com a parte mais biológica dos transtornos mentais. 
{PROFESSOR_ACTOR}: O psiquiatra é necessário quando é preciso incluir medicamentos ao tratamento. Já que os psicólogos não podem receitá-los. 
{PROFESSOR_ACTOR}: O psicólogo é um profissional formado em psicologia.  
{PROFESSOR_ACTOR}: Seu trabalho se utiliza de técnicas para auxiliar indivíduos a compreenderem melhor a si mesmos, e lidarem com determinadas dificuldades. 
{PROFESSOR_ACTOR}: São profissionais diferentes que se complementam para o tratamento de transtornos mentais. 
 
{PLAYER_ACTOR}: Oh, certo! Acho que entendi. 
{PLAYER_ACTOR}: E você acha que... as pessoas que precisam desses profissionais, sabe...?  
{PLAYER_ACTOR}: É por que elas são doidas? 
 

{PROFESSOR_ACTOR}:  O quê!!? 
{PROFESSOR_ACTOR}: Claro que não! 
{PROFESSOR_ACTOR}: Infelizmente, acho que isso é um preconceito bem comum. 
{PROFESSOR_ACTOR}: Mas pessoas que buscam ajuda para os problemas que tem são qualquer coisa menos “doidas”. 
{PROFESSOR_ACTOR}: Transtornos mentais são doenças e, como outras doenças, precisam ser tratadas para que quem as têm possa viver uma vida melhor. 
{PROFESSOR_ACTOR}: Buscar tratamento para um transtorno mental não é diferente de buscar um médico ao perceber sintomas de gripe ou pressão alta. 

{PROFESSOR_ACTOR}: Claro, além do tratamento, ter uma qualidade de vida boa, com uma alimentação saudável, praticar exercícios e até meditação podem ajudar.
{PROFESSOR_ACTOR}: Mas buscar o tratamento adequado ainda é muito importante!
 
{PLAYER_ACTOR}:  Muito obrigado, professora {format_name(professor_name)}! 
{PLAYER_ACTOR}:  Acho que você me fez entender mais do que eu esperava. 
 
{PROFESSOR_ACTOR}: De nada. Fico feliz que você me veja como um adulto confiável e eu possa ter ajudado, {format_name(player_name)}! 
{PROFESSOR_ACTOR}: É preocupante como a ansiedade e depressão vem se tornando problemas muito frequentes hoje em dia. 
{PROFESSOR_ACTOR}: E ainda assim faltam informações. 
{PROFESSOR_ACTOR}: Pessoas de todas as idades sofrem, sem saber como lidar com estas questões. 
{PROFESSOR_ACTOR}: Mas conversar com você me deu uma ideia. 
{PROFESSOR_ACTOR}: Assim que esse período de provas terminar, vou tentar preparar uma aula especial sobre este assunto.

{PLAYER_ACTOR}: Acho que preciso ir agora antes que fique tarde. Muito obrigado novamente, professora!
{PROFESSOR_ACTOR}: Eu quem te agradeço por ter todo esse trabalho para me ajudar.
{PROFESSOR_ACTOR}: Até amanhã, {format_name(player_name)}!
{PLAYER_ACTOR}: Até amanhã!

~ gave_food_to_teacher = true