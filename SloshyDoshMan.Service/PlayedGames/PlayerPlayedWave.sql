CREATE TABLE public.player_played_wave
(
    steam_id bigint NOT NULL,
    wave integer NOT NULL,
    played_game_id uuid NOT NULL,
    kills integer NOT NULL DEFAULT 0,
    perk text COLLATE pg_catalog."default",
    CONSTRAINT px_player_played_wave PRIMARY KEY (steam_id, played_game_id, wave)
) WITH (OIDS = FALSE) TABLESPACE pg_default;

ALTER TABLE public.player_played_wave OWNER to postgres;
GRANT ALL ON TABLE public.player_played_wave TO postgres;
GRANT ALL ON TABLE public.player_played_wave TO kf2statsuser;