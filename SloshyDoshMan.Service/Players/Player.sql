CREATE TABLE public.player
(
    steam_id bigint NOT NULL,
    last_known_ip text COLLATE pg_catalog."default" NOT NULL,
    name text COLLATE pg_catalog."default" NOT NULL,
    last_played_time timestamp without time zone,
    CONSTRAINT "PX_Player" PRIMARY KEY (steam_id)
) WITH (OIDS = FALSE) TABLESPACE pg_default;

ALTER TABLE public.player OWNER to postgres;

GRANT ALL ON TABLE public.player TO postgres;
GRANT ALL ON TABLE public.player TO kf2statsuser;