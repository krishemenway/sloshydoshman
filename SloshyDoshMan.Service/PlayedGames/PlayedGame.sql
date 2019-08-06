CREATE TABLE public.played_game
(
    played_game_id uuid NOT NULL,
    map text COLLATE pg_catalog."default" NOT NULL,
    game_type text COLLATE pg_catalog."default" NOT NULL,
    game_length text COLLATE pg_catalog."default",
    game_difficulty text COLLATE pg_catalog."default" NOT NULL,
    reached_wave smallint NOT NULL DEFAULT 0,
    time_started timestamp without time zone NOT NULL DEFAULT now(),
    time_finished timestamp without time zone,
    total_waves smallint,
    server_id uuid,
    players_won boolean,
    CONSTRAINT px_played_game PRIMARY KEY (played_game_id)
) WITH (OIDS = FALSE) TABLESPACE pg_default;

ALTER TABLE public.played_game OWNER to postgres;
GRANT ALL ON TABLE public.played_game TO postgres;
GRANT ALL ON TABLE public.played_game TO kf2statsuser;

CREATE INDEX ix_played_game_map ON public.played_game USING btree (map COLLATE pg_catalog."default") TABLESPACE pg_default;