CREATE TABLE public.player_ip
(
    steam_id varchar(22) NOT NULL,
    ip_address text COLLATE pg_catalog."default" NOT NULL,
    started_using_time timestamp without time zone NOT NULL DEFAULT now(),
    CONSTRAINT "PX_PlayerIP" PRIMARY KEY (steam_id, ip_address, started_using_time)
) WITH (OIDS = FALSE) TABLESPACE pg_default;

ALTER TABLE public.player_ip OWNER to postgres;
GRANT ALL ON TABLE public.player_ip TO postgres;
GRANT ALL ON TABLE public.player_ip TO kf2statsuser;