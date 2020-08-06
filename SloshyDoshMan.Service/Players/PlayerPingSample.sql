CREATE TABLE public.player_ping_sample
(
    steam_id varchar(22) NOT NULL,
    ping integer NOT NULL,
    sampled_time timestamp without time zone NOT NULL DEFAULT now()
) WITH (OIDS = FALSE) TABLESPACE pg_default;

ALTER TABLE public.player_ping_sample OWNER to postgres;
GRANT ALL ON TABLE public.player_ping_sample TO postgres;
GRANT ALL ON TABLE public.player_ping_sample TO kf2statsuser;