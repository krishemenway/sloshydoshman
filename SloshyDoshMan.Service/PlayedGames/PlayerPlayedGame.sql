CREATE TABLE public.player_played_game
(
    steam_id varchar(22) NOT NULL,
    played_game_id uuid NOT NULL,
    kills integer NOT NULL DEFAULT 0,
    CONSTRAINT px_player_played_game PRIMARY KEY (steam_id, played_game_id)
) WITH (OIDS = FALSE) TABLESPACE pg_default;

ALTER TABLE public.player_played_game OWNER to postgres;
GRANT ALL ON TABLE public.player_played_game TO postgres;
GRANT ALL ON TABLE public.player_played_game TO kf2statsuser;