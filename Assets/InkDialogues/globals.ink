CONST PLAYER_ACTOR = "player"
CONST A_ACTOR = "A"
CONST B_ACTOR = "B"
CONST PROFESSOR_ACTOR = "professor"

VAR player_name = "Bryan"
VAR a_name = "Alan"
VAR b_name = "Beatriz"
VAR professor_name = "Carla"

VAR last_finished_cutscene = -1

//endless runner varibles
VAR endless_runner_points = 0
VAR endless_runner_points_to_win = 30
VAR player_won_endless_runner_game = false

=== function format_name(name) ===
    ~ return format_important_text(name)

=== function format_important_text(text) ===
    ~ return "<style=\"Important\">{text}</style>"